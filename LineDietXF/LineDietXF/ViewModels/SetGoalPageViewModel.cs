using LineDietXF.Interfaces;
using LineDietXF.Types;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;

namespace LineDietXF.ViewModels
{
    /// <summary>
    /// Set a goal page shown modally from first tab of app or main menu
    /// </summary>
    public class SetGoalPageViewModel : BaseViewModel, INavigationAware
    {
        // Bindable Properties
        DateTime _startDate = DateTime.Today.Date;
        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                SetProperty(ref _startDate, value);
                UpdateStartWeightFromStartDate();
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        string _startWeight;
        public string StartWeight
        {
            get { return _startWeight; }
            set
            {
                SetProperty(ref _startWeight, value);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        DateTime _goalDate = DateTime.Today.AddMonths(Constants.App.SetGoalPage_DefaultGoalDateOffsetInMonths);
        public DateTime GoalDate
        {
            get { return _goalDate; }
            set
            {
                SetProperty(ref _goalDate, value);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        string _goalWeight;
        public string GoalWeight
        {
            get { return _goalWeight; }
            set
            {
                SetProperty(ref _goalWeight, value);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        // Services
        IDataService DataService { get; set; }
        ISettingsService SettingsService { get; set; }

        // Commands
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CloseCommand { get; set; }

        public SetGoalPageViewModel(INavigationService navigationService, IAnalyticsService analyticsService, ISettingsService settingsService, IPageDialogService dialogService, IDataService dataService) :
            base(navigationService, analyticsService, dialogService)
        {
            // Store off injected services
            DataService = dataService;
            SettingsService = settingsService;

            // Setup bindable commands
            SaveCommand = new DelegateCommand(Save, SaveCanExecute);
            CloseCommand = new DelegateCommand(Close);
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            UpdateStartWeightFromStartDate();
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            AnalyticsService.TrackPageView(Constants.Analytics.Page_SetGoal);

            TryLoadExistingGoal();
        }

        public void OnNavigatedFrom(NavigationParameters parameters) { }

        async void TryLoadExistingGoal()
        {
            WeightLossGoal existingGoal;
            try
            {
                IncrementPendingRequestCount(); // show loading

                existingGoal = await DataService.GetGoal();
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackFatalError($"{nameof(TryLoadExistingGoal)} - an exception occurred.", ex);
                // NOTE:: not showing an error here as this is not in response to user action. potentially should show a non-intrusive error banner
                return;
            }
            finally
            {
                DecrementPendingRequestCount(); // hide loading
            }

            if (existingGoal == null)
                return;

            // NOTE:: we could see if the goal date was already past and not load it in that case, but it would be better to still
            // bring in what they had before and just let them update it (ex: moving goal date forward / back)
            StartDate = existingGoal.StartDate;
            StartWeight = string.Format(Constants.Strings.Common_WeightFormat, existingGoal.StartWeight);
            GoalDate = existingGoal.GoalDate;
            GoalWeight = string.Format(Constants.Strings.Common_WeightFormat, existingGoal.GoalWeight);
        }

        async void UpdateStartWeightFromStartDate()
        {
            // pre-populate today's weight field if it has a value
            try
            {
                IncrementPendingRequestCount();

                var existingStartDateWeight = await DataService.GetWeightEntryForDate(StartDate);
                if (existingStartDateWeight != null)
                    StartWeight = existingStartDateWeight.ToString();
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackFatalError($"{nameof(UpdateStartWeightFromStartDate)} - an exception occurred.", ex);
                // NOTE:: not showing an error here as this is not in response to user action. potentially should show a non-intrusive error banner
            }
            finally
            {
                DecrementPendingRequestCount();
            }
        }

        bool SaveCanExecute()
        {
            // disable the save button if either weight field is empty
            if (string.IsNullOrWhiteSpace(StartWeight) || string.IsNullOrWhiteSpace(GoalWeight))
                return false;

            // disable the save button if their goal date is before their start date or the dates are equal
            if (GoalDate <= StartDate)
                return false;

            // disable the save button if either weight text field can't be parsed
            decimal startWeight, goalWeight;
            if (!decimal.TryParse(StartWeight, out startWeight) || !decimal.TryParse(GoalWeight, out goalWeight))
                return false;

            return true;
        }

        async void Save()
        {
            AnalyticsService.TrackEvent(Constants.Analytics.SetGoalCategory, Constants.Analytics.SetGoal_SavedGoal, 1);

            // convert entered value to a valid weight
            decimal startWeight, goalWeight;
            if (!decimal.TryParse(StartWeight, out startWeight) || !decimal.TryParse(GoalWeight, out goalWeight))
            {
                // show error about invalid value if we can't convert the entered value to a decimal
                await DialogService.DisplayAlertAsync(Constants.Strings.SetGoalPage_InvalidWeight_Title,
                    Constants.Strings.SetGoalPage_InvalidWeight_Message,
                    Constants.Strings.GENERIC_OK);

                return;
            }

            // give warning if goal weight is greater than start weight
            // NOTE:: we don't prevent this scenario as I have had friends intentionally use the previous version of line diet for
            // tracking weight gain during pregnancy or muscle building - so we just give a warning. We also don't prevent equal
            // start and goal weights in case they just want a line to show a maintenance weight they are trying to stay at
            if (goalWeight > startWeight)
            {
                // TODO:: analytics
                var result = await DialogService.DisplayAlertAsync(Constants.Strings.SetGoalPage_GoalWeightGreaterThanStartWeight_Title,
                    Constants.Strings.SetGoalpage_GoalWeightGreaterThanStartWeight_Message,
                    Constants.Strings.GENERIC_OK, Constants.Strings.GENERIC_CANCEL);

                if (!result)
                    return;
            }

            try
            {
                IncrementPendingRequestCount();

                // see if they've entered a different weight already for this date, if so warn them about it being updated
                var existingWeight = await DataService.GetWeightEntryForDate(StartDate);
                if (existingWeight != null)
                {
                    if (existingWeight.Weight != startWeight)
                    {
                        // show warning that an existing entry will be updated (is actually deleted and re-added), allow them to cancel
                        var result = await DialogService.DisplayAlertAsync(Constants.Strings.Common_UpdateExistingWeight_Title,
                            string.Format(Constants.Strings.Common_UpdateExistingWeight_Message, existingWeight.Weight, StartDate, startWeight),
                            Constants.Strings.GENERIC_OK,
                            Constants.Strings.GENERIC_CANCEL);

                        // if they canceled the dialog then return without changing anything
                        if (!result)
                            return;
                    }

                    // remove existing weight
                    if (!await DataService.RemoveWeightEntryForDate(StartDate))
                    {
                        AnalyticsService.TrackError($"{nameof(Save)} - Error when trying to remove existing weight entry for start date");

                        await DialogService.DisplayAlertAsync(Constants.Strings.Common_SaveError,
                            Constants.Strings.SetGoalPage_Save_RemoveExistingWeightFailed_Message, Constants.Strings.GENERIC_OK);
                        return;
                    }
                }

                var addStartWeightResult = await DataService.AddWeightEntry(new WeightEntry(StartDate, startWeight, SettingsService.WeightUnit));
                if (!addStartWeightResult)
                {
                    AnalyticsService.TrackError($"{nameof(Save)} - Error when trying to add weight entry for start date");

                    await DialogService.DisplayAlertAsync(Constants.Strings.Common_SaveError,
                        Constants.Strings.SetGoalPage_Save_AddingWeightFailed_Message, Constants.Strings.GENERIC_OK);
                    return;
                }

                var weightLossGoal = new WeightLossGoal(StartDate, startWeight, GoalDate.Date, goalWeight, SettingsService.WeightUnit);
                if (!await DataService.SetGoal(weightLossGoal))
                {
                    AnalyticsService.TrackError($"{nameof(Save)} - Error when trying to save new weight loss goal");

                    await DialogService.DisplayAlertAsync(Constants.Strings.Common_SaveError,
                        Constants.Strings.SetGoalPage_Save_AddingGoalFailed_Message, Constants.Strings.GENERIC_OK);
                    return;
                }

                await NavigationService.GoBackAsync(useModalNavigation: true);
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackFatalError($"{nameof(Save)} - an exception occurred.", ex);

                await DialogService.DisplayAlertAsync(Constants.Strings.Common_SaveError,
                    Constants.Strings.SetGoalPage_Save_Exception_Message, Constants.Strings.GENERIC_OK);
            }
            finally
            {
                DecrementPendingRequestCount();
            }
        }

        void Close()
        {
            NavigationService.GoBackAsync(useModalNavigation: true);
        }
    }
}

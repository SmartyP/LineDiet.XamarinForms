using LineDietXF.Interfaces;
using LineDietXF.Types;
using Prism.Commands;
using Prism.Events;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Threading.Tasks;

namespace LineDietXF.ViewModels
{
    /// <summary>
    /// There is an extra debug menu item when the app is run in debug mode - this is the debug page that is shown. It contains
    /// a few buttons for testing the color changing of the app and for populating weight/goal test data.
    /// </summary>
    public class DebugPageViewModel : BaseViewModel
    {
        // Services        
        IDataService DataService { get; set; }
        IEventAggregator EventAggregator { get; set; }
        IWindowColorService WindowColorService { get; set; }

        // Bindable Commands
        public DelegateCommand CloseCommand { get; set; }
        public DelegateCommand TurnGrayCommand { get; set; }
        public DelegateCommand TurnGreenCommand { get; set; }
        public DelegateCommand TurnRedCommand { get; set; }
        public DelegateCommand TestEndingAGoalCommand { get; set; }
        public DelegateCommand TestRealDataSetCommand { get; set; }

        public DebugPageViewModel(INavigationService navigationService, IAnalyticsService analyticsService, IPageDialogService dialogService, IDataService dataService, IEventAggregator eventAggregator, IWindowColorService windowColorService) :
            base(navigationService, analyticsService, dialogService)
        {
            // Store off injected services
            DataService = dataService;
            EventAggregator = eventAggregator;
            WindowColorService = windowColorService;

            // Wire up commands
            CloseCommand = new DelegateCommand(Close);
            TurnGrayCommand = new DelegateCommand(TurnGray);
            TurnGreenCommand = new DelegateCommand(TurnGreen);
            TurnRedCommand = new DelegateCommand(TurnRed);
            TestEndingAGoalCommand = new DelegateCommand(TestEndingAGoal);
            TestRealDataSetCommand = new DelegateCommand(TestRealDataSet);
        }

        async Task ClearData()
        {
            try
            {
                IncrementPendingRequestCount();

                // remove goal
                if (!await DataService.RemoveGoal())
                {
                    await DialogService.DisplayAlertAsync("Error", "An error occurred removing the goal", Constants.Strings.GENERIC_OK);
                    return;
                }

                // remove all weight entries
                var allEntries = await DataService.GetAllWeightEntries();
                foreach (var entry in allEntries)
                {
                    await DataService.RemoveWeightEntryForDate(entry.Date);
                }
            }
            finally
            {
                DecrementPendingRequestCount();
            }
        }

        async Task<bool> ShowLoseDataWarning()
        {
            return await DialogService.DisplayAlertAsync("Are you sure?", "This will delete all data and set it back up with sample data, are you sure?",
                Constants.Strings.GENERIC_OK, Constants.Strings.GENERIC_CANCEL);
        }

        async void TestEndingAGoal()
        {
            if (!await ShowLoseDataWarning())
                return;

            try
            {
                IncrementPendingRequestCount();

                await ClearData();

                // set a goal where today is the goal end date
                var goalStartDate = DateTime.Today.Date - TimeSpan.FromDays(30);
                var goalEndDate = DateTime.Today.Date;
                var goal = new WeightLossGoal(goalStartDate, 240, goalEndDate, 210);
                await DataService.SetGoal(goal);

                // add some weights
                await DataService.AddWeightEntry(new WeightEntry(goalStartDate + TimeSpan.FromDays(5), 235));
                await DataService.AddWeightEntry(new WeightEntry(goalStartDate + TimeSpan.FromDays(10), 230));
                await DataService.AddWeightEntry(new WeightEntry(goalStartDate + TimeSpan.FromDays(15), 225));
                await DataService.AddWeightEntry(new WeightEntry(goalStartDate + TimeSpan.FromDays(20), 220));
                await DataService.AddWeightEntry(new WeightEntry(goalStartDate + TimeSpan.FromDays(25), 215));
            }
            finally
            {
                DecrementPendingRequestCount();
            }

            Close();
        }

        // NOTE:: primarily used for screenshots, is based on specific dates
        async void TestRealDataSet()
        {
            if (!await ShowLoseDataWarning())
                return;

            try
            {
                IncrementPendingRequestCount();

                await ClearData();

                // set goal
                var goal = new WeightLossGoal(new DateTime(2017, 1, 1), 237.4M, new DateTime(2017, 7, 1), 200);
                await DataService.SetGoal(goal);

                // add some weights
                await DataService.AddWeightEntry(new WeightEntry(new DateTime(2017, 1, 1), 237.4M));
                await DataService.AddWeightEntry(new WeightEntry(new DateTime(2017, 1, 17), 235.2M));
                await DataService.AddWeightEntry(new WeightEntry(new DateTime(2017, 2, 4), 233.0M));
                await DataService.AddWeightEntry(new WeightEntry(new DateTime(2017, 2, 22), 230.8M));
                await DataService.AddWeightEntry(new WeightEntry(new DateTime(2017, 2, 27), 229.4M));
                await DataService.AddWeightEntry(new WeightEntry(new DateTime(2017, 2, 28), 228.6M));
                await DataService.AddWeightEntry(new WeightEntry(new DateTime(2017, 3, 1), 228.2M));
                await DataService.AddWeightEntry(new WeightEntry(new DateTime(2017, 3, 2), 227.0M));
                await DataService.AddWeightEntry(new WeightEntry(new DateTime(2017, 3, 3), 226.4M));
                await DataService.AddWeightEntry(new WeightEntry(new DateTime(2017, 3, 4), 227.0M));
                await DataService.AddWeightEntry(new WeightEntry(new DateTime(2017, 3, 5), 227.2M));
                await DataService.AddWeightEntry(new WeightEntry(new DateTime(2017, 3, 6), 226.0M));
                await DataService.AddWeightEntry(new WeightEntry(new DateTime(2017, 3, 7), 225.4M));
                await DataService.AddWeightEntry(new WeightEntry(new DateTime(2017, 3, 8), 225.4M));
                await DataService.AddWeightEntry(new WeightEntry(new DateTime(2017, 3, 9), 225.2M));
                await DataService.AddWeightEntry(new WeightEntry(new DateTime(2017, 3, 10), 225.8M));
                await DataService.AddWeightEntry(new WeightEntry(new DateTime(2017, 3, 11), 225.2M));
                await DataService.AddWeightEntry(new WeightEntry(new DateTime(2017, 3, 12), 223.8M));
                await DataService.AddWeightEntry(new WeightEntry(new DateTime(2017, 3, 13), 223.5M));
                await DataService.AddWeightEntry(new WeightEntry(new DateTime(2017, 3, 14), 223.0M));
                await DataService.AddWeightEntry(new WeightEntry(new DateTime(2017, 3, 15), 221.8M));
                await DataService.AddWeightEntry(new WeightEntry(new DateTime(2017, 3, 16), 221.6M));
                await DataService.AddWeightEntry(new WeightEntry(new DateTime(2017, 3, 17), 221.4M));
            }
            finally
            {
                DecrementPendingRequestCount();
            }

            Close();
        }

        async void TurnGray()
        {
            if (!await ShowLoseDataWarning())
                return;

            try
            {
                IncrementPendingRequestCount();

                await ClearData(); // screen will be gray after clearing data
            }
            finally
            {
                DecrementPendingRequestCount();
            }

            Close();
        }

        async void TurnRed()
        {
            if (!await ShowLoseDataWarning())
                return;

            try
            {
                IncrementPendingRequestCount();

                await ClearData();

                // set a goal where today is the goal end date
                var goalStartDate = DateTime.Today.Date - TimeSpan.FromDays(30);
                var goalEndDate = DateTime.Today.Date;
                var goal = new WeightLossGoal(goalStartDate, 240, goalEndDate, 210);
                await DataService.SetGoal(goal);

                await DataService.AddWeightEntry(new WeightEntry(DateTime.Today.Date, 250));
            }
            finally
            {
                DecrementPendingRequestCount();
            }

            Close();
        }

        async void TurnGreen()
        {
            if (!await ShowLoseDataWarning())
                return;

            try
            {
                IncrementPendingRequestCount();

                await ClearData();

                // set a goal where today is the goal end date
                var goalStartDate = DateTime.Today.Date - TimeSpan.FromDays(30);
                var goalEndDate = DateTime.Today.Date;
                var goal = new WeightLossGoal(goalStartDate, 240, goalEndDate, 210);
                await DataService.SetGoal(goal);

                await DataService.AddWeightEntry(new WeightEntry(DateTime.Today.Date, 210));
            }
            finally
            {
                DecrementPendingRequestCount();
            }

            Close();
        }

        async void Close()
        {
            await NavigationService.GoBackAsync(useModalNavigation: true);
        }
    }
}
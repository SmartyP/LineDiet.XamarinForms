﻿using LineDietXF.Enumerations;
using LineDietXF.Helpers;
using LineDietXF.Interfaces;
using LineDietXF.Types;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace LineDietXF.ViewModels
{
    public class SettingsPageViewModel : BaseViewModel
    {
        private ObservableCollection<MenuItem> _menuEntries;
        public ObservableCollection<MenuItem> MenuEntries
        {
            get { return _menuEntries; }
            set { SetProperty(ref _menuEntries, value); }
        }

        public MenuItem SelectedMenuItem
        {
            get { return null; }
            set
            {
                if (value != null)
                    HandleMenuItemSelected(value);
            }
        }

        // Services
        IDataService DataService;
        ISettingsService SettingsService;

        public SettingsPageViewModel(INavigationService navigationService, IAnalyticsService analyticsService, ISettingsService settingsService, IPageDialogService dialogService, IDataService dataService) :
            base(navigationService, analyticsService, dialogService)
        {
            // Store off services
            DataService = dataService;
            SettingsService = settingsService;

            // Build up settings menu
            SetupMenu();            
        }

        void SetupMenu()
        {
            var menuEntries = new List<MenuItem>();

            // add blank to top to add top space
            menuEntries.Add(new MenuItem(MenuItemEnum.Divider, string.Empty, true));

            var weightUnitsText = Constants.Strings.Settings_WeightUnits + SettingsService.WeightUnit.ToSettingsName();
            menuEntries.Add(new MenuItem(MenuItemEnum.Settings_SetUnitType, weightUnitsText, false));

            // add blank to top to add bottom space
            menuEntries.Add(new MenuItem(MenuItemEnum.Divider, string.Empty, true));

            MenuEntries = new ObservableCollection<MenuItem>(menuEntries);
        }

        private void HandleMenuItemSelected(MenuItem menuEntry)
        {
            switch (menuEntry.MenuType)
            {
                case MenuItemEnum.Settings_SetUnitType:
                    SetWeightUnitsTapped();
                    break;
                default:
#if DEBUG
                    Debugger.Break();
#endif
                    break;
            }
        }

        private void SetWeightUnitsTapped()
        {
            var imperialPoundsAction = ActionSheetButton.CreateButton(WeightUnitEnum.ImperialPounds.ToSettingsName(), 
                new DelegateCommand(() => { WeightUnitTypeSelected(WeightUnitEnum.ImperialPounds); }));
            var kilogramsAction = ActionSheetButton.CreateButton(WeightUnitEnum.Kilograms.ToSettingsName(),
                new DelegateCommand(() => { WeightUnitTypeSelected(WeightUnitEnum.Kilograms); }));
            var cancelAction = ActionSheetButton.CreateCancelButton(Constants.Strings.GENERIC_CANCEL, 
                new DelegateCommand(() => { }));

            DialogService.DisplayActionSheetAsync(Constants.Strings.Settings_ChangeWeightUnitsActionSheet, 
                imperialPoundsAction, kilogramsAction, cancelAction);
        }

        async void WeightUnitTypeSelected(WeightUnitEnum newUnits)
        {
            // already using this same setting, just return
            if (newUnits == SettingsService.WeightUnit)
                return;

            // see if there are existing weight entries not of this type
            IList<WeightEntry> allEntries = null;
            try
            {
                IncrementPendingRequestCount();
                allEntries = await DataService.GetAllWeightEntries();

                var weightsWithDifferentUnits = allEntries.Where(w => w.WeightUnit != newUnits);
                if (!weightsWithDifferentUnits.Any())
                {
                    SettingsService.WeightUnit = newUnits; // nothing to change, so just change the setting and return
                    SetupMenu(); // refresh the menu to show the new setting
                    return;
                }
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackFatalError($"{nameof(WeightUnitTypeSelected)} - an exception occurred getting all weights", ex);
                await DialogService.DisplayAlertAsync(Constants.Strings.Settings_ChangeWeightUnits_GetWeightsError_Title,
                    Constants.Strings.Settings_ChangeWeightUnits_GetWeightsError_Message, 
                    Constants.Strings.GENERIC_OK);
            }
            finally
            {
                DecrementPendingRequestCount();
            }

            // show warning of needing to convert these values
            var convertAction = ActionSheetButton.CreateButton(Constants.Strings.Settings_ChangeWeightUnits_ConvertWarning_ConvertWeightValues,
                new DelegateCommand(() => { UpdateWeightEntriesAndGoalUnits(newUnits, true); }));
            var changeUnitAction = ActionSheetButton.CreateButton(Constants.Strings.Settings_ChangeWeightUnits_ConvertWarning_ChangeUnits,
                new DelegateCommand(() => { UpdateWeightEntriesAndGoalUnits(newUnits, false); }));
            var cancelAction = ActionSheetButton.CreateCancelButton(Constants.Strings.GENERIC_CANCEL, 
                new DelegateCommand(() => { }));

            await DialogService.DisplayActionSheetAsync(Constants.Strings.Settings_ChangeWeightUnits_ConvertWarning,
                convertAction, changeUnitAction, cancelAction);
        }

        async void UpdateWeightEntriesAndGoalUnits(WeightUnitEnum newUnits, bool convertValues)
        {
            ResultWithErrorText result;
            try
            {
                IncrementPendingRequestCount();

                result = await DataService.ChangeWeightEntriesAndGoalUnitType(newUnits, convertValues);
                if (result.Success)
                {
                    // update setting
                    SettingsService.WeightUnit = newUnits;
                    SetupMenu(); // refresh the menu to show the new setting
                }
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackFatalError($"{nameof(UpdateWeightEntriesAndGoalUnits)} - an exception occurred calling {nameof(IDataService.ChangeWeightEntriesAndGoalUnitType)}, cannot continue.", ex);
                result = new ResultWithErrorText(false, $"Exception occurred in call to {nameof(IDataService.ChangeWeightEntriesAndGoalUnitType)}");
            }
            finally
            {
                DecrementPendingRequestCount();
            }

            if (result != null && !result.Success)
            {
                var msgText = convertValues ? Constants.Strings.Settings_ChangeWeightUnits_ConvertWeightValues_FatalError :
                    Constants.Strings.Settings_ChangeWeightUnits_ChangeUnits_FatalError;

                await DialogService.DisplayAlertAsync(Constants.Strings.Common_FatalError, msgText + result.ErrorText, Constants.Strings.GENERIC_OK);
                return;
            }
        }
    }
}
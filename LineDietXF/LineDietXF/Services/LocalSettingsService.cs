using System;
using LineDietXF.Enumerations;
using LineDietXF.Interfaces;
using Plugin.Settings;

// NOTE:: Uses Xam.Plugins.Settings (ref: https://github.com/jamesmontemagno/SettingsPlugin)
namespace LineDietXF.Services
{
    /// <summary>
    /// Used for storing app settings locally
    /// </summary>
    public class LocalSettingsService : ISettingsService
    {
        public bool HasDismissedStartupView
        {
            get
            {
                return CrossSettings.Current.GetValueOrDefault(Constants.App.Setting_DismissedStartupView_Key,
                                                                     Constants.App.Setting_DismissedStartupView_Default);
            }
            set
            {
                CrossSettings.Current.AddOrUpdateValue(Constants.App.Setting_DismissedStartupView_Key, value);
            }
        }

        public WeightUnitEnum WeightUnit
        { 
            get
            {
                return (WeightUnitEnum)CrossSettings.Current.GetValueOrDefault(Constants.App.Setting_WeightUnits_Key,
                    (int)Constants.App.Setting_WeightUnits_Default);
            }
            set
            {
                CrossSettings.Current.AddOrUpdateValue(Constants.App.Setting_WeightUnits_Key, (int)value);
            }
        }

        public void Initialize()
        {
        }
    }
}
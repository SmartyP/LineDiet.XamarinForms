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
                return CrossSettings.Current.GetValueOrDefault<bool>(Constants.App.Settings_HasDismissedStartupView,
                                                                     Constants.App.Settings_HasDismissedStartupView_DefaultValue);
            }
            set
            {
                CrossSettings.Current.AddOrUpdateValue<bool>(Constants.App.Settings_HasDismissedStartupView, value);
            }
        }

        public void Initialize()
        {
        }
    }
}
namespace LineDietXF.Interfaces
{
    /// <summary>
    /// Interface defining services which store app specific settings.
    /// </summary>
    public interface ISettingsService
    {
        bool HasDismissedStartupView { get; set; }

        void Initialize();
    }
}
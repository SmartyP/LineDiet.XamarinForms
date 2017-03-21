using LineDietXF.Interfaces;

namespace LineDietXF.MockServices
{
    public class MockSettingsService : ISettingsService
    {
        public bool HasDismissedStartupView { get; set; }
        public int WeightsSinceLastShareRequest { get; set; }
        public bool FacebookDontAskAgain { get; set; }

        public MockSettingsService()
        {
        }

        public void Initialize()
        {
        }
    }
}
namespace LineDietXF.Constants
{
    public static class App
    {
#if DEBUG        
        // NOTE:: DEBUG FLAGS - THESE SHOULD NEVER BE CHECKED IN AS TRUE
        public static readonly bool DEBUG_USE_MOCKS = false;
        public static readonly bool DEBUG_SIMULATE_SLOW_RESPONSE = false;
        public static readonly bool DEBUG_FORCE_SHOW_GETTING_STARTED = false;
        public const int DEBUG_SIMULATE_SLOW_RESPONSE_TIME = 1000; // in milliseconds
#endif

        public const string AppVersion = "0.5.0";
        public const string AppVersionDebugAddition = "b";

        public const string SQLite_DB_Filename = "Linediet.db";

        public static int HISTORY_WeightEntryMaxCount = 31;
        public const int SetGoalPage_DefaultGoalDateOffsetInMonths = 3; // default new goal end date to X months from now

        // Settings
        public const string Settings_HasDismissedStartupView = "HasDismissedStartupView";
        public const bool Settings_HasDismissedStartupView_DefaultValue = false;

        // Graphing
        // NOTE:: constants relative to break points in graph labeling are defined within GraphPageViewModel
        public const decimal Graph_WeightRange_DefaultMin = 160;
        public const decimal Graph_WeightRange_DefaultMax = 180;
        public const decimal Graph_WeightRange_MinPadding = 0.98M;
        public const decimal Graph_WeightRange_MaxPadding = 1.02M;
        public const decimal Graph_GoalOnly_Padding = 50.0M;
        public const int Graph_MinDateRangeVisible = 5;
        public const int Graph_MaxDateRangeVisible = 365;
        public const int Graph_MinWeightRangeVisible = 5;
        public const int Graph_MaxWeightRangeVisible = 100;

        // Urls
        public const string ShareUrl = "http://www.linedietapp.com/?ref=LDapp";
        public const string FeedbackUrl = "http://www.linedietapp.com/feedback";
        public const string SmartyPNetUrl = "http://www.smartyp.net";
        public const string SPCWebsiteUrl = "http://www.smartypantscoding.com";
        public const string GithubUrl = "https://github.com/SmartyP/LineDiet.XamarinForms.git";
    }
}
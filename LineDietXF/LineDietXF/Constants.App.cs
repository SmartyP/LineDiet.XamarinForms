using LineDietXF.Enumerations;

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

        public const string SQLite_DB_Filename = "Linediet.db";

        public static int HISTORY_WeightEntryMaxCount = int.MaxValue; // NOTE:: since everything is local currently we're not limiting how many weights are shown on graph and listing
        public const int SetGoalPage_DefaultGoalDateOffsetInMonths = 3; // default new goal end date to X months from now
        public const int PoundsInAStone = 14;

        // Settings
        public const string Settings_HasDismissedStartupView = "HasDismissedStartupView";
        public const string Settings_WeightUnits = "WeightUnits";
        public const bool Settings_HasDismissedStartupView_DefaultValue = false;
        public const WeightUnitEnum Settings_WeightUnits_DefaultValue = WeightUnitEnum.ImperialPounds;

        // Nav params
        public const string NavParam_FromGettingStarted = "FromGettingStarted";

        // Graphing
        // NOTE:: constants relative to break points in graph labeling are defined within GraphPageViewModel
        public const int Graph_MinDateRangeVisible = 5;
        public const int Graph_MaxDateRangeVisible = 365;
        public const decimal Graph_WeightRange_MinPadding = 0.98M;
        public const decimal Graph_WeightRange_MaxPadding = 1.02M;
        public const decimal Graph_WeightRange_Pounds_DefaultMin = 160;
        public const decimal Graph_WeightRange_Pounds_DefaultMax = 180;
        public const decimal Graph_WeightRange_Kilograms_DefaultMin = 72;
        public const decimal Graph_WeightRange_Kilograms_DefaultMax = 82;
        public const decimal Graph_GoalOnly_Pounds_Padding = 50.0M;
        public const decimal Graph_GoalOnly_Kilograms_Padding = 22.0M;
        public const int Graph_Pounds_MinWeightRangeVisible = 5;
        public const int Graph_Pounds_MaxWeightRangeVisible = 100;
        public const int Graph_Kilograms_MinWeightRangeVisible = 5;
        public const int Graph_Kilograms_MaxWeightRangeVisible = 45;


        // Weight conversions
        public const decimal PoundsToKilograms = 0.45359237M;
        public const decimal KilogramsToPounds = 2.2046226218M;

        // Urls
        public const string ShareUrl = "http://www.linedietapp.com/?ref=LDapp";
        public const string FeedbackUrl = "http://www.linedietapp.com/feedback";
        public const string SmartyPNetUrl = "http://www.smartyp.net";
        public const string SPCWebsiteUrl = "http://www.smartypantscoding.com";
        public const string GithubUrl = "https://github.com/SmartyP/LineDiet.XamarinForms.git";
    }
}
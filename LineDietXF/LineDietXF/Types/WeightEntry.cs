using SQLite;
using System;
using System.Diagnostics;

namespace LineDietXF.Types
{
    /// <summary>
    /// The WeightEntry object is one of the main data types of the app including information about a specific weight entry
    /// NOTE:: includes attributes necessary by SQLite for storage to SQL table
    /// </summary>
    [Table("WeightEntries")]
    public class WeightEntry
    {
        [PrimaryKey, Unique, NotNull]
        public DateTime Date { get; set; }

        [NotNull]
        public decimal Weight { get; set; }

        public WeightEntry()
        {
            Weight = decimal.MinValue;
        }

        public WeightEntry(DateTime dt, decimal wt)
        {
            Date = dt;
            Weight = wt;
        }

        public override string ToString()
        {
            if (Weight == decimal.MinValue)
            {
                if (Debugger.IsAttached)
                    Debugger.Break();                

                return "<error>";
            }

            return string.Format(Constants.Strings.Common_WeightFormat, Weight);
        }
    }
}
using LineDietXF.Enumerations;
using OxyPlot;
using Plugin.Share.Abstractions;
using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace LineDietXF.Extensions
{
    /// <summary>
    /// A set of extension methods for converting between various color types
    /// </summary>
    public static class ColorExtensions
    {
        public static OxyColor ToOxyColor(this Color color)
        {
            return OxyColor.FromArgb(Convert.ToByte(color.A * 255.0D),
                Convert.ToByte(color.R * 255.0D),
                Convert.ToByte(color.G * 255.0D),
                Convert.ToByte(color.B * 255.0D));
        }

        public static ShareColor ToShareColor(this Color color)
        {
            return new ShareColor(Convert.ToInt16(color.R * 255.0D),
                Convert.ToInt16(color.G * 255.0D),
                Convert.ToInt16(color.B * 255.0D));
        }

        public static Color GetLightColor(this BaseColorEnum baseColorEnum)
        {
            switch (baseColorEnum)
            {
                case BaseColorEnum.Gray:
                    return Constants.UI.COLOR_LIGHTGRAY_FILL;
                case BaseColorEnum.Green:
                    return Constants.UI.COLOR_LIGHTGREEN_FILL;
                case BaseColorEnum.Red:
                    return Constants.UI.COLOR_LIGHTRED_FILL;
                default:
                    if (Debugger.IsAttached)
                        Debugger.Break();

                    // This should never fall through, so setting to Purple to be obvious
                    return Color.Purple;
            }
        }

        public static Color GetDarkColor(this BaseColorEnum baseColorEnum)
        {
            switch (baseColorEnum)
            {
                case BaseColorEnum.Gray:
                    return Constants.UI.COLOR_DARKGRAY_FILL;
                case BaseColorEnum.Green:
                    return Constants.UI.COLOR_DARKGREEN_FILL;
                case BaseColorEnum.Red:
                    return Constants.UI.COLOR_DARKRED_FILL;
                default:
                    if (Debugger.IsAttached)
                        Debugger.Break();

                    // This should never fall through, so setting to Purple to be obvious
                    return Color.Purple;
            }
        }
    }
}

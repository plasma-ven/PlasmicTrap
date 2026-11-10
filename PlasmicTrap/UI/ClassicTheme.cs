using System.Windows;
using System.Windows.Media;
using Wpf.Ui.Appearance;

namespace PlasmicTrap.UI
{
    public static class ClassicTheme
    {
        private static readonly Color AccentColor = Color.FromRgb(0x00, 0xA2, 0xFF);

        private static readonly Color BackgroundColor = Color.FromRgb(0xE3, 0xE3, 0xE3);

        private static readonly string[] OverriddenKeys =
        {
            "ApplicationBackgroundColor",
            "ApplicationBackgroundBrush"
        };

        public static void Apply()
        {
            Accent.Apply(AccentColor, ThemeType.Light);

            var resources = Application.Current.Resources;

            resources["ApplicationBackgroundColor"] = BackgroundColor;
            resources["ApplicationBackgroundBrush"] = new SolidColorBrush(BackgroundColor);
        }

        public static void Clear()
        {
            var resources = Application.Current.Resources;

            foreach (string key in OverriddenKeys)
                resources.Remove(key);
        }
    }
}

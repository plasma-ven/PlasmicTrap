using Microsoft.Win32;

namespace PlasmicTrap.Extensions
{
    public static class ThemeEx
    {
        public static bool IsClassic(this Theme theme) => theme == Theme.Classic;

        public static Theme GetFinal(this Theme dialogTheme)
        {
            if (dialogTheme == Theme.Classic)
                return Theme.Light;

            if (dialogTheme != Theme.Default)
                return dialogTheme;

            using var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize");

            if (key?.GetValue("AppsUseLightTheme") is int value && value == 0)
                return Theme.Dark;

            return Theme.Light;
        }
    }
}

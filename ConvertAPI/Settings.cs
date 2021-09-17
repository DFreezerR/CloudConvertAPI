
using System.Runtime.CompilerServices;

namespace CloudConvertApp
{
    public static class Settings
    {
        public static string APIKey = Properties.Settings.Default.APIKey;

        public static void Update()
        {
            APIKey = Properties.Settings.Default.APIKey;
        }

    }
}
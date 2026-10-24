namespace PlasmicTrap.Utility
{
    public static class RobloxSingleton
    {
        private const string MutexName = "ROBLOX_singletonMutex";

        private static Mutex? _mutex;

        private static readonly object _lock = new();

        public static bool IsHeld => _mutex is not null;

        public static void Hold()
        {
            const string LOG_IDENT = "RobloxSingleton::Hold";

            lock (_lock)
            {
                if (_mutex is not null)
                    return;

                try
                {
                    _mutex = new Mutex(true, MutexName, out bool createdNew);

                    App.Logger.WriteLine(LOG_IDENT, createdNew
                        ? "Holding Roblox singleton mutex"
                        : "Roblox singleton mutex already exists, holding a handle to it");
                }
                catch (Exception ex)
                {
                    App.Logger.WriteLine(LOG_IDENT, "Failed to hold Roblox singleton mutex");
                    App.Logger.WriteException(LOG_IDENT, ex);
                }
            }
        }
    }
}

namespace USPInstaller
{
    public static class Globals
    {
#if QA
        public const bool HasQAMode = true;
        public static bool EnableQAMode { get; set; } = false;
#else
        public const bool HasQAMode = false;
#endif
    }
}

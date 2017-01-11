namespace MetroOverhaul.Extensions
{
    public static class NetInfoExtensions
    {
        public static bool IsUndergroundMetroStationTrack(this NetInfo netInfo)
        {
            if (netInfo?.name == null)
            {
                return false;
            }
            return netInfo.name == "Metro Station Track" || netInfo.name.Contains("Metro Station Track Tunnel");
        }
        public static bool IsUndergroundMetroTrack(this NetInfo netInfo)
        {
            if (netInfo?.name == null)
            {
                return false;
            }
            return netInfo.name.Contains("Metro Track Tunnel");
        }
    }
}
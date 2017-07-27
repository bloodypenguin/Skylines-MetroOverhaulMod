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
            return IsUndergroundSidePlatformMetroStationTrack(netInfo) || IsUndergroundIslandPlatformStationTrack(netInfo);
        }
        public static bool IsUndergroundSidePlatformMetroStationTrack(this NetInfo netInfo)
        {
            if (netInfo?.name == null)
            {
                return false;
            }
            return netInfo.name == "Metro Station Track" || netInfo.name.Contains("Metro Station Track Tunnel");
        }
        public static bool IsUndergroundIslandPlatformStationTrack(this NetInfo netinfo)
        {
            if (netinfo?.name == null)
            {
                return false;
            }
            return netinfo.name == "Metro Station Track Island" || netinfo.name.Contains("Metro Station Track Tunnel Island");
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
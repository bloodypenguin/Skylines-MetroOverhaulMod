namespace MetroOverhaul.Extensions
{
    public static class NetInfoExtensions
    {
        public static bool IsAbovegroundMetroStationTrack(this NetInfo info)
        {
            if (info?.name == null)
            {
                return false;
            }
            return IsAbovegroundSidePlatformMetroStationTrack(info) || IsAbovegroundIslandPlatformStationTrack(info) || IsAbovegroundSmallStationTrack(info);
        }
        public static bool IsAbovegroundSidePlatformMetroStationTrack(this NetInfo netInfo)
        {
            if (netInfo?.name == null)
            {
                return false;
            }
            return netInfo.name.Contains("Metro Station Track ") && netInfo.name.Contains("Tunnel") == false && netInfo.name.Contains("Island") == false && netInfo.name.Contains("Small") == false;
        }
        public static bool IsAbovegroundIslandPlatformStationTrack(this NetInfo netinfo)
        {
            if (netinfo?.name == null)
            {
                return false;
            }
            return netinfo.name.Contains("Metro Station Track") && netinfo.name.Contains("Island") && netinfo.name.Contains("Tunnel") == false;
        }
        public static bool IsAbovegroundSmallStationTrack(this NetInfo netinfo)
        {
            if (netinfo?.name == null)
            {
                return false;
            }
            return netinfo.name.Contains("Metro Station Track") && netinfo.name.Contains("Small") && netinfo.name.Contains("Tunnel") == false;
        }
        public static bool IsUndergroundMetroStationTrack(this NetInfo netInfo)
        {
            if (netInfo?.name == null)
            {
                return false;
            }
            return IsUndergroundSidePlatformMetroStationTrack(netInfo) || IsUndergroundIslandPlatformStationTrack(netInfo) || IsUndergroundSmallStationTrack(netInfo);
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
            return netinfo.name.Contains("Metro Station Track Tunnel Island");
        }

        public static bool IsUndergroundSmallStationTrack(this NetInfo netinfo)
        {
            if (netinfo?.name == null)
            {
                return false;
            }
            return netinfo.name.Contains("Metro Station Track Tunnel Small");
        }

        public static bool IsUndergroundMetroTrack(this NetInfo netInfo)
        {
            if (netInfo?.name == null)
            {
                return false;
            }
            return netInfo.name.Contains("Metro Track Tunnel");
        }
        public static bool IsMetroTrack(this NetInfo netInfo)
        {
            if (netInfo?.name == null)
            {
                return false;
            }
            return netInfo.name.Contains("Metro Track");
        }
        public static bool IsMetroStationTrack(this NetInfo netinfo)
        {
            if (netinfo?.name == null)
            {
                return false;
            }
            return netinfo.IsAbovegroundMetroStationTrack() || netinfo.IsUndergroundMetroStationTrack();
        }
        public static bool IsPedestrianNetwork(this NetInfo info)
        {
            return info != null && (info.name == "Pedestrian Connection Surface" || info.name == "Pedestrian Connection Inside" || info.name == "Pedestrian Connection Underground");
        }
    }
}
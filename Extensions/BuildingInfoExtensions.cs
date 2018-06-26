using System.Linq;

namespace MetroOverhaul.Extensions
{
    public static partial class BuildingInfoExtensions
    {
        public static bool IsMetroDepot(this BuildingInfo info)
        {
            return info?.m_class != null && info.m_buildingAI is DepotAI && info.m_class.m_service == ItemClass.Service.PublicTransport && info.m_class.m_subService == ItemClass.SubService.PublicTransportMetro;
        }

        public static bool IsAbovegroundMetroStation(this BuildingInfo info)
        {
            return IsMetroDepot(info) && info.m_buildingAI is TransportStationAI && HasAbovegroundMetroStationTracks(info);
        }
        public static bool HasAbovegroundMetroStationTracks(this BuildingInfo info)
        {
            return info?.m_paths != null && info.m_paths.Any(p => p?.m_netInfo != null && p.m_netInfo.IsAbovegroundMetroStationTrack());
        }
        public static bool HasAbovegroundSidePlatformMetroTracks(this BuildingInfo info)
        {
            return info?.m_paths != null && info.m_paths.Any(p => p?.m_netInfo != null && p.m_netInfo.IsAbovegroundSidePlatformMetroStationTrack());
        }
        public static bool HasAbovegroundIslandPlantformMetroTracks(this BuildingInfo info)
        {
            return info?.m_paths != null && info.m_paths.Any(p => p?.m_netInfo != null && p.m_netInfo.IsAbovegroundIslandPlatformStationTrack());
        }
        public static bool HasAbovegroundSmallMetroTracks(this BuildingInfo info)
        {
            return info?.m_paths != null && info.m_paths.Any(p => p?.m_netInfo != null && p.m_netInfo.IsAbovegroundSmallStationTrack());
        }

        public static bool IsUndergroundMetroStation(this BuildingInfo info)
        {
            return IsMetroDepot(info) && info.m_buildingAI is TransportStationAI && HasUndergroundMetroStationTracks(info);
        }
        public static bool HasUndergroundSidePlatformMetroTracks(this BuildingInfo info)
        {
            return info?.m_paths != null && info.m_paths.Any(p => p?.m_netInfo != null && p.m_netInfo.IsUndergroundSidePlatformMetroStationTrack());
        }
        public static bool HasUndergroundIslandPlantformMetroTracks(this BuildingInfo info)
        {
            return info?.m_paths != null && info.m_paths.Any(p => p?.m_netInfo != null && p.m_netInfo.IsUndergroundIslandPlatformStationTrack());
        }
        public static bool HasUndergroundSmallPlantformMetroTracks(this BuildingInfo info)
        {
            return info?.m_paths != null && info.m_paths.Any(p => p?.m_netInfo != null && p.m_netInfo.IsUndergroundSmallStationTrack());
        }
        public static bool HasUndergroundMetroStationTracks(this BuildingInfo info)
        {
            return info?.m_paths != null && info.m_paths.Any(p => p?.m_netInfo != null && p.m_netInfo.IsUndergroundMetroStationTrack());
        }
    }
}

using MetroOverhaul.NEXT.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MetroOverhaul
{
    public static class MetroStations
    {
        public static void UpdateMetro(float stationDepthDist = 12, float stationLengthDist = 144)
        {
            var vanillaMetroTrack = PrefabCollection<NetInfo>.FindLoaded("Metro Track");
            vanillaMetroTrack.m_buildHeight = -12;
            var vanillaMetroStation = PrefabCollection<BuildingInfo>.FindLoaded("Metro Entrance");
            var vanillaMetroStationTrack = PrefabCollection<NetInfo>.FindLoaded("Metro Station Track");
            vanillaMetroStationTrack.m_buildHeight = -12;
            vanillaMetroStationTrack.m_maxHeight = -1;
            vanillaMetroStationTrack.m_minHeight = -36;
            foreach (var info in Resources.FindObjectsOfTypeAll<BuildingInfo>())
            {
                if (!info.IsStation())
                {
                    continue;
                }
                if (info.m_buildingAI.GetType() != typeof(DepotAI))
                {
                    var transportStationAi = ((TransportStationAI)info.m_buildingAI);
                    transportStationAi.m_maxVehicleCount = 0;
                }

                info.m_UnlockMilestone = vanillaMetroStation.m_UnlockMilestone;
                ((DepotAI)info.m_buildingAI).m_createPassMilestone = ((DepotAI)vanillaMetroStation.m_buildingAI).m_createPassMilestone;
            }
        }
        public static bool IsStation(this BuildingInfo info)
        {
            if (info == null || info.m_class == null || info.m_paths == null || !(info.m_buildingAI is DepotAI) || info.m_class.m_subService != ItemClass.SubService.PublicTransportMetro)
            {
                return false;
            }
            else
            {
                var metroStations = info.m_paths.Where(p => p != null && p.m_netInfo != null && p.m_netInfo.name == "Metro Station Track");
                if (metroStations == null || metroStations.Count() == 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}

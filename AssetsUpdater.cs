using MetroOverhaul.Extensions;
using MetroOverhaul.NEXT;
using UnityEngine;

namespace MetroOverhaul
{
    public class AssetsUpdater
    {
        public void UpdateExistingAssets()
        {
            UpdateVanillaMetroTracks();
            UpdateMetroStations();
            UpdateTrainTracks();
            UpdateMetroTrainEffects();
        }

        private static void UpdateTrainTracks()
        {
            foreach (var netInfo in Object.FindObjectsOfType<NetInfo>())
            {
                if (netInfo.m_class.m_service != ItemClass.Service.PublicTransport ||
                    netInfo.m_class.m_subService != ItemClass.SubService.PublicTransportTrain)
                {
                    continue;
                }
                var version = Initializer.DetectVersion(netInfo);
                var multiplier = (version == NetInfoVersion.Tunnel || version == NetInfoVersion.Slope || version == NetInfoVersion.Elevated || version == NetInfoVersion.Bridge)? 1.5f : 1.0f;
                var trainTrackInfo = PrefabCollection<NetInfo>.FindLoaded("Train Track");

                //TODO(earalov): patch costs
            }
        }

        private static void UpdateVanillaMetroTracks()
        {
            var vanillaMetroTrack = PrefabCollection<NetInfo>.FindLoaded("Metro Track");
            vanillaMetroTrack.m_buildHeight = -12;
            Initializer.SetCosts(vanillaMetroTrack, NetInfoVersion.Tunnel);
            var vanillaMetroStationTrack = PrefabCollection<NetInfo>.FindLoaded("Metro Station Track");
            vanillaMetroStationTrack.m_buildHeight = -12;
            vanillaMetroStationTrack.m_maxHeight = -1;
            vanillaMetroStationTrack.m_minHeight = -36;
        }

        private static void UpdateMetroStations()
        {
            var vanillaMetroStation = PrefabCollection<BuildingInfo>.FindLoaded("Metro Entrance");

            foreach (var info in Resources.FindObjectsOfTypeAll<BuildingInfo>())
            {
                if (!info.IsMetroDepot())
                {
                    continue;
                }
                if (info.m_buildingAI.GetType() != typeof(DepotAI))
                {
                    var transportStationAi = (TransportStationAI)info.m_buildingAI;
                    transportStationAi.m_maxVehicleCount = 0;
                }

                info.m_UnlockMilestone = vanillaMetroStation.m_UnlockMilestone;
                ((DepotAI)info.m_buildingAI).m_createPassMilestone = ((DepotAI)vanillaMetroStation.m_buildingAI).m_createPassMilestone;
            }
        }

        //this method is supposed to be called before level loading
        public static void PreventVanillaMetroTrainSpawning()
        {
            var metro = PrefabCollection<VehicleInfo>.FindLoaded("Metro");
            metro.m_class = ScriptableObject.CreateInstance<ItemClass>();
        }

        private static void UpdateMetroTrainEffects()
        {
            var vanillaMetro = PrefabCollection<VehicleInfo>.FindLoaded("Metro");
            var arriveEffect = ((MetroTrainAI)vanillaMetro.m_vehicleAI).m_arriveEffect;
            for (uint i = 0; i < PrefabCollection<VehicleInfo>.LoadedCount(); i++)
            {
                var info = PrefabCollection<VehicleInfo>.GetLoaded(i);
                var metroTrainAI = info?.m_vehicleAI as MetroTrainAI;
                if (metroTrainAI == null)
                {
                    continue;
                }
                info.m_effects = vanillaMetro.m_effects;
                metroTrainAI.m_arriveEffect = arriveEffect;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using ICities;
using MetroOverhaul.Extensions;
using MetroOverhaul.NEXT;
using MetroOverhaul.OptionsFramework;
using UnityEngine;

namespace MetroOverhaul
{
    public class AssetsUpdater
    {

        public void UpdateExistingAssets(LoadMode mode)
        {
            UpdateMetroTrainEffects();
            if (mode == LoadMode.LoadAsset || mode == LoadMode.NewAsset)
            {
                return;
            }
            try
            {
                UpdateTrainTracks();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
            try
            {
                UpdateMetroStationsMeta();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
        }

        private static void UpdateTrainTracks()
        {
            var vanillaTracksNames = new[] { "Train Track", "Train Track Elevated", "Train Track Bridge", "Train Track Slope", "Train Track Tunnel" };
            var vanillaTracksCosts = vanillaTracksNames.ToDictionary(Initializer.DetectVersion, GetTrackCost);
            var toGroundMultipliers = vanillaTracksCosts.ToDictionary(keyValue => keyValue.Key,
                keyValue => keyValue.Value == vanillaTracksCosts[NetInfoVersion.Ground] ? 1f : keyValue.Value / (float)vanillaTracksCosts[NetInfoVersion.Ground]);

            var baseMultiplier = GetTrackCost("Metro Track Ground") / (float)GetTrackCost("Train Track");
            for (ushort i = 0; i < PrefabCollection<NetInfo>.LoadedCount(); i++)
            {
                var netInfo = PrefabCollection<NetInfo>.GetLoaded(i);
                var ai = netInfo?.m_netAI as PlayerNetAI;
                if (ai == null || netInfo.m_class.m_service != ItemClass.Service.PublicTransport || netInfo.m_class.m_subService != ItemClass.SubService.PublicTransportTrain)
                {
                    continue;
                }
                var version = Initializer.DetectVersion(netInfo.name);
                var wasCost = GetTrackCost(netInfo);
                if (wasCost == 0)
                {
                    continue;
                }
                var newCost = wasCost / toGroundMultipliers[version] *
                                     Initializer.GetCostMultiplier(version) * GetAdditionalCostMultiplier(version) * baseMultiplier;
#if DEBUG
                UnityEngine.Debug.Log($"Updating asset {netInfo.name} cost. Was cost: {wasCost}. New cost: {newCost}");
#endif
                ai.m_constructionCost = (int)newCost;
                ai.m_maintenanceCost = (int)(newCost / 10f);
            }
        }

        private static float GetAdditionalCostMultiplier(NetInfoVersion version)
        {
            return (version == NetInfoVersion.Tunnel || version == NetInfoVersion.Slope || version == NetInfoVersion.Elevated || version == NetInfoVersion.Bridge) ? 1.5f : 1.0f;
        }

        private static int GetTrackCost(string prefabName)
        {
            var netInfo = PrefabCollection<NetInfo>.FindLoaded(prefabName);
            return GetTrackCost(netInfo);
        }

        private static int GetTrackCost(NetInfo netInfo)
        {
            return ((PlayerNetAI)netInfo.m_netAI).m_constructionCost;
        }

        //this method is supposed to be called from LoadingExtension
        public static void UpdateBuildingsMetroPaths(LoadMode mode, bool toVanilla = false)
        {
#if !DEBUG
            if (mode == LoadMode.NewAsset || mode == LoadMode.NewAsset)
            {
                return;
            }
#endif
            for (uint i = 0; i < PrefabCollection<BuildingInfo>.LoadedCount(); i++)
            {
                try
                {
                    var prefab = PrefabCollection<BuildingInfo>.GetPrefab(i);
                    if (prefab == null)
                    {
                        continue;
                    }
                    if (!toVanilla)
                    {
                        if (!OptionsWrapper<Options>.Options.metroUi)
                        {
                            SetStationCustomizations.ModifyStation(prefab, 12, 144, 0, UI.TrackType.SidePlatform, UI.TrackType.SidePlatform);
                        }
                    }
                    SetupTunnelTracks(prefab, toVanilla);

                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
            }
        }

        private static void SetupTunnelTracks(BuildingInfo info, bool toVanilla = false)
        {
            if (info?.m_paths == null)
            {
                return;
            }
            foreach (var path in info.m_paths)
            {
                if (path?.m_netInfo?.name == null)
                {
                    continue;
                }
                if (toVanilla)
                {
                    if (path.m_netInfo.name.Contains("Metro Station Track Tunnel"))
                    {
                        path.m_netInfo = PrefabCollection<NetInfo>.FindLoaded("Metro Station Track");
                    }
                    else if (path.m_netInfo.name.Contains("Metro Track Tunnel"))
                    {
                        path.m_netInfo = PrefabCollection<NetInfo>.FindLoaded("Metro Track");
                    }
                }
                else
                {
                    if (path.m_netInfo.name == "Metro Station Track")
                    {
                        path.m_netInfo = PrefabCollection<NetInfo>.FindLoaded("Metro Station Track Tunnel");
                    }
                    else if (path.m_netInfo.name == "Metro Track")
                    {
                        path.m_netInfo = PrefabCollection<NetInfo>.FindLoaded("Metro Track Tunnel");
                    }
                }
            }
        }

        private static void UpdateMetroStationsMeta()
        {
            var vanillaMetroStation = PrefabCollection<BuildingInfo>.FindLoaded("Metro Entrance");

            var infos = Resources.FindObjectsOfTypeAll<BuildingInfo>();
            if (infos == null)
            {
                return;
            }
            foreach (var info in infos)
            {
                try
                {
                    if (info == null || info.m_buildingAI == null || !info.IsMetroDepot())
                    {
                        continue;
                    }

                    var ai = info.m_buildingAI as TransportStationAI;
                    if (ai != null)
                    {
                        var transportStationAi = ai;
                        transportStationAi.m_maxVehicleCount = 0;
                    }
                    info.m_UnlockMilestone = vanillaMetroStation.m_UnlockMilestone;
                    ((DepotAI) info.m_buildingAI).m_createPassMilestone = ((DepotAI) vanillaMetroStation.m_buildingAI)
                        .m_createPassMilestone;
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError($"MOM: Failed to update meta of {info?.name}:");
                    UnityEngine.Debug.LogException(e);
                }
            }
        }

        //this method is supposed to be called before level loading
        public static void PreventVanillaMetroTrainSpawning()
        {
            var metro = PrefabCollection<VehicleInfo>.FindLoaded("Metro");
            if (metro == null)
            {
                return;
            }
            metro.m_class = ScriptableObject.CreateInstance<ItemClass>();
        }

        //this method is supposed to be called before level loading
        public static void UpdateVanillaMetroTracks()
        {
            var vanillaMetroTrack = PrefabCollection<NetInfo>.FindLoaded("Metro Track");
            if (vanillaMetroTrack != null)
            {
                vanillaMetroTrack.m_availableIn = ItemClass.Availability.Editors;
            }
            var vanillaMetroStationTrack = PrefabCollection<NetInfo>.FindLoaded("Metro Station Track");
            if (vanillaMetroStationTrack != null)
            {
                vanillaMetroStationTrack.m_availableIn = ItemClass.Availability.Editors;
            }
        }

        private static void UpdateMetroTrainEffects()
        {
            var vanillaMetro = PrefabCollection<VehicleInfo>.FindLoaded("Metro");
            var arriveEffect = ((MetroTrainAI)vanillaMetro.m_vehicleAI).m_arriveEffect;
            for (uint i = 0; i < PrefabCollection<VehicleInfo>.LoadedCount(); i++)
            {
                try
                {
                    var info = PrefabCollection<VehicleInfo>.GetLoaded(i);
                    var metroTrainAI = info?.m_vehicleAI as MetroTrainAI;
                    if (metroTrainAI == null)
                    {
                        continue;
                    }
                    if (info.m_effects == null || info.m_effects.Length == 0)
                    {
                        info.m_effects = vanillaMetro.m_effects;
                    }
                    else
                    {
                        for (var j = 0; j < info.m_effects.Length; j++)
                        {
                            if (info.m_effects[j].m_effect?.name == "Train Movement")
                            {
                                info.m_effects[j] = vanillaMetro.m_effects[0];
                            }
                        }
                    }
                    var arriveEffectName = metroTrainAI.m_arriveEffect?.name;
                    if (arriveEffectName == null || arriveEffectName == "Transport Arrive")
                    {
                        metroTrainAI.m_arriveEffect = arriveEffect;
                    }
                }
                catch
                {
                    //swallow
                }
            }
        }
    }
}
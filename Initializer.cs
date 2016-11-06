using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.Globalization;
using MetroOverhaul.InitializationSteps;
using UnityEngine;
using MetroOverhaul.NEXT;
using MetroOverhaul.NEXT.Extensions;
using MetroOverhaul.OptionsFramework;

namespace MetroOverhaul
{
    public class Initializer : AbstractInitializer
    {

        protected override void InitializeImpl()
        {
            CreateTracks();
            AssetsUpdater.PreventVanillaMetroTrainSpawning();
        }

        private void CreateTracks()
        {
            var trainTrackInfo = FindOriginalNetInfo("Train Track");
            var elevatedInfo = FindOriginalNetInfo("Basic Road Elevated");
            var tunnelInfo = FindOriginalNetInfo("Train Track Tunnel");
            try
            {
                CreateFullPrefab(
                    ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
                        Chain(CustomizaionSteps.CommonConcreteCustomization).
                        Chain(CustomizaionSteps.ReplaceTrackIcon).
                        Chain(SetupMesh.Setup12mMesh, elevatedInfo, trainTrackInfo).
                        Chain(SetupMesh.Setup12mMeshBar, elevatedInfo).
                        Chain(SetupTexture.Setup12mTexture).
                        Chain((info, version) => { LoadingExtension.EnqueueLateBuildUpAction(() => { LateBuildUp.BuildUp(info, version); }); }),
                    NetInfoVersion.All, null, null, new Dictionary<NetInfoVersion, string> { { NetInfoVersion.Tunnel, "Metro Track" } }
                );
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("Exception happened when setting up concrete tracks");
                UnityEngine.Debug.LogException(e);
            }

            if (OptionsWrapper<Options>.Options.steelTracks)
            {
                try
                {
                    CreateFullPrefab(
                        ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
                            Chain(SetupSteelMesh.Setup12mSteelMesh, elevatedInfo, trainTrackInfo).
                            Chain(SetupSteelMesh.Setup12mSteelMeshBar, elevatedInfo).
                            Chain(SetupSteelTexture.Setup12mSteelTexture).
                            Chain((info, version) => { LoadingExtension.EnqueueLateBuildUpAction(() => { LateBuildUpSteel.BuildUp(info, version); }); }),
                        NetInfoVersion.All, null, prefabName => "Steel " + prefabName
                    );
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError("Exception happened when setting up steel tracks");
                    UnityEngine.Debug.LogException(e);
                }
            }

            if (OptionsWrapper<Options>.Options.concreteTracksNoBar)
            {
                try
                {
                    CreateFullPrefab(
                        ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
                            Chain(CustomizaionSteps.CommonConcreteCustomization).
                            Chain(CustomizaionSteps.CommonCustomizationNoBar).
                            Chain(SetupMesh.Setup12mMesh, elevatedInfo, trainTrackInfo).
                            Chain(SetupMesh.Setup12mMeshNoBar, elevatedInfo, trainTrackInfo).
                            Chain(SetupTexture.Setup12mTexture),
                        NetInfoVersion.Ground,
                        ActionExtensions.BeginChain<NetInfo, Action<NetInfo, NetInfoVersion>>().
                            Chain<NetInfo, Action<NetInfo, NetInfoVersion>, Func<string, string>, NetInfoVersion>(LinkToNonGroundVersions, null,
                            NetInfoVersion.Slope | NetInfoVersion.Tunnel | NetInfoVersion.Elevated | NetInfoVersion.Bridge)
                        , prefabName => prefabName + " NoBar"
                    );
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError("Exception happened when setting up nobar concrete tracks");
                    UnityEngine.Debug.LogException(e);
                }
            }

            if (OptionsWrapper<Options>.Options.steelTracksNoBar)
            {
                try
                {
                    CreateFullPrefab(
                        ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
                            Chain(CustomizaionSteps.CommonCustomizationNoBar).
                            Chain(SetupSteelMesh.Setup12mSteelMesh, elevatedInfo, trainTrackInfo).
                            Chain(SetupSteelMesh.Setup12mSteelMeshNoBar, elevatedInfo, trainTrackInfo).
                            Chain(SetupSteelTexture.Setup12mSteelTexture),
                        NetInfoVersion.Ground,
                        ActionExtensions.BeginChain<NetInfo, Action<NetInfo, NetInfoVersion>>().
                            Chain<NetInfo, Action<NetInfo, NetInfoVersion>, Func<string, string>, NetInfoVersion>(LinkToNonGroundVersions, prefabName => "Steel " + prefabName,
                            NetInfoVersion.Slope | NetInfoVersion.Tunnel | NetInfoVersion.Elevated | NetInfoVersion.Bridge)
                        , prefabName => "Steel " + prefabName + " NoBar"
                    );
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError("Exception happened when setting up nobar steel tracks");
                    UnityEngine.Debug.LogException(e);
                }
            }

            try
            {
                CreateFullStationPrefab(
                    ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
                        Chain(CustomizaionSteps.CommonConcreteCustomization).
                        Chain(SetupMesh.Setup12mMeshStation, elevatedInfo, tunnelInfo).
                        Chain(SetupTexture.Setup12mTexture), null, "Metro Station Track"
                );
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("Exception happened when setting up concrete station tracks");
                UnityEngine.Debug.LogException(e);
            }
        }

        protected void CreateFullPrefab(Action<NetInfo, NetInfoVersion> customizationStep,
            NetInfoVersion versions,
            Action<NetInfo, Action<NetInfo, NetInfoVersion>> setupOtherVersionsStep,
            Func<string, string> nameModifier = null, Dictionary<NetInfoVersion, string> replacements = null)
        {
            if (nameModifier == null)
            {
                nameModifier = s => s;
            }
            NetInfo groundVersion = null;
            if ((versions & NetInfoVersion.Ground) != 0)
            {
                string replaces = null;
                replacements?.TryGetValue(NetInfoVersion.Ground, out replaces);
                groundVersion = CreateNetInfo(nameModifier.Invoke("Metro Track Ground"), "Train Track",
                    ActionExtensions.BeginChain<NetInfo>().
                        Chain(ReplaceAI, NetInfoVersion.Ground).
                        Chain(SetupMetroTrackMeta, NetInfoVersion.Ground).
                        Chain(p =>
                        {
                            setupOtherVersionsStep?.Invoke(p, customizationStep);
                            p.GetComponent<TrainTrackAI>().m_connectedElevatedInfo = null;
                            p.GetComponent<TrainTrackAI>().m_connectedInfo = null;
                        }).
                        Chain(SetupTrackModel, customizationStep.Chain(SetCosts)), replaces
                );
            }
            foreach (var version in new[] { NetInfoVersion.Bridge, NetInfoVersion.Tunnel, NetInfoVersion.Elevated, NetInfoVersion.Slope, })
            {
                if ((versions & version) == 0)
                {
                    continue;
                }
                string replaces = null;
                replacements?.TryGetValue(version, out replaces);
                CreateNetInfo(nameModifier.Invoke("Metro Track " + version), "Train Track " + version,
                    ActionExtensions.BeginChain<NetInfo>().
                        Chain(ReplaceAI, version).
                        Chain(SetupMetroTrackMeta, version).
                        Chain(CommonSteps.SetVersion, groundVersion, version).
                        Chain(SetupTrackModel, customizationStep.Chain(SetCosts)), replaces
                    );
            }
        }

        protected void LinkToNonGroundVersions(NetInfo p, Action<NetInfo, NetInfoVersion> customizationStep,
            Func<string, string> nameModifier = null, NetInfoVersion versions = NetInfoVersion.Slope | NetInfoVersion.Tunnel | NetInfoVersion.Elevated | NetInfoVersion.Bridge)
        {
            if (nameModifier == null)
            {
                nameModifier = s => s;
            }
            foreach (var version in new[] { NetInfoVersion.Bridge, NetInfoVersion.Tunnel, NetInfoVersion.Elevated, NetInfoVersion.Slope, })
            {
                if ((versions & version) == 0)
                {
                    continue;
                }
                CommonSteps.SetVersion(FindCustomNetInfo(nameModifier.Invoke("Metro Track " + version)), p, version);
            }
        }

        //TODO(earalov): refactor like CreateFullPrefab()
        private void CreateFullStationPrefab(Action<NetInfo, NetInfoVersion> customizationStep, Func<string, string> nameModifier = null, string tunnelReplaces = "")
        {
            if (nameModifier == null)
            {
                nameModifier = s => s;
            }
            CreateNetInfo(nameModifier.Invoke("Metro Station Track Ground"), "Train Station Track",
                ActionExtensions.BeginChain<NetInfo>().
                Chain(ReplaceAI, NetInfoVersion.Ground).
                Chain(SetupMetroTrackMeta, NetInfoVersion.Ground).
                Chain(SetupStationTrack, NetInfoVersion.Ground).
                Chain(p =>
                {
                    //TODO(earalov): provide a track with narrow ped. lanes for Mr.Maison's stations
                    CreateNetInfo(nameModifier.Invoke("Metro Station Track Elevated"), "Train Station Track",
                        ActionExtensions.BeginChain<NetInfo>().
                        Chain(ReplaceAI, NetInfoVersion.Elevated).
                        Chain(SetupMetroTrackMeta, NetInfoVersion.Elevated).
                        Chain(CommonSteps.SetVersion, p, NetInfoVersion.Elevated).
                        Chain(SetupStationTrack, NetInfoVersion.Elevated).
                        Chain(SetupElevatedStationTrack).
                        Chain(SetupTrackModel, customizationStep)
                    );
                    CreateNetInfo(nameModifier.Invoke("Metro Station Track Tunnel"), "Train Station Track",
                        ActionExtensions.BeginChain<NetInfo>().
                        Chain(ReplaceAI, NetInfoVersion.Tunnel).
                        Chain(SetupMetroTrackMeta, NetInfoVersion.Tunnel).
                        Chain(CommonSteps.SetVersion, p, NetInfoVersion.Tunnel).
                        Chain(SetupStationTrack, NetInfoVersion.Tunnel).
                        Chain(SetupTunnelStationTrack).
                        Chain(SetupTrackModel, customizationStep),
                        tunnelReplaces
                    );
                    CreateNetInfo(nameModifier.Invoke("Metro Station Track Sunken"), "Train Station Track", //TODO(earalov): test. check if AI to be replaced with MetroTrackAI
                        ActionExtensions.BeginChain<NetInfo>().
                        Chain(ReplaceAI, NetInfoVersion.Ground).
                        Chain(SetupMetroTrackMeta, NetInfoVersion.Ground).
                        Chain(SetupStationTrack, NetInfoVersion.Ground).
                        Chain(SetupSunkenStationTrack).
                        Chain(SetupTrackModel, customizationStep)
                    );
                }).
                Chain(SetupTrackModel, customizationStep)
            );
        }

        private static void ReplaceAI(NetInfo prefab, NetInfoVersion version)
        {
            var originalAi = prefab.GetComponent<PlayerNetAI>(); //milestone, construction and maintenance costs to be overriden later
            var canModify = originalAi.CanModify();
            int noiseAccumulation;
            float noiseRadius;
            originalAi.GetNoiseAccumulation(out noiseAccumulation, out noiseRadius);
            if (originalAi is TrainTrackTunnelAI || version == NetInfoVersion.Tunnel)
            {
                if (originalAi is TrainTrackTunnelAI && version == NetInfoVersion.Slope)
                {
                    GameObject.DestroyImmediate(originalAi);
                    var ai = prefab.gameObject.AddComponent<TrainTrackTunnelAIMetro>();
                    ai.m_canModify = canModify;
                    ai.m_noiseAccumulation = noiseAccumulation;
                    ai.m_noiseRadius = noiseRadius;
                    ai.m_info = prefab;
                    prefab.m_netAI = ai;
                }
                else
                {
                    GameObject.DestroyImmediate(originalAi);
                    var ai = prefab.gameObject.AddComponent<MetroTrackAI>();
                    ai.m_info = prefab;
                    ai.m_transportInfo = PrefabCollection<TransportInfo>.FindLoaded("Train");
                    prefab.m_netAI = ai;
                }

            }
            else if (!(originalAi is TrainTrackBridgeAI) && (version == NetInfoVersion.Bridge || version == NetInfoVersion.Elevated))
            {
                GameObject.DestroyImmediate(originalAi);
                var ai = prefab.gameObject.AddComponent<TrainTrackBridgeAI>();
                ai.m_canModify = canModify;
                ai.m_noiseAccumulation = noiseAccumulation;
                ai.m_noiseRadius = noiseRadius;
                ai.m_info = prefab;
                prefab.m_netAI = ai;
            }
        }

        public static void SetupElevatedStationTrack(NetInfo prefab)
        {
        }

        public static void SetupSunkenStationTrack(NetInfo prefab)
        {
            prefab.m_maxHeight = -1;
            prefab.m_minHeight = -3;
            prefab.m_lowerTerrain = false;
            prefab.m_class.m_layer = ItemClass.Layer.MetroTunnels | ItemClass.Layer.Default;
        }

        public static void SetupTunnelStationTrack(NetInfo prefab)
        {
            prefab.m_maxHeight = -1;
            prefab.m_minHeight = -5;
            prefab.m_lowerTerrain = false;
            prefab.m_class.m_layer = ItemClass.Layer.MetroTunnels;
        }

        public static void SetupStationTrack(NetInfo prefab, NetInfoVersion version)
        {
            prefab.m_followTerrain = false;
            prefab.m_flattenTerrain = false;
            prefab.m_createGravel = false;
            prefab.m_createPavement = false;
            prefab.m_createRuining = false;
            prefab.m_requireSurfaceMaps = false;
            prefab.m_snapBuildingNodes = false;
            prefab.m_placementStyle = ItemClass.Placement.Procedural;
            prefab.m_useFixedHeight = true;
            prefab.m_availableIn = ItemClass.Availability.Game;
            prefab.m_intersectClass = null;
            if (version == NetInfoVersion.Ground)
            {
                prefab.m_lowerTerrain = false;
                prefab.m_clipTerrain = true;
            }
            else
            {
                prefab.m_clipTerrain = false;
            }
        }

        public static NetInfoVersion DetectVersion(string infoName)
        {
            if (infoName.Contains("Elevated"))
            {
                return NetInfoVersion.Elevated;
            }
            if (infoName.Contains("Bridge"))
            {
                return NetInfoVersion.Bridge;
            }
            if (infoName.Contains("Slope"))
            {
                return NetInfoVersion.Slope;
            }
            return infoName.Contains("Tunnel") ? NetInfoVersion.Tunnel : NetInfoVersion.Ground;
        }

        private static void SetupTrackModel(NetInfo prefab, Action<NetInfo, NetInfoVersion> customizationStep)
        {
            const int defaultHalfWidth = 6;
            const float defaultPavementWidth = 3.5f;

            prefab.m_minHeight = 0; //TODO(earalov): is that minHeight correct for all types of tracks?
            var version = DetectVersion(prefab.name);
            switch (version)
            {
                case NetInfoVersion.Elevated:
                    prefab.m_halfWidth = defaultHalfWidth;
                    prefab.m_pavementWidth = 3;
                    break;
                case NetInfoVersion.Bridge:
                    prefab.m_halfWidth = 5.9999f;
                    prefab.m_pavementWidth = 3;
                    break;
                case NetInfoVersion.Slope:
                    prefab.m_halfWidth = defaultHalfWidth;
                    prefab.m_pavementWidth = defaultPavementWidth;
                    break;
                case NetInfoVersion.Tunnel:
                    prefab.m_pavementWidth = 4.8f;
                    prefab.m_halfWidth = 7.5f;
                    break;
                case NetInfoVersion.Ground:
                    prefab.m_halfWidth = defaultHalfWidth;
                    prefab.m_pavementWidth = defaultPavementWidth;
                    break;
            }
            customizationStep.Invoke(prefab, version);
        }

        private static void SetupMetroTrackMeta(NetInfo prefab, NetInfoVersion version)
        {
            var vanillaMetroTrack = FindOriginalNetInfo("Metro Track");
            var milestone = vanillaMetroTrack.GetComponent<PlayerNetAI>().m_createPassMilestone;
            prefab.GetComponent<PlayerNetAI>().m_createPassMilestone = milestone;
            prefab.m_class = ScriptableObject.CreateInstance<ItemClass>();
            prefab.m_class.m_subService = ItemClass.SubService.PublicTransportMetro;
            prefab.m_class.m_layer = ItemClass.Layer.MetroTunnels;
            prefab.m_class.m_service = ItemClass.Service.PublicTransport;
            prefab.m_class.m_level = ItemClass.Level.Level1;
            prefab.m_UIPriority = vanillaMetroTrack.m_UIPriority;
            prefab.SetUICategory("PublicTransportMetro");
            if (version == NetInfoVersion.Tunnel)
            {
                prefab.m_class.m_layer = ItemClass.Layer.MetroTunnels;
                prefab.m_setVehicleFlags = Vehicle.Flags.Transition | Vehicle.Flags.Underground;
                prefab.m_setCitizenFlags = CitizenInstance.Flags.Transition | CitizenInstance.Flags.Underground;
            }
            else
            {
                prefab.m_class.m_layer = ItemClass.Layer.Default;
            }
            prefab.m_class.hideFlags = HideFlags.None;
            prefab.m_class.name = prefab.name;
            prefab.m_maxBuildAngle = 90;
            prefab.m_maxTurnAngleCos = Mathf.Cos(prefab.m_maxBuildAngle);
            prefab.m_maxTurnAngle = 60;
            prefab.m_maxTurnAngleCos = Mathf.Cos(prefab.m_maxTurnAngle);
            prefab.m_averageVehicleLaneSpeed = vanillaMetroTrack.m_averageVehicleLaneSpeed;
            prefab.m_UnlockMilestone = vanillaMetroTrack.m_UnlockMilestone;
            prefab.m_createGravel = false;
            prefab.m_createPavement = false;
            prefab.m_isCustomContent = true; //this line is responsible for moving tracks to the end of the list and that's not what we're interested in  

            var speedLimit = vanillaMetroTrack.m_lanes.First(l => l.m_vehicleType != VehicleInfo.VehicleType.None).m_speedLimit;

            foreach (var lane in prefab.m_lanes)
            {
                if (lane.m_vehicleType == VehicleInfo.VehicleType.None)
                {
                    lane.m_stopType = VehicleInfo.VehicleType.Metro;
                }
                else
                {
                    lane.m_vehicleType = VehicleInfo.VehicleType.Metro;
                    lane.m_speedLimit = speedLimit;
                }
            }

            Modifiers.RemoveElectricityPoles(prefab);
        }

        public static void SetCosts(PrefabInfo newPrefab, NetInfoVersion version)
        {
            var metroTrackInfo = FindOriginalNetInfo("Metro Track");
            var baseConstructionCost = metroTrackInfo.GetComponent<PlayerNetAI>().m_constructionCost;
            var baseMaintenanceCost = metroTrackInfo.GetComponent<PlayerNetAI>().m_maintenanceCost;
            var newAi = newPrefab.GetComponent<PlayerNetAI>();

            var multiplier = GetCostMultiplier(version);
            newAi.m_constructionCost = (int)(baseConstructionCost * multiplier);
            newAi.m_maintenanceCost = (int)(baseMaintenanceCost * multiplier);
        }

        public static float GetCostMultiplier(NetInfoVersion version)
        {
            float multiplier;
            switch (version)
            {
                case NetInfoVersion.Elevated:
                    multiplier = 3f;
                    break;
                case NetInfoVersion.Bridge:
                    multiplier = 4.5f;
                    break;
                case NetInfoVersion.Tunnel:
                case NetInfoVersion.Slope:
                    multiplier = 9f;
                    break;
                default:
                    multiplier = 1f;
                    break;
            }
            return multiplier;
        }
    }
}

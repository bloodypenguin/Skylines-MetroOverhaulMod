using System;
using System.Linq;
using MetroOverhaul.InitializationSteps;
using UnityEngine;
using MetroOverhaul.NEXT;
using MetroOverhaul.NEXT.Extensions;

namespace MetroOverhaul
{
    public class Initializer : AbstractInitializer
    {
        protected override void InitializeImpl()
        {
            CreatePillars();
            CreateTracks();
        }

        private void CreateTracks()
        {
            var trainTrackInfo = FindOriginalNetInfo("Train Track");
            var elevatedInfo = FindOriginalNetInfo("Basic Road Elevated");
            var tunnelInfo = FindOriginalNetInfo("Train Track Tunnel");
            CreateFullPrefab(
                ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
                    Chain(CustomizaionSteps.CommonConcreteCustomization).
                    Chain(SetupMesh.Setup12mMesh, elevatedInfo, trainTrackInfo).
                    Chain(SetupMesh.Setup12mMeshNonAlt, elevatedInfo).
                    Chain(SetupTexture.Setup12mTexture).
                    Chain((info, version) => { LoadingExtension.EnqueueLateBuildUpAction(() => { LateBuildUp.BuildUp(info, version); }); })
                );
            CreateFullPrefab(
                ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
                    Chain(SetupSteelMesh.Setup12mSteelMesh, elevatedInfo, trainTrackInfo).
                    Chain(SetupSteelMesh.Setup12mSteelMeshNonAlt, elevatedInfo).
                    Chain(SetupSteelTexture.Setup12mSteelTexture).
                    Chain((info, version) => { LoadingExtension.EnqueueLateBuildUpAction(() => { LateBuildUpSteel.BuildUp(info, version); }); })
                , prefabName => "Steel " + prefabName
                );
            //TODO(earalov): provide alternative tracks
            //            CreateFullPrefab(
            //                ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
            //                Chain(CustomizaionSteps.CommonConcreteCustomization).
            //                Chain(CustomizaionSteps.CommonCustomizationAlt).
            //                Chain(SetupMesh.Setup12mMesh, elevatedInfo, trainTrackInfo).
            //                Chain(SetupMesh.Setup12mMeshAlt, elevatedInfo, trainTrackInfo).
            //                Chain(SetupTexture.Setup12mTexture)
            //                , prefabName => prefabName + "Alt"
            //            );
            //            CreateFullPrefab(
            //                ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
            //                Chain(CustomizaionSteps.CommonCustomizationAlt).
            //                Chain(SetupSteelMesh.Setup12mSteelMesh, elevatedInfo, trainTrackInfo).
            //                Chain(SetupSteelMesh.Setup12mSteelMeshAlt, elevatedInfo, trainTrackInfo).
            //                Chain(SetupSteelTexture.Setup12mSteelTexture)
            //                , prefabName => "Steel " + prefabName + "Alt"
            //            );

            CreateFullStationPrefab(
                ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
                    Chain(CustomizaionSteps.CommonConcreteCustomization).
                    Chain(SetupMesh.Setup12mMeshStation, tunnelInfo).
                    Chain(SetupTexture.Setup12mTexture)
                );
        }

        private void CreatePillars()
        {
            CreatePillarPrefab( //TODO(earalov): learn how to do that earlier. It's too late when level is already loaded
                pillar => { LoadingExtension.EnqueueLateBuildUpAction(() => { SetupElevatedPillar.SetMeshAndTexture(pillar); }); },
                pillar => { LoadingExtension.EnqueueLateBuildUpAction(() => { SetupBridgePillar.SetMeshAndTexture(pillar); }); }
                );
            CreatePillarPrefab( //TODO(earalov): learn how to do that earlier. It's too late when level is already loaded
                pillar => { LoadingExtension.EnqueueLateBuildUpAction(() => { SetupSteelElevatedPillar.SetMeshAndTexture(pillar); }); },
                pillar => { LoadingExtension.EnqueueLateBuildUpAction(() => { SetupSteelBridgePillar.SetMeshAndTexture(pillar); }); },
                prefabName => "Steel " + prefabName
                );
        }


        protected void CreateFullPrefab(Action<NetInfo, NetInfoVersion> customizationStep, Func<string, string> nameModifier = null)
        {
            if (nameModifier == null)
            {
                nameModifier = s => s;
            }
            CreateNetInfo(nameModifier.Invoke("Metro Track Ground"), "Train Track",
                ActionExtensions.BeginChain<NetInfo>().
                Chain(SetupMetroTrackMeta).
                Chain(SetCosts, "Train Track").
                Chain(p =>
                {
                    CreateNetInfo(nameModifier.Invoke("Metro Track Bridge"), "Train Track Bridge",
                        ActionExtensions.BeginChain<NetInfo>().
                        Chain(SetupMetroTrackMeta).
                        Chain(CommonSteps.SetBridge, p).
                        Chain(SetCosts, "Train Track Bridge").
                        Chain(SetupTrackModel, customizationStep)
                    );
                    CreateNetInfo(nameModifier.Invoke("Metro Track Elevated"), "Train Track Elevated",
                        ActionExtensions.BeginChain<NetInfo>().
                        Chain(SetupMetroTrackMeta).
                        Chain(CommonSteps.SetElevated, p).
                        Chain(SetCosts, "Train Track Elevated").
                        Chain(SetupTrackModel, customizationStep)
                    );
                    CreateNetInfo(nameModifier.Invoke("Metro Track Slope"), "Train Track Slope",
                        ActionExtensions.BeginChain<NetInfo>().
                        Chain(SetupMetroTrackMeta).
                        Chain(CommonSteps.SetSlope, p).
                        Chain(SetCosts, "Train Track Slope").
                        Chain(SetupTrackModel, customizationStep)
                    );
                    CreateNetInfo(nameModifier.Invoke("Metro Track Tunnel"), "Train Track Tunnel",
                        ActionExtensions.BeginChain<NetInfo>().
                        Chain(SetupMetroTrackMeta).
                        Chain(CommonSteps.SetTunnel, p).
                        Chain(SetCosts, "Train Track Tunnel").
                        Chain(SetupTrackModel, customizationStep)
                    );
                    //TODO(earalov): why can't we just set needed meshes etc. for vanilla track?
                    p.GetComponent<TrainTrackAI>().m_connectedElevatedInfo = null;
                    p.GetComponent<TrainTrackAI>().m_connectedInfo = null;
                }).
                Chain(SetupTrackModel, customizationStep));

        }

        private void CreateFullStationPrefab(Action<NetInfo, NetInfoVersion> customizationStep, Func<string, string> nameModifier = null)
        {
            if (nameModifier == null)
            {
                nameModifier = s => s;
            }
            CreateNetInfo(nameModifier.Invoke("Metro Station Track Ground"), "Train Station Track",
                ActionExtensions.BeginChain<NetInfo>().
                Chain(SetupMetroTrackMeta).
                Chain(SetupStationTrack).
                Chain(p =>
                {
                    //TODO(earalov): provide a track with narrow ped. lanes for Mr.Maison's stations
                    CreateNetInfo(nameModifier.Invoke("Metro Station Track Elevated"), "Train Station Track",
                        ActionExtensions.BeginChain<NetInfo>().
                        Chain(SetupMetroTrackMeta).
                        Chain(CommonSteps.SetElevated, p).
                        Chain(SetupStationTrack).
                        Chain(SetupElevatedStationTrack).
                        Chain(SetupTrackModel, customizationStep)
                    );
                    CreateNetInfo(nameModifier.Invoke("Metro Station Track Tunnel"), "Train Station Track",
                        ActionExtensions.BeginChain<NetInfo>().
                        Chain(SetupMetroTrackMeta).
                        Chain(CommonSteps.SetTunnel, p).
                        Chain(SetupStationTrack).
                        Chain(SetupTunnelStationTrack).
                        Chain(SetupTrackModel, customizationStep)
                    );
                    CreateNetInfo(nameModifier.Invoke("Metro Station Track Sunken"), "Train Station Track",
                        ActionExtensions.BeginChain<NetInfo>().
                        Chain(SetupMetroTrackMeta).
                        Chain(SetupStationTrack).
                        Chain(SetupSunkenStationTrack).
                        Chain(SetupTrackModel, customizationStep)
                    );
                }).
                Chain(SetupTrackModel, customizationStep)
            );
        }

        private void CreatePillarPrefab(Action<BuildingInfo> customizationStepElevated, Action<BuildingInfo> customizationStepBridge,
            Func<string, string> nameModifier = null)
        {
            if (nameModifier == null)
            {
                nameModifier = s => s;
            }
            CreateBuildingInfo(nameModifier.Invoke("Metro Elevated Pillar"), "RailwayElevatedPillar",
                ActionExtensions.BeginChain<BuildingInfo>()
                .Chain(SetupPillar)
                .Chain(customizationStepElevated)
            );
            CreateBuildingInfo(nameModifier.Invoke("Metro Bridge Pillar"), "RailwayBridgePillar",
                ActionExtensions.BeginChain<BuildingInfo>()
                .Chain(SetupPillar)
                .Chain(customizationStepBridge)
            );
        }

        private static void SetupPillar(BuildingInfo prefab)
        {
            prefab.m_collisionHeight = -1;
        }

        public static void SetupElevatedStationTrack(NetInfo prefab)
        {
            var trackAi = prefab.GetComponent<TrainTrackAI>();
            trackAi.m_elevatedInfo = prefab;
        }

        public static void SetupSunkenStationTrack(NetInfo prefab)
        {
            var trackAi = prefab.GetComponent<TrainTrackAI>();
            trackAi.m_tunnelInfo = prefab;
            prefab.m_maxHeight = -1;
            prefab.m_minHeight = -3;
            prefab.m_lowerTerrain = false;
            prefab.m_class.m_layer = ItemClass.Layer.MetroTunnels | ItemClass.Layer.Default;
        }

        public static void SetupTunnelStationTrack(NetInfo prefab)
        {
            var trackAi = prefab.GetComponent<TrainTrackAI>();
            trackAi.m_tunnelInfo = prefab;
            prefab.m_maxHeight = -1;
            prefab.m_minHeight = -5;
            prefab.m_lowerTerrain = false;
            prefab.m_class.m_layer = ItemClass.Layer.MetroTunnels; ;
        }

        public static void SetupStationTrack(NetInfo prefab)
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
            var prefabNameParts = prefab.name.Split(' ');
            if (prefabNameParts.Last() == "Ground")
            {
                prefab.m_lowerTerrain = false;
                prefab.m_clipTerrain = true;
            }
            else
            {
                prefab.m_clipTerrain = false;
            }
        }

        private static void SetupTrackModel(NetInfo prefab, Action<NetInfo, NetInfoVersion> customizationStep)
        {
            const int defaultHalfWidth = 6;
            const float defaultPavementWidth = 3.5f;

            prefab.m_minHeight = 0;

            var prefabNameParts = prefab.name.Split(' ');
            NetInfoVersion version;
            switch (prefabNameParts.Last())
            {
                case "Elevated":
                    version = NetInfoVersion.Elevated;
                    prefab.m_halfWidth = defaultHalfWidth;
                    prefab.m_pavementWidth = 3;
                    break;
                case "Bridge":
                    version = NetInfoVersion.Bridge;
                    prefab.m_halfWidth = 5.9999f;
                    prefab.m_pavementWidth = 3;
                    break;
                case "Slope":
                    version = NetInfoVersion.Slope;
                    prefab.m_halfWidth = defaultHalfWidth;
                    prefab.m_pavementWidth = defaultPavementWidth;
                    break;
                case "Tunnel":
                    version = NetInfoVersion.Tunnel;
                    prefab.m_pavementWidth = 4.8f;
                    prefab.m_halfWidth = 7.5f;
                    break;
                default:
                    version = NetInfoVersion.Ground;
                    prefab.m_halfWidth = defaultHalfWidth;
                    prefab.m_pavementWidth = defaultPavementWidth;
                    break;
            }
            customizationStep.Invoke(prefab, version);
        }

        private static void SetupMetroTrackMeta(NetInfo prefab)
        {
            var vanillaMetroTrack = FindOriginalNetInfo("Metro Track");
            var milestone = vanillaMetroTrack.GetComponent<MetroTrackAI>().m_createPassMilestone;
            PrefabCollection<VehicleInfo>.FindLoaded("Metro").m_class =
                ScriptableObject.CreateInstance<ItemClass>();
            prefab.GetComponent<TrainTrackBaseAI>().m_createPassMilestone = milestone;
            prefab.m_class = ScriptableObject.CreateInstance<ItemClass>();
            prefab.m_class.m_subService = ItemClass.SubService.PublicTransportMetro;
            prefab.m_class.m_layer = ItemClass.Layer.MetroTunnels;
            prefab.m_class.m_service = ItemClass.Service.PublicTransport;
            prefab.m_class.m_level = ItemClass.Level.Level1;
            prefab.m_UIPriority = vanillaMetroTrack.m_UIPriority;
            prefab.SetUICategory("PublicTransportMetro");
            if (prefab.name.Contains("Tunnel"))
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
            prefab.m_isCustomContent = true;


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

        private static void SetCosts(PrefabInfo newPrefab, string originalPrefabName)
        {
            var originalPrefab = FindOriginalNetInfo(originalPrefabName);
            var trainTrackTunnel = FindOriginalNetInfo("Train Track Tunnel");
            var metroTrack = FindOriginalNetInfo("Metro Track");

            var constructionCost = originalPrefab.GetComponent<PlayerNetAI>().m_constructionCost *
                                        metroTrack.GetComponent<PlayerNetAI>().m_constructionCost /
                                        trainTrackTunnel.GetComponent<PlayerNetAI>().m_constructionCost;
            newPrefab.GetComponent<PlayerNetAI>().m_constructionCost = constructionCost;
            var maintenanceCost = originalPrefab.GetComponent<PlayerNetAI>().m_maintenanceCost *
                        metroTrack.GetComponent<PlayerNetAI>().m_maintenanceCost /
                        trainTrackTunnel.GetComponent<PlayerNetAI>().m_maintenanceCost;
            newPrefab.GetComponent<PlayerNetAI>().m_maintenanceCost = maintenanceCost;
        }
    }
}

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
            var trainTrackInfo = FindOriginalNetInfo("Train Track");
            var elevatedInfo = FindOriginalNetInfo("Basic Road Elevated");

            CreateFullPrefab(
                ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
                Chain(CommonConcreteCustomization, elevatedInfo, trainTrackInfo).
                Chain(SetupMesh.Setup12mMeshNonAlt, elevatedInfo).
                Chain(SetupTexture.Setup12mTexture)
            );
            CreateFullPrefab(
                ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
                Chain(SetupSteelMesh.Setup12mSteelMesh, elevatedInfo, trainTrackInfo).
                Chain(SetupSteelMesh.Setup12mSteelMeshNonAlt, elevatedInfo).
                Chain(SetupSteelTexture.Setup12mSteelTexture)
                , prefabName => "Steel " + prefabName
            );

            CreateFullPrefab(
                ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
                Chain(CommonConcreteCustomization, elevatedInfo, trainTrackInfo).
                Chain(CommonCustomizationAlt).
                Chain(SetupMesh.Setup12mMeshAlt, elevatedInfo, trainTrackInfo).
                Chain(SetupTexture.Setup12mTexture)
                , prefabName => prefabName + "Alt"
            );
            CreateFullPrefab(
                ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
                Chain(SetupSteelMesh.Setup12mSteelMesh, elevatedInfo, trainTrackInfo).
                Chain(CommonCustomizationAlt).
                Chain(SetupSteelMesh.Setup12mSteelMeshAlt, elevatedInfo, trainTrackInfo).
                Chain(SetupSteelTexture.Setup12mSteelTexture)
                , prefabName => "Steel " + prefabName + "Alt"
            );

            CreateFullStationPrefab(
                ActionExtensions.BeginChain<NetInfo, NetInfoVersion>().
                Chain(CommonConcreteCustomization, elevatedInfo, trainTrackInfo).
                Chain(SetupMesh.Setup12mMeshNonAlt, elevatedInfo).
                Chain(SetupTexture.Setup12mTexture)
            );

            CreatePillarPrefab(prefabName => "Steel " + prefabName);
            CreatePillarPrefab();
        }

        private static void CommonConcreteCustomization(NetInfo prefab, NetInfoVersion version, NetInfo elevatedInfo, NetInfo trainTrackInfo)
        {
            if (version == NetInfoVersion.Slope)
            {
                prefab.m_halfWidth = 7.5f;
                prefab.m_pavementWidth = 4.8f;
            }
            SetupMesh.Setup12mMesh(prefab, version, elevatedInfo, trainTrackInfo);
        }

        private static void CommonCustomizationAlt(NetInfo prefab, NetInfoVersion version)
        {
            if (version != NetInfoVersion.Ground)
            {
                return;
            }
            prefab.m_halfWidth = 5;
        }

        protected void CreateFullPrefab(Action<NetInfo, NetInfoVersion> customizationStep = null, Func<string, string> nameModifier = null)
        {
            if (nameModifier == null)
            {
                nameModifier = s => s;
            }
            CreateNetInfo(nameModifier.Invoke("Metro Track Ground"), "Train Track", SetupMetroTrackMeta().Chain(SetupTrackModel(customizationStep)).Chain(p =>
            {
                CreateNetInfo(nameModifier.Invoke("Metro Track Bridge"), "Train Track Bridge",
                    SetupMetroTrackMeta().Chain(SetupTrackModel(customizationStep)).Chain(CommonSteps.SetBridge(p)).Chain(SetCosts("Train Track Bridge")));
                CreateNetInfo(nameModifier.Invoke("Metro Track Elevated"), "Train Track Elevated",
                    SetupMetroTrackMeta().Chain(SetupTrackModel(customizationStep)).Chain(CommonSteps.SetElevated(p)).Chain(SetCosts("Train Track Elevated")));
                CreateNetInfo(nameModifier.Invoke("Metro Track Slope"), "Train Track Slope",
                    SetupMetroTrackMeta().Chain(SetupTrackModel(customizationStep)).Chain(CommonSteps.SetSlope(p)).Chain(SetCosts("Train Track Slope")));
                CreateNetInfo(nameModifier.Invoke("Metro Track Tunnel"), "Train Track Tunnel",
                    SetupMetroTrackMeta().Chain(SetupTrackModel(customizationStep)).Chain(CommonSteps.SetTunnel(p)).Chain(SetCosts("Train Track Tunnel"))); //TODO(earalov): why can't we just set needed meshes etc. for vanilla track?
                p.GetComponent<TrainTrackAI>().m_connectedElevatedInfo = null;
                p.GetComponent<TrainTrackAI>().m_connectedInfo = null;
            }).Chain(SetCosts("Train Track")));
        }

        private void CreateFullStationPrefab(Action<NetInfo, NetInfoVersion> customizationStep = null, Func<string, string> nameModifier = null)
        {
            if (nameModifier == null)
            {
                nameModifier = s => s;
            }
            CreateNetInfo(nameModifier.Invoke("Metro Station Track Ground"), "Train Station Track", SetupMetroTrackMeta().Chain(SetupTrackModel(customizationStep)).Chain(SetupStationTrack).Chain(p =>
            {
                CreateNetInfo(nameModifier.Invoke("Metro Station Track Elevated"), "Train Station Track",
                    SetupMetroTrackMeta().Chain(SetupTrackModel(customizationStep)).Chain(CommonSteps.SetElevated(p)).Chain(SetupStationTrack).Chain(SetupElevatedStationTrack));
                CreateNetInfo(nameModifier.Invoke("Metro Station Track Tunnel"), "Train Station Track",
                    SetupMetroTrackMeta().Chain(SetupTrackModel(customizationStep)).Chain(CommonSteps.SetTunnel(p)).Chain(SetupStationTrack).Chain(SetupTunnelStationTrack));
                CreateNetInfo(nameModifier.Invoke("Metro Station Track Sunken"), "Train Station Track",
                    SetupMetroTrackMeta().Chain(SetupTrackModel(customizationStep)).Chain(SetupStationTrack).Chain(SetupSunkenStationTrack));
            }));
        }

        private void CreatePillarPrefab(Func<string, string> nameModifier = null)
        {
            if (nameModifier == null)
            {
                nameModifier = s => s;
            }
            CreateBuildingInfo(nameModifier.Invoke("Metro Elevated Pillar"), "RailwayElevatedPillar", SetupPillar);
            CreateBuildingInfo(nameModifier.Invoke("Metro Bridge Pillar"), "RailwayBridgePillar", SetupPillar);
        }

        private static void SetupPillar(BuildingInfo prefab)
        {
            prefab.m_collisionHeight = -1;
        }

        public static void SetupElevatedStationTrack(NetInfo prefab)
        {
            var trackAi = prefab.GetComponent<TrainTrackAI>();
            trackAi.m_elevatedInfo = prefab;
            var segment0 = prefab.m_segments[0].ShallowClone();
            var segment1 = prefab.m_segments[1];
            var node0 = prefab.m_nodes[0].ShallowClone();
            var node1 = prefab.m_nodes[1];

            segment0
                .SetMeshes
                    (@"Meshes\Elevated_Station_Pavement.obj",
                    @"Meshes\Elevated_Station_Pavement_LOD.obj");
            node0
                .SetMeshes
                    (@"Meshes\Elevated_Station_Node_Pavement.obj",
                    @"Meshes\Elevated_Node_Pavement_LOD.obj");

            prefab.m_segments = new[] { segment0, segment1 };
            prefab.m_nodes = new[] { node0, node1 };
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
            var tunnelInfo = PrefabCollection<NetInfo>.FindLoaded("Train Track Tunnel");
            var segment0 = tunnelInfo.m_segments[0];
            var segment1 = prefab.m_segments[1].ShallowClone();
            var segment2 = prefab.m_segments[2];
            var node0 = tunnelInfo.m_nodes[0];
            var node1 = prefab.m_nodes[1].ShallowClone();
            var node2 = prefab.m_nodes[2];

            segment1
                .SetMeshes
                (@"Meshes\Tunnel_Station_Pavement.obj",
                @"Meshes\Ground_NoBar_Pavement_LOD.obj");
            node1
                .SetMeshes
                (@"Meshes\Tunnel_Station_Node_Pavement.obj",
                @"Meshes\Tunnel_Node_Pavement_LOD.obj");
            prefab.m_segments = new[] { segment0, segment1, segment2 };
            prefab.m_nodes = new[] { node0, node1, node2 };

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
                var segment0 = prefab.m_segments[0].ShallowClone(); ;
                var segment1 = prefab.m_segments[1];
                var node0 = prefab.m_nodes[0].ShallowClone(); ;
                var node1 = prefab.m_nodes[1];
                var node2 = prefab.m_nodes[2];
                var node3 = prefab.m_nodes[3];

                segment0
                    .SetMeshes
                        (@"Meshes\Ground_Station_Pavement.obj",
                        @"Meshes\Ground_NoBar_Pavement_LOD.obj");
                node0
                    .SetMeshes
                    (@"Meshes\Ground_NoBar_Node_Pavement.obj",
                    @"Meshes\Ground_NoBar_Node_Pavement_LOD.obj")
                    .SetConsistentUVs(true);

                prefab.m_segments = new[] { segment0, segment1 };
                prefab.m_nodes = new[] { node0, node1, node2, node3 };
            }
            else
            {
                prefab.m_clipTerrain = false;
            }
        }

        private static Action<NetInfo> SetupTrackModel(Action<NetInfo, NetInfoVersion> customizationStep)
        {
            return prefab =>
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
            };
        }

        private static Action<NetInfo> SetupMetroTrackMeta()
        {
            return prefab =>
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
            };
        }

        private static Action<NetInfo> SetCosts(string originalPrefabName)
        {
            return newPrefab =>
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

            };
        }

    }
}

using System;
using System.Linq;
using UnityEngine;
using MetroOverhaul.SetupPrefab;
using MetroOverhaul.NEXT;
using MetroOverhaul.NEXT.Extensions;

namespace MetroOverhaul
{
    public class Initializer : AbstractInitializer
    {
        protected override void InitializeImpl()
        {
            try
            {
                CreateFullPrefab("");
                //CreateFullPrefab("", true);
                CreateFullStationPrefab("");
            }
            catch (Exception ex)
            {
                Next.Debug.Log(ex);
            }
        }
        protected void CreateFullPrefab(string alias, bool isAlt = false)
        {
            var altText = "";
            if (isAlt)
                altText = "Alt ";

            var mTunnelInfo = FindOriginalPrefab("Metro Track");
            CreatePrefab($"{alias}Metro Track {altText}Ground", "Train Track", SetupMetroTrack().Apply(mTunnelInfo).Chain(p =>
            {
                CreatePrefab($"{alias}Metro Track {altText}Bridge", "Train Track Bridge",
                    SetupMetroTrack(isAlt).Apply(mTunnelInfo).Chain(p1 => p.GetComponent<TrainTrackAI>().m_bridgeInfo = p1).Chain(SetCosts().Apply("Train Track Bridge")));
                CreatePrefab($"{alias}Metro Track {altText}Elevated", "Train Track Elevated",
                    SetupMetroTrack(isAlt).Apply(mTunnelInfo).Chain(p2 => p.GetComponent<TrainTrackAI>().m_elevatedInfo = p2).Chain(SetCosts().Apply("Train Track Elevated")));
                CreatePrefab($"{alias}Metro Track {altText}Slope", "Train Track Slope",
                    SetupMetroTrack(isAlt).Apply(mTunnelInfo).Chain(p3 => p.GetComponent<TrainTrackAI>().m_slopeInfo = p3).Chain(SetCosts().Apply("Train Track Slope")));
                CreatePrefab($"{alias}Metro Track {altText}Tunnel", "Train Track Tunnel",
                    SetupMetroTrack(isAlt).Apply(mTunnelInfo).Chain(p4 => p.GetComponent<TrainTrackAI>().m_tunnelInfo = p4).Chain(SetCosts().Apply("Train Track Tunnel"))); //TODO(earalov): why can't we just set needed meshes etc. for vanilla track?
                p.GetComponent<TrainTrackAI>().m_connectedElevatedInfo = null;
                p.GetComponent<TrainTrackAI>().m_connectedInfo = null;
            }).Chain(SetCosts().Apply("Train Track")));
        }

        private void CreateFullStationPrefab(string alias, bool isAlt = false)
        {
            var altText = "";
            if (isAlt)
                altText = "Alt ";

            var mTunnelInfo = FindOriginalPrefab("Metro Track");
            CreatePrefab($"{alias}Metro Station Track {altText}Ground", "Train Station Track", SetupMetroTrack(isAlt).Apply(mTunnelInfo).Chain(SetupStationTrack()).Chain(p =>
            {
                CreatePrefab($"{alias}Metro Station Track {altText}Elevated", "Train Station Track",
                    SetupMetroTrack(isAlt).Apply(mTunnelInfo).Chain(p1 => p.GetComponent<TrainTrackAI>().m_elevatedInfo = p1).Chain(SetupStationTrack()).Chain(SetupElevatedStationTrack()));
                CreatePrefab($"{alias}Metro Station Track {altText}Tunnel", "Train Station Track",
                    SetupMetroTrack(isAlt).Apply(mTunnelInfo).Chain(p2 => p.GetComponent<TrainTrackAI>().m_tunnelInfo = p2).Chain(SetupStationTrack()).Chain(SetupTunnelStationTrack()));
            }));
        }
        public static Action<NetInfo> SetupElevatedStationTrack()
        {
            return prefab =>
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
            };
        }

        public static Action<NetInfo> SetupSunkenStationTrack()
        {
            return prefab =>
            {
                var trackAi = prefab.GetComponent<TrainTrackAI>();
                trackAi.m_tunnelInfo = prefab;
                prefab.m_maxHeight = -1;
                prefab.m_minHeight = -3;
                prefab.m_lowerTerrain = false;
                prefab.m_class.m_layer = ItemClass.Layer.MetroTunnels | ItemClass.Layer.Default;
            };
        }

        public static Action<NetInfo> SetupTunnelStationTrack()
        {
            return prefab =>
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
            };
        }
        public static Action<NetInfo> SetupStationTrack()
        {
            return prefab =>
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

                    prefab.m_segments = new[] { segment0, segment1 };
                    prefab.m_nodes = new[] { node0, node1, node2, node3 };
               }
                else
                {
                    prefab.m_clipTerrain = false;
                } 
            };
        }

        private static Action<NetInfo, NetInfo> SetupMetroTrack(bool isAlt = false)
        {
            var elevatedInfo = FindOriginalPrefab("Basic Road Elevated");
            var metroInfo = FindOriginalPrefab("Metro Track");
            NetInfo trainTrackInfo = null;
            var version = NetInfoVersion.Ground;
            return (prefab, metroTunnel) =>
            {
                prefab.m_minHeight = 0;
                prefab.m_halfWidth = isAlt ? 5 : 6;
                prefab.m_pavementWidth = 3.5f;
                var prefabNameParts = prefab.name.Split(' ');
                switch (prefabNameParts.Last())
                {
                    case "Elevated":
                        version = NetInfoVersion.Elevated;
                        prefab.m_pavementWidth = 3;
                        break;
                    case "Bridge":
                        version = NetInfoVersion.Bridge;
                        prefab.m_halfWidth = 5.9999f;
                        prefab.m_pavementWidth = 3;
                        break;
                    case "Slope":
                        version = NetInfoVersion.Slope;
                        prefab.m_halfWidth = 7.5f;
                        prefab.m_pavementWidth = 4.8f;
                        break;
                    case "Tunnel":
                        version = NetInfoVersion.Tunnel;
                        prefab.m_pavementWidth = 4.8f;
                        trainTrackInfo = FindOriginalPrefab("Train Track");
                        prefab.m_halfWidth = 7.5f;
                        break;
                }

                SetupMesh.Setup12mMesh(prefab, version, elevatedInfo, trainTrackInfo, isAlt);
                SetupTexture.Setup12mTexture(prefab, version, isAlt);

                var milestone = metroTunnel.GetComponent<MetroTrackAI>().m_createPassMilestone;
                PrefabCollection<VehicleInfo>.FindLoaded("Metro").m_class =
                    ScriptableObject.CreateInstance<ItemClass>();
                prefab.GetComponent<TrainTrackBaseAI>().m_createPassMilestone = milestone;
                prefab.m_class = ScriptableObject.CreateInstance<ItemClass>();
                prefab.m_class.m_subService = ItemClass.SubService.PublicTransportMetro;
                prefab.m_class.m_layer = ItemClass.Layer.MetroTunnels;
                prefab.m_class.m_service = ItemClass.Service.PublicTransport;
                prefab.m_class.m_level = ItemClass.Level.Level1;
                prefab.m_UIPriority = metroInfo.m_UIPriority;
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
                prefab.m_averageVehicleLaneSpeed = metroTunnel.m_averageVehicleLaneSpeed;
                prefab.m_UnlockMilestone = metroTunnel.m_UnlockMilestone;
                prefab.m_createGravel = false;
                prefab.m_createPavement = false;
                prefab.m_isCustomContent = true;


                var speedLimit = metroTunnel.m_lanes.First(l => l.m_vehicleType != VehicleInfo.VehicleType.None).m_speedLimit;

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

        private static Action<NetInfo, string> SetCosts()
        {
            return (newPrefab, originalPrefabName) =>
            {
                var originalPrefab = FindOriginalPrefab(originalPrefabName);
                var trainTrackTunnel = FindOriginalPrefab("Train Track Tunnel");
                var metroTrack = FindOriginalPrefab("Metro Track");

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

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ElevatedTrainStationTrack
{
    public class Initializer : MonoBehaviour
    {
        private bool _isInitialized;
        private Dictionary<string, NetInfo> _customPrefabs;
        private static readonly Dictionary<string, NetInfo> OriginalPrefabs = new Dictionary<string, NetInfo>();

        public void Awake()
        {
            DontDestroyOnLoad(this);
            _customPrefabs = new Dictionary<string, NetInfo>();
            OriginalPrefabs.Clear();
        }

        public void OnLevelWasLoaded(int level)
        {
            if (level == 6)
            {
                _customPrefabs.Clear();
                OriginalPrefabs.Clear();
                _isInitialized = false;
            }
        }

        public void Update()
        {
            if (_isInitialized)
            {
                return;
            }
            try
            {
                GameObject.Find("Public Transport").GetComponent<NetCollection>();
            }
            catch (Exception)
            {
                return;
            }
            Loading.QueueLoadingAction(Loading.ActionWrapper(InitializeImpl));
            _isInitialized = true;
        }

        private void InitializeImpl()
        {
            CreatePrefab("Station Track Eleva", "Train Station Track", SetupElevatedPrefab);
            CreatePrefab("Station Track Tunnel", "Train Station Track", SetupTunnelPrefab);
            CreatePrefab("Station Track Sunken", "Train Station Track", SetupSunkenPrefab);
            var oneWay = CreatePrefab("Oneway Train Track", "Train Track", SetupOneWayPrefab);
            var oneWayTunnel = CreatePrefab("Oneway Train Track Tunnel", "Train Track Tunnel", SetupOneWayPrefab);
            var oneWayBridge = CreatePrefab("Oneway Train Track Bridge", "Train Track Bridge", SetupOneWayPrefab);
            var oneWayElevated = CreatePrefab("Oneway Train Track Elevated", "Train Track Elevated", SetupOneWayPrefab);
            var oneWaySlope = CreatePrefab("Oneway Train Track Slope", "Train Track Slope", SetupOneWayPrefab);
            if (oneWay != null)
            {
                var ai = oneWay.GetComponent<TrainTrackAI>();
                ai.m_tunnelInfo = oneWayTunnel;
                ai.m_bridgeInfo = oneWayBridge;
                ai.m_elevatedInfo = oneWayElevated;
                ai.m_slopeInfo = oneWaySlope;
            }
            PrefabCollection<NetInfo>.InitializePrefabs("Rail Extensions", _customPrefabs.Values.ToArray(), null);
        }

        private NetInfo CreatePrefab(string newPrefabName, string originalPrefabName, Action<NetInfo, NetInfo> setupAction)
        {
            var originalPrefab = FindOriginalPrefab(originalPrefabName);

            if (originalPrefab == null)
            {
                Debug.LogErrorFormat("ElevatedTrainStationTrack - Prefab '{0}' not found (required for '{1}')", originalPrefabName, newPrefabName);
                return null;
            }
            if (_customPrefabs.ContainsKey(newPrefabName))
            {
                return _customPrefabs[newPrefabName];
            }
            var newPrefab = Util.ClonePrefab(originalPrefab, newPrefabName, transform);
            if (newPrefab != null)
            {
                setupAction.Invoke(newPrefab, originalPrefab);
                _customPrefabs.Add(newPrefabName, newPrefab);
                return newPrefab;
            }
            else
            {
                Debug.LogErrorFormat("ElevatedTrainStationTrack - Couldn't make prefab '{0}'", newPrefabName);
            }
            return null;
        }

        private static NetInfo FindOriginalPrefab(string originalPrefabName)
        {
            NetInfo foundPrefab;
            if (OriginalPrefabs.TryGetValue(originalPrefabName, out foundPrefab))
            {
                return foundPrefab;
            }
            foundPrefab = Resources.FindObjectsOfTypeAll<NetInfo>().
            FirstOrDefault(netInfo => netInfo.name == originalPrefabName);
            OriginalPrefabs.Add(originalPrefabName, foundPrefab);
            return foundPrefab;
        }


        private static void SetupElevatedPrefab(NetInfo elevatedPrefab, NetInfo originalPrefab)
        {
            var stationAI = elevatedPrefab.GetComponent<TrainTrackAI>();
            stationAI.m_elevatedInfo = elevatedPrefab;

            elevatedPrefab.m_followTerrain = false;
            elevatedPrefab.m_flattenTerrain = false;
            elevatedPrefab.m_createGravel = false;
            elevatedPrefab.m_createPavement = false;
            elevatedPrefab.m_createRuining = false;
            elevatedPrefab.m_requireSurfaceMaps = false;
            elevatedPrefab.m_clipTerrain = false;
            elevatedPrefab.m_snapBuildingNodes = false;
            elevatedPrefab.m_placementStyle = ItemClass.Placement.Procedural;
            elevatedPrefab.m_useFixedHeight = true;
            elevatedPrefab.m_lowerTerrain = true;
            elevatedPrefab.m_availableIn = ItemClass.Availability.GameAndAsset;
            foreach (var lane in elevatedPrefab.m_lanes)
            {
                var mLaneProps = lane.m_laneProps;
                if (mLaneProps == null)
                {
                    continue;
                }
                var props = mLaneProps.m_props;
                if (props == null)
                {
                    continue;
                }
                lane.m_laneProps = new NetLaneProps
                {
                    m_props = (from prop in props
                               where prop != null
                               let mProp = prop.m_prop
                               where mProp != null
                               where mProp.name != "RailwayPowerline"
                               select prop).ToArray()
                };
            }
            var elevatedTrack = FindOriginalPrefab("Train Track Elevated");
            if (elevatedTrack == null)
            {
                return;
            }
            var etstMesh = Util.LoadMesh(string.Concat(Util.AssemblyDirectory, "/TTNR.obj"), "ETST ");
            var etstSegmentLodMesh = Util.LoadMesh(string.Concat(Util.AssemblyDirectory, "/TTNR_LOD.obj"), "ETST_SLOD");
            var etstNodeLodMesh = Util.LoadMesh(string.Concat(Util.AssemblyDirectory, "/TTNR_Node_LOD.obj"), "ETST_NLOD");
            elevatedPrefab.m_segments[0].m_segmentMaterial = elevatedTrack.m_segments[0].m_segmentMaterial;
            elevatedPrefab.m_segments[0].m_material = elevatedTrack.m_segments[0].m_material;
            elevatedPrefab.m_segments[0].m_mesh = etstMesh;
            elevatedPrefab.m_segments[0].m_segmentMesh = etstMesh;
            elevatedPrefab.m_segments[0].m_lodMaterial = elevatedTrack.m_segments[0].m_lodMaterial;
            elevatedPrefab.m_segments[0].m_lodMesh = etstSegmentLodMesh;
            elevatedPrefab.m_nodes[0].m_material = elevatedTrack.m_nodes[0].m_material;
            elevatedPrefab.m_nodes[0].m_nodeMaterial = elevatedTrack.m_nodes[0].m_nodeMaterial;
            elevatedPrefab.m_nodes[0].m_lodMaterial = elevatedTrack.m_nodes[0].m_lodMaterial;
            elevatedPrefab.m_nodes[0].m_lodMesh = etstNodeLodMesh;
            elevatedPrefab.m_nodes[0].m_nodeMesh = etstMesh;
            elevatedPrefab.m_nodes[0].m_mesh = etstMesh;
        }

        private static void SetupTunnelPrefab(NetInfo tunnelPrefab, NetInfo originalPrefab)
        {
            SetupSunkenPrefab(tunnelPrefab, originalPrefab);
            tunnelPrefab.m_canCollide = false;
            foreach (var lane in tunnelPrefab.m_lanes)
            {
                lane.m_laneProps = null;
            }
            var metroStation = FindOriginalPrefab("Metro Station Track");
            if (metroStation != null)
            {
                tunnelPrefab.m_segments = new[] { metroStation.m_segments[0] };
                //TODO(earalov): make a shallow copy of segment and change some properties
                tunnelPrefab.m_nodes = new[] { metroStation.m_nodes[0] };
                //TODO(earalov): make a shallow copy of segment and change some properties
            }
            else
            {
                Debug.LogWarning("ElevatedTrainStationTrack - Couldn't find metro station track");
            }
        }

        private static void SetupSunkenPrefab(NetInfo sunkenPrefab, NetInfo originalPrefab)
        {
            var stationAI = sunkenPrefab.GetComponent<TrainTrackAI>();
            stationAI.m_tunnelInfo = sunkenPrefab;

            sunkenPrefab.m_clipTerrain = false;

            sunkenPrefab.m_createGravel = false;
            sunkenPrefab.m_createPavement = false;
            sunkenPrefab.m_createRuining = false;

            sunkenPrefab.m_flattenTerrain = false;
            sunkenPrefab.m_followTerrain = false;

            sunkenPrefab.m_intersectClass = null;

            sunkenPrefab.m_maxHeight = -1;
            sunkenPrefab.m_minHeight = -3;

            sunkenPrefab.m_requireSurfaceMaps = false;
            sunkenPrefab.m_snapBuildingNodes = false;

            sunkenPrefab.m_placementStyle = ItemClass.Placement.Procedural;
            sunkenPrefab.m_useFixedHeight = true;
            sunkenPrefab.m_lowerTerrain = false;
            sunkenPrefab.m_availableIn = ItemClass.Availability.GameAndAsset;
        }

        private static void SetupOneWayPrefab(NetInfo newPrefab, NetInfo originalPrefab)
        {
            foreach (var lane in newPrefab.m_lanes)
            {
                if (lane.m_direction == NetInfo.Direction.Backward)
                {
                    lane.m_direction = NetInfo.Direction.None;
                }
            }
            newPrefab.m_hasBackwardVehicleLanes = false;
        }
    }
}

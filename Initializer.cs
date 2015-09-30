using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ColossalFramework;
using UnityEngine;

namespace ElevatedTrainStationTrack
{
    public class Initializer : MonoBehaviour
    {
        public bool isInitialized;
        Dictionary<string, NetInfo> m_customPrefabs;

        public void Awake()
        {
            DontDestroyOnLoad(this);
            m_customPrefabs = new Dictionary<string, NetInfo>();
        }

        public void OnLevelWasLoaded(int level)
        {
            if (level == 6)
            {
                m_customPrefabs.Clear();
                isInitialized = false;
            }
        }

        public void Update()
        {
            if (isInitialized) return;
            try
            {
                GameObject.Find("Public Transport").GetComponent<NetCollection>();
            }
            catch (Exception)
            {
                return;
            }
            Loading.QueueLoadingAction(Loading.ActionWrapper(InitializeImpl));
            isInitialized = true;
        }

        private void InitializeImpl()
        {

            var originalPrefab =
                Resources.FindObjectsOfTypeAll<NetInfo>().
                FirstOrDefault(netInfo => netInfo.name == "Train Station Track");
            if (originalPrefab == null)
            {
                Debug.LogError("ElevatedTrainStationTrack - Station track prefab not found");
                return;
            }
            NetInfo elevatedPrefab = null;
            var stationTrackEleva = "Station Track Eleva";
            if (!m_customPrefabs.ContainsKey(stationTrackEleva))
            {
                elevatedPrefab = ClonePrefab(originalPrefab, stationTrackEleva);
                if (elevatedPrefab != null)
                {
                    SetupElevatedPrefab(elevatedPrefab, originalPrefab);
                    m_customPrefabs.Add(stationTrackEleva, elevatedPrefab);
                }
                else
                {
                    Debug.LogError("ElevatedTrainStationTrack - Couldn't make elevated prefab");
                }  
            }
            var stationTrackTunnel = "Station Track Tunnel";
            if (!m_customPrefabs.ContainsKey(stationTrackTunnel))
            {
                var tunnelPrefab = ClonePrefab(originalPrefab, stationTrackTunnel);
                if (tunnelPrefab != null)
                {
                    SetupTunnelPrefab(tunnelPrefab, originalPrefab, elevatedPrefab);
                    m_customPrefabs.Add(stationTrackTunnel, tunnelPrefab);
                }
                else
                {
                    Debug.LogError("ElevatedTrainStationTrack - Couldn't make tunnel prefab");
                }
            }

            var stationTrackSunken = "Station Track Sunken";
            if (!m_customPrefabs.ContainsKey(stationTrackSunken))
            {
                var sunkenPrefab = ClonePrefab(originalPrefab, stationTrackSunken);
                if (sunkenPrefab != null)
                {
                    SetupSunkenPrefab(sunkenPrefab, originalPrefab);
                    m_customPrefabs.Add(stationTrackSunken, sunkenPrefab);
                }
                else
                {
                    Debug.LogError("ElevatedTrainStationTrack - Couldn't make sunken prefab");
                }
            }

            foreach (var mCustomPrefab in m_customPrefabs)
            {
                PrefabCollection<NetInfo>.InitializePrefabs(mCustomPrefab.Key, new[] { mCustomPrefab.Value }, new string[] { null });
            }
        }

        private static void SetupElevatedPrefab(NetInfo elevatedPrefab, NetInfo originalPrefab)
        {
            var ai = elevatedPrefab.GetComponent<TrainTrackAI>();
            ai.m_elevatedInfo = elevatedPrefab;
            ai.m_outsideConnection = originalPrefab.GetComponent<TrainTrackAI>().m_outsideConnection;
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
            var elevatedTrack = Resources.FindObjectsOfTypeAll<NetInfo>().
                FirstOrDefault(netInfo => netInfo.name == "Train Track Elevated");
            if (elevatedTrack != null)
            {
                var etstMesh = Util.LoadMesh(String.Concat(Util.AssemblyDirectory, "\\TTNR.obj"), "ETST ");
                var etstSegmentLodMesh = Util.LoadMesh(String.Concat(Util.AssemblyDirectory, "\\TTNR_LOD.obj"), "ETST_SLOD");
                var etstNodeLodMesh = Util.LoadMesh(String.Concat(Util.AssemblyDirectory, "\\TTNR_Node_LOD.obj"), "ETST_NLOD");
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
        }

        private static void SetupTunnelPrefab(NetInfo tunnelPrefab, NetInfo originalPrefab, NetInfo elevatedPrefab)
        {
            var stationAI = tunnelPrefab.GetComponent<TrainTrackAI>();
            DestroyImmediate(stationAI);

            var tunnelAI = tunnelPrefab.gameObject.AddComponent<TrainTrackTunnelAI>();
            tunnelPrefab.m_netAI = tunnelAI;
            tunnelAI.m_outsideConnection = originalPrefab.GetComponent<TrainTrackAI>().m_outsideConnection;
            tunnelAI.m_constructionCost = 0;
            tunnelAI.m_maintenanceCost = 0;
            tunnelAI.m_info = tunnelPrefab;

            if (elevatedPrefab != null)
            {
                var elevatedPrefabAi = elevatedPrefab.GetComponent<TrainTrackAI>();
                elevatedPrefabAi.m_tunnelInfo = tunnelPrefab;
            }

            tunnelPrefab.m_clipTerrain = false;

            tunnelPrefab.m_createGravel = false;
            tunnelPrefab.m_createPavement = false;
            tunnelPrefab.m_createRuining = false;

            tunnelPrefab.m_flattenTerrain = false;
            tunnelPrefab.m_followTerrain = false;

            tunnelPrefab.m_intersectClass = null;

            tunnelPrefab.m_maxHeight = -1;
            tunnelPrefab.m_minHeight = -3;

            tunnelPrefab.m_requireSurfaceMaps = false;
            tunnelPrefab.m_snapBuildingNodes = false;

            tunnelPrefab.m_placementStyle = ItemClass.Placement.Procedural;
            tunnelPrefab.m_useFixedHeight = true;
            tunnelPrefab.m_lowerTerrain = false;
            tunnelPrefab.m_availableIn = ItemClass.Availability.GameAndAsset;

            foreach (var lane in tunnelPrefab.m_lanes)
            {
                lane.m_laneProps = null;
            }

            var metroStation =
                Resources.FindObjectsOfTypeAll<NetInfo>().FirstOrDefault(netInfo => netInfo.name == "Metro Station Track");
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
            DestroyImmediate(stationAI);

            var tunnelAI = sunkenPrefab.gameObject.AddComponent<TrainTrackTunnelAI>();
            sunkenPrefab.m_netAI = tunnelAI;
            tunnelAI.m_outsideConnection = originalPrefab.GetComponent<TrainTrackAI>().m_outsideConnection;
            tunnelAI.m_constructionCost = 0;
            tunnelAI.m_maintenanceCost = 0;
            tunnelAI.m_info = sunkenPrefab;

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

        NetInfo ClonePrefab(NetInfo originalPrefab, string newName)
        {
            var instance = Instantiate(originalPrefab.gameObject);
            instance.name = newName;
            instance.transform.SetParent(transform);
            instance.transform.localPosition = new Vector3(-7500, -7500, -7500);
            var newPrefab = instance.GetComponent<NetInfo>();
            instance.SetActive(false);
            newPrefab.m_prefabInitialized = false;
            return newPrefab;
        }
    }
}

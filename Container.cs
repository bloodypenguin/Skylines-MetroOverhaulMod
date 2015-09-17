using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using ColossalFramework;
using UnityEngine;

namespace ElevatedTrainStationTrack
{
    public class Container : MonoBehaviour
    {
        public bool ClonedPrefab;

        public void Awake()
        {
            DontDestroyOnLoad(this);
        }

        public void OnLevelWasLoaded(int level)
        {
            if (level == 6)
            {
                ClonedPrefab = false;
            }
        }

        public void Update()
        {
            if (ClonedPrefab) return;
            try
            {
                GameObject.Find("Public Transport").GetComponent<NetCollection>();
            }
            catch (Exception)
            {
                return;
            }
            ClonePrefab();
            ClonedPrefab = true;
        }

        private void ClonePrefab()
        {
            var originalPrefab =
                Resources.FindObjectsOfTypeAll<NetInfo>().
                FirstOrDefault(netInfo => netInfo.name == "Train Station Track");
            if (originalPrefab == null)
            {
                Debug.LogError("ElevatedTrainStationTrack - Station track prefab not found");
                return;
            }

            var elevatedPrefab = ClonePrefab(originalPrefab, "Station Track Eleva");
            var tunnelPrefab = ClonePrefab(originalPrefab, "Station Track Tunnel");
            if (elevatedPrefab != null)
            {
                later(() =>
                {
                    ((TrainTrackAI)elevatedPrefab.m_netAI).m_elevatedInfo = elevatedPrefab;
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
                        elevatedPrefab.m_segments[0].m_segmentMaterial.shader = elevatedTrack.m_segments[0].m_material.shader;
                        elevatedPrefab.m_segments[0].m_lodMaterial = elevatedTrack.m_segments[0].m_lodMaterial;
                        elevatedPrefab.m_segments[0].m_lodMesh = elevatedTrack.m_segments[0].m_lodMesh;
                        elevatedPrefab.m_nodes[0].m_nodeMaterial.shader = elevatedTrack.m_nodes[0].m_material.shader;
                        elevatedPrefab.m_nodes[0].m_lodMaterial = elevatedTrack.m_nodes[0].m_lodMaterial;
                        elevatedPrefab.m_nodes[0].m_lodMesh = elevatedTrack.m_nodes[0].m_lodMesh;
                    }

                    if (tunnelPrefab != null)
                    {
                        var stationAI = tunnelPrefab.GetComponent<TrainTrackAI>();
                        DestroyImmediate(stationAI);

                        var tunnelAI = tunnelPrefab.gameObject.AddComponent<TrainTrackTunnelAI>();
                        tunnelPrefab.m_netAI = tunnelAI;
                        tunnelAI.m_outsideConnection = originalPrefab.GetComponent<TrainTrackAI>().m_outsideConnection;
                        tunnelAI.m_constructionCost = 0;
                        tunnelAI.m_maintenanceCost = 0;
                        tunnelAI.m_info = tunnelPrefab;
                        
                        ((TrainTrackAI)elevatedPrefab.m_netAI).m_tunnelInfo = tunnelPrefab;

                        
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

                        var metroStation = Resources.FindObjectsOfTypeAll<NetInfo>().FirstOrDefault(netInfo => netInfo.name == "Metro Station Track");
                        if (metroStation != null)
                        {
                            tunnelPrefab.m_segments = new[]{metroStation.m_segments[0]}; //TODO(earalov): make a shallow copy of segment and change some properties
                            tunnelPrefab.m_nodes = new[] { metroStation.m_nodes[0] }; //TODO(earalov): make a shallow copy of segment and change some properties
                        }
                        else
                        {
                            Debug.LogWarning("ElevatedTrainStationTrack - Couldn't find metro station track");    
                        }
                    }
                    else
                    {
                        Debug.LogError("ElevatedTrainStationTrack - Couldn't make tunnel prefab");
                    }

                });
            }
            else
            {
                Debug.LogError("ElevatedTrainStationTrack - Couldn't make elevated prefab");
            }
        }

        private void later(Action a)
        {
            Singleton<LoadingManager>.instance.QueueLoadingAction(InCoroutine(a));
        }

        private static IEnumerator InCoroutine(Action a)
        {
            a();
            yield break;
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
            newPrefab.m_netAI = null;

            MethodInfo initMethod = typeof(NetCollection).GetMethod("InitializePrefabs", BindingFlags.Static | BindingFlags.NonPublic);
            Singleton<LoadingManager>.instance.QueueLoadingAction((IEnumerator)
            initMethod.Invoke(null, new object[] { newName, new[] { newPrefab }, new string[] { null } }));

            newPrefab.m_prefabInitialized = false;

            return newPrefab;
        }
    }
}

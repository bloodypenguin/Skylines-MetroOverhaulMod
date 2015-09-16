using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using ColossalFramework;
using UnityEngine;
using Object = UnityEngine.Object;


namespace ElevatedTrainStationTrack
{
    public class Container : MonoBehaviour
    {
        public bool clonedPrefab = false;

        private void Awake()
        {
            Object.DontDestroyOnLoad(this);
        }

        private void OnLevelWasLoaded(int level)
        {
            if (level == 6)
            {
                this.clonedPrefab = false;
            }
        }

        private void Update()
        {
            if (!clonedPrefab)
            {
                try
                {
                    GameObject.Find("Public Transport").GetComponent<NetCollection>();
                }
                catch (Exception)
                {
                    return;
                }
                ClonePrefab();
                clonedPrefab = true;
            }
        }

        private void ClonePrefab()
        {
            const string prefabName = "Train Station Track";

            var originalPrefab = FindPrefab(prefabName);
            if (originalPrefab == null)
            {
                UnityEngine.Debug.LogError("ElevatedTrainStationTrack - Station track prefab not found");
                return;
            }

            var elevatedPrefab = ClonePrefab(originalPrefab, "Station Track Eleva");
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
                    //TODO(earalov):
                    //var customMesh = Util.CreateMesh();
                    //elevatedPrefab.m_segments[0].m_segmentMesh = customMesh;
                    //elevatedPrefab.m_nodes[0].m_nodeMesh = customMesh;
                });
            }
            else
            {
                UnityEngine.Debug.LogError("ElevatedTrainStationTrack - Couldn't make elevated prefab");
            }
            var tunnelPrefab = ClonePrefab(originalPrefab, "Station Track Tunnel");
            if (tunnelPrefab != null)
            {
                TrainTrackAI stationAI = elevatedPrefab.GetComponent<TrainTrackAI>();
                TrainTrackTunnelAI tunnelAI = elevatedPrefab.gameObject.AddComponent<TrainTrackTunnelAI>();
                tunnelAI.m_outsideConnection = stationAI.m_outsideConnection;
                tunnelAI.m_constructionCost = 0;
                tunnelAI.m_maintenanceCost = 0;
                GameObject.DestroyImmediate(stationAI);

                later(() =>
                {
                    ((TrainTrackAI)tunnelPrefab.m_netAI).m_tunnelInfo = tunnelPrefab;
                    tunnelPrefab.m_clipTerrain = false;

                    tunnelPrefab.m_createGravel = false;
                    tunnelPrefab.m_createPavement = false;
                    tunnelPrefab.m_createRuining = false;

                    tunnelPrefab.m_flattenTerrain = false;
                    tunnelPrefab.m_followTerrain = false;

                    tunnelPrefab.m_intersectClass = null;

                    tunnelPrefab.m_minHeight = -1;

                    tunnelPrefab.m_requireSurfaceMaps = false;
                    tunnelPrefab.m_snapBuildingNodes = false;

                    tunnelPrefab.m_placementStyle = ItemClass.Placement.Procedural;
                    tunnelPrefab.m_useFixedHeight = true;
                    tunnelPrefab.m_lowerTerrain = false;
                    tunnelPrefab.m_availableIn = ItemClass.Availability.GameAndAsset;
                });
            }
            else
            {
                UnityEngine.Debug.LogError("ElevatedTrainStationTrack - Couldn't make tunnel prefab");
            }
        }



        private void later(Action a)
        {
            Singleton<LoadingManager>.instance.QueueLoadingAction(this.inCoroutine(a));
        }

        private IEnumerator inCoroutine(Action a)
        {
            a();
            yield break;
        }

        NetInfo ClonePrefab(NetInfo originalPrefab, string newName)
        {
            GameObject instance = GameObject.Instantiate<GameObject>(originalPrefab.gameObject);
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
        private NetInfo FindPrefab(string sourceName)
        {
            NetCollection[] array = Object.FindObjectsOfType<NetCollection>();
            for (int i = 0; i < array.Length; i++)
            {
                NetCollection netCollection = array[i];
                NetInfo[] prefabs = netCollection.m_prefabs;
                for (int j = 0; j < prefabs.Length; j++)
                {
                    NetInfo netInfo = prefabs[j];
                    if (netInfo.name == sourceName)
                    {
                        return netInfo;
                    }
                }
            }
            return null;
        }
    }
}

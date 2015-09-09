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
            string prefabName = "Train Station Track";
            string newName = "Station Track Eleva";
            var prefab = ClonePrefab(prefabName, newName, "");
            if (prefab == null)
            {
                UnityEngine.Debug.LogError("Couldn't clone prefab!");
                return;
            }
            later(() =>
            {
                ((TrainTrackAI)prefab.m_netAI).m_elevatedInfo = prefab;
                prefab.m_followTerrain = false;
                prefab.m_flattenTerrain = false;
                prefab.m_createGravel = false;
                prefab.m_createPavement = false;
                prefab.m_createRuining = false;
                prefab.m_requireSurfaceMaps = false;
                prefab.m_clipTerrain = false;
                prefab.m_snapBuildingNodes = false;
                prefab.m_placementStyle = ItemClass.Placement.Procedural;
                prefab.m_useFixedHeight = true;
                prefab.m_lowerTerrain = true;
                prefab.m_availableIn = ItemClass.Availability.GameAndAsset;
            });
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

        private NetInfo ClonePrefab(string sourceName, string name, string desc)
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
                        return this.ClonePrefab(netInfo, name);
                    }
                }
            }
            return null;
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
    }
}

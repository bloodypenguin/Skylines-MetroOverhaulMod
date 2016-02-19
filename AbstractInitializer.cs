using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OneWayTrainTrack
{
    public abstract class AbstractInitializer : MonoBehaviour
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
                var parent = GameObject.Find(SimulationManager.instance.m_metaData.m_environment + " Collections");
                foreach (var t in from Transform t in parent.transform where t.name == "Public Transport" select t)
                {
                    t.gameObject.GetComponent<NetCollection>();
                }
            }
            catch (Exception)
            {
                return;
            }
            Loading.QueueLoadingAction(() =>
            {
                InitializeImpl();
                PrefabCollection<NetInfo>.InitializePrefabs("Rail Extensions", _customPrefabs.Values.ToArray(), null);
            });
            _isInitialized = true;
        }

        protected abstract void InitializeImpl();

        protected void CreatePrefab(string newPrefabName, string originalPrefabName, Action<NetInfo> setupAction)
        {
            var originalPrefab = FindOriginalPrefab(originalPrefabName);

            if (originalPrefab == null)
            {
                Debug.LogErrorFormat("AbstractInitializer#CreatePrefab - Prefab '{0}' not found (required for '{1}')", originalPrefabName, newPrefabName);
                return;
            }
            if (_customPrefabs.ContainsKey(newPrefabName))
            {
                return;
            }
            var newPrefab = Util.ClonePrefab(originalPrefab, newPrefabName, transform);
            if (newPrefab == null)
            {
                Debug.LogErrorFormat("AbstractInitializer#CreatePrefab - Couldn't make prefab '{0}'", newPrefabName);
                return;
            }
            setupAction.Invoke(newPrefab);
            _customPrefabs.Add(newPrefabName, newPrefab);

        }

        protected static NetInfo FindOriginalPrefab(string originalPrefabName)
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

    }
}
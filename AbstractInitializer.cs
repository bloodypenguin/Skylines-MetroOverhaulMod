using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MetroOverhaul
{
    public abstract class AbstractInitializer : MonoBehaviour
    {
        private bool _isInitialized;
        private Dictionary<string, NetInfo> _customNetInfos;
        private Dictionary<string, BuildingInfo> _customBuildingInfos;
        private static readonly Dictionary<string, NetInfo> OriginalNetInfos = new Dictionary<string, NetInfo>();
        private static readonly Dictionary<string, BuildingInfo> OriginalBuildingInfos = new Dictionary<string, BuildingInfo>();
        private List<string> _netReplacements;

        public void Awake()
        {
            DontDestroyOnLoad(this);
            _customNetInfos = new Dictionary<string, NetInfo>();
            _customBuildingInfos = new Dictionary<string, BuildingInfo>();
            _netReplacements = new List<string>();
            OriginalNetInfos.Clear();
            OriginalBuildingInfos.Clear();
        }

        public void OnLevelWasLoaded(int level)
        {
            if (level == 6)
            {
                _netReplacements.Clear();
                _customNetInfos.Clear();
                OriginalNetInfos.Clear();
                _customBuildingInfos.Clear();
                OriginalBuildingInfos.Clear();
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
                PrefabCollection<NetInfo>.InitializePrefabs("Rail Extensions", _customNetInfos.Values.ToArray(), _netReplacements.ToArray());
                PrefabCollection<BuildingInfo>.InitializePrefabs("Rail Building Extensions", _customBuildingInfos.Values.ToArray(), null);
            });
            _isInitialized = true;
        }

        protected abstract void InitializeImpl();

        protected NetInfo CreateNetInfo(string newNetInfoName, string originalNetInfoName, Action<NetInfo> setupAction, string replaces = "")
        {
            var originalPrefab = FindOriginalNetInfo(originalNetInfoName);

            if (originalPrefab == null)
            {
                Debug.LogErrorFormat("AbstractInitializer#CreatePrefab - Prefab '{0}' not found (required for '{1}')", originalNetInfoName, newNetInfoName);
                return null;
            }
            if (_customNetInfos.ContainsKey(newNetInfoName))
            {
                Debug.LogErrorFormat("AbstractInitializer#CreatePrefab - Prefab '{0}' was already created", newNetInfoName);
                return null;
            }
            var newPrefab = Util.ClonePrefab(originalPrefab, newNetInfoName, transform);
            if (newPrefab == null)
            {
                Debug.LogErrorFormat("AbstractInitializer#CreatePrefab - Couldn't make prefab '{0}'", newNetInfoName);
                return null;
            }
            setupAction.Invoke(newPrefab);
            _customNetInfos.Add(newNetInfoName, newPrefab);
            _netReplacements.Add(replaces);
            return newPrefab;
        }
        protected void CreateBuildingInfo(string newBuildingInfoName, string originalBuildingInfoName, Action<BuildingInfo> setupAction = null)
        {
            var originalPrefab = FindOriginalBuildingInfo(originalBuildingInfoName);

            if (originalPrefab == null)
            {
                Debug.LogErrorFormat("AbstractInitializer#CreatePrefab - Prefab '{0}' not found (required for '{1}')", originalBuildingInfoName, newBuildingInfoName);
                return;
            }
            if (_customBuildingInfos.ContainsKey(newBuildingInfoName))
            {
                return;
            }
            var newPrefab = Util.ClonePrefab(originalPrefab, newBuildingInfoName, transform);
            if (newPrefab == null)
            {
                Debug.LogErrorFormat("AbstractInitializer#CreatePrefab - Couldn't make prefab '{0}'", newBuildingInfoName);
                return;
            }
            setupAction?.Invoke(newPrefab);
            _customBuildingInfos.Add(newBuildingInfoName, newPrefab);
        }

        protected NetInfo FindCustomNetInfo(string customNetInfoName)
        {
            return _customNetInfos[customNetInfoName];
        }

        protected static NetInfo FindOriginalNetInfo(string originalNetInfoName)
        {
            NetInfo foundNetInfo;
            if (OriginalNetInfos.TryGetValue(originalNetInfoName, out foundNetInfo))
            {
                return foundNetInfo;
            }
            foundNetInfo = Resources.FindObjectsOfTypeAll<NetInfo>().FirstOrDefault(netInfo => netInfo.name == originalNetInfoName);
            if (foundNetInfo == null)
            {
                return null;
            }
            OriginalNetInfos.Add(originalNetInfoName, foundNetInfo);
            return foundNetInfo;
        }

        protected static BuildingInfo FindOriginalBuildingInfo(string originalBuildingInfoName)
        {
            BuildingInfo foundBuildingInfo;
            if (OriginalBuildingInfos.TryGetValue(originalBuildingInfoName, out foundBuildingInfo))
            {
                return foundBuildingInfo;
            }
            foundBuildingInfo = Resources.FindObjectsOfTypeAll<BuildingInfo>().FirstOrDefault(BuildingInfo => BuildingInfo.name == originalBuildingInfoName);
            if (foundBuildingInfo == null)
            {
                return null;
            }
            OriginalBuildingInfos.Add(originalBuildingInfoName, foundBuildingInfo);
            return foundBuildingInfo;
        }
    }
}
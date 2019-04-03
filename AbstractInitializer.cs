using ColossalFramework;
using MetroOverhaul.Extensions;
using MetroOverhaul.InitializationSteps;
using MetroOverhaul.NEXT.Extensions;
using MetroOverhaul.OptionsFramework;
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
            if (OptionsWrapper<Options>.Options.ghostMode)
            {
                //return;
            }
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
                InitializeNetInfoImpl();
                PrefabCollection<NetInfo>.InitializePrefabs("Metro Extensions", _customNetInfos.Values.ToArray(), !OptionsWrapper<Options>.Options.ghostMode ? _netReplacements.ToArray() : null);
            });
            _isInitialized = true;
        }
        public void DoSomething() {
            Loading.QueueLoadingAction(() =>
            {
                InitializeBuildingInfoImpl();
                PrefabCollection<BuildingInfo>.InitializePrefabs("Metro Building Extensions", _customBuildingInfos.Values.ToArray(), null);
                PrefabCollection<BuildingInfo>.BindPrefabs();
            });
        }
        protected abstract void InitializeNetInfoImpl();
        public abstract void InitializeBuildingInfoImpl();

        protected void CreateStationClones(Action<BuildingInfo> setupAction = null) {
            var railStationInfos = Resources.FindObjectsOfTypeAll<BuildingInfo>();
            var targetRailStationInfos = railStationInfos.Where(rsi =>
                rsi?.m_buildingAI is TransportStationAI &&
                ((rsi.m_class.m_subService == ItemClass.SubService.PublicTransportTrain && rsi.HasAbovegroundTrainStationTracks())||
                (rsi.m_class.m_subService == ItemClass.SubService.PublicTransportMetro && rsi.HasAbovegroundMetroStationTracks())));
            foreach (var railStationInfo in targetRailStationInfos) {
                CreateBuildingInfo(railStationInfo.name + "_Analog", railStationInfo, setupAction);
            }
        }
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
            CreateBuildingInfo(newBuildingInfoName, originalPrefab, setupAction);

        }
        private void CreateBuildingInfo(string newBuildingInfoName, BuildingInfo originalBuildingInfo, Action<BuildingInfo> setupAction = null) {
            if (originalBuildingInfo == null)
            {
                Debug.LogErrorFormat("AbstractInitializer#CreatePrefab - Prefab '{0}' not found (required for '{1}')", originalBuildingInfo.name, newBuildingInfoName);
                return;
            }
            if (_customBuildingInfos.ContainsKey(newBuildingInfoName))
            {
                return;
            }
            //var newPrefab = originalBuildingInfo.Clone(newBuildingInfoName, transform);
            var newPrefab = Util.ClonePrefab(originalBuildingInfo, newBuildingInfoName, transform);
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
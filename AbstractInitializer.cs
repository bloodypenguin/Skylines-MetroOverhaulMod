using MetroOverhaul.Extensions;
using MetroOverhaul.NEXT.Extensions;
using MetroOverhaul.OptionsFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MetroOverhaul {
    public abstract class AbstractInitializer : MonoBehaviour
    {
        private bool _isInitialized;
        protected static Dictionary<string, NetInfoMetadata> _customNetInfoMetadata;
        private Dictionary<string, BuildingInfo> _customBuildingInfos;
        private static readonly Dictionary<string, NetInfo> OriginalNetInfos = new Dictionary<string, NetInfo>();
        private static readonly Dictionary<string, BuildingInfo> OriginalBuildingInfos = new Dictionary<string, BuildingInfo>();
        private List<string> _netReplacements;
        private List<string> _registeredWids;

        public void OnEnable()
        {
            SceneManager.sceneLoaded += OnLevelFinishedLoading;
        }

        public void Awake()
        {
            DontDestroyOnLoad(this);
            _customBuildingInfos = new Dictionary<string, BuildingInfo>();
            _customNetInfoMetadata = new Dictionary<string, NetInfoMetadata>();
            _netReplacements = new List<string>();
            _registeredWids = new List<string>();
            OriginalNetInfos.Clear();
            OriginalBuildingInfos.Clear();
        }

        public void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
        {
            if (scene.buildIndex == 6)
            {
                _netReplacements.Clear();
                OriginalNetInfos.Clear();
                _customBuildingInfos.Clear();
                OriginalBuildingInfos.Clear();
                _customNetInfoMetadata.Clear();
                _registeredWids.Clear();
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
                InitializeNetInfoImpl();
                PrefabCollection<NetInfo>.InitializePrefabs("Metro Extensions", _customNetInfoMetadata.Values.Select(m => m.Info).ToArray(), null);
            });
            _isInitialized = true;
        }

        protected abstract void InitializeNetInfoImpl();
        public abstract void InitializeBuildingInfoImpl(BuildingInfo info);

        //public bool RegisterWid(BuildingInfo info, bool isPreInitialization)
        //{
        //    long workshopId;
        //    if (Util.TryGetWorkshopId(info, out workshopId))
        //    {
        //        if (_registeredWids.IndexOf(info.name) > -1 || info.name.IndexOf(ModTrackNames.ANALOG_PREFIX) > -1)
        //        {
        //            return false;
        //        }
        //        _registeredWids.Add(info.name);
        //    }
        //    var retval = (isPreInitialization && workshopId > -1) || (!isPreInitialization && workshopId == -1);
        //    return retval;
        //}

        public static NetInfoMetadata GetNetInfoMetadata(string name)
        {
            return _customNetInfoMetadata.ContainsKey(name) ? _customNetInfoMetadata[name] : null;
        }

        public static NetInfoMetadata LookupNetInfoMetadata(NetInfoMetadata netInfoMetadata)
        {
            foreach (var metaData in _customNetInfoMetadata.Values)
            {
                if(metaData.TrackStyle == netInfoMetadata.TrackStyle)
                {
                    if (metaData.Version == netInfoMetadata.Version)
                    {
                        if ((metaData.TrackType != NetInfoTrackType.None && metaData.TrackType == netInfoMetadata.TrackType) ||
                            metaData.StationTrackType != NetInfoStationTrackType.None && metaData.StationTrackType == netInfoMetadata.StationTrackType)
                        {
                            return metaData;
                        }
                    }
                }
            }
            return null;
        }

        protected void CreateStationClone(BuildingInfo info, Action<BuildingInfo> setupAction = null) {
                var suffix = "";
                if (info.IsMetroStation()) {
                    suffix = TrackVehicleType.Train.ToString();
                }
                else if (info.IsTrainStation()) {
                    suffix = TrackVehicleType.Metro.ToString();
                }
                CreateBuildingInfo(info.name + ModTrackNames.ANALOG_PREFIX + suffix, info, setupAction);
        }
        protected NetInfo CreateNetInfo(string newNetInfoName, string originalNetInfoName, Action<NetInfo> setupAction, 
            NetInfoMetadata netInfoMetaData, string replaces = "")
        {
            var originalPrefab = FindOriginalNetInfo(originalNetInfoName);

            if (originalPrefab == null)
            {
                Debug.LogErrorFormat("AbstractInitializer#CreatePrefab - Prefab '{0}' not found (required for '{1}')", originalNetInfoName, newNetInfoName);
                return null;
            }
            if (_customNetInfoMetadata.ContainsKey(newNetInfoName))
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
            if (netInfoMetaData.GroundInfo == null && netInfoMetaData.Version == NEXT.NetInfoVersion.Ground)
                netInfoMetaData.GroundInfo = newPrefab;
            netInfoMetaData.Info = newPrefab;
            _customNetInfoMetadata.Add(newNetInfoName, netInfoMetaData);
            _netReplacements.Add(replaces);
            return newPrefab;
        }

        protected void AddNetInfoMetadata(NetInfoMetadata netInfoMetadata)
        {
            string netInfoName = netInfoMetadata.Info.name;
            _customNetInfoMetadata.Add(netInfoName, netInfoMetadata);
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
            var newPrefab = Util.ClonePrefab(originalBuildingInfo, newBuildingInfoName, transform);
            if (newPrefab == null)
            {
                Debug.LogErrorFormat("AbstractInitializer#CreatePrefab - Couldn't make prefab '{0}'", newBuildingInfoName);
                return;
            }
            
            setupAction?.Invoke(newPrefab);
            _customBuildingInfos.Add(newBuildingInfoName, newPrefab);
            Debug.Log("Prefab Made: " + newPrefab.name);
            PrefabCollection<BuildingInfo>.InitializePrefabs("Metro Building Extensions", newPrefab, null);
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

        public void OnDisable()
        {
            SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        }
    }
}
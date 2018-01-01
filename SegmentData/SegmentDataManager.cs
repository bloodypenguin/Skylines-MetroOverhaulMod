using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ColossalFramework.IO;
using ICities;
using NetworkSkins.Detour;
using NetworkSkins.Net;
using UnityEngine;

namespace NetworkSkins.Data
{
    public class SegmentDataManager : SerializableDataExtensionBase
    {
        public class OptionsData : IDataContainer
        {
            public void Serialize(DataSerializer s)
            {
                var count = SegmentDataManager.Instance._selectedSegmentOptions.Count;
                s.WriteInt32(count);

#if DEBUG
                Debug.Log($"Serializing {count} active options");
#endif

                foreach (var selectedOption in SegmentDataManager.Instance._selectedSegmentOptions)
                {
#if DEBUG
                    Debug.Log($"{selectedOption.Key.name} --> {selectedOption.Value.ToString()}");
#endif

                    s.WriteUniqueString(selectedOption.Key.name);
                    selectedOption.Value.Serialize(s);
                }
            }

            public void Deserialize(DataSerializer s)
            {
                var count = s.ReadInt32();

#if DEBUG
            Debug.Log($"Deserializing {count} active options");
#endif

                for (var i = 0; i < count; i++)
                {
                    var prefabName = s.ReadUniqueString();

                    var prefab = PrefabCollection<NetInfo>.FindLoaded(prefabName);
                    if (prefab == null) continue;

                    var options = new SegmentData();
                    options.Deserialize(s);

#if DEBUG
                    Debug.Log($"{prefabName} --> {options.ToString()}");
#endif
                    SegmentDataManager.Instance.SetActiveOptions(prefab, options);
                }
            }

            public void AfterDeserialize(DataSerializer s)
            {
            }
        }

        private const string SegmentDataId = "NetworkSkins_SEGMENTS";
        private const string SelectedOptionsId = "NetworkSkins_SELECTED_OPTIONS";
        private const uint DataVersion = 0;

        public static SegmentDataManager Instance;

        // stores all segment data instances that are currently in use (selected in options or applied to network segments)
        private readonly List<SegmentData> _usedSegmentData = new List<SegmentData>();

        // stores which options were selected by the user, per prefab
        private readonly Dictionary<NetInfo, SegmentData> _selectedSegmentOptions = new Dictionary<NetInfo, SegmentData>();

        // stores which options were selected by the user, per prefab
        private readonly Dictionary<NetInfo, SegmentData> _assetSegmentOptions = new Dictionary<NetInfo, SegmentData>();

        // stores which options should be used (user or asset mode)
        private bool _assetMode = false;

        // stores which data is applied to a network segment
        // this is an array field for high lookup performance
        public SegmentData[] SegmentToSegmentDataMap;

        public void SetAssetMode(bool value) // TODO make property
        {
            _assetMode = value;

            foreach (var entry in _assetSegmentOptions)
            {
                entry.Value.UsedCount--;
                DeleteIfNotInUse(entry.Value);
            }
            _assetSegmentOptions.Clear();
        }

        public bool AssetMode => _assetMode;

        public override void OnCreated(ISerializableData serializableData)
        {
            base.OnCreated(serializableData);
            Instance = this;

            RenderManagerDetour.EventUpdateDataPost += OnUpdateData;

            NetManagerDetour.EventSegmentCreate += OnSegmentCreate;
            NetManagerDetour.EventSegmentRelease += OnSegmentRelease;
            NetManagerDetour.EventSegmentTransferData += OnSegmentTransferData;
        }

        /// <summary>
        /// Like OnLevelLoaded, but executed earlier.
        /// </summary>
        /// <param name="mode"></param>
        public void OnUpdateData(SimulationManager.UpdateMode mode)
        {
            if (mode != SimulationManager.UpdateMode.LoadGame && mode != SimulationManager.UpdateMode.LoadMap) return;

            DeserializeSegmentDataMap();
            DeserializeActiveOptions();

            foreach (var segmentData in _usedSegmentData)
            {
                segmentData.UsedCount = SegmentToSegmentDataMap.Count(segmentData.Equals);
                segmentData.FindPrefabs(); // Find the prefabs for the loaded names
            }

            CleanupData();
        }

        private void DeserializeSegmentDataMap()
        {
            var data = serializableDataManager.LoadData(SegmentDataId);

            if (data != null)
            {
                try
                {
                    using (var stream = new MemoryStream(data))
                    {
                        SegmentToSegmentDataMap = DataSerializer.DeserializeArray<SegmentData>(stream, DataSerializer.Mode.Memory);
                    }

                    Debug.LogFormat("Network Skins: Selected Data loaded (Data length: {0})", data.Length);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            if (SegmentToSegmentDataMap == null)
            {
                SegmentToSegmentDataMap = new SegmentData[NetManager.instance.m_segments.m_size];
                Debug.Log("Network Skins: No segment data found!");
            }

            _usedSegmentData.AddRange(SegmentToSegmentDataMap.Distinct().Where(segmentData => segmentData != null));
        }

        private void DeserializeActiveOptions()
        {
            var selectedData = serializableDataManager.LoadData(SelectedOptionsId);
            if (selectedData == null)
            {
                Debug.Log("Network Skins: No select options data found!");
                return;
            }

            try
            {
                using (var stream = new MemoryStream(selectedData))
                {
                    DataSerializer.Deserialize<OptionsData>(stream, DataSerializer.Mode.Memory);
                }

                Debug.LogFormat("Network Skins: Selected Options loaded (Data length: {0})", selectedData.Length);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return;
            }
        }

        public void OnLevelLoaded()
        {
            if (SegmentToSegmentDataMap == null)
            {
                SegmentToSegmentDataMap = new SegmentData[NetManager.instance.m_segments.m_size];
            }
        }

        public void OnLevelUnloaded()
        {
            _usedSegmentData.Clear();
            _selectedSegmentOptions.Clear();
            _assetSegmentOptions.Clear();
            SegmentToSegmentDataMap = null;
        }

        public override void OnReleased()
        {
            base.OnReleased();

            RenderManagerDetour.EventUpdateDataPost -= OnUpdateData;

            NetManagerDetour.EventSegmentCreate -= OnSegmentCreate;
            NetManagerDetour.EventSegmentCreate -= OnSegmentRelease;
            NetManagerDetour.EventSegmentTransferData -= OnSegmentTransferData;

            Instance = null;
        }

        public override void OnSaveData()
        {
            base.OnSaveData();

            SerializeSegmentData();
            SerializeActiveOptions();
        }

        private void SerializeSegmentData()
        {
            var saveRequired = CleanupData();

            // check if data must be saved
            if (saveRequired)
            {
                byte[] data;

                using (var stream = new MemoryStream())
                {
                    DataSerializer.SerializeArray(stream, DataSerializer.Mode.Memory, DataVersion, SegmentToSegmentDataMap);
                    data = stream.ToArray();
                }

                serializableDataManager.SaveData(SegmentDataId, data);

                Debug.LogFormat("Network Skins: Segment Data Saved (Data length: {0})", data.Length);
            }
            else
            {
                serializableDataManager.EraseData(SegmentDataId);

                Debug.Log("Network Skins: Segment Data Cleared!");
            }
        }

        private void SerializeActiveOptions()
        {
            var saveRequired = _selectedSegmentOptions.Count > 0;

            // check if data must be saved
            if (saveRequired)
            {
                byte[] data;

                using (var stream = new MemoryStream())
                {
                    DataSerializer.Serialize(stream, DataSerializer.Mode.Memory, DataVersion, new OptionsData());
                    data = stream.ToArray();
                }

                serializableDataManager.SaveData(SelectedOptionsId, data);

                Debug.LogFormat("Network Skins: Selected Options Saved (Data length: {0})", data.Length);
            }
            else
            {
                serializableDataManager.EraseData(SelectedOptionsId);

                Debug.Log("Network Skins: Selected Options Data Cleared!");
            }
        }

        public SegmentData GetActiveOptions(NetInfo prefab)
        {
            var options = _assetMode ? _assetSegmentOptions : _selectedSegmentOptions;

            SegmentData segmentData;
            options.TryGetValue(prefab, out segmentData);
            return segmentData;
        }

        public void SetActiveOptions(NetInfo prefab, SegmentData segmentOptions)
        {
            var options = _assetMode ? _assetSegmentOptions : _selectedSegmentOptions;

            // Delete existing data if it is not used anymore
            var activeSegmentData = GetActiveOptions(prefab);
            if (activeSegmentData != null)
            {
                options.Remove(prefab);
                activeSegmentData.UsedCount--;
                DeleteIfNotInUse(activeSegmentData);
            }

            // No new data? Stop here
            if (segmentOptions == null || segmentOptions.Features == SegmentData.FeatureFlags.None) return;

            // Check if there is an equal data object
            var equalSegmentData = _usedSegmentData.FirstOrDefault(segmentOptions.Equals);
            if (equalSegmentData != null)
            {
                // yes? use that, discard the one we created
                options[prefab] = equalSegmentData;
                equalSegmentData.UsedCount++;
            }
            else
            {
                // no? Use the one we got
                _usedSegmentData.Add(segmentOptions);
                options[prefab] = segmentOptions;
                segmentOptions.UsedCount++;
            }

        }

        public void OnSegmentCreate(ushort segment)
        {
            if (SegmentToSegmentDataMap == null) return;

            var prefab = NetManager.instance.m_segments.m_buffer[segment].Info;
            var segmentData = GetActiveOptions(prefab);
            SegmentToSegmentDataMap[segment] = segmentData;

            if (segmentData != null) segmentData.UsedCount++;

            //Debug.LogFormat("Segment {0} created!", segment);
        }

        public void OnSegmentRelease(ushort segment)
        {
            if (SegmentToSegmentDataMap == null) return;

            var segmentData = SegmentToSegmentDataMap[segment];
            if (segmentData != null)
            {
                segmentData.UsedCount--;
                DeleteIfNotInUse(segmentData);
            }

            SegmentToSegmentDataMap[segment] = null;

            //Debug.LogFormat("Segment {0} released!", segment);
        }

        public void OnSegmentTransferData(ushort oldSegment, ushort newSegment)
        {
            if (SegmentToSegmentDataMap == null) return;

            var segmentData = SegmentToSegmentDataMap[oldSegment];
            if (segmentData != null) segmentData.UsedCount++;

            SegmentToSegmentDataMap[newSegment] = SegmentToSegmentDataMap[oldSegment];

            //Debug.LogFormat("Transfer data from {0} to {1}!", oldSegment, newSegment);
        }

        /// <summary>
        /// Validates the data. Removes data which is no longer used (should never happen).
        /// </summary>
        /// <returns>If there is data applied to any segment</returns>
        private bool CleanupData()
        {
            var result = false;

            if(_usedSegmentData != null)
            foreach (var segmentData in _usedSegmentData.ToArray())
            {
                var segmentMapUsedCount = SegmentToSegmentDataMap.Count(segmentData.Equals);
                var segmentOptionsUsedCount = _selectedSegmentOptions.Values.Count(segmentData.Equals);
                var assetOptionsUsedCount = _assetSegmentOptions.Values.Count(segmentData.Equals);
                var calculatedUsedCount = segmentMapUsedCount + segmentOptionsUsedCount + assetOptionsUsedCount;

                if (segmentMapUsedCount > 0) result = true;

                if (segmentData.UsedCount != calculatedUsedCount)
                {
                    Debug.LogErrorFormat("Network Skins: Incorrect usedCount detected, should be {0} ({1})", calculatedUsedCount, segmentData);

                    segmentData.UsedCount = calculatedUsedCount;
                    DeleteIfNotInUse(segmentData);
                }
            }

            // check if data is applied to segments which no longer exist
            if(SegmentToSegmentDataMap != null)
            for (var i = 0; i <SegmentToSegmentDataMap.Length; i++)
            {
                var segmentData = SegmentToSegmentDataMap[i];

                if (segmentData != null && NetManager.instance.m_segments.m_buffer[i].m_flags == NetSegment.Flags.None)
                {
                    Debug.LogErrorFormat("Network Skins: Data was applied to released segment {0}", segmentData);

                    SegmentToSegmentDataMap[i] = null;
                    segmentData.UsedCount--;
                    DeleteIfNotInUse(segmentData);
                }
            }

            return result;
        }

        private void DeleteIfNotInUse(SegmentData segmentData)
        {
            if (segmentData.UsedCount <= 0)
            {
                _usedSegmentData.Remove(segmentData);
            }
        }
    }
}

using ColossalFramework;
using MetroOverhaul.Extensions;
using MetroOverhaul.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MetroOverhaul
{
    public class ReverseEngineerStationCustomizations
    {
        public MetroStationCustomizerUI m_UI { get; set; }
        public int Length { get; set; }
        public int Depth { get; set; }
        public int Angle { get; set; }
        public int Bend { get; set; }
        public StationTrackType StationTrackType { get; set; }
        public BuildingInfo BuildingPrefab { get; set; }
        public bool AllTracks { get; set; }
        private readonly ushort m_BuildingID;
        private readonly Building m_Building;
        private readonly ushort m_SegmentID;
        private readonly NetSegment m_Segment;

        public ReverseEngineerStationCustomizations(MetroStationCustomizerUI ui, int buildingID, ushort segmentID)
        {
            m_UI = ui;
            m_BuildingID = (ushort)buildingID;
            m_Building = Singleton<BuildingManager>.instance.m_buildings.m_buffer[m_BuildingID];
            BuildingPrefab = m_Building.Info;
            m_SegmentID = segmentID;
            if (m_SegmentID > 0)
                m_Segment = Singleton<NetManager>.instance.m_segments.m_buffer[m_SegmentID];
            AllTracks = segmentID == 0;
            Init();
        }

        public void Init()
        {
            Length = RestoreCustomizations(MetroStationTrackAlterType.Length, m_Building.m_children);
            Depth = RestoreCustomizations(MetroStationTrackAlterType.Depth, m_Building.m_childHealth);
            Angle = RestoreCustomizations(MetroStationTrackAlterType.Rotation, m_Building.m_seniors);
            Bend = RestoreCustomizations(MetroStationTrackAlterType.Bend, m_Building.m_seniorHealth);
            StationTrackType = (StationTrackType)(m_Building.m_teens);
        }
        private int RestoreCustomizations(MetroStationTrackAlterType trackAlterType, byte cachedValue)
        {
            return (int)(m_UI.SliderDataDict[trackAlterType].Min + (cachedValue * m_UI.SliderDataDict[trackAlterType].Step));
        }
    }
}

using ColossalFramework;
using MetroOverhaul.Extensions;
using MetroOverhaul.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static MetroOverhaul.StationTrackCustomizations;

namespace MetroOverhaul
{
    public class ReverseEngineerStationCustomizations
    {
        public MetroStationCustomizerUI m_UI { get; set; }
        public StationTrackCustomizations TrackCustomization { get; }
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
            TrackCustomization = new StationTrackCustomizations()
            {
                Horizontal = RestoreCustomizations(MetroStationTrackAlterType.Horizontal, m_Building.m_happiness),
                Vertical = RestoreCustomizations(MetroStationTrackAlterType.Vertical, m_Building.m_health),
                Length = RestoreCustomizations(MetroStationTrackAlterType.Length, m_Building.m_children),
                Depth = RestoreCustomizations(MetroStationTrackAlterType.Depth, m_Building.m_childHealth),
                Rotation = RestoreCustomizations(MetroStationTrackAlterType.Rotation, m_Building.m_seniors),
                Curve = RestoreCustomizations(MetroStationTrackAlterType.Curve, m_Building.m_seniorHealth),
                TrackType = (StationTrackType)(m_Building.m_teens)
            };
        }

        private int RestoreCustomizations(MetroStationTrackAlterType trackAlterType, byte cachedValue)
        {
            return (int)(m_UI.SliderDataDict[trackAlterType].Min + (cachedValue * m_UI.SliderDataDict[trackAlterType].Step));
        }
    }
}

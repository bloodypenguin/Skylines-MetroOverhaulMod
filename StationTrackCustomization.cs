using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroOverhaul
{
    public class StationTrackCustomizations
    {
        public float Horizontal { get; set; }
        public float Vertical { get; set; }
        public float Length { get; set; }
        public float Depth { get; set; }
        public float Rotation { get; set; }
        public float Curve { get; set; }
        public MetroStationTrackAlterType AlterType { get; set; }
        public StationTrackType TrackType { get; set; }
        public BuildingInfo.PathInfo Path { get; set; }
        public StationTrackCustomizations()
        {
            Length = SetStationCustomizations.DEF_LENGTH;
            Depth = SetStationCustomizations.DEF_DEPTH;
            Rotation = SetStationCustomizations.DEF_ROTATION;
            Curve = SetStationCustomizations.DEF_CURVE;
            AlterType = MetroStationTrackAlterType.All;
            TrackType = StationTrackType.SidePlatform;
        }

        public enum MetroStationTrackAlterType
        {
            None = 0,
            Horizontal = 1,
            Vertical = 2,
            Depth = 4,
            Length = 8,
            Rotation = 16,
            Curve = 32,
            All = 63
        }
    }
}

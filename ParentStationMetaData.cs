using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroOverhaul
{
    public class ParentStationMetaData
    {
        public List<BuildingInfo.PathInfo> LowestHighestPaths { get; set; }
        public float Angle { get; set; }
        public UnityEngine.Vector3 Position { get; set; }
    }
}

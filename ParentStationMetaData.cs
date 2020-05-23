using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroOverhaul
{
    public class ParentStationMetaData
    {
        public BuildingInfo Info { get; set; }
        public List<UnityEngine.Vector3> AnchorPoints { get; set; }
        public float Angle { get; set; }
        public UnityEngine.Vector3 Position { get; set; }
    }
}

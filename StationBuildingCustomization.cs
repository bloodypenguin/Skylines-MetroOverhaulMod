using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroOverhaul
{
    public static class StationBuildingCustomization
    {
        public static Dictionary<string, List<BuildingInfo.PathInfo>> StoredStationPaths { get; set; }
        public static Dictionary<string, List<BuildingInfo.SubInfo>> StoredStationSubbuildings { get; set; }
    }
}

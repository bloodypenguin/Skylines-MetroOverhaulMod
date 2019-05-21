using MetroOverhaul.NEXT;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MetroOverhaul.InitializationSteps
{
    public static class LateBuildUp
    {
        public static void BuildUp(NetInfo prefab, NetInfoVersion version)
        {
            var smallWord = "";
            switch (prefab.m_lanes.Where(l => l.m_vehicleType == VehicleInfo.VehicleType.Metro).GroupBy(g => Math.Round(g.m_position)).Count()) {
                case 1:
                    smallWord = "Small";
                    break;
                case 4:
                    smallWord = "Large";
                    break;
            }
            switch (version)
            {
                case NetInfoVersion.Elevated:
                    {
                        var epBuildingInfo = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName($"MetroElevatedPillar{smallWord}")}.MetroElevatedPillar{smallWord}_Data");
                        var epBuildingInfoNoCol = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName($"MetroElevatedPillar{smallWord}NoCol")}.MetroElevatedPillar{smallWord}NoCol_Data");

                        if (epBuildingInfo == null)
                        {
                            throw new Exception($"{prefab.name}: MetroElevatedPillar not found!");
                        }
                        var bridgeAI = prefab.GetComponent<TrainTrackBridgeAIMetro>();
                        if (bridgeAI != null)
                        {
                            bridgeAI.m_bridgePillarInfo = epBuildingInfo;
                            bridgeAI.m_bridgePillarOffset = -0.75f;
                            bridgeAI.pillarList = new List<BridgePillarItem>();
                            bridgeAI.pillarList.Add(new BridgePillarItem() {
                                WideMedianInfo = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("MetroElevatedPillarLarge")}.MetroElevatedPillarLarge_Data"),
                                WideInfo = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("Modern_Wide_O-R-F")}.Modern Wide O-R-F_Data"),
                                NarrowMedianInfo = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("Modern_Median_O-R-F")}.Modern Median O-R-F_Data"),
                                NarrowInfo = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("Modern_Narrow_O-R-F")}.Modern Narrow O-R-F_Data"),
                                WideMedianInfoNoCol = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("MetroElevatedPillarLargeNoCol")}.MetroElevatedPillarLargeNoCol_Data"),
                                WideInfoNoCol = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("Modern_Wide_O-R-F_NoCol")}.Modern Wide O-R-F NoCol_Data"),
                                NarrowMedianInfoNoCol = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("Modern_Median_O-R-F_NoCol")}.Modern Median O-R-F NoCol_Data"),
                                NarrowInfoNoCol = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("Modern_Narrow_O-R-F_NoCol")}.Modern Narrow O-R-F NoCol_Data"),
                                HeightLimit = 0,
                                HeightOffset = 0
                            });
                        }
                        break;
                    }
                case NetInfoVersion.Bridge:
                    {
                        var bpBuildingInfo = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName($"MetroBridgePillar{smallWord}")}.MetroBridgePillar{smallWord}_Data");
                        var bpBuildingInfoNoCol = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName($"MetroBridgePillar{smallWord}NoCol")}.MetroBridgePillar{smallWord}NoCol_Data");
                        if (bpBuildingInfo == null)
                        {
                            throw new Exception($"{prefab.name}: MetroBridgePillar not found!");
                        }
                        var bridgeAI = prefab.GetComponent<TrainTrackBridgeAIMetro>();
                        if (bridgeAI != null)
                        {
                            bridgeAI.m_bridgePillarInfo = bpBuildingInfo;
                            bridgeAI.pillarType = PillarType.WideMedian;
                            bridgeAI.pillarList = new List<BridgePillarItem>();
                            bridgeAI.pillarList.Add(new BridgePillarItem() { HeightLimit = 0, HeightOffset = 0, WideMedianInfo = bpBuildingInfo, WideMedianInfoNoCol = bpBuildingInfoNoCol });
                        }
                        break;
                    }
            }
        }

    }
}

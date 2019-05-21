using System;
using MetroOverhaul.NEXT;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace MetroOverhaul.InitializationSteps
{
    public static class LateBuildUpSteel
    {
        public static void BuildUp(NetInfo prefab, NetInfoVersion version)
        {
            var laneCount = prefab.m_lanes.Where(l => l.m_vehicleType == VehicleInfo.VehicleType.Metro).GroupBy(g => Math.Round(g.m_position)).Count();
            var smallWord = "";
            switch (prefab.m_lanes.Where(l => l.m_vehicleType == VehicleInfo.VehicleType.Metro).GroupBy(g => Math.Round(g.m_position)).Count())
            {
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
                        var lowPillarProp = PrefabCollection<PropInfo>.FindLoaded($"{Util.PackageName($"Classic {laneCount}L Pillar (Low) Prop")}.Classic {laneCount}L Pillar (Low) Prop_Data");
                        var highPillarProp = PrefabCollection<PropInfo>.FindLoaded($"{Util.PackageName($"Classic {laneCount}L Pillar (High) Prop")}.Classic {laneCount}L Pillar (High) Prop_Data");

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
                            bridgeAI.pillarList.Add(new BridgePillarItem()
                            {
                                WideMedianInfo = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("Classic 4L Pillar (Low)")}.Classic 4L Pillar (Low)_Data"),
                                WideInfo = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("Classic_Wide_O-R-F_Low")}.Classic Wide O-R-F Low_Data"),
                                NarrowMedianInfo = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("Classic_Median_O-R-F_Low")}.Classic Median O-R-F Low_Data"),
                                NarrowInfo = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("Classic 2L Pillar (Low)")}.Classic 2L Pillar (Low)_Data"),
                                WideMedianInfoNoCol = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("Classic 4L Pillar NoCol (Low)")}.Classic 4L Pillar NoCol (Low)_Data"),
                                WideInfoNoCol = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("Classic_Wide_O-R-F_Low_NoCol")}.Classic Wide O-R-F Low NoCol_Data"),
                                NarrowMedianInfoNoCol = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("Classic_Median_O-R-F_Low_NoCol")}.Classic Median O-R-F Low NoCol_Data"),
                                NarrowInfoNoCol = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("Classic 2L Pillar NoCol (Low)")}.Classic 2L Pillar NoCol (Low)_Data"),
                                HeightLimit = 18,
                                HeightOffset = 0
                            });
                            bridgeAI.pillarList.Add(new BridgePillarItem()
                            {
                                WideMedianInfo = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("Classic 4L Pillar (High)")}.Classic 4L Pillar (High)_Data"),
                                WideInfo = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("Classic_Wide_O-R-F_High")}.Classic Wide O-R-F High_Data"),
                                NarrowMedianInfo = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("Classic_Median_O-R-F_High")}.Classic Median O-R-F High_Data"),
                                NarrowInfo = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("Classic 2L Pillar (High)")}.Classic 2L Pillar (High)_Data"),
                                WideMedianInfoNoCol = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("Classic 4L Pillar NoCol (High)")}.Classic 4L Pillar NoCol (High)_Data"),
                                WideInfoNoCol = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("Classic_Wide_O-R-F_High_NoCol")}.Classic Wide O-R-F High NoCol_Data"),
                                NarrowMedianInfoNoCol = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("Classic_Median_O-R-F_High_NoCol")}.Classic Median O-R-F High NoCol_Data"),
                                NarrowInfoNoCol = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("Classic 2L Pillar NoCol (High)")}.Classic 2L Pillar NoCol (High)_Data"),
                                HeightLimit = 60,
                                HeightOffset = 0
                            });
                        }

                        //var pillarPropList = new List<BridgePillarPropItem>();

                        //pillarPropList.Add(new BridgePillarPropItem() { HeightLimit = 12, RepeatDistance = 60, Position = new Vector3(0, -15.75f, 0), Prop = lowPillarProp });
                        //pillarPropList.Add(new BridgePillarPropItem() { HeightLimit = 60, RepeatDistance = 60, Position = new Vector3(0, -24.9f, 0), Prop = highPillarProp });
                        //var lanes = prefab.m_lanes.ToList();
                        //var propLane = lanes.FirstOrDefault(l => l.m_laneType == NetInfo.LaneType.None);
                        //propLane.m_laneProps = ScriptableObject.CreateInstance<NetLaneProps>();
                        //var propsList = new List<NetLaneProps.Prop>();
                        //if (pillarPropList != null && pillarPropList.Count > 0)
                        //{
                        //    for (var i = 0; i < pillarPropList.Count; i++)
                        //    {
                        //        var thePillarPropInfo = pillarPropList[i];
                        //        if (thePillarPropInfo != null)
                        //        {
                        //            var prop = new NetLaneProps.Prop();
                        //            prop.m_prop = thePillarPropInfo.Prop;
                        //            prop.m_position = thePillarPropInfo.Position;
                        //            prop.m_finalProp = thePillarPropInfo.Prop;
                        //            prop.m_probability = 0;
                        //            prop.m_repeatDistance = thePillarPropInfo.RepeatDistance;
                        //            prop.m_segmentOffset = thePillarPropInfo.SegmentOffset;
                        //            propsList.Add(prop);
                        //        }
                        //    }
                        //}

                        //propLane.m_laneProps.m_props = propsList.ToArray();
                        break;
                    }
                case NetInfoVersion.Bridge:
                    {
                        var steelBridgePillarInfo = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName($"Classic {laneCount}L Pillar (Bridge)")}.Classic {laneCount}L Pillar (Bridge)_Data");
                        var steelNoColBridgePillarInfo = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName($"Classic {laneCount}L Pillar NoCol (Bridge)")}.Classic {laneCount}L Pillar NoCol (Bridge)_Data");
                        if (steelBridgePillarInfo == null)
                        {
                            throw new Exception($"{prefab.name}: MetroBridgePillar not found!");
                        }
                        var bridgeAI = prefab.GetComponent<TrainTrackBridgeAIMetro>();
                        if (bridgeAI != null)
                        {
                            bridgeAI.pillarType = PillarType.WideMedian;
                            bridgeAI.m_bridgePillarInfo = steelBridgePillarInfo;
                            bridgeAI.m_middlePillarInfo = steelBridgePillarInfo;
                            bridgeAI.pillarList = new List<BridgePillarItem>();
                            bridgeAI.pillarList.Add(new BridgePillarItem() { HeightLimit = 0, HeightOffset = 0, WideMedianInfo = steelBridgePillarInfo, WideMedianInfoNoCol = steelNoColBridgePillarInfo });
                            bridgeAI.m_bridgePillarOffset = 0.55f;
                        }
                        break;
                    }
            }
        }

    }
}

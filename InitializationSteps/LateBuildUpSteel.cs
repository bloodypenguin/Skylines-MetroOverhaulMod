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
                        var bridgeAI = prefab.GetComponent<TrainTrackBridgeAIMetro>();
                        if (bridgeAI != null)
                        {
                            var narrowInfo = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("Classic_Narrow_Low")}.Classic Narrow (Low)_Data");
                            bridgeAI.m_bridgePillarInfo = narrowInfo;
                            bridgeAI.m_bridgePillarOffset = -0.75f;
                            bridgeAI.pillarList = new List<BridgePillarItem>();
                            bridgeAI.pillarList.Add(new BridgePillarItem()
                            {
                                NarrowInfo = narrowInfo,
                                WideMedianInfo = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("Classic_Wide_Median_Low")}.Classic Wide Median (Low)_Data"),
                                WideInfo = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("Classic_Wide_Low")}.Classic Wide (Low)_Data"),
                                NarrowMedianInfo = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("Classic_Median_Low")}.Classic Median (Low)_Data"),
                                WideMedianInfoNoCol = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("Classic_Wide_Median_Low_NoCol")}.Classic Wide Median (Low) NoCol_Data"),
                                WideInfoNoCol = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("Classic_Wide_Low_NoCol")}.Classic Wide (Low) NoCol_Data"),
                                NarrowMedianInfoNoCol = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("Classic_Median_Low_NoCol")}.Classic Median (Low) NoCol_Data"),
                                NarrowInfoNoCol = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("Classic_Narrow_Low_NoCol")}.Classic Narrow (Low) NoCol_Data"),
                                HeightLimit = 18,
                                HeightOffset = 0
                            });
                            bridgeAI.pillarList.Add(new BridgePillarItem()
                            {
                                WideMedianInfo = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("Classic_Wide_Median_High")}.Classic Wide Median (High)_Data"),
                                WideInfo = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("Classic_Wide_High")}.Classic Wide (High)_Data"),
                                NarrowMedianInfo = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("Classic_Median_High")}.Classic Median (High)_Data"),
                                NarrowInfo = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("Classic_Narrow_High")}.Classic Narrow (High)_Data"),
                                WideMedianInfoNoCol = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("Classic_Wide_Median_High_NoCol")}.Classic Wide Median (High) NoCol_Data"),
                                WideInfoNoCol = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("Classic_Wide_High_NoCol")}.Classic Wide (High) NoCol_Data"),
                                NarrowMedianInfoNoCol = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("Classic_Median_High_NoCol")}.Classic Median (High) NoCol_Data"),
                                NarrowInfoNoCol = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("Classic_Narrow_High_NoCol")}.Classic Narrow (High) NoCol_Data"),
                                HeightLimit = 60,
                                HeightOffset = 0
                            });
                        }

                        //var pillarPropList = new List<BridgePillarPropItem>();

                        //pillarPropList.Add(new BridgePillarPropItem() { HeightLimit = 12, RepeatDistance = 60, Position = new Vector3(0, -15.75f, 0), Prop = (Low)PillarProp });
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
                        //var steelNoColBridgePillarInfo = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName($"Classic {laneCount}L Pillar NoCol (Bridge)")}.Classic {laneCount}L Pillar NoCol (Bridge)_Data");
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
                            bridgeAI.pillarList.Add(new BridgePillarItem() { HeightLimit = 0, HeightOffset = 0, WideMedianInfo = steelBridgePillarInfo, WideMedianInfoNoCol = steelBridgePillarInfo });
                            bridgeAI.m_bridgePillarOffset = 0.55f;
                        }
                        break;
                    }
                case NetInfoVersion.Tunnel:
                    {
                        if (prefab.name.Contains("Station"))
                        {
                            CustomizationSteps.SetupStationProps(prefab, version);
                        }
                        else
                        {
                            CustomizationSteps.SetupTrackProps(prefab, version);
                        }
                        break;
                    }
            }
        }

    }
}

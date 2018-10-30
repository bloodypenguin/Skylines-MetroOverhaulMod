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
            switch (version)
            {
                case NetInfoVersion.Elevated:
                    {
                        var pillarDict = new Dictionary<int, BuildingInfo>();
                        var laneCount = prefab.m_lanes.Where(l => l.m_vehicleType == VehicleInfo.VehicleType.Metro).Count();
                        var lowPillar = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName($"Classic {laneCount}L Pillar (Low)")}.Classic {laneCount}L Pillar (Low)_Data");
                        var highPillar = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName($"Classic {laneCount}L Pillar (High)")}.Classic {laneCount}L Pillar (High)_Data");
                        var lowPillarProp = PrefabCollection<PropInfo>.FindLoaded($"{Util.PackageName($"Classic {laneCount}L Pillar (Low) Prop")}.Classic {laneCount}L Pillar (Low) Prop_Data");
                        var highPillarProp = PrefabCollection<PropInfo>.FindLoaded($"{Util.PackageName($"Classic {laneCount}L Pillar (High) Prop")}.Classic {laneCount}L Pillar (High) Prop_Data");
                        if (lowPillar == null)
                        {
                            throw new Exception($"{prefab.name}: SteelMetroElevatedPillar not found!");
                        }
                        var bridgeAI = prefab.GetComponent<TrainTrackBridgeAIMetro>();
                        if (bridgeAI != null)
                        {
                            bridgeAI.m_bridgePillarInfo = lowPillar;
                            if (highPillar != null)
                            {
                                bridgeAI.pillarList = new List<BridgePillarItem>();
                                bridgeAI.pillarList.Add(new BridgePillarItem() { HeightLimit = 12, HeightOffset = 0, info = lowPillar });
                                bridgeAI.pillarList.Add(new BridgePillarItem() { HeightLimit = 60, HeightOffset = 0, info = highPillar });
                            }
                            // bridgeAI.m_bridgePillarOffset = 0.75f;
                        }

                        var pillarPropList = new List<BridgePillarPropItem>();
                        pillarPropList.Add(new BridgePillarPropItem() { HeightLimit = 12, RepeatDistance = 60, Position = new Vector3(0,-15.75f, 0), Prop = lowPillarProp });
                        pillarPropList.Add(new BridgePillarPropItem() { HeightLimit = 60, RepeatDistance = 60, Position = new Vector3(0,-24.9f, 0), Prop = highPillarProp });
                        var lanes = prefab.m_lanes.ToList();
                        var propLane = lanes.FirstOrDefault(l => l.m_laneType == NetInfo.LaneType.None);
                        propLane.m_laneProps = ScriptableObject.CreateInstance<NetLaneProps>();
                        var propsList = new List<NetLaneProps.Prop>();
                        if (pillarPropList != null && pillarPropList.Count > 0)
                        {
                            for (var i = 0; i < pillarPropList.Count; i++)
                            {
                                var thePillarPropInfo = pillarPropList[i];
                                if (thePillarPropInfo != null)
                                {
                                    var prop = new NetLaneProps.Prop();
                                    prop.m_prop = thePillarPropInfo.Prop;
                                    prop.m_position = thePillarPropInfo.Position;
                                    prop.m_finalProp = thePillarPropInfo.Prop;
                                    prop.m_probability = 0;
                                    prop.m_repeatDistance = thePillarPropInfo.RepeatDistance;
                                    prop.m_segmentOffset = thePillarPropInfo.SegmentOffset;
                                    propsList.Add(prop);
                                }
                            }
                        }

                        propLane.m_laneProps.m_props = propsList.ToArray();
                        break;
                    }
                case NetInfoVersion.Bridge:
                    {
                        var laneCount = prefab.m_lanes.Where(l => l.m_vehicleType == VehicleInfo.VehicleType.Metro).Count();
                        var steelBridgePillarInfo = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName($"Classic {laneCount}L Pillar (Bridge)")}.Classic {laneCount}L Pillar (Bridge)_Data");
                        if (steelBridgePillarInfo == null)
                        {
                            throw new Exception($"{prefab.name}: MetroBridgePillar not found!");
                        }
                        var bridgeAI = prefab.GetComponent<TrainTrackBridgeAI>();
                        if (bridgeAI != null)
                        {
                            bridgeAI.m_bridgePillarInfo = steelBridgePillarInfo;
                            bridgeAI.m_middlePillarInfo = steelBridgePillarInfo;
                            bridgeAI.m_bridgePillarOffset = 0.55f;
                        }
                        break;
                    }
            }
        }

    }
}

using System;
using MetroOverhaul.NEXT;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace MetroOverhaul.InitializationSteps {
    public static class LateBuildUpSteel
    {
        public static void BuildUp(NetInfo prefab, NetInfoVersion version)
        {
            var laneCount = prefab.m_lanes.Where(l => l.m_vehicleType == VehicleInfo.VehicleType.Metro).GroupBy(g => Math.Round(g.m_position)).Count();
            switch (version)
            {
                case NetInfoVersion.Elevated:
                    {

                        var lowPillar = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName($"Classic {laneCount}L Pillar (Low)")}.Classic {laneCount}L Pillar (Low)_Data");
                        var lowPillarNoCol = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName($"Classic {laneCount}L Pillar NoCol (Low)")}.Classic {laneCount}L Pillar NoCol (Low)_Data");

                        var highPillar = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName($"Classic {laneCount}L Pillar (High)")}.Classic {laneCount}L Pillar (High)_Data");
                        var highPillarNoCol = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName($"Classic {laneCount}L Pillar NoCol (High)")}.Classic {laneCount}L Pillar NoCol (High)_Data");

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
                                bridgeAI.pillarList.Add(new BridgePillarItem() { HeightLimit = 18, HeightOffset = 0, info = lowPillar, noCollisionInfo = lowPillarNoCol });
                                bridgeAI.pillarList.Add(new BridgePillarItem() { HeightLimit = 60, HeightOffset = 0, info = highPillar, noCollisionInfo = highPillarNoCol });
                            }
                            bridgeAI.m_bridgePillarOffset = 0.75f;
                        }

                        var pillarPropList = new List<BridgePillarPropItem>();

                        pillarPropList.Add(new BridgePillarPropItem() { HeightLimit = 12, RepeatDistance = 60, Position = new Vector3(0, -15.75f, 0), Prop = lowPillarProp });
                        pillarPropList.Add(new BridgePillarPropItem() { HeightLimit = 60, RepeatDistance = 60, Position = new Vector3(0, -24.9f, 0), Prop = highPillarProp });
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
                        var steelBridgePillarInfo = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName($"Classic {laneCount}L Pillar (Bridge)")}.Classic {laneCount}L Pillar (Bridge)_Data");
                        var steelNoColBridgePillarInfo = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName($"Classic {laneCount}L Pillar NoCol (Bridge)")}.Classic {laneCount}L Pillar NoCol (Bridge)_Data");
                        if (steelBridgePillarInfo == null)
                        {
                            throw new Exception($"{prefab.name}: MetroBridgePillar not found!");
                        }
                        var bridgeAI = prefab.GetComponent<TrainTrackBridgeAIMetro>();
                        if (bridgeAI != null)
                        {
                            bridgeAI.m_bridgePillarInfo = steelBridgePillarInfo;
                            bridgeAI.m_middlePillarInfo = steelBridgePillarInfo;
                            bridgeAI.pillarList = new List<BridgePillarItem>();
                            bridgeAI.pillarList.Add(new BridgePillarItem() { HeightLimit = 0, HeightOffset = 0, info = steelBridgePillarInfo, noCollisionInfo = steelNoColBridgePillarInfo });
                            bridgeAI.m_bridgePillarOffset = 0.55f;
                        }
                        break;
                    }
            }
        }

    }
}

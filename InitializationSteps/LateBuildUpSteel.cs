using System;
using MetroOverhaul.NEXT;
using UnityEngine;
using System.Collections.Generic;

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
                        var laneCount = prefab.name.ToLower().Contains("small") ? 1 : 2;
                        var lowPillar = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName($"Classic {laneCount}L Pillar (Low)")}.Classic {laneCount}L Pillar (Low)_Data");
                        var highPillar = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName($"Classic {laneCount}L Pillar (High)")}.Classic {laneCount}L Pillar (High)_Data");
                        if (lowPillar == null)
                        {
                            throw new Exception($"{prefab.name}: SteelMetroElevatedPillar not found!");
                        }
                        var bridgeAI = prefab.GetComponent<TrainTrackBridgeAIMetro>();
                        if (bridgeAI != null)
                        {
                            bridgeAI.m_bridgePillarInfo = lowPillar;
                            if (highPillar!= null)
                            {
                                bridgeAI.pillarList = new List<BridgePillarItem>();
                                bridgeAI.pillarList.Add(new BridgePillarItem() { HeightLimit = 15, HeightOffset = 0, info = lowPillar });
                                bridgeAI.pillarList.Add(new BridgePillarItem() { HeightLimit = 60, HeightOffset = 0, info = highPillar });
                            }
                           // bridgeAI.m_bridgePillarOffset = 0.75f;
                        }
                        break;
                    }
                case NetInfoVersion.Bridge:
                    {
                        var laneCount = prefab.name.ToLower().Contains("small") ? 1 : 2;
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

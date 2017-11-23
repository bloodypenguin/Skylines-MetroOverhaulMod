﻿using MetroOverhaul.NEXT;
using System;

namespace MetroOverhaul.InitializationSteps
{
    public static class LateBuildUp
    {
        public static void BuildUp(NetInfo prefab, NetInfoVersion version)
        {
            var smallWord = prefab.name.ToLower().Contains("small") ? "Small" : "";
            switch (version)
            {
                case NetInfoVersion.Elevated:
                    {
                        var epPropInfo = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName($"MetroElevatedPillar{smallWord}")}.MetroElevatedPillar{smallWord}_Data");
                        if (epPropInfo == null)
                        {
                            throw new Exception($"{prefab.name}: MetroElevatedPillar not found!");
                        }
                        var bridgeAI = prefab.GetComponent<TrainTrackBridgeAI>();
                        if (bridgeAI != null)
                        {
                            bridgeAI.m_bridgePillarInfo = epPropInfo;
                            bridgeAI.m_bridgePillarOffset = -0.75f;
                        }
                        break;
                    }
                case NetInfoVersion.Bridge:
                    {
                        var bpPropInfo = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName($"MetroBridgePillar{smallWord}")}.MetroBridgePillar{smallWord}_Data");
                        if (bpPropInfo == null)
                        {
                            throw new Exception($"{prefab.name}: MetroBridgePillar not found!");
                        }
                        var bridgeAI = prefab.GetComponent<TrainTrackBridgeAI>();
                        if (bridgeAI != null)
                        {
                            bridgeAI.m_bridgePillarInfo = bpPropInfo;
                        }
                        break;
                    }
            }
        }

    }
}

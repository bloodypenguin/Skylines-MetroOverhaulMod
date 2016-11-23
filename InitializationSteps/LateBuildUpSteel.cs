using System;
using MetroOverhaul.NEXT;
using UnityEngine;

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
                        var steelElevatedPillarInfo = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("SteelMetroElevatedPillar")}.SteelMetroElevatedPillar_Data");
                        if (steelElevatedPillarInfo == null)
                        {
                            throw new Exception($"{prefab.name}: SteelMetroElevatedPillar not found!");
                        }
                        var bridgeAI = prefab.GetComponent<TrainTrackBridgeAI>();
                        if (bridgeAI != null)
                        {
                            bridgeAI.m_bridgePillarInfo = steelElevatedPillarInfo;
                            bridgeAI.m_bridgePillarOffset = 0.75f;
                        }
                        break;
                    }
                case NetInfoVersion.Bridge:
                    {
                        var steelBridgePillarInfo = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("SteelMetroBridgePillar")}.SteelMetroBridgePillar_Data");
                        if (steelBridgePillarInfo == null)
                        {
                            throw new Exception($"{prefab.name}: SteelMetroBridgePillar not found!");
                        }
                        var bridgeAI = prefab.GetComponent<TrainTrackBridgeAI>();
                        if (bridgeAI != null)
                        {
                            bridgeAI.m_bridgePillarInfo = steelBridgePillarInfo;
                            bridgeAI.m_middlePillarInfo = steelBridgePillarInfo;
                        }
                        break;
                    }
            }
        }

    }
}

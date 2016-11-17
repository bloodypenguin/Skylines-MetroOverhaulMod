using MetroOverhaul.NEXT;
using System;

namespace MetroOverhaul.InitializationSteps
{
    public static class LateBuildUp
    {
        public static void BuildUp(NetInfo prefab, NetInfoVersion version)
        {
            switch (version)
            {
                case NetInfoVersion.Elevated:
                    {
                        var epPropInfo = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("MetroElevatedPillar")}.MetroElevatedPillar_Data");
                        if (epPropInfo == null)
                        {
                            throw new Exception($"{prefab.name}: MetroElevatedPillar not found!");
                        }
                        var bridgeAI = prefab.GetComponent<TrainTrackBridgeAI>();
                        if (bridgeAI != null)
                        {
                            bridgeAI.m_bridgePillarInfo = epPropInfo;
                        }
                        break;
                    }
                case NetInfoVersion.Bridge:
                    {
                        var bpPropInfo = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName("MetroBridgePillar")}.MetroBridgePillar_Data");
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

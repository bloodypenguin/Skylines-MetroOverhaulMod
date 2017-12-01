using MetroOverhaul.NEXT;
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
                        var epBuildingInfo = PrefabCollection<BuildingInfo>.FindLoaded($"{Util.PackageName($"MetroElevatedPillar{smallWord}")}.MetroElevatedPillar{smallWord}_Data");
                        var epPropInfo = PrefabCollection<PropInfo>.FindLoaded($"{Util.PackageName($"MetroElevatedPillar{smallWord}Prop")}.MetroElevatedPillar{smallWord}Prop_Data");
                        if (epBuildingInfo == null)
                        {
                            throw new Exception($"{prefab.name}: MetroElevatedPillar not found!");
                        }
                        var bridgeAI = prefab.GetComponent<TrainTrackBridgeAIMetro>();
                        if (bridgeAI != null)
                        {
                            bridgeAI.m_bridgePillarInfo = epBuildingInfo;
                            bridgeAI.m_bridgePillarOffset = -0.75f;
                            if (epPropInfo != null)
                            {
                                bridgeAI.m_ElevatedPillarPropInfo = epPropInfo;
                            }
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

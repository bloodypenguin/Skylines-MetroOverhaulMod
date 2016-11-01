using MetroOverhaul.NEXT;

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
                        var steelElevatedPillarInfo = PrefabCollection<BuildingInfo>.FindLoaded("Steel Metro Elevated Pillar");

                        var bridgeAI = prefab.GetComponent<TrainTrackBridgeAI>();
                        if (steelElevatedPillarInfo != null && bridgeAI != null)
                        {
                            bridgeAI.m_bridgePillarInfo = steelElevatedPillarInfo;
                            bridgeAI.m_bridgePillarOffset = 2;
                        }
                        break;
                    }
                case NetInfoVersion.Bridge:
                    {
                        var steelBridgePillarInfo = PrefabCollection<BuildingInfo>.FindLoaded("Steel Metro Bridge Pillar");

                        var bridgeAI = prefab.GetComponent<TrainTrackBridgeAI>();
                        if (steelBridgePillarInfo != null && bridgeAI != null)
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

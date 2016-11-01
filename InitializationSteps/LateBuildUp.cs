using MetroOverhaul.NEXT;

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
                        var elevatedPillarInfo = PrefabCollection<BuildingInfo>.FindLoaded("Metro Elevated Pillar");
                        var bridgeAI = prefab.GetComponent<TrainTrackBridgeAI>();
                        if (elevatedPillarInfo != null && bridgeAI != null)
                        {
                            bridgeAI.m_bridgePillarInfo = elevatedPillarInfo;
                        }
                        break;
                    }
                case NetInfoVersion.Bridge:
                    {
                        var bridgePillarInfo = PrefabCollection<BuildingInfo>.FindLoaded("Metro Bridge Pillar");
                        var bridgeAI = prefab.GetComponent<TrainTrackBridgeAI>();
                        if (bridgePillarInfo != null && bridgeAI != null)
                        {
                            bridgeAI.m_bridgePillarInfo = bridgePillarInfo;
                        }
                        break;
                    }
            }
        }

    }
}

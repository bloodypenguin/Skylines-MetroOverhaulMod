using MetroOverhaul.NEXT;

namespace MetroOverhaul.InitializationSteps {
    public static class CommonSteps
    {

        public static void SetVersion(NetInfo versionedPrefab, NetInfo prefab, NetInfoVersion version)
        {
            var trainTrackAi = prefab?.GetComponent<TrainTrackAI>();
            if (trainTrackAi == null)
            {
                return;
            }
            switch (version)
            {
                case NetInfoVersion.Tunnel:
                    trainTrackAi.m_tunnelInfo = versionedPrefab;
                    break;
                case NetInfoVersion.Slope:
                    trainTrackAi.m_slopeInfo = versionedPrefab;
                    break;
                case NetInfoVersion.Bridge:
                    trainTrackAi.m_bridgeInfo = versionedPrefab;
                    break;
                case NetInfoVersion.Elevated:
                    trainTrackAi.m_elevatedInfo = versionedPrefab;
                    break;
            }
        }
    }
}

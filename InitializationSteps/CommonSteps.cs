using MetroOverhaul.NEXT;

namespace MetroOverhaul.InitializationSteps {
    public static class CommonSteps
    {

        public static void SetVersion(NetInfo versionedPrefab, NetInfo prefab, NetInfoVersion version)
        {
            var MetroTrackAI = prefab?.GetComponent<MetroTrackAI>();
            if (MetroTrackAI == null)
            {
                return;
            }
            switch (version)
            {
                case NetInfoVersion.Tunnel:
                    MetroTrackAI.m_tunnelInfo = versionedPrefab;
                    break;
                case NetInfoVersion.Slope:
                    MetroTrackAI.m_slopeInfo = versionedPrefab;
                    break;
                case NetInfoVersion.Bridge:
                    MetroTrackAI.m_bridgeInfo = versionedPrefab;
                    break;
                case NetInfoVersion.Elevated:
                    MetroTrackAI.m_elevatedInfo = versionedPrefab;
                    break;
            }
        }
    }
}

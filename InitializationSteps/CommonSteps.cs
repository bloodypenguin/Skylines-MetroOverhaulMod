using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetroOverhaul.NEXT;
using UnityEngine;

namespace MetroOverhaul.InitializationSteps
{
    public static class CommonSteps
    {

        public static void SetVersion(NetInfo versionedPrefab, NetInfo prefab, NetInfoVersion version)
        {
            if (prefab == null)
            {
                return;
            }
            switch (version)
            {
                case NetInfoVersion.Tunnel:
                    prefab.GetComponent<TrainTrackAI>().m_tunnelInfo = versionedPrefab;
                    break;
                case NetInfoVersion.Slope:
                    prefab.GetComponent<TrainTrackAI>().m_slopeInfo = versionedPrefab;
                    break;
                case NetInfoVersion.Bridge:
                    prefab.GetComponent<TrainTrackAI>().m_bridgeInfo = versionedPrefab;
                    break;
                case NetInfoVersion.Elevated:
                    prefab.GetComponent<TrainTrackAI>().m_elevatedInfo = versionedPrefab;
                    break;
            }
        }
    }
}

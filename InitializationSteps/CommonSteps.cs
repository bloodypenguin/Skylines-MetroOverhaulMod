using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MetroOverhaul.InitializationSteps
{
    public static class CommonSteps
    {

        public static Action<NetInfo> SetTunnel(Component p)
        {
            return p4 => p.GetComponent<TrainTrackAI>().m_tunnelInfo = p4;
        }

        public static Action<NetInfo> SetSlope(Component p)
        {
            return p3 => p.GetComponent<TrainTrackAI>().m_slopeInfo = p3;
        }

        public static Action<NetInfo> SetElevated(Component p)
        {
            return p2 => p.GetComponent<TrainTrackAI>().m_elevatedInfo = p2;
        }

        public static Action<NetInfo> SetBridge(Component p)
        {
            return p1 => p.GetComponent<TrainTrackAI>().m_bridgeInfo = p1;
        }
    }
}

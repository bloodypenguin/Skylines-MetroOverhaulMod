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

        public static void SetTunnel(NetInfo p4, NetInfo p)
        {
            p.GetComponent<TrainTrackAI>().m_tunnelInfo = p4;
        }

        public static void SetSlope(NetInfo p3, NetInfo p)
        {
            p.GetComponent<TrainTrackAI>().m_slopeInfo = p3;
        }

        public static void SetElevated(NetInfo p2, NetInfo p)
        {
            p.GetComponent<TrainTrackAI>().m_elevatedInfo = p2;
        }

        public static void SetBridge(NetInfo p1, NetInfo p)
        {
            p.GetComponent<TrainTrackAI>().m_bridgeInfo = p1;
        }
    }
}

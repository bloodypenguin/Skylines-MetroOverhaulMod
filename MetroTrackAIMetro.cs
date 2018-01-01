using ColossalFramework;
using UnityEngine;

namespace MetroOverhaul
{
    public class MetroTrackAIMetro : MetroTrackAI
    {
        //taken from MetroAI
        public override Color GetColor(ushort segmentID, ref NetSegment data, InfoManager.InfoMode infoMode)
        {
            switch (infoMode)
            {
                case InfoManager.InfoMode.Transport:
                    return Singleton<TransportManager>.instance.m_properties.m_transportColors[(int)TransportInfo.TransportType.Metro] * 0.2f;
                case InfoManager.InfoMode.Traffic:
                    return Color.Lerp(Singleton<InfoManager>.instance.m_properties.m_modeProperties[(int)infoMode].m_targetColor, Singleton<InfoManager>.instance.m_properties.m_modeProperties[(int)infoMode].m_negativeColor, Mathf.Clamp01((float)data.m_trafficDensity * 0.01f)) * 0.2f;
                case InfoManager.InfoMode.Maintenance:
                    return Singleton<TransportManager>.instance.m_properties.m_transportColors[(int)TransportInfo.TransportType.Metro] * 0.2f;
                default:
                    return base.GetColor(segmentID, ref data, infoMode);
            }
        }

        //taken from MetroAI
        public override Color GetColor(ushort nodeID, ref NetNode data, InfoManager.InfoMode infoMode)
        {
            switch (infoMode)
            {
                case InfoManager.InfoMode.Transport:
                    return Singleton<TransportManager>.instance.m_properties.m_transportColors[(int)TransportInfo.TransportType.Metro] * 0.2f;
                case InfoManager.InfoMode.Traffic:
                    NetManager instance = Singleton<NetManager>.instance;
                    int num1 = 0;
                    int num2 = 0;
                    for (int index = 0; index < 8; ++index)
                    {
                        ushort segment = data.GetSegment(index);
                        if ((int)segment != 0)
                        {
                            num1 += (int)instance.m_segments.m_buffer[(int)segment].m_trafficDensity;
                            ++num2;
                        }
                    }
                    if (num2 != 0)
                        num1 /= num2;
                    return Color.Lerp(Singleton<InfoManager>.instance.m_properties.m_modeProperties[(int)infoMode].m_targetColor, Singleton<InfoManager>.instance.m_properties.m_modeProperties[(int)infoMode].m_negativeColor, Mathf.Clamp01((float)num1 * 0.01f)) * 0.2f;
                case InfoManager.InfoMode.Maintenance:
                    return Singleton<TransportManager>.instance.m_properties.m_transportColors[(int)TransportInfo.TransportType.Metro] * 0.2f;
                default:
                    return base.GetColor(nodeID, ref data, infoMode);
            }
        }

        public override void UpdateNodeFlags(ushort nodeID, ref NetNode data)
        {
            base.UpdateNodeFlags(nodeID, ref data);
            NetNode.Flags flags = data.m_flags & ~(NetNode.Flags.Transition | NetNode.Flags.LevelCrossing | NetNode.Flags.TrafficLights);
            int num = 0;
            int num2 = 0;
            uint num3 = 0u;
            int num4 = 0;
            NetManager instance = Singleton<NetManager>.instance;
            for (int i = 0; i < 8; i++)
            {
                ushort segment = data.GetSegment(i);
                if (segment != 0)
                {
                    NetInfo info = instance.m_segments.m_buffer[(int)segment].Info;
                    if (info != null)
                    {
                        uint num8 = 1u << (int)info.m_class.m_level;
                        if ((num3 & num8) == 0u)
                        {
                            num3 |= num8;
                            num4++;
                        }
                        if (info.m_createPavement)
                        {
                            flags |= NetNode.Flags.Transition;
                        }
                        if (info.m_class.m_service == ItemClass.Service.Road)
                        {
                            num++;
                        }
                        else if (info.m_class.m_service == ItemClass.Service.PublicTransport)
                        {
                            num2++;
                        }
                    }
                }
            }
            if (num2 >= 2 && num >= 1)
            {
                flags |= (NetNode.Flags.LevelCrossing | NetNode.Flags.TrafficLights);
            }
            if (num2 >= 2 && num4 >= 2)
            {
                flags |= NetNode.Flags.Transition;
            }
            else
            {
                flags &= ~NetNode.Flags.Transition;
            }
            data.m_flags = flags;
        }

    }
}
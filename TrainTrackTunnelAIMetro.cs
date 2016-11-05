using ColossalFramework;
using UnityEngine;

namespace MetroOverhaul
{
    public class TrainTrackTunnelAIMetro : TrainTrackTunnelAI
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

    }
}
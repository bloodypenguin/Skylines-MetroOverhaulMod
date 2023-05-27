using ColossalFramework;
using UnityEngine;

namespace MetroOverhaul
{
    public class MetroTrackAIMetro : MetroTrackAI
    {
        //taken from MetroAI
        //public override Color GetColor(ushort segmentID, ref NetSegment data, InfoManager.InfoMode infoMode)
        //{
        //    switch (infoMode)
        //    {
        //        case InfoManager.InfoMode.Transport:
        //            return Singleton<TransportManager>.instance.m_properties.m_transportColors[(int)TransportInfo.TransportType.Metro] * 0.2f;
        //        case InfoManager.InfoMode.Traffic:
        //            return Color.Lerp(Singleton<InfoManager>.instance.m_properties.m_modeProperties[(int)infoMode].m_targetColor, Singleton<InfoManager>.instance.m_properties.m_modeProperties[(int)infoMode].m_negativeColor, Mathf.Clamp01((float)data.m_trafficDensity * 0.01f)) * 0.2f;
        //        case InfoManager.InfoMode.Maintenance:
        //            return Singleton<TransportManager>.instance.m_properties.m_transportColors[(int)TransportInfo.TransportType.Metro] * 0.2f;
        //        default:
        //            return base.GetColor(segmentID, ref data, infoMode);
        //    }
        //}

        ////taken from MetroAI
        //public override Color GetColor(ushort nodeID, ref NetNode data, InfoManager.InfoMode infoMode)
        //{
        //    switch (infoMode)
        //    {
        //        case InfoManager.InfoMode.Transport:
        //            return Singleton<TransportManager>.instance.m_properties.m_transportColors[(int)TransportInfo.TransportType.Metro] * 0.2f;
        //        case InfoManager.InfoMode.Traffic:
        //            NetManager instance = Singleton<NetManager>.instance;
        //            int num1 = 0;
        //            int num2 = 0;
        //            for (int index = 0; index < 8; ++index)
        //            {
        //                ushort segment = data.GetSegment(index);
        //                if ((int)segment != 0)
        //                {
        //                    num1 += (int)instance.m_segments.m_buffer[(int)segment].m_trafficDensity;
        //                    ++num2;
        //                }
        //            }
        //            if (num2 != 0)
        //                num1 /= num2;
        //            return Color.Lerp(Singleton<InfoManager>.instance.m_properties.m_modeProperties[(int)infoMode].m_targetColor, Singleton<InfoManager>.instance.m_properties.m_modeProperties[(int)infoMode].m_negativeColor, Mathf.Clamp01((float)num1 * 0.01f)) * 0.2f;
        //        case InfoManager.InfoMode.Maintenance:
        //            return Singleton<TransportManager>.instance.m_properties.m_transportColors[(int)TransportInfo.TransportType.Metro] * 0.2f;
        //        default:
        //            return base.GetColor(nodeID, ref data, infoMode);
        //    }
        //}

        public override void UpdateNodeFlags(ushort nodeID, ref NetNode data)
        {
            base.UpdateNodeFlags(nodeID, ref data);
            NetNode.Flags flags = data.m_flags & ~(NetNode.Flags.Transition | NetNode.Flags.LevelCrossing | NetNode.Flags.TrafficLights);
            int num = 0;
            int num2 = 0;
            uint num3 = 0u;
            int num4 = 0;
            int num5 = 0;
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
                            if (info.name.ToLower().Contains("tram"))
                            {
                                num5++;
                            }
                            num++;
                        }
                        else if (info.m_class.m_service == ItemClass.Service.PublicTransport)
                        {
                            num2++;
                        }
                    }
                }
            }
            if (num2 >= 2 && num >= 1 && num5 == 0)
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
        public override ToolBase.ToolErrors CheckBuildPosition(bool test, bool visualize, bool overlay, bool autofix, ref NetTool.ControlPoint startPoint, ref NetTool.ControlPoint middlePoint, ref NetTool.ControlPoint endPoint, out BuildingInfo ownerBuilding, out Vector3 ownerPosition, out Vector3 ownerDirection, out int productionRate)
        {
            ToolBase.ToolErrors toolErrors = base.CheckBuildPosition(test, visualize, overlay, autofix, ref startPoint, ref middlePoint, ref endPoint, out ownerBuilding, out ownerPosition, out ownerDirection, out productionRate);
            if (test)
            {
                NetManager instance = Singleton<NetManager>.instance;
                ushort num = middlePoint.m_segment;
                if ((int)startPoint.m_segment == (int)num || (int)endPoint.m_segment == (int)num)
                    num = (ushort)0;
                //if ((int)num != 0 && (Singleton<NetManager>.instance.m_segments.m_buffer[(int)num].m_flags & NetSegment.Flags.Collapsed) == NetSegment.Flags.None)
                //{
                //    NetInfo info = instance.m_segments.m_buffer[(int)num].Info;
                //    if (this.m_connectedInfo == info)
                //        toolErrors |= ToolBase.ToolErrors.CannotUpgrade;
                //    if (this.m_elevatedInfo == info)
                //        toolErrors |= ToolBase.ToolErrors.CannotUpgrade;
                //    if (this.m_bridgeInfo == info)
                //        toolErrors |= ToolBase.ToolErrors.CannotUpgrade;
                //    if (this.m_tunnelInfo == info)
                //        toolErrors |= ToolBase.ToolErrors.CannotUpgrade;
                //    if (this.m_slopeInfo == info)
                //        toolErrors |= ToolBase.ToolErrors.CannotUpgrade;
                //}
                if ((int)startPoint.m_node != 0)
                {
                    for (int index = 0; index < 8; ++index)
                    {
                        ushort segment = instance.m_nodes.m_buffer[(int)startPoint.m_node].GetSegment(index);
                        if ((int)segment != 0 && (instance.m_segments.m_buffer[(int)segment].Info.m_vehicleTypes & VehicleInfo.VehicleType.Tram) != VehicleInfo.VehicleType.None)
                            toolErrors |= ToolBase.ToolErrors.CannotCrossTrack;
                    }
                }
                else if ((int)startPoint.m_segment != 0 && (instance.m_segments.m_buffer[(int)startPoint.m_segment].Info.m_vehicleTypes & VehicleInfo.VehicleType.Tram) != VehicleInfo.VehicleType.None)
                    toolErrors |= ToolBase.ToolErrors.CannotCrossTrack;
                if ((int)endPoint.m_node != 0)
                {
                    for (int index = 0; index < 8; ++index)
                    {
                        ushort segment = instance.m_nodes.m_buffer[(int)endPoint.m_node].GetSegment(index);
                        if ((int)segment != 0 && (instance.m_segments.m_buffer[(int)segment].Info.m_vehicleTypes & VehicleInfo.VehicleType.Tram) != VehicleInfo.VehicleType.None)
                            toolErrors |= ToolBase.ToolErrors.CannotCrossTrack;
                    }
                }
                else if ((int)endPoint.m_segment != 0 && (instance.m_segments.m_buffer[(int)endPoint.m_segment].Info.m_vehicleTypes & VehicleInfo.VehicleType.Tram) != VehicleInfo.VehicleType.None)
                    toolErrors |= ToolBase.ToolErrors.CannotCrossTrack;
            }
            return toolErrors;
        }
    }
}
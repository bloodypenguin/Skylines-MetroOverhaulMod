using ColossalFramework;
using MetroOverhaul.Redirection;
using MetroOverhaul.Redirection.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MetroOverhaul.Detours
{
    [TargetType(typeof(RoadAI))]
    class RoadAIDetour:RoadBaseAI
    {
        [RedirectMethod]
        public override ToolBase.ToolErrors CheckBuildPosition(bool test, bool visualize, bool overlay, bool autofix, ref NetTool.ControlPoint startPoint, ref NetTool.ControlPoint middlePoint, ref NetTool.ControlPoint endPoint, out BuildingInfo ownerBuilding, out Vector3 ownerPosition, out Vector3 ownerDirection, out int productionRate)
        {
            ToolBase.ToolErrors toolErrors = base.CheckBuildPosition(test, visualize, overlay, autofix, ref startPoint, ref middlePoint, ref endPoint, out ownerBuilding, out ownerPosition, out ownerDirection, out productionRate);
            if (test)
            {
                NetManager instance = Singleton<NetManager>.instance;
                ushort num = middlePoint.m_segment;
                if ((int)startPoint.m_segment == (int)num || (int)endPoint.m_segment == (int)num)
                    num = (ushort)0;
                if ((int)num != 0)
                {
                    NetInfo info = instance.m_segments.m_buffer[(int)num].Info;
                    //if ((Singleton<NetManager>.instance.m_segments.m_buffer[(int)num].m_flags & NetSegment.Flags.Collapsed) == NetSegment.Flags.None)
                    //{
                    //    if (this.m_elevatedInfo == info)
                    //        toolErrors |= ToolBase.ToolErrors.CannotUpgrade;
                    //    if (this.m_bridgeInfo == info)
                    //        toolErrors |= ToolBase.ToolErrors.CannotUpgrade;
                    //    if (this.m_tunnelInfo == info)
                    //        toolErrors |= ToolBase.ToolErrors.CannotUpgrade;
                    //    if (this.m_slopeInfo == info)
                    //        toolErrors |= ToolBase.ToolErrors.CannotUpgrade;
                    //}
                    if (info.m_netAI.IsUnderground() && !this.SupportUnderground())
                        toolErrors |= ToolBase.ToolErrors.CannotUpgrade;
                }
                if ((this.m_info.m_vehicleTypes & VehicleInfo.VehicleType.Tram) != VehicleInfo.VehicleType.None)
                {
                    if ((int)startPoint.m_node != 0)
                    {
                        for (int index = 0; index < 8; ++index)
                        {
                            ushort segment = instance.m_nodes.m_buffer[(int)startPoint.m_node].GetSegment(index);
                            if ((int)segment != 0 && (instance.m_segments.m_buffer[(int)segment].Info.m_vehicleTypes & (VehicleInfo.VehicleType.Train | VehicleInfo.VehicleType.Metro)) != VehicleInfo.VehicleType.None)
                                toolErrors |= ToolBase.ToolErrors.CannotCrossTrack;
                        }
                    }
                    else if ((int)startPoint.m_segment != 0 && (instance.m_segments.m_buffer[(int)startPoint.m_segment].Info.m_vehicleTypes & (VehicleInfo.VehicleType.Train | VehicleInfo.VehicleType.Metro)) != VehicleInfo.VehicleType.None)
                        toolErrors |= ToolBase.ToolErrors.CannotCrossTrack;
                    if ((int)endPoint.m_node != 0)
                    {
                        for (int index = 0; index < 8; ++index)
                        {
                            ushort segment = instance.m_nodes.m_buffer[(int)endPoint.m_node].GetSegment(index);
                            if ((int)segment != 0 && (instance.m_segments.m_buffer[(int)segment].Info.m_vehicleTypes & (VehicleInfo.VehicleType.Train | VehicleInfo.VehicleType.Metro)) != VehicleInfo.VehicleType.None)
                                toolErrors |= ToolBase.ToolErrors.CannotCrossTrack;
                        }
                    }
                    else if ((int)endPoint.m_segment != 0 && (instance.m_segments.m_buffer[(int)endPoint.m_segment].Info.m_vehicleTypes & (VehicleInfo.VehicleType.Train | VehicleInfo.VehicleType.Metro)) != VehicleInfo.VehicleType.None)
                        toolErrors |= ToolBase.ToolErrors.CannotCrossTrack;
                }
                //if (this.m_enableZoning && !Singleton<ZoneManager>.instance.CheckLimits())
                //    toolErrors |= ToolBase.ToolErrors.TooManyObjects;
            }
            return toolErrors;
        }
    }
}

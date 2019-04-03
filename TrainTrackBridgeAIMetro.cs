using ColossalFramework;
using ColossalFramework.Math;
using MetroOverhaul.Extensions;
using MetroOverhaul.NEXT.Extensions;
using MetroOverhaul.OptionsFramework;
using Next;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MetroOverhaul
{
    class TrainTrackBridgeAIMetro : TrainTrackBridgeAI
    {
        public List<BridgePillarItem> pillarList { get; set; }
        private NetTool m_NetTool = null;
        public ItemClass m_IntersectClass = null;
        public bool NoPillarCollision { get; set; }
        //public List<BridgePillarPropItem> pillarPropList { get; set; }
        //public PropInfo m_ElevatedPillarPropInfo { get; set; }
        private static NetNode NodeFrom(ushort nodeID)
        {
            return Singleton<NetManager>.instance.m_nodes.m_buffer[nodeID];
        }

        public override void GetNodeBuilding(ushort nodeID, ref NetNode data, out BuildingInfo building, out float heightOffset)
        {
            if ((data.m_flags & NetNode.Flags.Outside) == NetNode.Flags.None)
            {

                if (this.m_middlePillarInfo != null && (data.m_flags & NetNode.Flags.Double) != NetNode.Flags.None)
                {
                    building = this.m_middlePillarInfo;
                    heightOffset = this.m_middlePillarOffset - 1f - this.m_middlePillarInfo.m_generatedInfo.m_size.y;
                    return;
                }
                if (this.m_bridgePillarInfo != null)
                {
                    building = this.m_bridgePillarInfo;
                    heightOffset = this.m_bridgePillarOffset - 1f - this.m_bridgePillarInfo.m_generatedInfo.m_size.y;
                    if (pillarList != null && pillarList.Count > 0)
                    {
                        if (m_NetTool == null)
                        {
                            m_NetTool = FindObjectOfType<NetTool>();
                        }
                        if (m_NetTool != null) {
                            var elevation = m_NetTool.GetElevation();
                            var theList = pillarList.Where(d => d.HeightLimit == 0 || d.HeightLimit >= elevation).OrderBy(x => x.HeightLimit).ToList();
                            BridgePillarItem thePillarInfo = null;
                            if (theList == null || theList.Count == 0) {
                                thePillarInfo = pillarList.LastOrDefault();
                            } else {
                                thePillarInfo = theList.FirstOrDefault();
                            }
                            var prefab = m_NetTool.Prefab;
                            if (NoPillarCollision && elevation >= 0) {
                                building = thePillarInfo.noCollisionInfo;
                                building.m_elevated = true;
                                building.m_generatedInfo.m_min.y = 32;
                                if (m_IntersectClass == null) {
                                    m_IntersectClass = prefab.m_intersectClass;
                                }
                                prefab.m_intersectClass = null;
                            } else {
                                building = thePillarInfo.info;
                                if (m_IntersectClass != null) {
                                    prefab.m_intersectClass = m_IntersectClass;
                                }
                            }
                            heightOffset = thePillarInfo.HeightOffset - 1f - thePillarInfo.info.m_generatedInfo.m_size.y;
                        }
                    }
                    return;
                }
            }
            base.GetNodeBuilding(nodeID, ref data, out building, out heightOffset);
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
        private void GetSegmentPillarProps(float elevation)
        {
            //if (pillarPropList != null && pillarList.Count > 0)
            //{
            //    var thePropList = pillarPropList.Where(d => d.HeightLimit >= elevation).OrderBy(x => x.HeightLimit).ToList();
            //    BridgePillarPropItem thePillarPropInfo = null;
            //    if (thePropList == null || thePropList.Count == 0)
            //    {
            //        thePillarPropInfo = pillarPropList.LastOrDefault();
            //    }
            //    else
            //    {
            //        thePillarPropInfo = thePropList.FirstOrDefault();
            //    }
            //    if (thePillarPropInfo != null)
            //    {
            //        var prop = new NetLaneProps.Prop();
            //        m_ElevatedPillarPropInfo = thePillarPropInfo.Prop;
            //        prop.m_prop = m_ElevatedPillarPropInfo;
            //        prop.m_probability = 100;
            //        prop.m_repeatDistance = thePillarPropInfo.RepeatDistance;
            //        prop.m_segmentOffset = thePillarPropInfo.SegmentOffset;
            //        var prefab = FindObjectOfType<NetTool>().Prefab;
            //        var centerLane = prefab.m_lanes.FirstOrDefault(l => l != null && l.m_laneType == NetInfo.LaneType.None);
            //        if (centerLane == null)
            //        {
            //            centerLane = new NetInfo.Lane();
            //            centerLane.m_laneType = NetInfo.LaneType.None;
            //            centerLane.m_laneProps = ScriptableObject.CreateInstance<NetLaneProps>();
            //        }
            //        var laneProps = centerLane.m_laneProps.m_props.ToList();
            //        laneProps.Add(prop);
            //        centerLane.m_laneProps.m_props = laneProps.ToArray();
            //        var lanes = prefab.m_lanes.ToList();
            //        lanes.Add(centerLane);
            //        prefab.m_lanes = lanes.ToArray();
            //    }
            //}
        }
    }
}

public class BridgePillarItem
{
    public float HeightLimit { get; set; }
    public float HeightOffset { get; set; }
    public BuildingInfo info { get; set; }
    public BuildingInfo noCollisionInfo { get; set; }

    public BridgePillarItem()
    {
        HeightLimit = 0;
        HeightOffset = 0;
        info = null;
        noCollisionInfo = null;
    }
}
public class BridgePillarPropItem
{
    public float HeightLimit { get; set; }
    public PropInfo Prop { get; set; }
    public Vector3 Position { get; set; }
    public float SegmentOffset { get; set; }
    public float RepeatDistance { get; set; }
    public BridgePillarPropItem()
    {
        HeightLimit = 60;
        Position = Vector3.zero;
        SegmentOffset = 0;
        RepeatDistance = 60;
        Prop = null;
    }
}
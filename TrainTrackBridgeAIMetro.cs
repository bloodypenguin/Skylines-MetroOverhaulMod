using ColossalFramework;
using ColossalFramework.Math;
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
        public List<BridgePillarPropItem> pillarPropList { get; set; }
        public PropInfo m_ElevatedPillarPropInfo { get; set; }
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
                        var theNetTool = FindObjectOfType<NetTool>();
                        if (theNetTool != null && theNetTool.m_prefab != null)
                        {
                            var getElevationMethod = typeof(NetTool).GetMethod("GetElevation", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                            if (getElevationMethod != null)
                            {
                                var param = new List<object>();
                                param.Add(theNetTool.m_prefab);
                                object elevationObject = getElevationMethod.Invoke(theNetTool, param.ToArray());
                                if (elevationObject != null)
                                {
                                    var elevation = (float)elevationObject;
                                    var theList = pillarList.Where(d => d.HeightLimit >= elevation).OrderBy(x => x.HeightLimit).ToList();
                                    var thePropList = pillarPropList.Where(d => d.HeightLimit >= elevation).OrderBy(x => x.HeightLimit).ToList();
                                    if (theList == null || theList.Count == 0)
                                    {
                                        var thePillarInfo = pillarList.LastOrDefault();
                                        building = thePillarInfo.info;
                                        heightOffset = thePillarInfo.HeightOffset - 1f - thePillarInfo.info.m_generatedInfo.m_size.y;
                                    }
                                    else
                                    {
                                        var thePillarInfo = theList.FirstOrDefault();
                                        building = thePillarInfo.info;
                                        heightOffset = thePillarInfo.HeightOffset - 1f - thePillarInfo.info.m_generatedInfo.m_size.y;
                                    }

                                    if (thePropList == null || thePropList.Count == 0)
                                    {
                                        var thePillarPropInfo = pillarPropList.LastOrDefault();
                                        m_ElevatedPillarPropInfo = thePillarPropInfo.prop;
                                    }
                                    else
                                    {
                                        var thePillarPropInfo = pillarPropList.FirstOrDefault();
                                        m_ElevatedPillarPropInfo = thePillarPropInfo.prop;
                                    }
                                }
                            }
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
    }
}

public class BridgePillarItem
{
    public float HeightLimit { get; set; }
    public float HeightOffset { get; set; }
    public BuildingInfo info { get; set; }

    public BridgePillarItem()
    {
        HeightLimit = 60;
        HeightOffset = 0;
        info = null;
    }
}
public class BridgePillarPropItem
{
    public float HeightLimit { get; set; }
    public PropInfo prop { get; set; }

    public BridgePillarPropItem()
    {
        HeightLimit = 60;
        prop = null;
    }
}
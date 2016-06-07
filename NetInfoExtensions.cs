using System.Collections.Generic;
using System.Linq;
using Transit.Framework;
using UnityEngine;

namespace SingleTrainTrack
{
    public static class NetInfoExtensions
    {
        public static void ReplaceProps(NetInfo info, PropInfo newPropInfo, PropInfo oldPropInfo)
        {
            if (newPropInfo == null || oldPropInfo == null)
            {
                return;
            }

            for (int i = 0; i < info.m_lanes.Count(); i++)
            {
                var lane = info.m_lanes[i];
                if (lane.m_laneProps == null ||
                    lane.m_laneProps.m_props == null ||
                    lane.m_laneProps.m_props.Length == 0)
                {
                    continue;
                }

                var oldProps = lane
                    .m_laneProps
                    .m_props
                    .Where(prop => prop != null && prop.m_prop == oldPropInfo)
                    .ToList();

                if (oldProps.Count() > 0)
                {
                    var newPropsContent = new List<NetLaneProps.Prop>();
                    newPropsContent.AddRange(lane.m_laneProps.m_props.Where(prop => prop.m_prop != oldPropInfo));
                    for (int j = 0; j < oldProps.Count(); j++)
                    {
                        var newProp = oldProps[j].ShallowClone();
                        newProp.m_prop = newPropInfo;
                        newProp.m_finalProp = newPropInfo;
                        newPropsContent.Add(newProp);
                    }
                    var newProps = ScriptableObject.CreateInstance<NetLaneProps>();
                    newProps.name = lane.m_laneProps.name;
                    newProps.m_props = newPropsContent.ToArray();
                    lane.m_laneProps = newProps;
                }
            }
        }
    }
}
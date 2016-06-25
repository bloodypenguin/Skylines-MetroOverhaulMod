using System;
using System.Linq;
using UnityEngine;

namespace MetroOverhaul
{
    public static class Modifiers
    {
        public static void RemoveElectricityPoles(NetInfo prefab)
        {
            if (prefab?.m_lanes == null)
            {
                return;
            }
            foreach (var lane in prefab.m_lanes)
            {
                var mLaneProps = lane.m_laneProps;
                var props = mLaneProps?.m_props;
                if (props == null)
                {
                    continue;
                }
                lane.m_laneProps = ScriptableObject.CreateInstance<NetLaneProps>();
                lane.m_laneProps.m_props = (from prop in props
                    where prop != null
                    let mProp = prop.m_prop
                    where mProp != null
                    where mProp.name != "RailwayPowerline"
                    select prop).ToArray();
            }
        }

        public static void CreatePavement(NetInfo prefab)
        {
            if (prefab == null)
            {
                return;
            }
            prefab.m_createGravel = false;
            prefab.m_createRuining = false;
            prefab.m_createPavement = true;
        }

        public static void MakePedestrianLanesNarrow(NetInfo prefab)
        {
            if (prefab?.m_lanes == null)
            {
                return;
            }
            foreach (var lane in prefab.m_lanes.Where(lane => lane != null && lane.m_laneType == NetInfo.LaneType.Pedestrian))
            {
                lane.m_width = 2;
                lane.m_position = Math.Sign(lane.m_position) * (4 + .5f * lane.m_width);
            }
        }
    }

}
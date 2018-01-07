using ColossalFramework.Globalization;
using MetroOverhaul.NEXT;
using MetroOverhaul.NEXT.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

namespace MetroOverhaul.InitializationSteps
{
    public static class CustomizationSteps
    {
        public static void SetupTrackProps(NetInfo prefab, NetInfoVersion version)
        {
            if (version != NetInfoVersion.Tunnel)
            {
                return;
            }
            var lanes = prefab.m_lanes.ToList();
            var propLane = new NetInfo.Lane();
            propLane.m_laneProps = ScriptableObject.CreateInstance<NetLaneProps>();
            var propsList = new List<NetLaneProps.Prop>();
            var thePropInfo = PrefabCollection<PropInfo>.FindLoaded("Tunnel Light Small Road") ??
                              PrefabCollection<PropInfo>.FindLoaded("Wall Light White");

            propsList.AddBasicProp(thePropInfo, new Vector3(-3.5f, 6, 0), 270);
            propLane.m_laneProps.m_props = propsList.ToArray();
            lanes.Add(propLane);
            prefab.m_lanes = lanes.ToArray();
        }

        public static void SetupStationProps(NetInfo prefab, NetInfoVersion version)
        {
            var propLanes = prefab.m_lanes.Where(l => l.m_laneType == NetInfo.LaneType.Pedestrian).ToList();
            foreach (NetInfo.Lane t in propLanes)
            {
                t.m_laneProps = ScriptableObject.CreateInstance<NetLaneProps>();
                var propsList = new List<NetLaneProps.Prop>();
                if (version == NetInfoVersion.Tunnel)
                {
                    var thePropInfo = PrefabCollection<PropInfo>.FindLoaded("Wall Light White");
                    if (prefab.name.Contains("Island"))
                    {
                        propsList.AddBasicProp(thePropInfo, new Vector3(0, 6.3f, 0), 90, 14);
                    }
                    else
                    {
                        propsList.AddBasicProp(thePropInfo, new Vector3(-1, 6.7f, 0), 90, 10);
                    }

                }
                t.m_laneProps.m_props = propsList.ToArray();
            }
        }

        private static void AddBasicProp(this List<NetLaneProps.Prop> propList, PropInfo thePropInfo, Vector3 position, float angle, float repeatDistance = 0)
        {
            var theProp = new NetLaneProps.Prop();
            theProp.m_prop = thePropInfo;
            theProp.m_position = position;
            theProp.m_repeatDistance = repeatDistance;
            theProp.m_angle = angle;
            propList.Add(theProp);
        }

        public static void CommonConcreteCustomization(NetInfo prefab, NetInfoVersion version)
        {
            if (version == NetInfoVersion.Elevated || version == NetInfoVersion.Bridge)
            {
                prefab.m_class.m_level = ItemClass.Level.Level1;
            }
            else
            {
                prefab.m_class.m_level = ItemClass.Level.Level2;
            }
            var lanes = prefab.m_lanes.ToList();
            var propLane = new NetInfo.Lane();
            propLane.m_laneType = NetInfo.LaneType.None;
            propLane.m_laneProps = ScriptableObject.CreateInstance<NetLaneProps>();
            lanes.Add(propLane);
            prefab.m_lanes = lanes.ToArray();
        }

        public static void CommonSteelCustomization(NetInfo prefab, NetInfoVersion version)
        {
            if (version == NetInfoVersion.Elevated || version == NetInfoVersion.Bridge)
            {
                prefab.m_class.m_level = ItemClass.Level.Level3;
            }
            else
            {
                prefab.m_class.m_level = ItemClass.Level.Level4;
            }
            var lanes = prefab.m_lanes.ToList();
            var propLane = new NetInfo.Lane();
            propLane.m_laneType = NetInfo.LaneType.None;
            propLane.m_laneProps = ScriptableObject.CreateInstance<NetLaneProps>();
            lanes.Add(propLane);
            prefab.m_lanes = lanes.ToArray();
        }

        public static void CommonCustomization(NetInfo prefab, NetInfoVersion version)
        {
            if (prefab.name.Contains("Station"))
            {
                prefab.m_connectGroup = (NetInfo.ConnectGroup)64;
                prefab.m_nodeConnectGroups = (NetInfo.ConnectGroup)64 | (NetInfo.ConnectGroup)32 | NetInfo.ConnectGroup.NarrowTram;
                if (prefab.m_nodes.Count() > 1)
                {
                    prefab.m_nodes[1].m_connectGroup = (NetInfo.ConnectGroup)64 | (NetInfo.ConnectGroup)32 | NetInfo.ConnectGroup.NarrowTram;
                }
            }
            else
            {
                prefab.m_connectGroup = NetInfo.ConnectGroup.NarrowTram;
                prefab.m_nodeConnectGroups = NetInfo.ConnectGroup.NarrowTram;
                if (prefab.m_nodes.Count() > 1)
                {
                    prefab.m_nodes[1].m_connectGroup = NetInfo.ConnectGroup.NarrowTram;
                }
            }
            foreach (var lane in prefab.m_lanes)
            {
                if (lane.m_laneType == NetInfo.LaneType.Vehicle)
                {
                    lane.m_verticalOffset = 0.35f;
                }
            }
        }

        public static void CommonCustomizationTwoLaneOneWay(NetInfo prefab, NetInfoVersion version)
        {
            prefab.m_connectGroup = (NetInfo.ConnectGroup)32;
            prefab.m_nodeConnectGroups = (NetInfo.ConnectGroup)32 | (NetInfo.ConnectGroup)64 | NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.WideTram;

            if (prefab.m_nodes.Count() > 1)
            {
                prefab.m_nodes[1].m_connectGroup = (NetInfo.ConnectGroup)32;
            }

            foreach (var lane in prefab.m_lanes)
            {
                if (lane.m_laneType == NetInfo.LaneType.Vehicle)
                {
                    lane.m_verticalOffset = 0.35f;
                    if (lane.m_direction == NetInfo.Direction.Backward)
                    {
                        lane.m_direction = NetInfo.Direction.Forward;
                    }
                }
            }
        }
        public static void CommonCustomizationSmall(NetInfo prefab, NetInfoVersion version)
        {
            var isTwoWay = prefab.name.Contains("Station") || prefab.name.Contains("Two-Way");
            if (isTwoWay)
            {
                prefab.m_connectGroup = (NetInfo.ConnectGroup)16;
                prefab.m_nodeConnectGroups = (NetInfo.ConnectGroup)16 | (NetInfo.ConnectGroup)32 | (NetInfo.ConnectGroup)64 | NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.WideTram | NetInfo.ConnectGroup.CenterTram;
                if (prefab.m_nodes.Count() > 1)
                {
                    prefab.m_nodes[1].m_connectGroup = (NetInfo.ConnectGroup)16 | NetInfo.ConnectGroup.CenterTram;
                }
            }
            else
            {
                prefab.m_connectGroup = NetInfo.ConnectGroup.CenterTram;
                prefab.m_nodeConnectGroups = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.CenterTram | (NetInfo.ConnectGroup)64;
                if (prefab.m_nodes.Count() > 1)
                {
                    prefab.m_nodes[1].m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.Oneway;
                }
            }

            prefab.SetRoadLanes(version, new LanesConfiguration()
            {
                IsTwoWay = isTwoWay,
                LanesToAdd = isTwoWay ? 0 : -1,
                LayoutStyle = LanesLayoutStyle.AsymL1R2
            });
            var theLanes = prefab.m_lanes.ToList();
            var removedLanes = new List<NetInfo.Lane>();
            for (var i = 0; i < theLanes.Count; i++)
            {
                if (theLanes[i].m_laneType == NetInfo.LaneType.Pedestrian)
                {
                    if (Math.Sign(theLanes[i].m_position) > 0)
                    {
                        theLanes[i].m_position -= 2;
                    }
                    else if (Math.Sign(theLanes[i].m_position) < 0)
                    {
                        if (version == NetInfoVersion.Tunnel)
                        {
                            removedLanes.Add(theLanes[i]);
                        }
                        else
                        {
                            theLanes[i].m_position += 2;
                        }
                    }
                }
                if (theLanes[i].m_laneType == NetInfo.LaneType.Vehicle)
                {
                    if (Math.Sign(theLanes[i].m_position) > 0 || version == NetInfoVersion.Tunnel)
                    {
                        theLanes[i].m_stopOffset = 3;
                        theLanes[i].m_position = 0.0001f;
                    }
                    else if (Math.Sign(theLanes[i].m_position) < 0)
                    {
                        theLanes[i].m_stopOffset = -3;
                        theLanes[i].m_position = -0.0001f;
                    }
                    theLanes[i].m_verticalOffset = 0.35f;
                }
            }
            prefab.m_lanes = theLanes.Except(removedLanes).ToArray();
        }
        public static void CommonCustomizationLarge(NetInfo prefab, NetInfoVersion version)
        {
            prefab.m_connectGroup = NetInfo.ConnectGroup.WideTram;
            prefab.m_nodeConnectGroups = NetInfo.ConnectGroup.WideTram | NetInfo.ConnectGroup.NarrowTram | (NetInfo.ConnectGroup)16 | NetInfo.ConnectGroup.CenterTram;

            if (prefab.m_nodes.Count() > 1)
            {
                prefab.m_nodes[1].m_connectGroup = NetInfo.ConnectGroup.WideTram;
            }
            prefab.SetRoadLanes(version, new LanesConfiguration()
            {
                IsTwoWay = true,
                LanesToAdd = 2
            });
            for (var i = 0; i < prefab.m_lanes.Count(); i++)
            {
                var lane = prefab.m_lanes[i];
                var forwardDone = false;
                var backwardDone = false;
                if (lane.m_laneType == NetInfo.LaneType.Vehicle)
                {
                    if (!forwardDone && lane.m_direction == NetInfo.Direction.Forward)
                    {
                        lane.m_position = 5.89f;
                        forwardDone = true;
                    }
                    else if (!backwardDone && lane.m_direction == NetInfo.Direction.Backward)
                    {
                        lane.m_position = -5.89f;
                        backwardDone = true;
                    }
                }
                lane.m_verticalOffset = 0.35f;
            }
        }

        public static void CommonIsland16mCustomization(NetInfo prefab, NetInfoVersion version)
        {
            prefab.m_connectGroup = NetInfo.ConnectGroup.WideTram;
            prefab.m_nodeConnectGroups = NetInfo.ConnectGroup.WideTram | NetInfo.ConnectGroup.NarrowTram;
            if (prefab.m_nodes.Count() > 1)
            {
                prefab.m_nodes[1].m_connectGroup = NetInfo.ConnectGroup.NarrowTram;
            }

            var theLanes = prefab.m_lanes.ToList();
            for (var i = 0; i < theLanes.Count; i++)
            {
                if (theLanes[i].m_laneType == NetInfo.LaneType.Pedestrian)
                {
                    if (Math.Sign(theLanes[i].m_position) > 0)
                    {
                        theLanes[i].m_position = 2;
                    }
                    else if (Math.Sign(theLanes[i].m_position) < 0)
                    {
                        theLanes[i].m_position = -2;
                    }
                }
                if (theLanes[i].m_laneType == NetInfo.LaneType.Vehicle)
                {
                    if (Math.Sign(theLanes[i].m_position) > 0)
                    {
                        theLanes[i].m_position += 3;
                    }
                    else if (Math.Sign(theLanes[i].m_position) < 0)
                    {
                        theLanes[i].m_position += -3;
                    }
                    theLanes[i].m_verticalOffset = 0.35f;
                }
            }
            prefab.m_lanes = theLanes.ToArray();
        }
        public static void SetStandardTrackWidths(NetInfo prefab, NetInfoVersion version)
        {
            switch (version)
            {
                case NetInfoVersion.Elevated:
                    prefab.m_halfWidth = prefab.name.Contains("Steel") ? 5.0001f : 5; //Todo make proper enum for the styles
                    prefab.m_pavementWidth = 1.5f;
                    break;
                case NetInfoVersion.Bridge:
                    prefab.m_halfWidth = 4.9999f;
                    prefab.m_pavementWidth = 1.5f;
                    break;
                case NetInfoVersion.Slope:
                    prefab.m_halfWidth = prefab.name.Contains("Steel") ? 6.0001f : 6.5f;
                    prefab.m_pavementWidth = prefab.name.Contains("Steel") ? 2.5f : 3f;
                    break;
                case NetInfoVersion.Tunnel:
                    prefab.m_pavementWidth = 2.5f;
                    prefab.m_halfWidth = prefab.name.Contains("Steel") ? 6.0001f : 6;//Todo make proper enum for the styles
                    break;
                case NetInfoVersion.Ground:
                    prefab.m_halfWidth = 5;
                    prefab.m_pavementWidth = 2f;
                    break;
            }
        }
        public static void SetLargeTrackWidths(NetInfo prefab, NetInfoVersion version)
        {
            switch (version)
            {
                case NetInfoVersion.Elevated:
                    prefab.m_halfWidth = prefab.name.Contains("Steel") ? 9.0001f : 9; //Todo make proper enum for the styles
                    prefab.m_pavementWidth = 1.5f;
                    break;
                case NetInfoVersion.Bridge:
                    prefab.m_halfWidth = 8.9999f;
                    prefab.m_pavementWidth = 1.5f;
                    break;
                case NetInfoVersion.Slope:
                    prefab.m_halfWidth = prefab.name.Contains("Steel") ? 10 : 10.5f;
                    prefab.m_pavementWidth = prefab.name.Contains("Steel") ? 2.5f : 3f;
                    break;
                case NetInfoVersion.Tunnel:
                    prefab.m_halfWidth = prefab.name.Contains("Steel") ? 10.0001f : 10;//Todo make proper enum for the styles
                    prefab.m_pavementWidth = 2.5f;
                    break;
                case NetInfoVersion.Ground:
                    prefab.m_halfWidth = 9;
                    prefab.m_pavementWidth = 2f;
                    break;
            }
        }
        public static void SetSmallTrackWidths(NetInfo prefab, NetInfoVersion version)
        {
            switch (version)
            {
                case NetInfoVersion.Ground:
                    prefab.m_halfWidth = 3;
                    prefab.m_pavementWidth = 1.5f;
                    break;
                case NetInfoVersion.Elevated:
                    prefab.m_halfWidth = 3;
                    prefab.m_pavementWidth = 1.5f;
                    break;
                case NetInfoVersion.Bridge:
                    prefab.m_halfWidth = 2.9999f;
                    prefab.m_pavementWidth = 1.5f;
                    break;
                case NetInfoVersion.Slope:
                    prefab.m_halfWidth = prefab.name.Contains("Steel") ? 4.0001f : 4.5f;
                    prefab.m_pavementWidth = prefab.name.Contains("Steel") ? 2.5f : 3f;
                    break;
                case NetInfoVersion.Tunnel:
                    prefab.m_halfWidth = prefab.name.Contains("Steel") ? 4.0001f : 3.5f;
                    prefab.m_pavementWidth = prefab.name.Contains("Steel") ? 2.5f : 2f;
                    break;
            }
        }
        public static void SetSmallStationTrackWidths(NetInfo prefab, NetInfoVersion version)
        {
            switch (version)
            {
                case NetInfoVersion.Ground:
                    prefab.m_halfWidth = 3;
                    prefab.m_pavementWidth = 1.5f;
                    break;
                case NetInfoVersion.Elevated:
                    prefab.m_halfWidth = 3;
                    prefab.m_pavementWidth = 1.5f;
                    break;
                case NetInfoVersion.Tunnel:
                    prefab.m_halfWidth = 6f;
                    prefab.m_pavementWidth = 4.5f;
                    break;
            }
        }
        public static void Set16mTrackWidths(NetInfo prefab, NetInfoVersion version)
        {
            switch (version)
            {
                case NetInfoVersion.Ground:
                    prefab.m_halfWidth = 8;
                    prefab.m_pavementWidth = 1.5f;
                    break;
                case NetInfoVersion.Elevated:
                    prefab.m_halfWidth = 8;
                    prefab.m_pavementWidth = 1.5f;
                    break;
                case NetInfoVersion.Bridge:
                    prefab.m_halfWidth = 7.9999f;
                    prefab.m_pavementWidth = 1.5f;
                    break;
                case NetInfoVersion.Tunnel:
                    prefab.m_halfWidth = 9;
                    prefab.m_pavementWidth = 2.5f;
                    break;
            }
        }
        public static void ReplaceTrackIcon(NetInfo prefab, NetInfoVersion version)
        {
            if (version != NetInfoVersion.Ground)
            {
                return;
            }
            var metroTrack = PrefabCollection<NetInfo>.FindLoaded("Metro Track");
            prefab.m_Atlas = metroTrack.m_Atlas;
            prefab.m_Thumbnail = metroTrack.m_Thumbnail;
            prefab.m_InfoTooltipAtlas = metroTrack.m_InfoTooltipAtlas;
            prefab.m_InfoTooltipThumbnail = metroTrack.m_InfoTooltipThumbnail;
            prefab.m_isCustomContent = false;
            prefab.m_availableIn = ItemClass.Availability.All;
            var locale = LocaleManager.instance.GetLocale();
            var key = new Locale.Key { m_Identifier = "NET_TITLE", m_Key = prefab.name };
            if (!locale.Exists(key))
            {
                locale.AddLocalizedString(key, locale.Get(new Locale.Key { m_Identifier = "NET_TITLE", m_Key = "Metro Track" }));
            }
            key = new Locale.Key { m_Identifier = "NET_DESC", m_Key = prefab.name };
            if (!locale.Exists(key))
            {
                locale.AddLocalizedString(key, locale.Get(new Locale.Key { m_Identifier = "NET_DESC", m_Key = "Metro Track" }));
            }
        }
    }
}
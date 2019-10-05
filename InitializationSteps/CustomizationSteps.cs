using ColossalFramework.Globalization;
using MetroOverhaul.NEXT;
using MetroOverhaul.NEXT.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using MetroOverhaul.Extensions;
using ColossalFramework;
using MetroOverhaul.NEXT.Texturing;

namespace MetroOverhaul.InitializationSteps
{
    public static class CustomizationSteps
    {
        public static void SetupTrackProps(NetInfo prefab, NetInfoVersion version)
        {
            if (version == NetInfoVersion.Tunnel)
            {
                var lanes = prefab.m_lanes.ToList();
                var propLane = new NetInfo.Lane();
                propLane.m_laneProps = ScriptableObject.CreateInstance<NetLaneProps>();
                propLane.m_laneProps.name = "LightLane";
                var propsList = new List<NetLaneProps.Prop>();
                var thePropInfo = PrefabCollection<PropInfo>.FindLoaded("Tunnel Light Small Road").ShallowClone();
                thePropInfo.SetAsTunnelLightProp(1, 20);
                if (prefab.name.Contains("Steel"))
                {
                    if (prefab.name.Contains("Large"))
                    {
                        propsList.AddBasicProp(thePropInfo, new Vector3(-9.5f, 6, 0), 270, 60);
                        propsList.AddBasicProp(thePropInfo, new Vector3(9.5f, 6, 0), 90, 60);
                    }
                    else if (prefab.name.Contains("Small"))
                    {
                        propsList.AddBasicProp(thePropInfo, new Vector3(-3.5f, 6, 0), 270, 60);
                    }
                    else
                    {
                        propsList.AddBasicProp(thePropInfo, new Vector3(-5.5f, 6, 0), 270, 60);
                    }

                }
                else
                {
                    if (prefab.name.Contains("Large"))
                    {
                        propsList.AddBasicProp(thePropInfo, new Vector3(-7.5f, 6, 0), 270, 60);
                        propsList.AddBasicProp(thePropInfo, new Vector3(7.5f, 6, 0), 90, 60);
                    }
                    else if (prefab.name.Contains("Small"))
                    {
                        propsList.AddBasicProp(thePropInfo, new Vector3(-1.5f, 5.5f, 0), 270, 60);
                    }
                    else
                    {
                        propsList.AddBasicProp(thePropInfo, new Vector3(-3.5f, 6, 0), 270, 60);
                    }

                }

                propLane.m_laneProps.m_props = propsList.ToArray();
                lanes.Add(propLane);
                prefab.m_lanes = lanes.ToArray();
            }
        }

        public static void SetupStationProps(NetInfo prefab, NetInfoVersion version)
        {
            if (version == NetInfoVersion.Tunnel)
            {
                var propLanes = prefab.m_lanes.Where(l => l.m_laneType == NetInfo.LaneType.Pedestrian).ToList();
                foreach (NetInfo.Lane t in propLanes)
                {
                    t.m_laneProps = ScriptableObject.CreateInstance<NetLaneProps>();
                    var propsList = new List<NetLaneProps.Prop>();
                    var thePropInfo = PrefabCollection<PropInfo>.FindLoaded($"{Util.PackageName($"BP_flurotube_light_3")}.flurotube_light_3_Data");
                    thePropInfo.SetAsTunnelLightProp(3, 18);
                    propsList.AddBasicProp(thePropInfo, new Vector3(0, 5.8f, 0), 0, 20);
                    t.m_laneProps.m_props = propsList.ToArray();
                }
            }
        }

        private static void AddBasicProp(this List<NetLaneProps.Prop> propList, PropInfo thePropInfo, Vector3 position, float angle, float repeatDistance = 0)
        {
            var theProp = new NetLaneProps.Prop();
            theProp.m_prop = thePropInfo;
            theProp.m_finalProp = thePropInfo;
            theProp.m_position = position;
            theProp.m_repeatDistance = repeatDistance;
            theProp.m_angle = angle;
            propList.Add(theProp);
        }
        public static void CommonVanillaCustomization(NetInfo prefab, NetInfoVersion version)
        {
            if (version == NetInfoVersion.Elevated || version == NetInfoVersion.Bridge)
            {
                prefab.m_nodes[0].m_flagsForbidden |= NetNode.Flags.Transition;
                var nodeTrans = prefab.m_nodes[0].ShallowClone();
                nodeTrans
                    .SetFlags(NetNode.Flags.Transition, NetNode.Flags.LevelCrossing)
                    .SetMeshes
                    ($@"Meshes\10m\Elevated_Node_Pavement_Vanilla.obj");
                var nodeList = new List<NetInfo.Node>();
                nodeList.AddRange(prefab.m_nodes);
                nodeList.Add(nodeTrans);
                prefab.m_nodes = nodeList.ToArray();
                prefab.m_class.m_level = ItemClass.Level.Level1;
            }
            else
            {
                prefab.m_class.m_level = ItemClass.Level.Level2;
            }

            for (int i = 0; i < prefab.m_segments.Count(); i++)
            {
                if (prefab.m_segments[i].m_mesh.name.ToLower().Contains("rail"))
                {
                    prefab.m_segments[i]
                        .SetFlagsDefault()
                        .SetMeshes
                        ($@"Meshes\10m\Rail.obj",
                            $@"Meshes\10m\Rail_LOD.obj")
                        .SetConsistentUVs();
                    prefab.m_segments[i].SetTextures(new TextureSet
                        (@"Textures\Ground_Segment_Rail__MainTex.png",
                            @"Textures\Ground_Segment_Rail__AlphaMap.png",
                            @"Textures\Ground_Segment_Rail__XYSMap.png"));
                    break;
                }
            }
            for (int i = 0; i < prefab.m_nodes.Count(); i++)
            {
                if (prefab.m_nodes[i].m_mesh.name.ToLower().Contains("rail"))
                {
                    prefab.m_nodes[i]
                        .SetFlagsDefault()
                        .SetMeshes
                        ($@"Meshes\10m\Rail.obj",
                            $@"Meshes\10m\Rail_LOD.obj")
                        .SetConsistentUVs();
                    prefab.m_nodes[i].SetTextures(new TextureSet
                        (@"Textures\Ground_Segment_Rail__MainTex.png",
                            @"Textures\Ground_Segment_Rail__AlphaMap.png",
                            @"Textures\Ground_Segment_Rail__XYSMap.png"));
                    break;
                }
            }

        }
        public static void CommonConcreteCustomization(NetInfo prefab, NetInfoVersion version)
        {
            if (version == NetInfoVersion.Elevated || version == NetInfoVersion.Bridge)
            {
                prefab.m_class.m_level = ItemClass.Level.Level5;
            }
            else
            {
                prefab.m_class.m_level = (ItemClass.Level)6;
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
                prefab.m_connectGroup = NetInfo.ConnectGroup.MetroStation;
                //prefab.m_nodeConnectGroups = (NetInfo.ConnectGroup)2048 | NetInfo.ConnectGroup.MonorailStation | NetInfo.ConnectGroup.DoubleMetro;
                //if (version != NetInfoVersion.Tunnel)
                //{
                //    prefab.m_nodes[1].m_connectGroup = (NetInfo.ConnectGroup)2048 | NetInfo.ConnectGroup.MonorailStation;
                //}
            }
            else
            {
                prefab.m_connectGroup = NetInfo.ConnectGroup.DoubleMetro;
                //prefab.m_nodeConnectGroups = NetInfo.ConnectGroup.DoubleMetro;
                //if (version != NetInfoVersion.Tunnel)
                //{
                //    prefab.m_nodes[1].m_connectGroup = NetInfo.ConnectGroup.DoubleMetro;
                //}
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
            prefab.m_connectGroup = NetInfo.ConnectGroup.MonorailStation;
            //prefab.m_nodeConnectGroups = NetInfo.ConnectGroup.MonorailStation | (NetInfo.ConnectGroup)2048 | NetInfo.ConnectGroup.DoubleMetro | NetInfo.ConnectGroup.WideTram;

            //if (version != NetInfoVersion.Tunnel)
            //{
            //    prefab.m_nodes[1].m_connectGroup = NetInfo.ConnectGroup.MonorailStation;
            //}

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
            var isTwoWay = prefab.name.Contains("Two-Way") || prefab.name.Contains("Station");
            if (isTwoWay)
            {
                prefab.m_connectGroup = NetInfo.ConnectGroup.SingleMonorail;
                //prefab.m_nodeConnectGroups = NetInfo.ConnectGroup.SingleMonorail | NetInfo.ConnectGroup.MonorailStation | (NetInfo.ConnectGroup)2048 | NetInfo.ConnectGroup.DoubleMetro | NetInfo.ConnectGroup.WideTram | NetInfo.ConnectGroup.SingleMetro;
                //if (version != NetInfoVersion.Tunnel)
                //{
                //    prefab.m_nodes[1].m_connectGroup = NetInfo.ConnectGroup.SingleMonorail;
                //}
            }
            else
            {
                prefab.m_connectGroup = NetInfo.ConnectGroup.SingleMetro;
                //prefab.m_nodeConnectGroups = NetInfo.ConnectGroup.DoubleMetro | NetInfo.ConnectGroup.SingleMetro | (NetInfo.ConnectGroup)2048 | NetInfo.ConnectGroup.WideTram;
                //if (version != NetInfoVersion.Tunnel)
                //{
                //    prefab.m_nodes[1].m_connectGroup = NetInfo.ConnectGroup.SingleMetro | NetInfo.ConnectGroup.Oneway;
                //}
            }

            prefab.SetRoadLanes(version, new LanesConfiguration()
            {
                IsTwoWay = isTwoWay,
                VehicleLanesToAdd = isTwoWay ? 0 : -1,
                LayoutStyle = LanesLayoutStyle.AsymL1R2
            });
            var theLanes = prefab.m_lanes.ToList();
            var vehichleLanes = theLanes.Where(l => l.m_laneType == NetInfo.LaneType.Vehicle).ToList();
            var removedLanes = new List<NetInfo.Lane>();
            var isStation = prefab.name.Contains("Station");
            for (var i = 0; i < theLanes.Count; i++)
            {
                if (theLanes[i].m_laneType == NetInfo.LaneType.Pedestrian)
                {
                    if (Math.Sign(theLanes[i].m_position) > 0)
                    {
                        if (version == NetInfoVersion.Tunnel)
                        {
                            theLanes[i].m_position = 3.5f;
                            theLanes[i].m_width = 3;
                        }
                        else
                        {
                            theLanes[i].m_position = 4;
                            theLanes[i].m_width = 4;
                        }
                    }
                    else if (Math.Sign(theLanes[i].m_position) < 0)
                    {
                        removedLanes.Add(theLanes[i]);
                    }
                    theLanes[i].m_stopType = VehicleInfo.VehicleType.Metro;
                }
                if (theLanes[i].m_laneType == NetInfo.LaneType.Vehicle)
                {
                    if (vehichleLanes.Count > 1)
                    {
                        if (Math.Sign(theLanes[i].m_position) > 0 || version == NetInfoVersion.Tunnel)
                        {
                            if (isStation)
                            {
                                theLanes[i].m_stopOffset = 3;
                            }
                            theLanes[i].m_position = 0.0001f;
                        }
                        else if (Math.Sign(theLanes[i].m_position) < 0)
                        {
                            if (isStation)
                            {
                                theLanes[i].m_stopOffset = -3;
                            }
                            theLanes[i].m_position = -0.0001f;
                        }
                    }
                    else
                    {
                        theLanes[i].m_position = 0;
                        if (isStation)
                        {
                            theLanes[i].m_direction = NetInfo.Direction.AvoidBackward;
                        }
                    }
                    theLanes[i].m_verticalOffset = 0.35f;
                }
            }
            prefab.m_lanes = theLanes.Except(removedLanes).ToArray();
        }

        public static void CommonIslandCustomization(NetInfo prefab, NetInfoVersion version)
        {
            prefab.m_connectGroup = NetInfo.ConnectGroup.None;
            //prefab.m_nodeConnectGroups = NetInfo.ConnectGroup.WideTram | NetInfo.ConnectGroup.DoubleMetro;
            //if (version != NetInfoVersion.Tunnel)
            //{
            //    prefab.m_nodes[1].m_connectGroup = NetInfo.ConnectGroup.DoubleMetro | NetInfo.ConnectGroup.MonorailStation;
            //}

            var theLanes = prefab.m_lanes.ToList();
            for (var i = 0; i < theLanes.Count; i++)
            {
                if (theLanes[i].m_laneType == NetInfo.LaneType.Pedestrian)
                {
                    theLanes[i].m_width = 3.5f;
                    if (Math.Sign(theLanes[i].m_position) > 0)
                    {
                        theLanes[i].m_position = 2.95f;
                    }
                    else if (Math.Sign(theLanes[i].m_position) < 0)
                    {
                        theLanes[i].m_position = -2.95f;
                    }
                }
                if (theLanes[i].m_laneType == NetInfo.LaneType.Vehicle)
                {
                    if (Math.Sign(theLanes[i].m_position) > 0)
                    {
                        theLanes[i].m_position += 4.5f;
                    }
                    else if (Math.Sign(theLanes[i].m_position) < 0)
                    {
                        theLanes[i].m_position += -4.5f;
                    }
                    theLanes[i].m_verticalOffset = 0.35f;
                }
            }
            prefab.m_lanes = theLanes.ToArray();
        }
        public static void CommonCustomizationLarge(NetInfo prefab, NetInfoVersion version)
        {
            var isStation = prefab.name.Contains("Station");
            if (isStation)
            {
                prefab.m_connectGroup = NetInfo.ConnectGroup.None;
            }
            else
            {
                prefab.m_connectGroup = NetInfo.ConnectGroup.WideTram;
                prefab.m_nodeConnectGroups = NetInfo.ConnectGroup.WideTram | NetInfo.ConnectGroup.DoubleMetro | NetInfo.ConnectGroup.SingleMonorail;
                //if (version != NetInfoVersion.Tunnel)
                //{
                //    prefab.m_nodes[1].m_connectGroup = NetInfo.ConnectGroup.WideTram;
                //}
            }

            prefab.SetRoadLanes(version, new LanesConfiguration()
            {
                IsTwoWay = true,
                VehicleLanesToAdd = 2
            });
            prefab.m_minCornerOffset = 18;
            var vanillaMetroTrack = PrefabCollection<NetInfo>.FindLoaded(Util.GetMetroTrackName(NetInfoVersion.Tunnel, TrackStyle.Vanilla));
            var speedLimit = vanillaMetroTrack.m_lanes.First(l => l.m_vehicleType != VehicleInfo.VehicleType.None).m_speedLimit;
            var theLanes = prefab.m_lanes.ToList();
            var count = 0;
            for (var i = 0; i < theLanes.Count(); i++)
            {
                if (theLanes[i].m_laneType == NetInfo.LaneType.Vehicle)
                {
                    switch (count)
                    {
                        case 0:
                            theLanes[i].m_position = -5.885f;
                            theLanes[i].m_direction = isStation ? NetInfo.Direction.AvoidForward : NetInfo.Direction.Backward;
                            theLanes[i].m_speedLimit = speedLimit;
                            break;
                        case 1:
                            theLanes[i].m_position = -1.966f;
                            theLanes[i].m_direction = NetInfo.Direction.Backward;
                            theLanes[i].m_speedLimit = isStation ? speedLimit + 3 : speedLimit;
                            break;
                        case 2:
                            theLanes[i].m_position = 1.966f;
                            theLanes[i].m_direction = NetInfo.Direction.Forward;
                            theLanes[i].m_speedLimit = isStation ? speedLimit + 3 : speedLimit;
                            break;
                        case 3:
                            theLanes[i].m_position = 5.886f;
                            theLanes[i].m_direction = isStation ? NetInfo.Direction.AvoidBackward : NetInfo.Direction.Forward;
                            theLanes[i].m_speedLimit = speedLimit;
                            break;
                    }
                    count++;
                    theLanes[i].m_verticalOffset = 0.35f;
                }
                else if (theLanes[i].m_laneType == NetInfo.LaneType.Pedestrian)
                {
                    theLanes[i].m_stopType = VehicleInfo.VehicleType.Metro;
                    theLanes[i].m_position = Math.Sign(theLanes[i].m_position) * 10;
                    theLanes[i].m_width = 3;
                }
            }
            prefab.m_lanes = theLanes.ToArray();
        }

        public static void CommonLargeSideIslandCustomization(NetInfo prefab, NetInfoVersion version)
        {
            prefab.m_connectGroup = NetInfo.ConnectGroup.None;
            prefab.SetRoadLanes(version, new LanesConfiguration()
            {
                IsTwoWay = true,
                VehicleLanesToAdd = 2,
                PedestrianLanesToAdd = 2
            });
            prefab.m_minCornerOffset = 18;
            var vehicleCount = 0;
            var pedestrianCount = 0;
            var theLanes = prefab.m_lanes.ToList();
            for (var i = 0; i < theLanes.Count; i++)
            {
                var theLane = theLanes[i];
                if (theLanes[i].m_laneType == NetInfo.LaneType.Pedestrian)
                {
                    theLane.m_width = 3.5f;
                    theLane.m_stopType = VehicleInfo.VehicleType.Metro;
                    theLane.m_direction = NetInfo.Direction.Both;
                    switch (pedestrianCount)
                    {
                        case 0:
                            theLane.m_position = -12;
                            break;
                        case 1:
                            theLane.m_position = -2.95f;
                            theLane.m_centerPlatform = true;
                            break;
                        case 2:
                            theLane.m_position = 2.95f;
                            theLane.m_centerPlatform = true;
                            break;
                        case 3:
                            theLane.m_position = 12;
                            break;
                    }
                    pedestrianCount++;
                }
                else if (theLane.m_laneType == NetInfo.LaneType.Vehicle)
                {
                    switch (vehicleCount)
                    {
                        case 0:
                            theLane.m_position = -10.39f;
                            theLane.m_direction = NetInfo.Direction.AvoidForward;
                            break;
                        case 1:
                            theLane.m_position = -6.463f;
                            theLane.m_direction = NetInfo.Direction.AvoidForward;
                            theLane.m_speedLimit += 3;

                            break;
                        case 2:
                            theLane.m_position = 6.463f;
                            theLane.m_direction = NetInfo.Direction.AvoidBackward;
                            theLane.m_speedLimit += 3;
                            break;
                        case 3:
                            theLane.m_position = 10.39f;
                            theLane.m_direction = NetInfo.Direction.AvoidBackward;
                            break;
                    }
                    vehicleCount++;
                    theLane.m_verticalOffset = 0.35f;
                }
            }
            prefab.m_lanes = theLanes.ToArray();
        }
        public static void CommonLargeDualIslandCustomization(NetInfo prefab, NetInfoVersion version)
        {
            prefab.m_connectGroup = NetInfo.ConnectGroup.None;
            prefab.SetRoadLanes(version, new LanesConfiguration()
            {
                IsTwoWay = true,
                VehicleLanesToAdd = 2,
                PedestrianLanesToAdd = 2,
                PedestrianLaneWidth = 2.28f
            });
            prefab.m_minCornerOffset = 30;
            var vehicleLaneCount = 0;
            var pedestrianLaneCount = 0;
            var theLanes = prefab.m_lanes.ToList();
            for (var i = 0; i < theLanes.Count; i++)
            {
                if (theLanes[i].m_laneType == NetInfo.LaneType.Pedestrian)
                {
                    theLanes[i].m_direction = NetInfo.Direction.Both;
                    theLanes[i].m_stopType = VehicleInfo.VehicleType.Metro;
                    switch (pedestrianLaneCount)
                    {
                        case 0:
                            theLanes[i].m_position = -9.48f;
                            theLanes[i].m_centerPlatform = true;
                            theLanes[i].m_elevated = true;
                            break;
                        case 1:
                            theLanes[i].m_position = -4.8f;
                            theLanes[i].m_elevated = true;
                            break;
                        case 2:
                            theLanes[i].m_position = 4.8f;
                            break;
                        case 3:
                            theLanes[i].m_position = 9.48f;
                            theLanes[i].m_centerPlatform = true;
                            break;
                    }
                    pedestrianLaneCount++;
                }
                else if (theLanes[i].m_laneType == NetInfo.LaneType.Vehicle)
                {
                    switch (vehicleLaneCount)
                    {
                        case 0:
                            theLanes[i].m_position = -12.38f;
                            theLanes[i].m_direction = NetInfo.Direction.AvoidForward;
                            break;
                        case 1:
                            theLanes[i].m_position = -1.966f;
                            theLanes[i].m_direction = NetInfo.Direction.AvoidForward;
                            theLanes[i].m_speedLimit += 3;

                            break;
                        case 2:
                            theLanes[i].m_position = 1.966f;
                            theLanes[i].m_direction = NetInfo.Direction.AvoidBackward;
                            theLanes[i].m_speedLimit += 3;
                            break;
                        case 3:
                            theLanes[i].m_position = 12.38f;
                            theLanes[i].m_direction = NetInfo.Direction.AvoidBackward;
                            break;
                    }
                    vehicleLaneCount++;
                    theLanes[i].m_verticalOffset = 0.35f;
                }
            }
            prefab.m_lanes = theLanes.ToArray();
        }
        public static void CommonClonedStationCustomization(BuildingInfo info)
        {
            info.m_availableIn = ItemClass.Availability.None;
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
        public static void SetStandardStationTrackWidths(NetInfo prefab, NetInfoVersion version)
        {
            switch (version)
            {
                case NetInfoVersion.Ground:
                    prefab.m_halfWidth = 5;
                    prefab.m_pavementWidth = 2f;
                    break;
                case NetInfoVersion.Elevated:
                    prefab.m_halfWidth = prefab.name.Contains("Steel") ? 5.0001f : 5; //Todo make proper enum for the styles
                    prefab.m_pavementWidth = 1.5f;
                    break;
                case NetInfoVersion.Tunnel:
                    prefab.m_pavementWidth = 4.5f;
                    prefab.m_halfWidth = 8;
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
        public static void SetLargeStationTrackWidths(NetInfo prefab, NetInfoVersion version)
        {
            switch (version)
            {
                case NetInfoVersion.Ground:
                    prefab.m_halfWidth = 9;
                    prefab.m_pavementWidth = 2f;
                    break;
                case NetInfoVersion.Elevated:
                    prefab.m_halfWidth = prefab.name.Contains("Steel") ? 9.0001f : 9; //Todo make proper enum for the styles
                    prefab.m_pavementWidth = 1.5f;
                    break;
                case NetInfoVersion.Tunnel:
                    prefab.m_halfWidth = 12;
                    prefab.m_pavementWidth = 4.5f;
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
        public static void SetIslandTrackWidths(NetInfo prefab, NetInfoVersion version)
        {
            switch (version)
            {
                case NetInfoVersion.Ground:
                    prefab.m_halfWidth = 9.5f;
                    prefab.m_pavementWidth = 1.5f;
                    break;
                case NetInfoVersion.Elevated:
                    prefab.m_halfWidth = 9.5f;
                    prefab.m_pavementWidth = 1.5f;
                    break;
                case NetInfoVersion.Bridge:
                    prefab.m_halfWidth = 9.4999f;
                    prefab.m_pavementWidth = 1.5f;
                    break;
                case NetInfoVersion.Tunnel:
                    prefab.m_halfWidth = 10.5f;
                    prefab.m_pavementWidth = 2.5f;
                    break;
            }
        }

        public static void SetLargeIslandTrackWidths(NetInfo prefab, NetInfoVersion version)
        {
            switch (version)
            {
                case NetInfoVersion.Ground:
                    prefab.m_halfWidth = 15.5f;
                    prefab.m_pavementWidth = 1.5f;
                    break;
                case NetInfoVersion.Elevated:
                    prefab.m_halfWidth = 15.5f;
                    prefab.m_pavementWidth = 1.5f;
                    break;
                case NetInfoVersion.Tunnel:
                    prefab.m_halfWidth = 16.5f;
                    prefab.m_pavementWidth = 2.5f;
                    break;
            }
        }
        private const string SINGLE_TRACK_INFOTOOLTIP = "SingleTrackInfoToolTip";
        private const string DUAL_TRACK_INFOTOOLTIP = "DualTrackInfoToolTip";
        private const string QUAD_TRACK_INFOTOOLTIP = "QuadTrackInfoToolTip";
        private static ColossalFramework.UI.UITextureAtlas m_InfoToolTipAtlas = null;
        private static ColossalFramework.UI.UITextureAtlas InfoToolTipAtlas()
        {
            if (m_InfoToolTipAtlas == null)
            {
                m_InfoToolTipAtlas = UI.UIHelper.GenerateLinearAtlas("MOM_InfoToolTipAtlas", UI.UIHelper.InfoToolTips, 3, new string[] {
                    SINGLE_TRACK_INFOTOOLTIP,
                    DUAL_TRACK_INFOTOOLTIP,
                    QUAD_TRACK_INFOTOOLTIP
                });
            }
            return m_InfoToolTipAtlas;
        }

        public static void ReplaceTrackIcon(NetInfo prefab, NetInfoVersion version)
        {
            if (version != NetInfoVersion.Ground)
            {
                return;
            }

            prefab.m_InfoTooltipAtlas = InfoToolTipAtlas();
            string netTitle;
            string netDescription;
            if (prefab.name.Contains("Large"))
            {
                var atlas = UI.UIHelper.GenerateLinearAtlas("MOM_QuadMetroTrackAtlas", UI.UIHelper.QuadMetroTracks);
                prefab.m_Atlas = atlas;
                prefab.m_Thumbnail = atlas.name + "Bg";
                prefab.m_InfoTooltipThumbnail = QUAD_TRACK_INFOTOOLTIP;
                prefab.m_UIPriority = 3;
                netTitle = "Quad Metro Track";
                netDescription = "A four-lane metro track suitable for heavy traffic. Designate separate local and express lines for best results.";
            }
            else if (prefab.name.Contains("Small"))
            {
                var atlas = UI.UIHelper.GenerateLinearAtlas("MOM_SingleMetroTrackAtlas", UI.UIHelper.SingleMetroTracks);
                prefab.m_Atlas = atlas;
                prefab.m_Thumbnail = atlas.name + "Bg";
                prefab.m_InfoTooltipThumbnail = SINGLE_TRACK_INFOTOOLTIP;
                prefab.m_UIPriority = 1;
                netTitle = "Single Metro Track";
                netDescription = "A one-lane metro track. This track is suitable for light traffic or as a connector to other tracks.";
            }
            else
            {
                var atlas = UI.UIHelper.GenerateLinearAtlas("MOM_DualMetroTrackAtlas", UI.UIHelper.DualMetroTracks);
                prefab.m_Atlas = atlas;
                prefab.m_Thumbnail = atlas.name + "Bg";
                prefab.m_InfoTooltipThumbnail = DUAL_TRACK_INFOTOOLTIP;
                prefab.m_UIPriority = 2;
                netTitle = "Dual Metro Track";
                netDescription = "A two-lane metro track. This track supports moderate traffic and is adequate for most situations.";
            }
            prefab.m_isCustomContent = false;
            prefab.m_availableIn = ItemClass.Availability.All;
            var locale = LocaleManager.instance.GetLocale();
            var key = new Locale.Key { m_Identifier = "NET_TITLE", m_Key = prefab.name };
            if (!Locale.Exists("NET_TITLE", prefab.name))
                locale.AddLocalizedString(key, netTitle);
            var dkey = new Locale.Key { m_Identifier = "NET_DESC", m_Key = prefab.name };
            if (!Locale.Exists("NET_DESC", prefab.name))
                locale.AddLocalizedString(dkey, netDescription);
        }
    }
}
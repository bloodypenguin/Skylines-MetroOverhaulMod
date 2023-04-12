﻿using MetroOverhaul.NEXT;
using System.Linq;
using System.Text.RegularExpressions;

namespace MetroOverhaul.Extensions
{
    public static class NetInfoExtensions
    {
        //public static bool IsTrainTrack(this NetInfo info)
        //{
        //    return info != null && (info.IsTrainTrackAI() || info.IsTrainTrackBridgeAI() || info.IsTrainTrackTunnelAI());
        //}
        //public static bool IsMetroTrack(this NetInfo info)
        //{
        //    return info != null && (info.IsTrainTrackAIMetro() || info.IsTrainTrackBridgeAIMetro() || info.IsTrainTrackTunnelAIMetro());
        //}
        public static bool IsAbovegroundTrainStationTrack(this NetInfo info)
        {
            if (info?.name == null)
            {
                return false;
            }
            var retval = info.IsGroundSidePlatformTrainStationTrack() ||
                         info.IsElevatedSidePlatformTrainStationTrack() ||
                         info.IsSunkenSidePlatformTrainStationTrack() ||
                         info.IsElevatedSinglePlatformTrainStationTrack() ||
                         info.IsGroundSinglePlatformTrainStationTrack() ||
                         info.IsGroundIslandPlatformTrainStationTrack() ||
                         info.IsElevatedIslandPlatformTrainStationTrack() ||
                         info.IsGroundLargeSidePlatformTrainStationTrack() ||
                         info.IsElevatedLargeSidePlatformTrainStationTrack() ||
                         info.IsGroundDualIslandPlatformTrainStationTrack() ||
                         info.IsElevatedDualIslandPlatformTrainStationTrack();
            return retval;


        }
        public static bool IsGroundSidePlatformTrainStationTrack(this NetInfo info)
        {
            if (info?.name == null)
            {
                return false;
            }
            return info.name == ModTrackNames.TRAIN_STATION_TRACK_GROUND ||
                   info.name == ModTrackNames.TRAIN_STATION_TRACK_GROUND_C ||
                   info.name == ModTrackNames.TRAIN_STATION_TRACK_GROUND_NP ||
                   info.name == ModTrackNames.TRAIN_STATION_TRACK_GROUND_CNP ||
                   (info.IsTrainTrackAI() && info.IsStationTrack() && info.DeduceTrackType() == StationTrackType.SidePlatform);
        }
        public static bool IsElevatedSidePlatformTrainStationTrack(this NetInfo info)
        {
            if (info?.name == null)
            {
                return false;
            }
            return info.name == ModTrackNames.TRAIN_STATION_TRACK_ELEVA ||
                   info.name == ModTrackNames.TRAIN_STATION_TRACK_ELEVATED_C ||
                   info.name == ModTrackNames.TRAIN_STATION_TRACK_ELEVATED_NP ||
                   info.name == ModTrackNames.TRAIN_STATION_TRACK_ELEVATED_CNP ||
                   info.name == ModTrackNames.TRAIN_STATION_TRACK_ELEVATED_NARROW_C ||
                   info.name == ModTrackNames.TRAIN_STATION_TRACK_ELEVATED_NARROW_NP ||
                   info.name == ModTrackNames.TRAIN_STATION_TRACK_ELEVATED_NARROW_CNP ||
                   (info.IsTrainTrackBridgeAI() && info.IsStationTrack() && info.DeduceTrackType() == StationTrackType.SidePlatform);
        }
        public static bool IsSunkenSidePlatformTrainStationTrack(this NetInfo info)
        {
            if (info?.name == null)
            {
                return false;
            }
            return info.name == ModTrackNames.TRAIN_STATION_TRACK_SUNKEN ||
                (info.IsTrainTrackTunnelAI() && info.IsStationTrack() && info.DeduceTrackType() == StationTrackType.SidePlatform && info.name.Contains("Sunken"));
        }

        public static bool IsGroundSinglePlatformTrainStationTrack(this NetInfo info)
        {
            if (info?.name == null)
            {
                return false;
            }
            return info.name == ModTrackNames.TRAIN_STATION_TRACK_GROUND_SMALL1 || info.name == ModTrackNames.TRAIN_STATION_TRACK_GROUND_SMALL2 ||
                   (info.IsTrainTrackAI() && info.IsStationTrack() && info.DeduceTrackType() == StationTrackType.SinglePlatform);
        }
        public static bool IsElevatedSinglePlatformTrainStationTrack(this NetInfo info)
        {
            if (info?.name == null)
            {
                return false;
            }
            return info.name == ModTrackNames.TRAIN_STATION_TRACK_ELEVATED_SMALL ||
                   (info.IsTrainTrackBridgeAI() && info.IsStationTrack() && info.DeduceTrackType() == StationTrackType.SinglePlatform);
        }
        public static bool IsGroundIslandPlatformTrainStationTrack(this NetInfo info)
        {
            if (info?.name == null)
            {
                return false;
            }
            return info.name == ModTrackNames.TRAIN_STATION_TRACK_GROUND_ISLAND ||
                   (info.IsTrainTrackAI() && info.IsStationTrack() && info.DeduceTrackType() == StationTrackType.IslandPlatform);
        }
        public static bool IsElevatedIslandPlatformTrainStationTrack(this NetInfo info)
        {
            if (info?.name == null)
            {
                return false;
            }
            return info.name == ModTrackNames.TRAIN_STATION_TRACK_ELEVATED_ISLAND ||
                   (info.IsTrainTrackBridgeAI() && info.IsStationTrack() && info.DeduceTrackType() == StationTrackType.IslandPlatform);
        }

        public static bool IsGroundLargeSidePlatformTrainStationTrack(this NetInfo info)
        {
            if (info?.name == null)
            {
                return false;
            }
            return info.name == ModTrackNames.TRAIN_STATION_TRACK_GROUND_LARGE ||
                   (info.IsTrainTrackAI() && info.IsStationTrack() && info.DeduceTrackType() == StationTrackType.ExpressSidePlatform);
        }
        public static bool IsElevatedLargeSidePlatformTrainStationTrack(this NetInfo info)
        {
            if (info?.name == null)
            {
                return false;
            }
            return info.name == ModTrackNames.TRAIN_STATION_TRACK_ELEVATED_LARGE ||
                   (info.IsTrainTrackBridgeAI() && info.IsStationTrack() && info.DeduceTrackType() == StationTrackType.ExpressSidePlatform);
        }

        public static bool IsGroundDualIslandPlatformTrainStationTrack(this NetInfo info)
        {
            if (info?.name == null)
            {
                return false;
            }
            return info.name == ModTrackNames.TRAIN_STATION_TRACK_GROUND_LARGE_DUALISLAND ||
                   (info.IsTrainTrackAI() && info.IsStationTrack() && info.DeduceTrackType() == StationTrackType.DualIslandPlatform);
        }
        public static bool IsElevatedDualIslandPlatformTrainStationTrack(this NetInfo info)
        {
            if (info?.name == null)
            {
                return false;
            }
            return info.name == ModTrackNames.TRAIN_STATION_TRACK_ELEVATED_LARGE_DUALISLAND ||
                   (info.IsTrainTrackBridgeAI() && info.IsStationTrack() && info.DeduceTrackType() == StationTrackType.DualIslandPlatform);
        }

        public static bool IsStationTrack(this NetInfo info)
        {
            return info.m_lanes.Any(l => l.m_stopType != VehicleInfo.VehicleType.None);
        }
        public static bool IsAbovegroundMetroStationTrack(this NetInfo info)
        {
            if (info?.name == null)
            {
                return false;
            }
            var retval = info.IsGroundSidePlatformMetroStationTrack() ||
                         info.IsElevatedSidePlatformMetroStationTrack() ||
                         info.IsSunkenSidePlatformMetroStationTrack() ||
                         info.IsGroundSinglePlatformMetroStationTrack() ||
                         info.IsElevatedSinglePlatformMetroStationTrack() ||
                         info.IsGroundIslandPlatformMetroStationTrack() ||
                         info.IsElevatedIslandPlatformMetroStationTrack() ||
                         info.IsGroundLargeSidePlatformMetroStationTrack() ||
                         info.IsElevatedLargeSidePlatformMetroStationTrack() ||
                         info.IsGroundDualIslandPlatformMetroStationTrack() ||
                         info.IsElevatedDualIslandPlatformMetroStationTrack();
            return retval;


        }
        public static bool IsGroundSidePlatformMetroStationTrack(this NetInfo info)
        {
            if (info?.name == null)
            {
                return false;
            }
            return Regex.IsMatch(info.name, "(Steel )?" + ModTrackNames.MOM_STATION_TRACK_GROUND + "( 01)?\b") ||
                   (info.IsMetroTrackAI() && info.IsStationTrack() && info.DeduceTrackType() == StationTrackType.SidePlatform);
        }

        public static bool IsElevatedSidePlatformMetroStationTrack(this NetInfo info)
        {
            if (info?.name == null)
            {
                return false;
            }
            return Regex.IsMatch(info.name, "(Steel )?" + ModTrackNames.MOM_STATION_TRACK_ELEVATED + "( 01)?\b") ||
                   (info.IsMetroTrackBridgeAI() && info.IsStationTrack() && info.DeduceTrackType() == StationTrackType.SidePlatform);
        }

        public static bool IsSunkenSidePlatformMetroStationTrack(this NetInfo info)
        {
            if (info?.name == null)
            {
                return false;
            }
            return Regex.IsMatch(info.name, "(Steel )?" + ModTrackNames.MOM_STATION_TRACK_SUNKEN + "\b") ||
                (info.IsMetroTrackTunnelAI() && info.IsStationTrack() && info.DeduceTrackType() == StationTrackType.SidePlatform && info.name.Contains("Sunken"));
        }

        public static bool IsGroundSinglePlatformMetroStationTrack(this NetInfo info)
        {
            if (info?.name == null)
            {
                return false;
            }
            return Regex.IsMatch(info.name, "(Steel )?" + ModTrackNames.MOM_STATION_TRACK_GROUND_SMALL + "\b") ||
                   (info.IsMetroTrackAI() && info.IsStationTrack() && info.DeduceTrackType() == StationTrackType.SinglePlatform);
        }

        public static bool IsElevatedSinglePlatformMetroStationTrack(this NetInfo info)
        {
            if (info?.name == null)
            {
                return false;
            }
            return Regex.IsMatch(info.name, "(Steel )?" + ModTrackNames.MOM_STATION_TRACK_ELEVATED_SMALL + "\b") ||
                   (info.IsMetroTrackBridgeAI() && info.IsStationTrack() && info.DeduceTrackType() == StationTrackType.SinglePlatform);
        }

        public static bool IsGroundIslandPlatformMetroStationTrack(this NetInfo info)
        {
            if (info?.name == null)
            {
                return false;
            }
            return Regex.IsMatch(info.name, "(Steel )?" + ModTrackNames.MOM_STATION_TRACK_GROUND_ISLAND + "\b") ||
                   (info.IsMetroTrackAI() && info.IsStationTrack() && info.DeduceTrackType() == StationTrackType.IslandPlatform);
        }
        public static bool IsElevatedIslandPlatformMetroStationTrack(this NetInfo info)
        {
            if (info?.name == null)
            {
                return false;
            }
            return Regex.IsMatch(info.name, "(Steel )?" + ModTrackNames.MOM_STATION_TRACK_ELEVATED_ISLAND + "\b") ||
                   (info.IsMetroTrackBridgeAI() && info.IsStationTrack() && info.DeduceTrackType() == StationTrackType.IslandPlatform);
        }

        public static bool IsGroundLargeSidePlatformMetroStationTrack(this NetInfo info)
        {
            if (info?.name == null)
            {
                return false;
            }
            return Regex.IsMatch(info.name, "(Steel )?" + ModTrackNames.MOM_STATION_TRACK_GROUND_LARGE + "\b") ||
                   (info.IsMetroTrackAI() && info.IsStationTrack() && info.DeduceTrackType() == StationTrackType.ExpressSidePlatform);
        }
        public static bool IsElevatedLargeSidePlatformMetroStationTrack(this NetInfo info)
        {
            if (info?.name == null)
            {
                return false;
            }
            return Regex.IsMatch(info.name, "(Steel )?" + ModTrackNames.MOM_STATION_TRACK_ELEVATED_LARGE + "\b") ||
                   (info.IsMetroTrackBridgeAI() && info.IsStationTrack() && info.DeduceTrackType() == StationTrackType.ExpressSidePlatform);
        }

        public static bool IsGroundDualIslandPlatformMetroStationTrack(this NetInfo info)
        {
            if (info?.name == null)
            {
                return false;
            }
            return Regex.IsMatch(info.name, "(Steel )?" + ModTrackNames.MOM_STATION_TRACK_GROUND_LARGE_DUALISLAND + "\b") ||
                   (info.IsMetroTrackAI() && info.IsStationTrack() && info.DeduceTrackType() == StationTrackType.DualIslandPlatform);
        }
        public static bool IsElevatedDualIslandPlatformMetroStationTrack(this NetInfo info)
        {
            if (info?.name == null)
            {
                return false;
            }
            return Regex.IsMatch(info.name, "(Steel )?" + ModTrackNames.MOM_STATION_TRACK_ELEVATED_LARGE_DUALISLAND + "\b") ||
                   (info.IsMetroTrackBridgeAI() && info.IsStationTrack() && info.DeduceTrackType() == StationTrackType.DualIslandPlatform);
        }
        
        public static bool IsUndergroundMetroStationTrack(this NetInfo netInfo)
        {
            if (netInfo?.name == null)
            {
                return false;
            }
            return IsUndergroundSidePlatformMetroStationTrack(netInfo) || IsUndergroundIslandPlatformStationTrack(netInfo) ||
                   IsUndergroundSmallStationTrack(netInfo) || IsUndergroundPlatformLargeMetroStationTrack(netInfo) || IsUndergroundDualIslandPlatformMetroStationTrack(netInfo);
        }
        public static bool IsUndergroundSidePlatformMetroStationTrack(this NetInfo netInfo)
        {
            if (netInfo?.name == null)
            {
                return false;
            }
            return netInfo.name == Util.GetMetroStationTrackName(NetInfoVersion.Tunnel, TrackStyle.Vanilla) || netInfo.name == Util.GetMetroStationTrackName(NetInfoVersion.Tunnel, TrackStyle.Modern);
        }
        public static bool IsUndergroundPlatformLargeMetroStationTrack(this NetInfo netInfo)
        {
            return netInfo?.name != null && netInfo.name.Contains("Metro Station Track Tunnel Large") && !netInfo.name.Contains("Island");
        }
        public static bool IsUndergroundDualIslandPlatformMetroStationTrack(this NetInfo netInfo)
        {
            return netInfo?.name != null && netInfo.name.Contains("Metro Station Track Tunnel Large Dual Island");
        }
        //public static bool IsUndergroundSideIslandPlatformMetroStationTrack(this NetInfo netInfo)
        //{
        //    return netInfo?.name != null && netInfo.name.Contains("Concrete Metro Station Track Tunnel Large Side Island");
        //}

        public static bool IsUndergroundIslandPlatformStationTrack(this NetInfo netinfo)
        {
            if (netinfo?.name == null)
            {
                return false;
            }
            return netinfo.name.Contains("Metro Station Track Tunnel Island");
        }

        public static bool IsUndergroundSmallStationTrack(this NetInfo netinfo)
        {
            if (netinfo?.name == null)
            {
                return false;
            }
            return netinfo.name.Contains("Metro Station Track Tunnel Small");
        }

        public static bool IsUndergroundMetroTrack(this NetInfo netInfo)
        {
            if (netInfo?.name == null)
            {
                return false;
            }
            return netInfo.name.Contains("Metro Track Tunnel");
        }
        public static bool IsMetroTrack(this NetInfo netInfo)
        {
            if (netInfo?.name == null)
            {
                return false;
            }
            return netInfo.name.Contains("Metro Track");
        }
        public static bool IsMetroStationTrack(this NetInfo netinfo)
        {
            if (netinfo?.name == null)
            {
                return false;
            }
            return netinfo.IsAbovegroundMetroStationTrack() || netinfo.IsUndergroundMetroStationTrack();
        }
        public static bool IsTrainTrack(this NetInfo netInfo)
        {
            if (netInfo?.name == null)
            {
                return false;
            }
            return netInfo.name.Contains(ModTrackNames.TRAIN_TRACK) || netInfo.name.Contains(ModTrackNames.TRAIN_SINGLE_TRACK) || netInfo.name.StartsWith("Rail1L");
        }
        public static bool IsTrainSingleTrack(this NetInfo netInfo)
        {
            if (netInfo?.name == null)
            {
                return false;
            }
            return netInfo.name == ModTrackNames.TRAIN_SINGLE_TRACK || netInfo.name == ModTrackNames.TRAIN_SINGLE_TRACK_RAIL1L1W || netInfo.name == ModTrackNames.TRAIN_SINGLE_TRACK_RAIL1L2W;
        }
        public static bool IsPedestrianNetwork(this NetInfo info)
        {
            return info != null && info.name.StartsWith("Pedestrian Connection");
        }
        public static bool IsTrainTrackAI(this NetInfo info)
        {
            var ai = info.GetComponent<NetAI>();
            return ai != null && ai is MetroTrackAI;
        }
        public static bool IsTrainTrackBridgeAI(this NetInfo info)
        {
            var ai = info.GetComponent<NetAI>();
            return ai != null && ai is TrainTrackBridgeAI;
        }
        public static bool IsTrainTrackTunnelAI(this NetInfo info)
        {
            var ai = info.GetComponent<NetAI>();
            return ai != null && ai is TrainTrackTunnelAI;
        }
        public static bool IsMetroTrackAI(this NetInfo info)
        {
            var ai = info.GetComponent<NetAI>();
            return ai != null && ai is MetroTrackAI;
        }
        public static bool IsMetroTrackBridgeAI(this NetInfo info)
        {
            var ai = info.GetComponent<NetAI>();
            return ai != null && ai is MetroTrackBridgeAI;
        }
        public static bool IsMetroTrackTunnelAI(this NetInfo info)
        {
            var ai = info.GetComponent<NetAI>();
            return ai != null && ai is MetroTrackTunnelAI;
        }
        public static NetInfoVersion GetNetInfoVersion(this NetInfo info)
        {
            if (info == null)
                return NetInfoVersion.None;
            var ai = info.GetAI();
            if (ai == null)
                return NetInfoVersion.None;
            if (ai is MetroTrackAI)
                return NetInfoVersion.Ground;
            if (ai is MetroTrackBridgeAI)
            {
                if (info.IsStationTrack())
                    return NetInfoVersion.Elevated;
                else
                    return info.name.IndexOf("Bridge") > -1 ? NetInfoVersion.Bridge : NetInfoVersion.Elevated;
            }
            else if(ai is MetroTrackTunnelAI)
            {
                if (info.IsStationTrack())
                    return NetInfoVersion.Tunnel;
                else
                {
                    if (info.name == Util.GetMetroTrackName(NetInfoVersion.Tunnel, TrackStyle.Vanilla))
                        return NetInfoVersion.Tunnel;
                    else
                    {
                        if (info.name.IndexOf("Tunnel") > -1)
                            return NetInfoVersion.Tunnel;
                        else if (info.name.IndexOf("Slope") > -1)
                            return NetInfoVersion.Slope;
                        else
                            return NetInfoVersion.Sunken;
                    }

                }
            }
            return NetInfoVersion.None;
        }
        public static StationTrackType DeduceTrackType(this NetInfo info)
        {
            if (info.IsStationTrack())
            {
                var sortedVehicleLanes = info.m_lanes.Where(l => l.m_laneType == NetInfo.LaneType.Vehicle && (l.m_vehicleType & (VehicleInfo.VehicleType.Train | VehicleInfo.VehicleType.Metro)) != VehicleInfo.VehicleType.None).OrderBy(o => o.m_position).ToArray();
                switch (sortedVehicleLanes.Count())
                {
                    case 1:
                        return StationTrackType.SinglePlatform;
                    case 2:
                        if (sortedVehicleLanes.All(s => s.m_position > -0.1f && s.m_position < 0.1f))
                        {
                            return StationTrackType.SinglePlatform;
                        }
                        if (sortedVehicleLanes[0].m_position < -1.9f && sortedVehicleLanes[0].m_position > -2.1f)
                        {
                            if (sortedVehicleLanes[1].m_position > 1.9f && sortedVehicleLanes[1].m_position < 2.1f)
                            {
                                return StationTrackType.SidePlatform;
                            }
                        }
                        else if (sortedVehicleLanes[0].m_position <= -6f && sortedVehicleLanes[0].m_position > -6.6f)
                        {
                            if (sortedVehicleLanes[1].m_position >= 6f && sortedVehicleLanes[1].m_position < 6.6f)
                            {
                                return StationTrackType.IslandPlatform;
                            }
                        }
                        break;
                    case 4:
                        if (sortedVehicleLanes[0].m_position < -5.785f && sortedVehicleLanes[0].m_position > -5.985f)
                        {
                            if (sortedVehicleLanes[1].m_position < -1.9f && sortedVehicleLanes[1].m_position > -2.1f)
                            {
                                if (sortedVehicleLanes[2].m_position > 1.9f && sortedVehicleLanes[2].m_position < 2.1f)
                                {
                                    if (sortedVehicleLanes[3].m_position > 5.785f && sortedVehicleLanes[3].m_position < 5.985f)
                                    {
                                        return StationTrackType.ExpressSidePlatform;
                                    }
                                }
                            }
                        }
                        else if (sortedVehicleLanes[0].m_position < -12.28f && sortedVehicleLanes[0].m_position > -12.48f)
                        {
                            if (sortedVehicleLanes[1].m_position < -1.9f && sortedVehicleLanes[1].m_position > -2.1f)
                            {
                                if (sortedVehicleLanes[2].m_position > 1.9f && sortedVehicleLanes[2].m_position < 2.1f)
                                {
                                    if (sortedVehicleLanes[3].m_position > 12.28f && sortedVehicleLanes[3].m_position < 12.48f)
                                    {
                                        return StationTrackType.DualIslandPlatform;
                                    }
                                }
                            }
                        }
                        break;
                }
            }
            return StationTrackType.None;
        }
    }
}
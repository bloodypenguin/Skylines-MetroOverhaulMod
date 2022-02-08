using MetroOverhaul.NEXT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MetroOverhaul.Extensions
{
    public static class NetInfoExtensions
    {
        public static bool IsMOMMetro(this NetInfo info)
        {
            return info != null && info.GetAI() is IMOMMetroTrackAI;
        }
        public static bool IsUndergroundMOMMetro(this NetInfo info)
        {
            return info != null && info.GetAI() is MOMMetroTrackTunnelAI && (info.m_setCitizenFlags | CitizenInstance.Flags.Underground) != CitizenInstance.Flags.None;
        }
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
                   (info.IsTrainTrackAI() && info.IsStationTrack() && info.DeduceTrackType() == NetInfoStationTrackType.SidePlatform);
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
                   (info.IsTrainTrackBridgeAI() && info.IsStationTrack() && info.DeduceTrackType() == NetInfoStationTrackType.SidePlatform);
        }
        public static bool IsSunkenSidePlatformTrainStationTrack(this NetInfo info)
        {
            if (info?.name == null)
            {
                return false;
            }
            return info.name == ModTrackNames.TRAIN_STATION_TRACK_SUNKEN ||
                (info.IsTrainTrackTunnelAI() && info.IsStationTrack() && info.DeduceTrackType() == NetInfoStationTrackType.SidePlatform && info.name.Contains("Sunken"));
        }

        public static bool IsGroundSinglePlatformTrainStationTrack(this NetInfo info)
        {
            if (info?.name == null)
            {
                return false;
            }
            return info.name == ModTrackNames.TRAIN_STATION_TRACK_GROUND_SMALL1 || info.name == ModTrackNames.TRAIN_STATION_TRACK_GROUND_SMALL2 ||
                   (info.IsTrainTrackAI() && info.IsStationTrack() && info.DeduceTrackType() == NetInfoStationTrackType.SinglePlatform);
        }
        public static bool IsElevatedSinglePlatformTrainStationTrack(this NetInfo info)
        {
            if (info?.name == null)
            {
                return false;
            }
            return info.name == ModTrackNames.TRAIN_STATION_TRACK_ELEVATED_SMALL ||
                   (info.IsTrainTrackBridgeAI() && info.IsStationTrack() && info.DeduceTrackType() == NetInfoStationTrackType.SinglePlatform);
        }
        public static bool IsGroundIslandPlatformTrainStationTrack(this NetInfo info)
        {
            if (info?.name == null)
            {
                return false;
            }
            return info.name == ModTrackNames.TRAIN_STATION_TRACK_GROUND_ISLAND ||
                   (info.IsTrainTrackAI() && info.IsStationTrack() && info.DeduceTrackType() == NetInfoStationTrackType.IslandPlatform);
        }
        public static bool IsElevatedIslandPlatformTrainStationTrack(this NetInfo info)
        {
            if (info?.name == null)
            {
                return false;
            }
            return info.name == ModTrackNames.TRAIN_STATION_TRACK_ELEVATED_ISLAND ||
                   (info.IsTrainTrackBridgeAI() && info.IsStationTrack() && info.DeduceTrackType() == NetInfoStationTrackType.IslandPlatform);
        }

        public static bool IsGroundLargeSidePlatformTrainStationTrack(this NetInfo info)
        {
            if (info?.name == null)
            {
                return false;
            }
            return info.name == ModTrackNames.TRAIN_STATION_TRACK_GROUND_LARGE ||
                   (info.IsTrainTrackAI() && info.IsStationTrack() && info.DeduceTrackType() == NetInfoStationTrackType.ExpressSidePlatform);
        }
        public static bool IsElevatedLargeSidePlatformTrainStationTrack(this NetInfo info)
        {
            if (info?.name == null)
            {
                return false;
            }
            return info.name == ModTrackNames.TRAIN_STATION_TRACK_ELEVATED_LARGE ||
                   (info.IsTrainTrackBridgeAI() && info.IsStationTrack() && info.DeduceTrackType() == NetInfoStationTrackType.ExpressSidePlatform);
        }

        public static bool IsGroundDualIslandPlatformTrainStationTrack(this NetInfo info)
        {
            if (info?.name == null)
            {
                return false;
            }
            return info.name == ModTrackNames.TRAIN_STATION_TRACK_GROUND_LARGE_DUALISLAND ||
                   (info.IsTrainTrackAI() && info.IsStationTrack() && info.DeduceTrackType() == NetInfoStationTrackType.DualIslandPlatform);
        }
        public static bool IsElevatedDualIslandPlatformTrainStationTrack(this NetInfo info)
        {
            if (info?.name == null)
            {
                return false;
            }
            return info.name == ModTrackNames.TRAIN_STATION_TRACK_ELEVATED_LARGE_DUALISLAND ||
                   (info.IsTrainTrackBridgeAI() && info.IsStationTrack() && info.DeduceTrackType() == NetInfoStationTrackType.DualIslandPlatform);
        }

        public static bool IsStationTrack(this NetInfo info)
        {
            return info != null && info.m_lanes.Any(l => l.m_stopType != VehicleInfo.VehicleType.None);
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
                   (info.IsMetroTrackAI() && info.IsStationTrack() && info.DeduceTrackType() == NetInfoStationTrackType.SidePlatform);
        }

        public static bool IsElevatedSidePlatformMetroStationTrack(this NetInfo info)
        {
            if (info?.name == null)
            {
                return false;
            }
            return Regex.IsMatch(info.name, "(Steel )?" + ModTrackNames.MOM_STATION_TRACK_ELEVATED + "( 01)?\b") ||
                   (info.IsMetroTrackBridgeAI() && info.IsStationTrack() && info.DeduceTrackType() == NetInfoStationTrackType.SidePlatform);
        }

        public static bool IsSunkenSidePlatformMetroStationTrack(this NetInfo info)
        {
            if (info?.name == null)
            {
                return false;
            }
            return Regex.IsMatch(info.name, "(Steel )?" + ModTrackNames.MOM_STATION_TRACK_SUNKEN + "\b") ||
                (info.IsMetroTrackTunnelAI() && info.IsStationTrack() && info.DeduceTrackType() == NetInfoStationTrackType.SidePlatform && info.name.Contains("Sunken"));
        }

        public static bool IsGroundSinglePlatformMetroStationTrack(this NetInfo info)
        {
            if (info?.name == null)
            {
                return false;
            }
            return Regex.IsMatch(info.name, "(Steel )?" + ModTrackNames.MOM_STATION_TRACK_GROUND_SMALL + "\b") ||
                   (info.IsMetroTrackAI() && info.IsStationTrack() && info.DeduceTrackType() == NetInfoStationTrackType.SinglePlatform);
        }

        public static bool IsElevatedSinglePlatformMetroStationTrack(this NetInfo info)
        {
            if (info?.name == null)
            {
                return false;
            }
            return Regex.IsMatch(info.name, "(Steel )?" + ModTrackNames.MOM_STATION_TRACK_ELEVATED_SMALL + "\b") ||
                   (info.IsMetroTrackBridgeAI() && info.IsStationTrack() && info.DeduceTrackType() == NetInfoStationTrackType.SinglePlatform);
        }

        public static bool IsGroundIslandPlatformMetroStationTrack(this NetInfo info)
        {
            if (info?.name == null)
            {
                return false;
            }
            return Regex.IsMatch(info.name, "(Steel )?" + ModTrackNames.MOM_STATION_TRACK_GROUND_ISLAND + "\b") ||
                   (info.IsMetroTrackAI() && info.IsStationTrack() && info.DeduceTrackType() == NetInfoStationTrackType.IslandPlatform);
        }
        public static bool IsElevatedIslandPlatformMetroStationTrack(this NetInfo info)
        {
            if (info?.name == null)
            {
                return false;
            }
            return Regex.IsMatch(info.name, "(Steel )?" + ModTrackNames.MOM_STATION_TRACK_ELEVATED_ISLAND + "\b") ||
                   (info.IsMetroTrackBridgeAI() && info.IsStationTrack() && info.DeduceTrackType() == NetInfoStationTrackType.IslandPlatform);
        }

        public static bool IsGroundLargeSidePlatformMetroStationTrack(this NetInfo info)
        {
            if (info?.name == null)
            {
                return false;
            }
            return Regex.IsMatch(info.name, "(Steel )?" + ModTrackNames.MOM_STATION_TRACK_GROUND_LARGE + "\b") ||
                   (info.IsMetroTrackAI() && info.IsStationTrack() && info.DeduceTrackType() == NetInfoStationTrackType.ExpressSidePlatform);
        }
        public static bool IsElevatedLargeSidePlatformMetroStationTrack(this NetInfo info)
        {
            if (info?.name == null)
            {
                return false;
            }
            return Regex.IsMatch(info.name, "(Steel )?" + ModTrackNames.MOM_STATION_TRACK_ELEVATED_LARGE + "\b") ||
                   (info.IsMetroTrackBridgeAI() && info.IsStationTrack() && info.DeduceTrackType() == NetInfoStationTrackType.ExpressSidePlatform);
        }

        public static bool IsGroundDualIslandPlatformMetroStationTrack(this NetInfo info)
        {
            if (info?.name == null)
            {
                return false;
            }
            return Regex.IsMatch(info.name, "(Steel )?" + ModTrackNames.MOM_STATION_TRACK_GROUND_LARGE_DUALISLAND + "\b") ||
                   (info.IsMetroTrackAI() && info.IsStationTrack() && info.DeduceTrackType() == NetInfoStationTrackType.DualIslandPlatform);
        }
        public static bool IsElevatedDualIslandPlatformMetroStationTrack(this NetInfo info)
        {
            if (info?.name == null)
            {
                return false;
            }
            return Regex.IsMatch(info.name, "(Steel )?" + ModTrackNames.MOM_STATION_TRACK_ELEVATED_LARGE_DUALISLAND + "\b") ||
                   (info.IsMetroTrackBridgeAI() && info.IsStationTrack() && info.DeduceTrackType() == NetInfoStationTrackType.DualIslandPlatform);
        }

        public static bool IsUndergroundMetroStationTrack(this NetInfo netInfo)
        {
            return netInfo != null && netInfo.IsStationTrack() && netInfo.GetAI() is MOMMetroTrackTunnelAI;
        }
        public static bool IsUndergroundSidePlatformMetroStationTrack(this NetInfo netInfo)
        {
            if (netInfo?.name == null)
            {
                return false;
            }
            return netInfo.name == Util.GetMetroStationTrackName(NetInfoVersion.Tunnel, NetInfoTrackStyle.Vanilla) || netInfo.name == Util.GetMetroStationTrackName(NetInfoVersion.Tunnel, NetInfoTrackStyle.Modern);
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
            return netInfo != null && !netInfo.IsStationTrack() && netInfo?.GetAI() is MOMMetroTrackTunnelAI;
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

        private static NetInfoStationTrackType DeduceTrackType(this NetInfo info)
        {
            if (info.IsStationTrack())
            {
                var sortedVehicleLanes = info.m_lanes.Where(l => l.m_laneType == NetInfo.LaneType.Vehicle && (l.m_vehicleType & (VehicleInfo.VehicleType.Train | VehicleInfo.VehicleType.Metro)) != VehicleInfo.VehicleType.None).OrderBy(o => o.m_position).ToArray();
                switch (sortedVehicleLanes.Count())
                {
                    case 1:
                        return NetInfoStationTrackType.SinglePlatform;
                    case 2:
                        if (sortedVehicleLanes.All(s => s.m_position > -0.1f && s.m_position < 0.1f))
                        {
                            return NetInfoStationTrackType.SinglePlatform;
                        }
                        if (sortedVehicleLanes[0].m_position < -1.9f && sortedVehicleLanes[0].m_position > -2.1f)
                        {
                            if (sortedVehicleLanes[1].m_position > 1.9f && sortedVehicleLanes[1].m_position < 2.1f)
                            {
                                return NetInfoStationTrackType.SidePlatform;
                            }
                        }
                        else if (sortedVehicleLanes[0].m_position < -6.4f && sortedVehicleLanes[0].m_position > -6.6f)
                        {
                            if (sortedVehicleLanes[1].m_position > 6.4f && sortedVehicleLanes[1].m_position < 6.6f)
                            {
                                return NetInfoStationTrackType.IslandPlatform;
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
                                        return NetInfoStationTrackType.ExpressSidePlatform;
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
                                        return NetInfoStationTrackType.DualIslandPlatform;
                                    }
                                }
                            }
                        }
                        break;
                }
            }
            return NetInfoStationTrackType.None;
        }

        public static NetInfoStationTrackType GetStationTrackType(this NetInfo info)
        {
            if (info.IsStationTrack())
            {
                var vehicleLaneCount = info.m_lanes.Where(l => l.m_laneType == NetInfo.LaneType.Vehicle).Count();
                if (vehicleLaneCount == 1)
                {
                    return NetInfoStationTrackType.SinglePlatform;
                }

                var sortedFilteredLanes = new List<NetInfo.Lane>();
                for (int i = 0; i < info.m_sortedLanes.Count(); i++)
                {
                    var laneIndex = info.m_sortedLanes[i];
                    var lane = info.m_lanes[laneIndex];
                    if (lane != null)
                    {
                        if (lane.m_laneType == NetInfo.LaneType.Vehicle || lane.m_laneType == NetInfo.LaneType.Pedestrian)
                        {
                            if (i == 0 || lane.m_position != info.m_lanes[laneIndex - 1].m_position)
                            {
                                sortedFilteredLanes.Add(lane);
                            }
                        }
                    }
                }

                var sortedFilteredLaneCount = sortedFilteredLanes.Count();
                int tmp;
                int stationTrackCode = 1 << sortedFilteredLaneCount;
                for (int i = 0; i < sortedFilteredLaneCount; i++)
                {
                    tmp = sortedFilteredLanes[i].m_laneType == NetInfo.LaneType.Vehicle ? 1 : 0;
                    stationTrackCode |= tmp << (sortedFilteredLaneCount - i - 1);
                }
                if (Enum.IsDefined(typeof(NetInfoStationTrackType), stationTrackCode))
                    return (NetInfoStationTrackType)stationTrackCode;
            }
            return NetInfoStationTrackType.None;
        }

        public static NetInfoTrackType GetTrackType(this NetInfo info)
        {
            if (!info.IsStationTrack())
            {
                var sortedFilteredLanes = new List<NetInfo.Lane>();
                if (info.m_lanes != null && info.m_lanes.Count() > 0)
                {
                    var vehicleLanes = new List<NetInfo.Lane>();
                    for (int i = 0; i < info.m_sortedLanes.Count(); i++)
                    {
                        var lane = info.m_lanes[info.m_sortedLanes[i]];
                        if (lane.m_laneType == NetInfo.LaneType.Vehicle)
                        {
                            sortedFilteredLanes.Add(lane);
                        }
                    }
                }
                switch (sortedFilteredLanes.Count())
                {
                    case 1:
                        return NetInfoTrackType.OneLaneOneWay;
                    case 2:
                        var lanePosition = sortedFilteredLanes[0].m_position;
                        var allInOne = true;
                        for (int i = 1; i < sortedFilteredLanes.Count(); i++)
                        {
                            if (sortedFilteredLanes[i].m_position != lanePosition)
                            {
                                allInOne = false;
                            }
                        }
                        if (allInOne)
                            return NetInfoTrackType.OneLaneTwoWay;

                        if (sortedFilteredLanes[0].m_similarLaneCount == sortedFilteredLanes.Count())
                            return NetInfoTrackType.TwoLaneOneWay;

                        return NetInfoTrackType.TwoLaneTwoWay;
                    case 4:
                        return NetInfoTrackType.Quad;
                }
            }
            return NetInfoTrackType.None;
        }

        public static NetInfo ConvertTrack(this NetInfo netInfo, NetInfoMetadata fieldsToChange)
        {
            var netInfoMetadata = AbstractInitializer.GetNetInfoMetadata(netInfo.name);
            if (netInfoMetadata != null)
            {
                if (netInfo.IsStationTrack())
                {
                    if (fieldsToChange.StationTrackType == NetInfoStationTrackType.None)
                    {
                        fieldsToChange.StationTrackType = netInfoMetadata.StationTrackType;
                    }
                }
                else
                {
                    if (fieldsToChange.TrackType == NetInfoTrackType.None)
                    {
                        fieldsToChange.TrackType = netInfoMetadata.TrackType;
                    }
                }
                if (fieldsToChange.Version == NetInfoVersion.None)
                {
                    fieldsToChange.Version = netInfoMetadata.Version;
                }
                if (fieldsToChange.TrackStyle == NetInfoTrackStyle.None)
                {
                    fieldsToChange.TrackStyle = netInfoMetadata.TrackStyle;
                }

                return fieldsToChange.Lookup().Info;
            }
            return null;
        }
    }
}
using MetroOverhaul.NEXT.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MetroOverhaul.Extensions
{
    public static partial class BuildingInfoExtensions
    {
        public static bool IsMetroDepot(this BuildingInfo info)
        {
            if (info.m_buildingAI == null && info.m_class == null)
            {
                return false;
            }
            return info.m_buildingAI.GetType() == typeof(DepotAI) && info.m_class.m_service == ItemClass.Service.PublicTransport && info.m_class.m_subService == ItemClass.SubService.PublicTransportMetro;
        }
        
        public static bool IsTrainStation(this BuildingInfo info)
        {
            if (info.m_buildingAI == null && info.m_class == null)
            {
                return false;
            }
            return info.m_buildingAI is TransportStationAI && info.m_class.m_subService == ItemClass.SubService.PublicTransportTrain;
        }

        public static bool IsMetroStation(this BuildingInfo info)
        {
            if (info.m_buildingAI == null && info.m_class == null)
            {
                return false;
            }
            return info.m_buildingAI is TransportStationAI && info.m_class.m_subService == ItemClass.SubService.PublicTransportMetro;
        }

        public static bool HasAbovegroundTrainStationTracks(this BuildingInfo info)
        {
            if (info?.m_paths != null && info.m_paths.Any(p => (p?.m_finalNetInfo != null && p.m_finalNetInfo.IsAbovegroundTrainStationTrack()) || (p?.m_netInfo != null && p.m_netInfo.IsAbovegroundTrainStationTrack())))
            {
                return true;
            }
            else if (info.m_subBuildings != null)
            {
                for (int i = 0; i < info.m_subBuildings.Length; i++)
                {
                    var subBuilding = info.m_subBuildings[i]?.m_buildingInfo;
                    if (subBuilding != null)
                    {
                        if (subBuilding.HasAbovegroundTrainStationTracks())
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public static bool HasAbovegroundMetroStationTracks(this BuildingInfo info)
        {
            if (info?.m_paths != null && info.m_paths.Any(p => (p?.m_finalNetInfo != null && p.m_finalNetInfo.IsAbovegroundMetroStationTrack()) || (p?.m_netInfo != null && p.m_netInfo.IsAbovegroundMetroStationTrack())))
            {
                return true;
            }
            else if (info.m_subBuildings != null)
            {
                for (int i = 0; i < info.m_subBuildings.Length; i++)
                {
                    var subBuilding = info.m_subBuildings[i]?.m_buildingInfo;
                    if (subBuilding != null)
                    {
                        if (subBuilding.HasAbovegroundMetroStationTracks())
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool HasUndergroundMetroStationTracks(this BuildingInfo info, bool isDeep = true)
        {
            if (info?.m_paths != null && info.m_paths.Any(p => (p?.m_finalNetInfo != null && p.m_finalNetInfo.IsUndergroundMetroStationTrack()) || (p?.m_netInfo != null && p.m_netInfo.IsUndergroundMetroStationTrack())))
            {
                return true;
            }
            else if (isDeep && info.m_subBuildings != null)
            {
                for (int i = 0; i < info.m_subBuildings.Length; i++)
                {
                    var subBuilding = info.m_subBuildings[i]?.m_buildingInfo;
                    if (subBuilding != null)
                    {
                        if (subBuilding.HasUndergroundMetroStationTracks())
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public static string GetAnalogName(this BuildingInfo info)
        {
            if (info.name.IndexOf(ModTrackNames.ANALOG_PREFIX) == -1)
            {
                if (info.IsTrainStation())
                {
                    return info.name + ModTrackNames.ANALOG_PREFIX + TrackVehicleType.Metro.ToString();
                }
                if (info.IsMetroStation())
                {
                    return info.name + ModTrackNames.ANALOG_PREFIX + TrackVehicleType.Train.ToString();
                }
                throw new Exception($"Getting analog name for unsupported building type. Asset: {info?.name}");
            }
            else
            {
                return info.name.Substring(0, info.name.IndexOf(ModTrackNames.ANALOG_PREFIX));
            }
        }
        public static void SetTrackVehicleType(this BuildingInfo info, int inx, TrackVehicleType vehicleType)
        {
            var path = info.AbovegroundStationPaths()[inx];
            switch (vehicleType)
            {
                case TrackVehicleType.Default:
                    if ((path.m_finalNetInfo.IsElevatedSidePlatformTrainStationTrack() || path.m_finalNetInfo.IsGroundSinglePlatformTrainStationTrack()) && (path.m_netInfo.IsElevatedSidePlatformMetroStationTrack() || path.m_netInfo.IsGroundSinglePlatformMetroStationTrack()) ||
                        (path.m_finalNetInfo.IsElevatedSidePlatformMetroStationTrack() || path.m_finalNetInfo.IsGroundSinglePlatformMetroStationTrack()) && (path.m_netInfo.IsElevatedSidePlatformTrainStationTrack() || path.m_netInfo.IsGroundSinglePlatformTrainStationTrack()))
                    {
                        InvertPath(path);
                    }
                    path.AssignNetInfo(path.m_netInfo);
                    break;
                case TrackVehicleType.Train:
                    if (path.m_finalNetInfo.IsAbovegroundMetroStationTrack())
                    {
                        if (path.m_finalNetInfo.IsElevatedSidePlatformMetroStationTrack())
                        {
                            path.AssignNetInfo(ModTrackNames.TRAIN_STATION_TRACK_ELEVA, false); //somehow expand this for the other types
                        }
                        else if (path.m_finalNetInfo.IsSunkenSidePlatformMetroStationTrack())
                        {
                            path.AssignNetInfo(ModTrackNames.TRAIN_STATION_TRACK_SUNKEN, false);
                        }
                        else if (path.m_finalNetInfo.IsGroundSidePlatformMetroStationTrack())
                        {
                            path.AssignNetInfo(ModTrackNames.TRAIN_STATION_TRACK_GROUND, false);
                        }
                        else if (path.m_finalNetInfo.IsElevatedSinglePlatformMetroStationTrack())
                        {
                            InvertPath(path);
                            path.AssignNetInfo(ModTrackNames.TRAIN_STATION_TRACK_ELEVATED_SMALL, false);
                        }
                        else if (path.m_finalNetInfo.IsGroundSinglePlatformMetroStationTrack())
                        {
                            InvertPath(path);
                            path.AssignNetInfo(ModTrackNames.TRAIN_STATION_TRACK_GROUND_SMALL1, false);
                        }
                        else if (path.m_finalNetInfo.IsElevatedIslandPlatformMetroStationTrack())
                        {
                            path.AssignNetInfo(ModTrackNames.TRAIN_STATION_TRACK_ELEVATED_ISLAND, false);
                        }
                        else if (path.m_finalNetInfo.IsGroundIslandPlatformMetroStationTrack())
                        {
                            path.AssignNetInfo(ModTrackNames.TRAIN_STATION_TRACK_GROUND_ISLAND, false);
                        }
                        else if (path.m_finalNetInfo.IsElevatedLargeSidePlatformMetroStationTrack())
                        {
                            path.AssignNetInfo(ModTrackNames.TRAIN_STATION_TRACK_ELEVATED_LARGE, false);
                        }
                        else if (path.m_finalNetInfo.IsGroundLargeSidePlatformMetroStationTrack())
                        {
                            path.AssignNetInfo(ModTrackNames.TRAIN_STATION_TRACK_GROUND_LARGE, false);
                        }
                        else if (path.m_finalNetInfo.IsElevatedDualIslandPlatformMetroStationTrack())
                        {
                            path.AssignNetInfo(ModTrackNames.TRAIN_STATION_TRACK_ELEVATED_LARGE_DUALISLAND, false);
                        }
                        else if (path.m_finalNetInfo.IsGroundDualIslandPlatformMetroStationTrack())
                        {
                            path.AssignNetInfo(ModTrackNames.TRAIN_STATION_TRACK_GROUND_LARGE_DUALISLAND, false);
                        }
                    }
                    else if (path.m_finalNetInfo.IsMetroTrack())
                    {
                        if (path.m_finalNetInfo)
                            path.AssignNetInfo(ModTrackNames.GetTrackAnalogName(path.m_finalNetInfo.name), false);
                        //FixSlopes(info, inx, vehicleType);
                    }
                    break;
                case TrackVehicleType.Metro:
                    if (path.m_finalNetInfo.IsAbovegroundTrainStationTrack())
                    {
                        if (path.m_finalNetInfo.IsElevatedSidePlatformTrainStationTrack())
                        {
                            path.AssignNetInfo(ModTrackNames.MOM_STATION_TRACK_ELEVATED, false);
                        }
                        else if (path.m_finalNetInfo.IsSunkenSidePlatformTrainStationTrack())
                        {
                            path.AssignNetInfo(ModTrackNames.MOM_STATION_TRACK_SUNKEN, false);
                        }
                        else if (path.m_finalNetInfo.IsGroundSidePlatformTrainStationTrack())
                        {
                            path.AssignNetInfo(ModTrackNames.MOM_STATION_TRACK_GROUND, false);
                        }
                        else if (path.m_finalNetInfo.IsElevatedSinglePlatformTrainStationTrack())
                        {
                            InvertPath(path);
                            path.AssignNetInfo(ModTrackNames.MOM_STATION_TRACK_ELEVATED_SMALL, false);
                        }
                        else if (path.m_finalNetInfo.IsGroundSinglePlatformTrainStationTrack())
                        {
                            InvertPath(path);
                            path.AssignNetInfo(ModTrackNames.MOM_STATION_TRACK_GROUND_SMALL, false);
                        }
                        else if (path.m_finalNetInfo.IsElevatedIslandPlatformTrainStationTrack())
                        {
                            path.AssignNetInfo(ModTrackNames.MOM_STATION_TRACK_ELEVATED_ISLAND, false);
                        }
                        else if (path.m_finalNetInfo.IsGroundIslandPlatformTrainStationTrack())
                        {
                            path.AssignNetInfo(ModTrackNames.MOM_STATION_TRACK_GROUND_ISLAND, false);
                        }
                        else if (path.m_finalNetInfo.IsElevatedLargeSidePlatformTrainStationTrack())
                        {
                            path.AssignNetInfo(ModTrackNames.MOM_STATION_TRACK_ELEVATED_LARGE, false);
                        }
                        else if (path.m_finalNetInfo.IsGroundLargeSidePlatformTrainStationTrack())
                        {
                            path.AssignNetInfo(ModTrackNames.MOM_STATION_TRACK_GROUND_LARGE, false);
                        }
                        else if (path.m_finalNetInfo.IsElevatedDualIslandPlatformTrainStationTrack())
                        {
                            path.AssignNetInfo(ModTrackNames.MOM_STATION_TRACK_ELEVATED_LARGE_DUALISLAND, false);
                        }
                        else if (path.m_finalNetInfo.IsGroundDualIslandPlatformTrainStationTrack())
                        {
                            path.AssignNetInfo(ModTrackNames.MOM_STATION_TRACK_GROUND_LARGE_DUALISLAND, false);
                        }
                    }
                    else if (path.m_finalNetInfo.IsTrainTrack())
                    {
                        path.AssignNetInfo(ModTrackNames.GetTrackAnalogName(path.m_finalNetInfo.name), false);
                        //FixSlopes(info, inx, vehicleType);
                    }
                    break;
            }
        }

        private static void FixSlopes(BuildingInfo info, int inx, TrackVehicleType vehicletype)
        {
            var path = info.m_paths[inx];
            var horizontalDistance = Vector3.Distance(
                new Vector3(path.m_nodes[0].x, path.m_nodes[0].y, path.m_nodes[0].z),
                new Vector3(path.m_nodes[1].x, path.m_nodes[0].y,
                    path.m_nodes[1]
                        .z)); //Math.Pow(Math.Pow(path.m_nodes[0].x - path.m_nodes[1].x, 2) + Math.Pow(path.m_nodes[0].x - path.m_nodes[1].x, 2), 0.5f);

            var verticalDistance = Math.Abs(path.m_nodes[0].y - path.m_nodes[1].y);
            if (path.m_netInfo.m_maxSlope < Math.Abs(verticalDistance / horizontalDistance))
            {
                bool isTargetVehicleTrackType = false;
                float slopeDist = ((float) Math.Abs(verticalDistance / horizontalDistance)) - path.m_netInfo.m_maxSlope;

                var pathList = new List<BuildingInfo.PathInfo>();
                Vector3 targetNode = Vector3.zero;
                Vector3 otherNode = Vector3.zero;
                Vector3 adjTargetNode = Vector3.zero;
                for (int i = 0; i < info.m_paths.Length; i++)
                {
                    if (info.m_paths[i]?.m_netInfo?.name == null)
                    {
                        continue;
                    }

                    if (targetNode != Vector3.zero)
                    {
                        break;
                    }

                    isTargetVehicleTrackType = false;
                    switch (vehicletype)
                    {
                        case TrackVehicleType.Train:
                            isTargetVehicleTrackType = info.m_paths[i].m_netInfo.IsTrainTrackAI();
                            break;
                        case TrackVehicleType.Metro:
                            isTargetVehicleTrackType = info.m_paths[i].m_netInfo.IsMetroTrackAI();
                            break;
                    }

                    if (isTargetVehicleTrackType && i != inx)
                    {
                        Debug.Log("info to compare is " + info.m_paths[i].m_netInfo.name);
                        //if (info.m_paths[i].m_netInfo.name.Contains("Ground") ||
                        //    info.m_paths[i].m_netInfo.name == ModTrackNames.TRAIN_TRACK ||
                        //    info.m_paths[i].m_netInfo.name == ModTrackNames.TRAIN_STATION_TRACK_GROUND) ;
                        //{
                        for (int j = 0; j < info.m_paths[i].m_nodes.Length; j++)
                        {
                            var targetIndex = Array.IndexOf(path.m_nodes, info.m_paths[i].m_nodes[j]);
                            if (targetIndex > -1)
                            {
                                otherNode = path.m_nodes[((targetIndex + 1) % 2)];
                                targetNode = path.m_nodes[targetIndex];
                                adjTargetNode = otherNode + ((targetNode - otherNode) * (1 + slopeDist));
                                break;
                            }
                        }

                        //}
                    }
                }

                if (targetNode != Vector3.zero)
                {
                    Vector3[] nodeArray = null;
                    var findCount = 0; //debug stuff
                    for (int i = 0; i < info.m_paths.Length; i++)
                    {
                        isTargetVehicleTrackType = false;
                        switch (vehicletype)
                        {
                            case TrackVehicleType.Train:
                                isTargetVehicleTrackType = info.m_paths[i].m_netInfo.IsTrainTrackAI();
                                //groundName = ModTrackNames.TRAIN_TRACK;
                                break;
                            case TrackVehicleType.Metro:
                                isTargetVehicleTrackType = info.m_paths[i].m_netInfo.IsMetroTrackAI();
                                //groundName = ModTrackNames.MOM_TRACK_GROUND + " NoBar";
                                break;
                        }

                        if (isTargetVehicleTrackType && i != inx)
                        {
                            for (int j = 0; j < info.m_paths[i].m_nodes.Length; j++)
                            {
                                if (info.m_paths[i].m_nodes[j] == targetNode)
                                {
                                    otherNode = info.m_paths[i].m_nodes[((j + 1) % 2)];
                                    nodeArray = new Vector3[2];
                                    nodeArray[j] = adjTargetNode;
                                    nodeArray[((j + 1) % 2)] = otherNode;
                                    info.m_paths[i].m_nodes = nodeArray;
                                    findCount++;
                                    break;
                                }
                            }
                        }
                    }

                    otherNode = Vector3.zero;
                    nodeArray = null;
                    for (int i = 0; i < path.m_nodes.Length; i++)
                    {
                        if (path.m_nodes[i] == targetNode)
                        {
                            nodeArray = new Vector3[2];
                            otherNode = path.m_nodes[((i + 1) % 2)];
                            nodeArray[i] = adjTargetNode;
                            nodeArray[((i + 1) % 2)] = otherNode;
                            break;
                        }
                    }

                    if (nodeArray != null)
                    {
                        path.m_nodes = nodeArray;
                    }
                }
            }

        }

        public static List<BuildingInfo.PathInfo> UndergroundStationPaths(this BuildingInfo info, bool isDeep = true)
        {
            var trackList = new List<BuildingInfo.PathInfo>();
            if (isDeep && info?.m_subBuildings != null)
            {
                foreach (var subBuilding in info.m_subBuildings)
                {
                    if (subBuilding?.m_buildingInfo != null && subBuilding.m_buildingInfo.HasUndergroundMetroStationTracks())
                    {
                        if (subBuilding.m_buildingInfo.m_subBuildings != null)
                        {
                            trackList.AddRange(UndergroundStationPaths(subBuilding.m_buildingInfo));
                        }
                    }
                }
            }
            if (info != null)
            {
                foreach (var path in info.m_paths)
                {
                    if (path?.m_netInfo != null && path.m_netInfo.IsUndergroundMetroStationTrack())
                    {
                        trackList.Add(path);
                    }
                }
            }

            return trackList;
        }

        public static List<BuildingInfo.PathInfo> AbovegroundStationPaths(this BuildingInfo info, bool isDeep = true)
        {
            var trackList = new List<BuildingInfo.PathInfo>();
            if (isDeep && info?.m_subBuildings != null)
            {
                foreach (var subBuilding in info.m_subBuildings)
                {
                    if (subBuilding?.m_buildingInfo != null && (subBuilding.m_buildingInfo.HasAbovegroundMetroStationTracks() || subBuilding.m_buildingInfo.HasAbovegroundTrainStationTracks()))
                    {
                        if (subBuilding.m_buildingInfo.m_subBuildings != null)
                        {
                            trackList.AddRange(AbovegroundStationPaths(subBuilding.m_buildingInfo));
                        }
                    }
                }
            }
            if (info != null)
            {
                foreach (var path in info.m_paths)
                {
                    if (path?.m_netInfo != null && (path.m_netInfo.IsAbovegroundMetroStationTrack() || path.m_netInfo.IsAbovegroundTrainStationTrack()))
                    {
                        trackList.Add(path);
                    }
                }
            }

            return trackList;
        }

        private static int m_Count = 0;
        public static KeyValuePair<BuildingInfo, BuildingInfo.PathInfo> StationPathAtIndex(this BuildingInfo info, int index, bool isDeep = true)
        {
            KeyValuePair<BuildingInfo, BuildingInfo.PathInfo> buildingTrack = default;
            if (isDeep && info?.m_subBuildings != null)
            {
                foreach (var subBuilding in info.m_subBuildings)
                {
                    if (subBuilding?.m_buildingInfo != null && subBuilding.m_buildingInfo.HasUndergroundMetroStationTracks())
                    {
                        if (subBuilding.m_buildingInfo.m_subBuildings != null)
                        {
                            buildingTrack = subBuilding.m_buildingInfo.StationPathAtIndex(index);
                            if (buildingTrack.Key != null)
                                break;
                        }
                    }
                }
            }
            if (info != null && buildingTrack.Key == null)
            {
                foreach (var path in info.m_paths)
                {
                    if (path?.m_netInfo != null && path.m_netInfo.IsUndergroundMetroStationTrack())
                    {
                        if (index == m_Count)
                        {
                            buildingTrack = new KeyValuePair<BuildingInfo, BuildingInfo.PathInfo>(info, path);
                            m_Count = 0;
                            break;
                        }
                        else
                            m_Count++;
                    }
                }
            }

            return buildingTrack;
        }

        private static BuildingInfo.PathInfo m_CachedPath;
        private static BuildingInfo m_CachedBuilding;
        private static void GetCachedStuff(this BuildingInfo info, int index)
        {
            if (m_CachedPath == null)
            {
                if (index > -1 && info.UndergroundStationPaths().Count() > index)
                {
                    var buildingPath = info.StationPathAtIndex(index);
                    if (buildingPath.Key != null)
                    {
                        m_CachedBuilding = buildingPath.Key;
                        m_CachedPath = buildingPath.Value;
                    }

                }
            }
        }
        public static BuildingInfo.PathInfo RemoveStationPath(this BuildingInfo info, int atIndex)
        {
            m_CachedPath = null;
            info.GetCachedStuff(atIndex);
            BuildingInfo.PathInfo retval = null;
            if (HasRemovedStationPath(info, atIndex))
                retval = m_CachedPath;
            return retval;
        }
        private static bool HasRemovedStationPath(BuildingInfo info, int atIndex)
        {
            var retval = false;
            var pathToRemove = m_CachedPath;
            var pathList = m_CachedBuilding.m_paths.ToList();
            if (pathList.Contains(pathToRemove) && info.UndergroundStationPaths().Count > 1)
            {
                pathList.Remove(pathToRemove);
                m_CachedBuilding.m_paths = pathList.ToArray();
                retval = true;
            }

            return retval;
        }
        public static BuildingInfo.PathInfo AddStationPath(this BuildingInfo info, int atIndex)
        {
            m_CachedPath = null;
            return m_AddStationPath(info, atIndex);
        }
        private static BuildingInfo.PathInfo m_AddStationPath(BuildingInfo info, int atIndex)
        {
            info.GetCachedStuff(atIndex);
            if (m_CachedPath == null)
            {
                return null;
            }
            var pathList = m_CachedBuilding.m_paths.ToList();
            var clonePath = m_CachedPath.ShallowClone();
            var verticalOffset = 0;
            var horizontalOffset = 0;
            var xOffset = clonePath.m_nodes.Last().x - clonePath.m_nodes.First().x;
            var zOffset = clonePath.m_nodes.Last().z - clonePath.m_nodes.First().z;
            if (xOffset > zOffset)
            {
                verticalOffset = 40;
            }
            else
            {
                horizontalOffset = 40;
            }
            var nodeList = new List<Vector3>();
            for (int i = 0; i < clonePath.m_nodes.Count(); i++)
            {
                nodeList.Add(new Vector3(clonePath.m_nodes[i].x + horizontalOffset, clonePath.m_nodes[i].y, clonePath.m_nodes[i].z + verticalOffset));
            }
            clonePath.m_nodes = nodeList.ToArray();
            clonePath.SetCurveTargets();
            pathList.Add(clonePath);
            m_CachedBuilding.m_paths = pathList.ToArray();
            if (clonePath == null)
                Debug.Log("Cloned path came out null");
            return clonePath;
        }
        public static List<BuildingInfo.PathInfo> GetPaths(this BuildingInfo prefab, bool compensateSubNodes = false)
        {
            if (prefab != null)
            {
                var retval = new List<BuildingInfo.PathInfo>();
                retval.AddRange(prefab.m_paths);
                if (prefab.m_subBuildings != null && prefab.m_subBuildings.Count() > 0)
                {
                    for (int i = 0; i < prefab.m_subBuildings.Count(); i++)
                    {
                        if (prefab?.m_subBuildings[i]?.m_buildingInfo?.m_paths != null)
                        {
                            if (prefab.m_subBuildings[i].m_buildingInfo.HasAbovegroundMetroStationTracks() || prefab.m_subBuildings[i].m_buildingInfo.HasAbovegroundTrainStationTracks())
                            {
                                //if (compensateSubNodes)
                                //{
                                //    foreach (var path in prefab.m_subBuildings[i].m_buildingInfo.m_paths)
                                //    {
                                //        var nodeList = new List<Vector3>();
                                //        for (int j = 0; j < path.m_nodes.Count(); j++)
                                //        {
                                //            nodeList.Add(path.m_nodes[j] + prefab.m_subBuildings[i].m_position);
                                //        }
                                //        path.m_nodes = nodeList.ToArray();
                                //        retval.Add(path);
                                //    }
                                //}
                                //else
                                //{
                                retval.AddRange(prefab.m_subBuildings[i].m_buildingInfo.m_paths);
                                //}
                            }
                        }
                    }
                }
                return retval;
            }
            return null;
        }
        private static int depth = 0;
        public static List<int> LinkedPaths(this BuildingInfo info, int inx, List<int> inxSoFar = null)
        {
            List<int> retval = new List<int>();

            var paths = info.AbovegroundStationPaths();
            var targetPath = paths[inx];

            if (inxSoFar == null)
            {
                inxSoFar = new List<int>();
            }
            else
            {
                retval.Add(inx);
            }
            inxSoFar.Add(inx);
            var trackPaths = paths.Where(p => p?.m_netInfo != null && (p.m_netInfo.IsTrainTrackAI() || p.m_netInfo.IsTrainTrackBridgeAI() || ((p.m_netInfo.IsTrainTrackTunnelAI() || p.m_netInfo.IsMetroTrackAI() || p.m_netInfo.IsMetroTrackBridgeAI() || p.m_netInfo.IsMetroTrackTunnelAI()) && (p.m_netInfo.name.Contains("Sunken") || !p.m_netInfo.IsStationTrack()))));
            for (int i = 0; i < paths.Count(); i++)
            {
                if (trackPaths.Contains(paths[i]))
                {
                    if (!inxSoFar.Contains(i))
                    {
                        if (targetPath.m_nodes.Any(n => paths[i].m_nodes.Contains(n)) && targetPath.m_nodes != paths[i].m_nodes)
                        {
                            depth++;
                            retval.AddRange(info.LinkedPaths(i, inxSoFar));
                            depth--;
                        }
                    }
                }
            }
            return retval;
        }

        public static List<BuildingInfo.PathInfo> LowestHighPaths(this BuildingInfo info)
        {
            var retval = new List<BuildingInfo.PathInfo>();
            retval = info.m_paths.Where(p => p.IsPedestrianPath() && p.m_nodes.Any(n => n.y > -4) && p.m_nodes.Any(nd => nd.y <= -4)).ToList();
            if (retval.Count == 0)
            {
                retval.Add(info.m_paths.Where(p => IsPedestrianPath(p))
                    .OrderByDescending(p => p.m_nodes[0].y)
                    .FirstOrDefault());
            }
            return retval;
        }

        public static Vector3 FindAverageNode(this BuildingInfo info, bool isDeep)
        {
            var center = Vector3.zero;
            var nodeList = new List<Vector3>();
            if (isDeep && info.m_subBuildings != null)
            {
                for (int i = 0; i < info.m_subBuildings.Length; i++)
                {
                    var subBuilding = info.m_subBuildings[i];
                    if (subBuilding?.m_buildingInfo != null && subBuilding.m_buildingInfo.HasUndergroundMetroStationTracks())
                    {
                        var parentMetaData = new ParentStationMetaData()
                        {
                            Angle = subBuilding.m_angle,
                            Position = subBuilding.m_position
                        };
                        var subStationPaths = subBuilding.m_buildingInfo.UndergroundStationPaths();
                        for (int j = 0; j < subStationPaths.Count(); j++)
                        {
                            var path = subStationPaths[j];
                            if (path?.m_nodes != null)
                            {
                                for (int k = 0; k < path.m_nodes.Length; k++)
                                {
                                    nodeList.Add(SetStationCustomizations.AdjustNodeForParent(path.m_nodes[k], parentMetaData));
                                }
                            }
                        }
                    }
                }
            }
            nodeList.AddRange(info.UndergroundStationPaths(false).SelectMany(p => p.m_nodes).ToArray());
            for (int i = 0; i < nodeList.Count(); i++)
            {
                center += nodeList[i];
            }
            Debug.Log("Node list count: " + nodeList.Count());
            center /= nodeList.Count();
            return center;
        }
        public static void StoreBuildingDefault(this BuildingInfo info)
        {
            if (StationBuildingCustomization.StoredStationPaths == null)
            {
                StationBuildingCustomization.StoredStationPaths = new Dictionary<string, List<BuildingInfo.PathInfo>>();
            }
            if (!StationBuildingCustomization.StoredStationPaths.ContainsKey(info.name))
            {
                if (info.m_subBuildings != null)
                {
                    for (int i = 0; i < info.m_subBuildings.Count(); i++)
                    {
                        var subBuildingInfo = info.m_subBuildings[i].m_buildingInfo;
                        if (subBuildingInfo != null && subBuildingInfo.HasUndergroundMetroStationTracks())
                        {
                            StoreBuildingDefault(subBuildingInfo);
                        }
                    }
                }
                if (info.HasUndergroundMetroStationTracks(false))
                {
                    List<BuildingInfo.PathInfo> pathList = new List<BuildingInfo.PathInfo>();
                    foreach (var path in info.m_paths)
                        pathList.Add(path.Clone());
                    StationBuildingCustomization.StoredStationPaths.Add(info.name, pathList);

                }
            }
        }

        public static void RestoreBuildingDefault(this BuildingInfo info)
        {
            if (info.m_subBuildings != null)
            {
                for (int i = 0; i < info.m_subBuildings.Count(); i++)
                {
                    var subBuildingInfo = info.m_subBuildings[i].m_buildingInfo;
                    if (subBuildingInfo != null && subBuildingInfo.HasUndergroundMetroStationTracks())
                    {
                        RestoreBuildingDefault(subBuildingInfo);
                    }
                }
            }
            if (info.HasUndergroundMetroStationTracks(false) && StationBuildingCustomization.StoredStationPaths.ContainsKey(info.name))
            {
                List<BuildingInfo.PathInfo> pathList = new List<BuildingInfo.PathInfo>();
                foreach (var path in StationBuildingCustomization.StoredStationPaths[info.name])
                    pathList.Add(path.Clone());
                info.m_paths = pathList.ToArray();
                var chosenPath = info.m_paths.FirstOrDefault(p => p?.m_netInfo != null && p.m_netInfo.IsUndergroundMetroStationTrack());
                Debug.Log("Building " + info.name + " recalled. First path has coordinates " + chosenPath.m_nodes.First().ToString() + " / " + chosenPath.m_nodes.Last().ToString());
            }
        }
    }
}

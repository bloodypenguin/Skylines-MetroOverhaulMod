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
            return info?.m_class != null && info.m_buildingAI is DepotAI && info.m_class.m_service == ItemClass.Service.PublicTransport && info.m_class.m_subService == ItemClass.SubService.PublicTransportMetro;
        }

        public static bool IsAbovegroundMetroStation(this BuildingInfo info)
        {
            return IsMetroDepot(info) && info.m_buildingAI is TransportStationAI && HasAbovegroundMetroStationTracks(info);
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

        public static bool HasUndergroundMetroStationTracks(this BuildingInfo info)
        {
            if (info?.m_paths != null && info.m_paths.Any(p => (p?.m_finalNetInfo != null && p.m_finalNetInfo.IsUndergroundMetroStationTrack()) || (p?.m_netInfo != null && p.m_netInfo.IsUndergroundMetroStationTrack())))
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
                else
                {
                    return info.name + ModTrackNames.ANALOG_PREFIX + TrackVehicleType.Train.ToString();
                }
            }
            else
            {
                return info.name.Substring(0, info.name.IndexOf(ModTrackNames.ANALOG_PREFIX));
            }
        }
        public static void SetTrackVehicleType(this BuildingInfo info, int inx, TrackVehicleType vehicleType)
        {
            var path = info.GetPaths()[inx];
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
                            path.AssignNetInfo(ModTrackNames.TRAIN_STATION_TRACK_GROUND_SMALL, false);
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
            var horizontalDistance = Vector3.Distance(new Vector3(path.m_nodes[0].x, path.m_nodes[0].y, path.m_nodes[0].z),
                                                     new Vector3(path.m_nodes[1].x, path.m_nodes[0].y, path.m_nodes[1].z)); //Math.Pow(Math.Pow(path.m_nodes[0].x - path.m_nodes[1].x, 2) + Math.Pow(path.m_nodes[0].x - path.m_nodes[1].x, 2), 0.5f);

            var verticalDistance = Math.Abs(path.m_nodes[0].y - path.m_nodes[1].y);
            if (path.m_netInfo.m_maxSlope < Math.Abs(verticalDistance / horizontalDistance))
            {
                Debug.Log("Triggered on " + info.name);
                Debug.Log("verticalDistance = " + verticalDistance);
                Debug.Log("horizontalDistance = " + horizontalDistance);
                Debug.Log("actual vs maxSlope = " + (float)Math.Abs(verticalDistance / horizontalDistance) + " vs " + path.m_netInfo.m_maxSlope);
                bool isTargetVehicleTrackType = false;
                float slopeDist = ((float)Math.Abs(verticalDistance / horizontalDistance)) - path.m_netInfo.m_maxSlope;

                var pathList = new List<BuildingInfo.PathInfo>();
                Vector3 targetNode = Vector3.zero;
                Vector3 otherNode = Vector3.zero;
                Vector3 adjTargetNode = Vector3.zero;
                Debug.Log("Finding a mate for inx: " + inx + " called: " + info.m_paths[inx].m_netInfo.name);
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
                            isTargetVehicleTrackType = info.m_paths[i].m_netInfo.IsTrainTrackAIMetro();
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
                                Debug.Log("targetNode/otherNode/adjTargetNode = " + targetNode.ToString() + "/" + otherNode.ToString() + "/" + adjTargetNode.ToString());
                                break;
                            }
                        }
                        //}
                    }
                }

                if (targetNode != Vector3.zero)
                {
                    Debug.Log("In the process of setting targetNode: " + targetNode.ToString() + " to adjTargetNode: " + adjTargetNode);
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
                                isTargetVehicleTrackType = info.m_paths[i].m_netInfo.IsTrainTrackAIMetro();
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
                        Debug.Log("path at inx: " + inx + " has " + findCount + " adjoining path(s) linked at " + targetNode.ToString() + " which was moved to " + adjTargetNode.ToString());
                    }
                }
            }

        }
        public static bool IsTrainStation(this BuildingInfo info)
        {
            return info.m_class.m_subService == ItemClass.SubService.PublicTransportTrain;
        }

        public static bool IsMetroStation(this BuildingInfo info)
        {
            return info.m_class.m_subService == ItemClass.SubService.PublicTransportMetro;
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

            var paths = info.GetPaths(true);
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
            var trackPaths = paths.Where(p => p?.m_netInfo != null && (p.m_netInfo.IsTrainTrackAI() || p.m_netInfo.IsTrainTrackBridgeAI() || p.m_netInfo.IsTrainTrackAIMetro() || p.m_netInfo.IsTrainTrackBridgeAIMetro() || ((p.m_netInfo.IsTrainTrackTunnelAI() || p.m_netInfo.IsTrainTrackTunnelAIMetro()) && (p.m_netInfo.name.Contains("Sunken") || !p.m_netInfo.IsStationTrack()))));
            for (int i = 0; i < paths.Count(); i++)
            {
                if (trackPaths.Contains(paths[i]))
                {
                    if (!inxSoFar.Contains(i))
                    {
                        if (targetPath.m_nodes.Any(n => paths[i].m_nodes.Contains(n)) && targetPath.m_nodes != paths[i].m_nodes)
                        {
                            Debug.Log("At a depth of " + depth + " from inx " + inx + ", we are drilling into " + i + " and we have already explored " + string.Join(",", retval.Select(x => x.ToString()).ToArray()));
                            depth++;
                            retval.AddRange(info.LinkedPaths(i, inxSoFar));
                            depth--;
                        }
                    }
                }
            }
            Debug.Log("At a depth of " + depth + " we have a retval of " + string.Join(",",retval.Select(x => x.ToString()).ToArray()));
            return retval;
        }
    }
}

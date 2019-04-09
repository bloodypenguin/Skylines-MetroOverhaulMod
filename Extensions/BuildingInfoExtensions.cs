using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MetroOverhaul.Extensions {
    public static partial class BuildingInfoExtensions {
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
            return info?.m_paths != null && info.m_paths.Any(p => p?.m_netInfo != null && p.m_netInfo.IsAbovegroundTrainStationTrack());
        }
        public static bool HasAbovegroundMetroStationTracks(this BuildingInfo info)
        {
            return info?.m_paths != null && info.m_paths.Any(p => p?.m_netInfo != null && p.m_netInfo.IsAbovegroundMetroStationTrack());
        }

        public static bool IsUndergroundMetroStation(this BuildingInfo info)
        {
            return IsMetroDepot(info) && info.m_buildingAI is TransportStationAI && HasUndergroundMetroStationTracks(info);
        }
        public static bool HasUndergroundSidePlatformMetroTracks(this BuildingInfo info)
        {
            return info?.m_paths != null && info.m_paths.Any(p => p?.m_netInfo != null && p.m_netInfo.IsUndergroundSidePlatformMetroStationTrack());
        }
        public static bool HasUndergroundIslandPlantformMetroTracks(this BuildingInfo info)
        {
            return info?.m_paths != null && info.m_paths.Any(p => p?.m_netInfo != null && p.m_netInfo.IsUndergroundIslandPlatformStationTrack());
        }
        public static bool HasUndergroundSmallPlantformMetroTracks(this BuildingInfo info)
        {
            return info?.m_paths != null && info.m_paths.Any(p => p?.m_netInfo != null && p.m_netInfo.IsUndergroundSmallStationTrack());
        }
        public static bool HasUndergroundMetroStationTracks(this BuildingInfo info)
        {
            return info?.m_paths != null && info.m_paths.Any(p => p?.m_netInfo != null && p.m_netInfo.IsUndergroundMetroStationTrack());
        }

        public static void SetTrackVehicleType(this BuildingInfo info, int inx, TrackVehicleType vehicleType)
        {
            var path = info.m_paths[inx];
            switch (vehicleType)
            {
                case TrackVehicleType.Train:
                    if (path.m_netInfo.IsAbovegroundMetroStationTrack())
                    {
                        if (path.m_netInfo.IsElevatedSidePlatformMetroStationTrack())
                        {
                            path.AssignNetInfo(ModTrackNames.TRAIN_STATION_TRACK_ELEVA); //somehow expand this for the other types
                        }
                        else if (path.m_netInfo.IsGroundSidePlatformMetroStationTrack())
                        {
                            path.AssignNetInfo(ModTrackNames.TRAIN_STATION_TRACK_GROUND);
                        }
                        else if (path.m_netInfo.IsElevatedIslandPlatformMetroStationTrack())
                        {
                            path.AssignNetInfo(ModTrackNames.TRAIN_STATION_TRACK_ELEVATED_ISLAND);
                        }
                        else if (path.m_netInfo.IsGroundIslandPlatformMetroStationTrack())
                        {
                            path.AssignNetInfo(ModTrackNames.TRAIN_STATION_TRACK_GROUND_ISLAND);
                        }
                        else if (path.m_netInfo.IsElevatedLargeSidePlatformMetroStationTrack())
                        {
                            path.AssignNetInfo(ModTrackNames.TRAIN_STATION_TRACK_ELEVATED_LARGE);
                        }
                        else if (path.m_netInfo.IsGroundLargeSidePlatformMetroStationTrack())
                        {
                            path.AssignNetInfo(ModTrackNames.TRAIN_STATION_TRACK_GROUND_LARGE);
                        }
                        else if (path.m_netInfo.IsElevatedDualIslandPlatformMetroStationTrack())
                        {
                            path.AssignNetInfo(ModTrackNames.TRAIN_STATION_TRACK_ELEVATED_LARGE_DUALISLAND);
                        }
                        else if (path.m_netInfo.IsGroundDualIslandPlatformMetroStationTrack())
                        {
                            path.AssignNetInfo(ModTrackNames.TRAIN_STATION_TRACK_GROUND_LARGE_DUALISLAND);
                        }
                    }
                    else if (path.m_netInfo.IsMetroTrack())
                    {
                        path.AssignNetInfo(ModTrackNames.GetTrackAnalogName(path.m_netInfo.name));
                        FixSlopes(info, inx, vehicleType);
                    }
                    break;
                case TrackVehicleType.Metro:
                    if (path.m_netInfo.IsAbovegroundTrainStationTrack())
                    {
                        if (path.m_netInfo.IsElevatedSidePlatformTrainStationTrack())
                        {
                            path.AssignNetInfo(ModTrackNames.MOM_STATION_TRACK_ELEVATED);
                        }
                        else if (path.m_netInfo.IsGroundSidePlatformTrainStationTrack())
                        {
                            path.AssignNetInfo(ModTrackNames.MOM_STATION_TRACK_GROUND);
                        }
                        else if (path.m_netInfo.IsElevatedIslandPlatformTrainStationTrack())
                        {
                            path.AssignNetInfo(ModTrackNames.MOM_STATION_TRACK_ELEVATED_ISLAND);
                        }
                        else if (path.m_netInfo.IsGroundIslandPlatformTrainStationTrack())
                        {
                            path.AssignNetInfo(ModTrackNames.MOM_STATION_TRACK_GROUND_ISLAND);
                        }
                        else if (path.m_netInfo.IsElevatedLargeSidePlatformTrainStationTrack())
                        {
                            path.AssignNetInfo(ModTrackNames.MOM_STATION_TRACK_ELEVATED_LARGE);
                        }
                        else if (path.m_netInfo.IsGroundLargeSidePlatformTrainStationTrack())
                        {
                            path.AssignNetInfo(ModTrackNames.MOM_STATION_TRACK_GROUND_LARGE);
                        }
                        else if (path.m_netInfo.IsElevatedDualIslandPlatformTrainStationTrack())
                        {
                            path.AssignNetInfo(ModTrackNames.MOM_STATION_TRACK_ELEVATED_LARGE_DUALISLAND);
                        }
                        else if (path.m_netInfo.IsGroundDualIslandPlatformTrainStationTrack())
                        {
                            path.AssignNetInfo(ModTrackNames.MOM_STATION_TRACK_GROUND_LARGE_DUALISLAND);
                        }
                    }
                    else if (path.m_netInfo.IsTrainTrack())
                    {
                        path.AssignNetInfo(ModTrackNames.GetTrackAnalogName(path.m_netInfo.name));
                        FixSlopes(info, inx, vehicleType);
                    }
                    //var nodeArray = new Vector3[2];

                    //for (int i = 0; i < path.m_nodes.Length; i++)
                    //{
                    //    var connectedGroundPaths = info.m_paths.Where(p => p != path && p.m_netInfo.name.StartsWith(ModTrackNames.MOM_TRACK_GROUND) && p.m_nodes.Any(n => n == path.m_nodes[i])).ToList();
                    //    if (connectedGroundPaths != null && connectedGroundPaths.Count() > 0)
                    //    {
                    //        var targetNode = path.m_nodes[i];
                    //        var otherNode = path.m_nodes[(i + 1) % 2];
                    //        var adjTargetNode = (otherNode - targetNode) * slopeDist;


                    //        nodeArray[i] = adjTargetNode;
                    //        nodeArray[(i + 1) % 2] = otherNode;

                    //        for (int j = 0; j < connectedGroundPaths.Count(); j++)
                    //        {
                    //            for (int k = 0; k < connectedGroundPaths[j].m_nodes.Length; k++)
                    //            {
                    //                if (connectedGroundPaths[j].m_nodes[k] == path.m_nodes[i])
                    //                {

                    //                }
                    //            }
                    //        }

                    //        break;
                    //    }
                    //}
                    //path.m_nodes = nodeArray;
                    break;
                case TrackVehicleType.Default:
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
    }
}

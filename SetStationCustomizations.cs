using MetroOverhaul.NEXT.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using MetroOverhaul.Extensions;
using UnityEngine;
using static MetroOverhaul.StationTrackCustomizations;

namespace MetroOverhaul
{
    public static class SetStationCustomizations
    {
        public static float maxHorizontal = 10;
        public static float minHorizontal = -10;
        public static float defHorizontal = 0;
        public static float maxVertical = 10;
        public static float minVertical = -10;
        public static float defVertical = 0;
        public const float MAX_DEPTH = 36;
        public const float MIN_DEPTH = 12;
        public const float DEF_DEPTH = 15;
        public const float MAX_ROTATION = 180;
        public const float MIN_ROTATION = -180;
        public const float DEF_ROTATION = 0;
        public const float MAX_LENGTH = 184;
        public const float MIN_LENGTH = 144;
        public const float DEF_LENGTH = 144;
        public const float MAX_CURVE = 90;
        public const float MIN_CURVE = -90;
        public const float DEF_CURVE = 0;
        public static int m_PremierPath = -1;
        public static float m_PrevAngle = 0;
        private static List<BuildingInfo.PathInfo> m_StationPaths;
        private static Dictionary<BuildingInfo.PathInfo, StationTrackCustomizations> m_PathCustomizationDict = new Dictionary<BuildingInfo.PathInfo, StationTrackCustomizations>();
        public static Dictionary<BuildingInfo, List<BuildingInfo.PathInfo>> StationBuildingCustomizations;
        private static StationTrackCustomizations m_PathCustomization;
        private static Stack<BuildingInfo> m_BuildingStack = new Stack<BuildingInfo>();
        private static Stack<List<BuildingInfo.PathInfo>> m_StationPathStack = new Stack<List<BuildingInfo.PathInfo>>();
        private static BuildingInfo m_Info;
        private static Stack<ParentStationMetaData> m_ParentMetaDataStack = new Stack<ParentStationMetaData>();
        private static ParentStationMetaData m_ParentMetaData;
        private static float StairCoeff { get { return 0.2f; } }// (11f / 64f); } }

        private static List<Vector3> m_connectorNode = null;
        private static bool IsAlterOfType(MetroStationTrackAlterType type)
        {
            return (PathCustomization.AlterType & type) != MetroStationTrackAlterType.None;
        }
        public static void ResetPathCustomizationDict()
        {
            m_PathCustomization = null;
            m_PathCustomizationDict.Clear();
            m_PathCustomizationDict = null;
        }
        public static Dictionary<BuildingInfo.PathInfo, StationTrackCustomizations> PathCustomizationDict {
            get
            {
                if (m_PathCustomizationDict == null)
                {
                    m_PathCustomizationDict = new Dictionary<BuildingInfo.PathInfo, StationTrackCustomizations>();
                }
                return m_PathCustomizationDict;
            }
            private set
            {
                m_PathCustomizationDict = value;
            }
        }
        public static StationTrackCustomizations PathCustomization {
            get
            {
                if (m_PathCustomization == null)
                {
                    m_PathCustomization = new StationTrackCustomizations();
                }
                return m_PathCustomization;
            }
            private set
            {
                m_PathCustomization = value;
            }
        }
        private static Vector3 AdjForOffset(Vector3 node, Vector3 offset, bool subToSuper = true)
        {
            var multiplier = subToSuper ? 1 : -1;
            return new Vector3(node.x + (multiplier * offset.x), node.y, node.z - (multiplier * offset.z));
        }
        private static void FillInPathCustomizations()
        {
            foreach (var path in m_StationPaths)
            {
                if (PathCustomizationDict.ContainsKey(path))
                {
                    PathCustomizationDict[path] = PathCustomization;
                }
                else
                {
                    PathCustomizationDict.Add(path, PathCustomization);
                }
            }
            if (PathCustomizationDict.ContainsKey(new BuildingInfo.PathInfo()))
            {
                PathCustomizationDict[new BuildingInfo.PathInfo()] = PathCustomization;
            }
            else
            {
                PathCustomizationDict.Add(new BuildingInfo.PathInfo(), PathCustomization);
            }
        }
        public static List<BuildingInfo.PathInfo> m_LowestHighPaths = null;
        public static List<BuildingInfo.PathInfo> LowestHighPaths {
            get
            {
                if (m_LowestHighPaths == null)
                    m_LowestHighPaths = m_Info.LowestHighPaths();
                return m_LowestHighPaths;
            }
        }
        private static double m_AngleDelta = 0;

        public static void ModifyStation(BuildingInfo info, StationTrackCustomizations pathCustomization, ParentStationMetaData parentMetaData = null)
        {

            Debug.Log("XXXXX STARTING " + info.name + " XXXXXXX");
            m_BuildingStack.Push(info);
            m_Info = info;
            m_ParentMetaDataStack.Push(parentMetaData);
            m_ParentMetaData = parentMetaData;
            m_StationPaths = info.UndergroundStationPaths(false);
            var hasPaths = m_StationPaths.Count() > 0;
            PathCustomization = pathCustomization;

            if (PathCustomization.AlterType == MetroStationTrackAlterType.None)
                PathCustomization.AlterType = MetroStationTrackAlterType.All;
            //if (PathCustomization.Path?.m_nodes == null)
            //{
            //    FillInPathCustomizations();
            //}
            //else if (PathCustomizationDict.ContainsKey(PathCustomization.Path))
            //{
            //    PathCustomizationDict[PathCustomization.Path] = PathCustomization;
            //}
            //else
            //{
            //    PathCustomizationDict.Add(PathCustomization.Path, PathCustomization);
            //}
            m_ParentMetaData = parentMetaData;

            if (PathCustomization.Depth <= 0 || PathCustomization.Length <= 0 || (!info.HasUndergroundMetroStationTracks() && (info?.m_subBuildings == null || info?.m_subBuildings.Count() == 0)))
            {
                return;
            }
            Debug.Log("PathIndex = " + m_StationPaths.IndexOf(PathCustomization.Path));
            Debug.Log("PathCount = " + m_StationPaths.Count());
            Debug.Log("Has a path = " + (PathCustomization.Path != null));
            Debug.Log("Path has nodes = " + (PathCustomization.Path?.m_nodes != null));
            if (PathCustomization.Path?.m_nodes != null)
                Debug.Log("Path has positive nodes = " + PathCustomization.Path?.m_nodes.Count());
            else
                Debug.Log("Path has no positive nodes");

            Debug.Log("Horizontal = " + PathCustomization.Horizontal);
            Debug.Log("Vertical = " + PathCustomization.Vertical);
            Debug.Log("Length = " + PathCustomization.Length);
            Debug.Log("Depth = " + PathCustomization.Depth);
            Debug.Log("Rotation = " + PathCustomization.Rotation);
            Debug.Log("Curve = " + PathCustomization.Curve);
            Debug.Log("AlterType " + (int)PathCustomization.AlterType);
            CleanUpPaths();
            HandleSubBuildings();
            if (hasPaths)
            {
                Debug.Log("info " + m_Info.name + " has " + m_StationPaths.Count() + " station paths");
                if (m_Info.HasUndergroundMetroStationTracks(false))
                    RemovePedPaths();
                if (IsAlterOfType(MetroStationTrackAlterType.Length))
                    ResizeUndergroundStationTracks();
                ChangeStationDepthAndRotation();
                if (IsAlterOfType(MetroStationTrackAlterType.Vertical | MetroStationTrackAlterType.Horizontal) && !IsAlterOfType(MetroStationTrackAlterType.Rotation))
                    ChangeStationPlanarPosition();
                ReconfigureStationAccess();
                ConnectStations();
                CurveStationTrack();
                RecalculateSpawnPoints();
                info.StoreBuildingDefault();
            }

            var originalInfo = m_BuildingStack.Pop();
            if (originalInfo != null)
            {
                m_Info = originalInfo;
                m_StationPaths = originalInfo.UndergroundStationPaths(false);
            }
            m_ParentMetaData = m_ParentMetaDataStack.Pop();
            m_LowestHighPaths = null;
        }

        private static NetInfo m_SpecialNetInfo = null;
        private static NetInfo SpecialNetInfo {
            get
            {
                if (m_SpecialNetInfo == null)
                {
                    m_SpecialNetInfo = FindNetInfo("Pedestrian Connection Inside").ShallowClone();
                    m_SpecialNetInfo.m_maxSlope = 100;
                    m_SpecialNetInfo.m_maxTurnAngle = 180;
                    m_SpecialNetInfo.m_maxTurnAngleCos = -1;
                    var lanes = m_SpecialNetInfo.m_lanes.ToList();
                    lanes.First().m_speedLimit = 10;
                    m_SpecialNetInfo.m_lanes = lanes.ToArray();
                }
                return m_SpecialNetInfo;
            }
        }
        private static BuildingInfo.PathInfo TrackPath { get; set; }
        private static BuildingInfo.PathInfo NewPath { get; set; }
        private static Vector3 CurveVector { get; set; }
        private static Vector3 Crossing { get; set; }
        private static float xCoeff { get; set; }
        private static float zCoeff { get; set; }
        private static float StairsLengthX { get; set; }
        private static float StairsLengthZ { get; set; }
        private static List<BuildingInfo.PathInfo> SetStationPaths(params float[] connectorHalfWidths)
        {
            var retval = new List<BuildingInfo.PathInfo>();
            if (connectorHalfWidths != null)
            {
                var newNodes = new List<Vector3>();
                var connectorHalfWidthList = new List<float>();
                connectorHalfWidthList.AddRange(connectorHalfWidths);
                connectorHalfWidthList.Sort();
                var offset = connectorHalfWidthList[0];
                var forbiddenList = new List<bool>();

                for (int i = 0; i < connectorHalfWidthList.Count(); i++)
                {
                    if (i == 0)
                    {
                        newNodes.Add(new Vector3()
                        {
                            x = Crossing.x + (connectorHalfWidthList[0] * zCoeff) - (3 * xCoeff),
                            y = TrackPath.m_nodes.Last().y + 8,
                            z = Crossing.z + (connectorHalfWidthList[0] * xCoeff) + (3 * zCoeff)
                        });
                        newNodes[0] = new Vector3(newNodes[0].x - CurveVector.x, newNodes[0].y, newNodes[0].z + CurveVector.z);
                    }
                    else
                    {
                        offset -= connectorHalfWidthList[i];
                        newNodes.Add(new Vector3()
                        {
                            x = newNodes[i - 1].x + (Math.Abs(offset) * zCoeff),
                            y = TrackPath.m_nodes.Last().y + 8,
                            z = newNodes[i - 1].z + (Math.Abs(offset) * xCoeff)
                        });
                        offset = connectorHalfWidthList[i];
                    }
                    forbiddenList.Add(true);

                }

                if (!ConnectDict.ContainsKey(TrackPath))
                {
                    var connectBounds = new Vector3[2];
                    connectBounds[0] = newNodes.First();
                    connectBounds[1] = newNodes.Last();
                    ConnectDict.Add(TrackPath, connectBounds);
                }

                var stairNodes = new List<Vector3>();

                for (int i = 0; i < newNodes.Count(); i++)
                {
                    stairNodes.Add(new Vector3()
                    {
                        x = newNodes[i].x + StairsLengthX,
                        y = TrackPath.m_nodes.Last().y,
                        z = newNodes[i].z + StairsLengthZ,
                    });
                }

                NewPath.m_nodes = newNodes.ToArray();
                NewPath.m_forbidLaneConnection = forbiddenList.ToArray();

                retval.Add(NewPath);

                if (!StairDict.ContainsKey(TrackPath))
                    StairDict.Add(TrackPath, new List<BuildingInfo.PathInfo>());

                for (var i = 0; i < stairNodes.Count(); i++)
                {
                    var branchPathStair = CreatePath(NewPath, newNodes[i], stairNodes[i]);
                    branchPathStair.m_forbidLaneConnection = new[] { true, false };
                    branchPathStair.AssignNetInfo(m_SpecialNetInfo);
                    StairDict[TrackPath].Add(branchPathStair);
                    retval.Add(branchPathStair);
                }
            }
            return retval;
        }
        private static Dictionary<BuildingInfo.PathInfo, List<BuildingInfo.PathInfo>> m_StairDict = null;
        private static Dictionary<BuildingInfo.PathInfo, List<BuildingInfo.PathInfo>> StairDict {
            get
            {
                if (m_StairDict == null)
                    m_StairDict = new Dictionary<BuildingInfo.PathInfo, List<BuildingInfo.PathInfo>>();
                return m_StairDict;
            }
            set
            {
                m_StairDict = value;
            }
        }
        private static Dictionary<BuildingInfo.PathInfo, Vector3[]> m_ConnectDict = null;
        private static Dictionary<BuildingInfo.PathInfo, Vector3[]> ConnectDict {
            get
            {
                if (m_ConnectDict == null)
                    m_ConnectDict = new Dictionary<BuildingInfo.PathInfo, Vector3[]>();
                return m_ConnectDict;
            }
            set
            {
                m_ConnectDict = value;
            }
        }
        private static bool IsATargetPath(BuildingInfo.PathInfo path)
        {
            return (m_PathCustomization.Path?.m_nodes == null || m_PathCustomization.Path == path);
        }
        private static void ReconfigureStationAccess()
        {
            var pathList = m_Info.m_paths.ToList();
            var directionList = new List<string>();
            var aPath = pathList.FirstOrDefault(p => p.IsPedestrianPath());
            if (aPath == null)
            {
                aPath = new BuildingInfo.PathInfo();
            }
            Debug.Log(m_Info.name + " has " + pathList.Count() + " paths with " + pathList.Where(p => p?.m_netInfo != null && p.m_netInfo.IsUndergroundDualIslandPlatformMetroStationTrack()));
            if (m_ConnectDict != null)
            {
                m_ConnectDict.Clear();
                m_ConnectDict = null;
            }
            if (m_StairDict != null)
            {
                m_StairDict.Clear();
                m_StairDict = null;
            }

            for (int i = 0; i < pathList.Count; i++)
            {
                if (pathList[i]?.m_netInfo != null && pathList[i].m_netInfo.IsUndergroundMetroStationTrack())
                {
                    Debug.Log("On " + m_Info.name + " Track on index" + i);
                    TrackPath = pathList[i];
                    if (PathCustomizationDict.ContainsKey(TrackPath))
                    {
                        var curve = PathCustomizationDict[TrackPath].Curve;
                        var middleCurve = TrackPath.GetMiddle(curve);
                        var middle = TrackPath.GetMiddle();
                        CurveVector = middleCurve - middle;

                        NewPath = aPath.ShallowClone();
                        NewPath.AssignNetInfo(SpecialNetInfo);
                        NewPath.MarkPathGenerated();

                        xCoeff = -(TrackPath.m_nodes[0].x - TrackPath.m_nodes.Last().x) / Vector3.Distance(TrackPath.m_nodes[0], TrackPath.m_nodes.Last());
                        zCoeff = (TrackPath.m_nodes[0].z - TrackPath.m_nodes.Last().z) / Vector3.Distance(TrackPath.m_nodes[0], TrackPath.m_nodes.Last());
                        var stationLength = Vector3.Distance(TrackPath.m_nodes[0], TrackPath.m_nodes.Last());
                        StairsLengthX = ((0.12f * curve) + 1) * (stationLength * StairCoeff) * -xCoeff;
                        StairsLengthZ = ((0.12f * curve) + 1) * (stationLength * StairCoeff) * zCoeff;
                        var interpolant = 0.6f;
                        Crossing = Vector3.Lerp(TrackPath.m_nodes.First(), TrackPath.m_nodes.Last(), interpolant);
                        if (TrackPath.m_netInfo.IsUndergroundIslandPlatformStationTrack())
                        {
                            pathList.AddRange(SetStationPaths(0));
                        }
                        //else if (TrackPath.m_netInfo.IsUndergroundSideIslandPlatformMetroStationTrack())
                        //{
                        //    pathList.AddRange(SetStationPaths(-14, 0, 14));
                        //}
                        else if (TrackPath.m_netInfo.IsUndergroundSmallStationTrack())
                        {
                            pathList.AddRange(SetStationPaths(5));
                        }
                        else if (TrackPath.m_netInfo.IsUndergroundPlatformLargeMetroStationTrack())
                        {
                            pathList.AddRange(SetStationPaths(-11, 11));
                        }
                        else if (TrackPath.m_netInfo.IsUndergroundDualIslandPlatformMetroStationTrack())
                        {
                            pathList.AddRange(SetStationPaths(-8.8f, -5.5f, 5.5f, 8.8f));
                        }
                        else if (TrackPath.m_netInfo.IsUndergroundSidePlatformMetroStationTrack())
                        {
                            pathList.AddRange(SetStationPaths(-7, 7));
                        }
                    }
                    else
                    {
                        Debug.Log("PathCustomizationDict does not contain this trackpath...");
                    }
                }
            }
            if (/*m_SuperInfo == null &&*/ m_Info.m_paths.Count() >= 2)
            {
                CheckPedestrianConnections();
            }
            m_Info.m_paths = CleanPaths(pathList).ToArray();

        }
        private static List<BuildingInfo.PathInfo> TrackPool;
        private static void ConnectStations()
        {
            if (m_StationPaths.Count > 0)
            {
                Debug.Log("info is null for some reason " + (m_Info?.m_paths == null));
                Debug.Log("Connecting up " + m_Info.name);
                var pathList = m_Info.m_paths.ToList();
                var connectPoints = new List<Vector3>();
                //if (m_ParentMetaData?.LowestHighestPaths != null && m_ParentMetaData?.LowestHighestPaths.Count() > 0)
                //{
                //    Debug.Log("Building " + m_Info.name + " has a parent");
                //    Vector3 subVector = Vector3.zero;
                //    Vector3 parentVector = Vector3.zero;
                //    float distance = 0;
                //    for (int i = 0; i < LowestHighPaths.Count(); i++)
                //    {
                //        var subLowestHighPath = LowestHighPaths[i];
                //        if (subLowestHighPath?.m_nodes != null)
                //        {
                //            var subMidpoint = Vector3.Lerp(subLowestHighPath.m_nodes.First(), subLowestHighPath.m_nodes.Last(), 0.5f);
                //            for (int j = 0; j < m_ParentMetaData.LowestHighestPaths.Count(); j++)
                //            {
                //                var subLowestHighestPath = m_ParentMetaData?.LowestHighestPaths[j];
                //                if (subLowestHighestPath?.m_nodes != null)
                //                {
                //                    var adjustedNodeList = new List<Vector3>();
                //                    foreach (var subNode in subLowestHighestPath.m_nodes)
                //                    {
                //                        adjustedNodeList.Add(AdjustNodeForParent(subNode));
                //                    }
                //                    var parentMidpoint = Vector3.Lerp(adjustedNodeList.First(), adjustedNodeList.Last(), 0.5f);
                //                    var newDistance = Vector3.Distance(parentMidpoint, subMidpoint);
                //                    if (distance == 0 || newDistance < distance)
                //                    {
                //                        distance = newDistance;
                //                        subVector = subLowestHighPath.m_nodes.OrderBy(n => Vector3.Distance(n, parentMidpoint)).FirstOrDefault();
                //                        Debug.Log("subVector is " + subVector.ToString());
                //                        var planarSubVector = new Vector2(subVector.x, subVector.z);
                //                        parentVector = adjustedNodeList.Where(n => Vector2.Distance(new Vector2(n.x, n.z), planarSubVector) >= 4).OrderBy(n => Vector3.Distance(n, subMidpoint)).FirstOrDefault();
                //                        Debug.Log("parentVector is " + parentVector.ToString());
                //                    }
                //                }
                //            }
                //        }
                //    }
                //    Debug.Log("final subVector is " + subVector.ToString());
                //    Debug.Log("final parentVector is " + parentVector.ToString());
                //    if (distance > 0)
                //    {
                //        Debug.Log("pathlist has " + pathList.Count());
                //        pathList.Add(CreatePath(NewPath, subVector, parentVector));
                //        Debug.Log("pathlist has " + pathList.Count());
                //    }

                //}
                //else
                //{
                if (LowestHighPaths != null && LowestHighPaths.Count > 0)
                {
                    TrackPool = new List<BuildingInfo.PathInfo>();
                    TrackPool.AddRange(m_StationPaths);
                    var originVectors = new List<Vector3>();
                    Vector3 nearestReasonableNode;
                    if (m_ParentMetaData?.LowestHighestPaths != null)
                    {
                        var reasonableNodes = m_ParentMetaData.Info.m_paths.Where(p => p.IsPedestrianPath() && !p.IsPathGenerated()).SelectMany(p => p.m_nodes).Where(n => n.y <= 0).ToList();
                        var nearestTrackToCenter = TrackPool.OrderBy(p => Vector3.Magnitude(Vector3.Lerp(AdjustNodeForParent(p.m_nodes.First()), AdjustNodeForParent(p.m_nodes.Last()), 0.5f))).FirstOrDefault();
                        var nearestConnectNodes = ConnectDict[nearestTrackToCenter];
                        var nearestConnectNodesMidpoint = Vector3.Lerp(nearestConnectNodes.First(), nearestConnectNodes.Last(), 0.5f);
                        nearestReasonableNode = AdjustNodeForParent(reasonableNodes.Where(n => Vector3.Distance(AdjustNodeForParent(n), nearestConnectNodesMidpoint) >= 4).OrderBy(n => Vector3.Distance(AdjustNodeForParent(n), nearestConnectNodesMidpoint)).FirstOrDefault());
                        var nearestConnectNodeOrder = nearestConnectNodes.OrderBy(n => Vector3.Distance(nearestReasonableNode, n));
                        pathList.Add(CreatePath(StairDict[nearestTrackToCenter].First(), nearestReasonableNode, nearestConnectNodeOrder.First()));
                        TrackPool.Remove(nearestTrackToCenter);
                        nearestReasonableNode = nearestConnectNodeOrder.Last();
                    }
                    else
                    {
                        var reasonableNodes = m_Info.m_paths.Where(p => p.IsPedestrianPath() && !p.IsPathGenerated()).SelectMany(p => p.m_nodes).Where(n => n.y <= 0).ToList();
                        var nearestTrackToCenter = TrackPool.OrderBy(p => Vector3.Magnitude(Vector3.Lerp(p.m_nodes.First(), p.m_nodes.Last(), 0.5f))).FirstOrDefault();
                        var nearestConnectNodes = ConnectDict[nearestTrackToCenter];
                        var nearestConnectNodesMidpoint = Vector3.Lerp(nearestConnectNodes.First(), nearestConnectNodes.Last(), 0.5f);
                        nearestReasonableNode = reasonableNodes.Where(n => Vector3.Distance(n, nearestConnectNodesMidpoint) >= 4).OrderBy(n => Vector3.Distance(n, nearestConnectNodesMidpoint)).FirstOrDefault();
                    }
                    var trackPathCount = TrackPool.Count();
                    if (trackPathCount > 0)
                    {
                        for (int i = 0; i < trackPathCount; i++)
                        {
                            var closestTrack = TrackPool.OrderBy(p => Vector3.Distance(nearestReasonableNode, Vector3.Lerp(p.m_nodes.First(), p.m_nodes.Last(), 0.5f))).FirstOrDefault();
                            if (closestTrack != null && ConnectDict.ContainsKey(closestTrack))
                            {
                                Debug.Log("Closest track exists and connectdict contains closest track");
                                var closestConnectVectors = ConnectDict[closestTrack].OrderBy(n => Vector3.Distance(nearestReasonableNode, n));
                                if (closestConnectVectors != null)
                                {
                                    pathList.Add(CreatePath(StairDict[closestTrack].First(), nearestReasonableNode, closestConnectVectors.First()));
                                    nearestReasonableNode = closestConnectVectors.Last();
                                }
                                TrackPool.Remove(closestTrack);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    //for(var i = 0; i < TrackPath.cou)
                    //while (TrackPool.Count > 0) //Do away with this!!!!
                    //{
                    //    var newOriginVectors = new List<Vector3>();
                    //var aLowNode = originVectors[0];
                    //    //foreach (var lowNode in originVectors)
                    //    //{
                    //        Debug.Log("lownode is " + aLowNode.ToString());
                    //        var closestTrack = TrackPool.OrderBy(p => Vector3.Distance(aLowNode, Vector3.Lerp(p.m_nodes.First(), p.m_nodes.Last(), 0.5f))).FirstOrDefault();
                    //        if (closestTrack != null && ConnectDict.ContainsKey(closestTrack))
                    //        {
                    //            Debug.Log("Closest track exists and connectdict contains closest track");
                    //            var closestConnectVectors = ConnectDict[closestTrack].OrderBy(n => Vector3.Distance(aLowNode, n));
                    //            if (closestConnectVectors != null)
                    //            {
                    //                Debug.Log("Path created between " + aLowNode.ToString() + " and " + closestConnectVectors.First().ToString());
                    //                pathList.Add(CreatePath(StairDict[closestTrack].First(), aLowNode, closestConnectVectors.First()));
                    //                TrackPool.Remove(closestTrack);
                    //                newOriginVectors.Add(closestConnectVectors.Last());
                    //                //if (TrackPool.Count == 0)
                    //                    //break;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            if (closestTrack == null)
                    //            {
                    //                Debug.Log("There is no closest track...huh?");
                    //            }
                    //            else
                    //            {
                    //                Debug.Log("ConnectDict does not contain the specified closest track. There are " + ConnectDict.Count() + " tracks in ConnectDict.");
                    //            }
                    //        }
                    //    //}
                    //    originVectors = newOriginVectors;
                    //}
                }
                //if (m_ParentMetaData?.LowestHighestPaths != null && m_ParentMetaData?.LowestHighestPaths.Count() > 0)
                //{
                //    var parentLowestHighNodes = m_ParentMetaData.LowestHighestPaths.SelectMany(p => AdjustNodeForParent(p.m_nodes));
                //    var averageNode = Vector3.zero;
                //    foreach (var node in parentLowestHighNodes)
                //    {
                //        averageNode += node;
                //    }
                //    averageNode /= parentLowestHighNodes.Count();
                //    var closestTrack = ConnectDict.Keys.OrderBy(p => Vector3.Distance(Vector3.Lerp(p.m_nodes.First(), p.m_nodes.Last(), 0.5f), averageNode)).FirstOrDefault();
                //    var closestNode = parentLowestHighNodes.OrderBy(n => Vector3.Distance(n, Vector3.Lerp(closestTrack.m_nodes.First(), closestTrack.m_nodes.Last(), 0.5f))).FirstOrDefault();
                //    var closestConnectVector = ConnectDict.Values.SelectMany(n => n).OrderBy(n2 => Vector3.Distance(n2, closestNode)).FirstOrDefault();
                //    pathList.Add(CreatePath(StairDict[closestTrack].First(), closestNode, closestConnectVector));

                //}
                m_Info.m_paths = CleanPaths(pathList).ToArray();
                //if (LowestHighPaths != null && LowestHighPaths.Count > 0)
                //{
                //    foreach (BuildingInfo.PathInfo p in LowestHighPaths)
                //    {
                //        if (p?.m_nodes != null && p.m_nodes.Count() > 0)
                //        {
                //            connectPoints.Add(p.m_nodes.OrderByDescending(n => n.y).ThenBy(n => n.z).LastOrDefault());
                //        }

                //    }
                //}
                //var currentVector = connectPoints.FirstOrDefault();
                //var stationPath = m_Info.m_paths.FirstOrDefault(p => p.m_netInfo.IsUndergroundMetroStationTrack());
                //if (m_Info.m_paths.Any(p => p.m_netInfo != null && p.m_netInfo.IsUndergroundMetroStationTrack()))
                //{
                //    if (connectPoints != null && connectPoints.Count > 0)
                //    {
                //        var pool = new List<Vector3[]>();
                //        var pivotPath = pathList.FirstOrDefault(p => p.m_nodes.Any(n => n == connectPoints.FirstOrDefault()));
                //        pool.AddRange(connectList);
                //        for (var i = 0; i < connectList.Count; i++)
                //        {
                //            var closestVector = pool.SelectMany(n => n).OrderBy(n => (currentVector.x - n.x) + (100 * (connectPoints.FirstOrDefault().y - n.y)) + (currentVector.z - n.z)).LastOrDefault();
                //            var closestPath = pathList.FirstOrDefault(p => p.m_nodes.Any(n => n == closestVector));
                //            BuildingInfo.PathInfo branch = null;
                //            if (currentVector == connectPoints.FirstOrDefault())
                //            {
                //                branch = ChainPath(pivotPath, closestVector, Array.IndexOf(pivotPath.m_nodes, connectPoints.FirstOrDefault()));
                //            }
                //            else
                //            {
                //                branch = ChainPath(closestPath, currentVector, Array.IndexOf(closestPath.m_nodes, closestVector));
                //            }
                //            branch.AssignNetInfo(SpecialNetInfo);
                //            branch.m_forbidLaneConnection = new[] { true, true };
                //            pathList.Add(branch);
                //            var nodeArrayToLose = pool.FirstOrDefault(na => na.Any(n => n == closestVector));
                //            if (nodeArrayToLose != null)
                //            {
                //                currentVector = nodeArrayToLose.OrderBy(n => Vector3.Distance(closestVector, n)).LastOrDefault();
                //                pool.Remove(nodeArrayToLose);
                //            }
                //        }
                //        if (connectPoints.Count > 1)
                //        {
                //            for (var i = 1; i < connectPoints.Count(); i++)
                //            {
                //                Vector3 node = connectPoints[i];
                //                Vector3 closestVector = connectList.SelectMany(n => n).OrderBy(n => (node.x - n.x) + (100 * (node.y - n.y)) + (node.z - n.z)).FirstOrDefault();
                //                var closestPath = pathList.FirstOrDefault(p => p != null && p.m_nodes.Any(n => n == closestVector));
                //                var branch = ChainPath(closestPath, node, Array.IndexOf(closestPath.m_nodes, closestVector));
                //                branch.AssignNetInfo(SpecialNetInfo);
                //                branch.m_forbidLaneConnection = new[] { true, true };
                //                pathList.Add(branch);
                //            }
                //        }
                //    }
                //}
                //m_Info.m_paths = CleanPaths(pathList).ToArray();
            }
        }
        public static List<Vector3> AdjustNodeForParent(Vector3[] nodes, ParentStationMetaData parentmetadata = null)
        {
            List<Vector3> retList = new List<Vector3>();
            foreach (var node in nodes)
            {
                retList.Add(AdjustNodeForParent(node, parentmetadata));
            }
            return retList;
        }
        public static Vector3 AdjustNodeForParent(Vector3 node, ParentStationMetaData parentMetaData = null)
        {
            if (parentMetaData == null)
                parentMetaData = m_ParentMetaData;
            var retNode = node;
            if (parentMetaData != null)
            {
                var angle = (parentMetaData.Angle * Math.PI) / 180;
                retNode = new Vector3
                {
                    x = (float)Math.Round(((node.x - parentMetaData.Position.x) * Math.Cos(angle)) - ((node.z + parentMetaData.Position.z) * Math.Sin(angle))),
                    y = node.y,
                    z = (float)Math.Round(((node.x - parentMetaData.Position.x) * Math.Sin(angle)) + ((node.z + parentMetaData.Position.z) * Math.Cos(angle)))
                };
            }
            return retNode;
        }
        private static List<BuildingInfo.PathInfo> CleanPaths(List<BuildingInfo.PathInfo> pathList)
        {
            var tinyPaths = pathList.Where(p => Vector3.Distance(p.m_nodes.First(), p.m_nodes.Last()) <= 4).ToList();
            var mergePathDict = new Dictionary<Vector3, Vector3>();
            if (tinyPaths.Count > 0)
            {
                foreach (var path in tinyPaths)
                {
                    var average = (path.m_nodes.First() + path.m_nodes.Last()) / 2;

                    if (!(mergePathDict.ContainsKey(path.m_nodes.First())))
                    {
                        mergePathDict.Add(path.m_nodes.First(), average);
                    }
                    else if (!mergePathDict.ContainsKey(path.m_nodes.Last()))
                    {
                        mergePathDict.Add(path.m_nodes.Last(), mergePathDict[path.m_nodes.First()]);
                    }
                    if (!(mergePathDict.ContainsKey(path.m_nodes.Last())))
                    {
                        mergePathDict.Add(path.m_nodes.Last(), average);
                    }
                    else if (!mergePathDict.ContainsKey(path.m_nodes.First()))
                    {
                        mergePathDict.Add(path.m_nodes.First(), mergePathDict[path.m_nodes.Last()]);
                    }
                }
            }

            var finalPathList = new List<BuildingInfo.PathInfo>();
            for (var i = 0; i < pathList.Count(); i++)
            {
                var path = pathList[i];
                if (tinyPaths.Contains(path))
                {
                    continue;
                }
                var nodeList = new List<Vector3>();
                for (var j = 0; j < path.m_nodes.Count(); j++)
                {
                    var newNode = path.m_nodes[j];
                    if (mergePathDict.ContainsKey(path.m_nodes[j]))
                    {
                        newNode = mergePathDict[path.m_nodes[j]];
                    }
                    nodeList.Add(newNode);
                }
                path.m_nodes = nodeList.ToArray();
                path.SetCurveTargets();
                finalPathList.Add(path);
            }
            return finalPathList;
        }
        private static NetInfo FindNetInfo(string prefabName)
        {
            return PrefabCollection<NetInfo>.FindLoaded(prefabName);
        }
        private static BuildingInfo.PathInfo ChainPath(BuildingInfo.PathInfo startPath, Vector3 endNode, int startNodeIndex = -1, NetInfo info = null)
        {

            var newNodes = new List<Vector3>();
            var newPath = startPath.ShallowClone();
            if (startNodeIndex == -1 || startNodeIndex >= startPath.m_nodes.Count())
            {
                startNodeIndex = startPath.m_nodes.Count() - 1;
            }
            if (info != null)
            {
                newPath.AssignNetInfo(info);
            }
            var startNode = startPath.m_nodes[startNodeIndex];
            newNodes.Add(startNode);
            newNodes.Add(endNode);

            newPath.m_nodes = newNodes.ToArray();
            newPath.MarkPathGenerated();

            return newPath;
        }
        private static BuildingInfo.PathInfo CreatePath(BuildingInfo.PathInfo templatePath, Vector3 vectorA, Vector3 vectorB)
        {
            var newPath = templatePath.ShallowClone();
            var nodeList = new List<Vector3>();
            nodeList.Add(vectorA);
            nodeList.Add(vectorB);
            newPath.m_nodes = nodeList.ToArray();
            newPath.MarkPathGenerated();
            newPath.AssignNetInfo(SpecialNetInfo);
            NewPath.SetCurveTargets();
            return newPath;
        }

        private static void CurveStationTrack()
        {
            for (var i = 0; i < m_StationPaths.Count(); i++)
            {
                var stationPath = m_StationPaths[i];
                if (PathCustomizationDict.ContainsKey(stationPath))
                {
                    var curve = PathCustomizationDict[stationPath].Curve;
                    CurveStationTrack(stationPath, curve);
                    if (StairDict.ContainsKey(stationPath))
                    {
                        var stairPaths = m_Info.m_paths.Where(p => StairDict[stationPath].Contains(p)).ToList();
                        for (var j = 0; j < stairPaths.Count(); j++)
                        {
                            CurveStationTrack(stairPaths[j], curve, -StairCoeff);
                        }
                    }
                }
            }
        }

        private static void CurveStationTrack(BuildingInfo.PathInfo targetPath, float curve, float curveCoefficient = 1)
        {
            var aCurveStrength = curve * curveCoefficient;
            var middle = targetPath.GetMiddle();
            var newX = (targetPath.m_nodes.First().z - targetPath.m_nodes.Last().z) / 2;
            var newY = (targetPath.m_nodes.First().y + targetPath.m_nodes.Last().y) / 2;
            var newZ = -(targetPath.m_nodes.First().x - targetPath.m_nodes.Last().x) / 2;
            var newCurve = middle + (aCurveStrength * new Vector3(newX, newY, newZ));
            var curveTargetsList = new List<Vector3>();
            if (targetPath.m_curveTargets != null)
            {
                curveTargetsList.AddRange(targetPath.m_curveTargets);
            }

            if (curveTargetsList.Count() > 0)
            {
                curveTargetsList[0] = newCurve;
            }
            else
            {
                curveTargetsList.Add(newCurve);
            }
            targetPath.m_curveTargets = curveTargetsList.ToArray();
        }
        private static void CleanUpPaths(BuildingInfo info)
        {
            info.m_paths = info.m_paths.Where(p => !p.IsPathGenerated()).ToArray();
        }

        private static void CleanUpPaths()
        {
            m_Info.m_paths = m_Info.m_paths.Where(p => !p.IsPathGenerated()).ToArray();
        }

        private static void RecalculateSpawnPoints()
        {
            var buildingAI = m_Info?.GetComponent<DepotAI>();
            var paths = m_Info?.m_paths;
            if (buildingAI == null || paths == null)
            {
                return;
            }
            var spawnPoints = (from path in paths
                               where IsVehicleStop(path) && PathCustomizationDict.ContainsKey(path)
                               select path.GetMiddle(PathCustomizationDict[path].Curve)).Distinct().ToArray();

            switch (spawnPoints.Length)
            {
                case 0:
                    buildingAI.m_spawnPosition = Vector3.zero;
                    buildingAI.m_spawnTarget = Vector3.zero;
                    buildingAI.m_spawnPoints = new DepotAI.SpawnPoint[] { };
                    break;
                case 1:
                    buildingAI.m_spawnPosition = spawnPoints[0];
                    buildingAI.m_spawnTarget = spawnPoints[0];
                    buildingAI.m_spawnPoints = new[]
                    {
                        new DepotAI.SpawnPoint
                        {
                            m_position =  spawnPoints[0],
                            m_target =  spawnPoints[0]
                        }
                    };
                    break;
                default:
                    buildingAI.m_spawnPosition = Vector3.zero;
                    buildingAI.m_spawnTarget = Vector3.zero;
                    var spawnPointList = new List<DepotAI.SpawnPoint>();
                    foreach (var msp in buildingAI.m_spawnPoints)
                    {
                        spawnPointList.Add(msp);
                    }
                    foreach (var sp in spawnPoints)
                    {
                        spawnPointList.Add(new DepotAI.SpawnPoint() { m_position = sp, m_target = sp });
                    }
                    buildingAI.m_spawnPoints = spawnPointList.ToArray();
                    break;
            }
        }

        public static void HandleSubBuildings()
        {
            if (m_Info?.m_subBuildings != null && m_Info.m_subBuildings.Count() > 0)
            {
                for (int i = 0; i < m_Info.m_subBuildings.Count(); i++)
                {
                    var subBuilding = m_Info.m_subBuildings[i];
                    if (subBuilding?.m_buildingInfo?.m_paths != null && subBuilding.m_buildingInfo.HasUndergroundMetroStationTracks())
                    {
                        CleanUpPaths(subBuilding.m_buildingInfo);
                        var metaData = new ParentStationMetaData()
                        {
                            Info = m_Info,
                            LowestHighestPaths = LowestHighPaths,
                            Angle = subBuilding.m_angle,
                            Position = subBuilding.m_position
                        };
                        var modifiedPathCustomization = PathCustomization;
                        modifiedPathCustomization.Curve *= 90;
                        ModifyStation(subBuilding.m_buildingInfo, modifiedPathCustomization, metaData);
                    }
                }
            }
        }
        private static bool IsVehicleStop(BuildingInfo.PathInfo path)
        {
            return (path?.m_nodes?.Length ?? 0) > 1 && (path?.m_netInfo?.IsUndergroundMetroStationTrack() ?? false);
        }

        private static void RemovePedPaths()
        {
            var pathList = new List<BuildingInfo.PathInfo>();
            foreach (var path in m_Info.m_paths)
            {
                if (path.m_netInfo.IsPedestrianNetwork())
                {
                    if (path.m_nodes.All(n => n.y <= -8) && !LowestHighPaths.Contains(path))
                    {
                        continue;
                    }
                }
                pathList.Add(path);
            }
            m_Info.m_paths = pathList.ToArray();
        }

        private static void ChangeStationPlanarPosition()
        {
            var center = Vector3.zero;
            if (PathCustomization.Path?.m_nodes == null)
            {
                center = m_Info.FindAverageNode(true);
            }
            else
            {
                center = Vector3.Lerp(PathCustomization.Path.m_nodes.First(), PathCustomization.Path.m_nodes.Last(), 0.5f);
            }

            var diff = new Vector3(PathCustomization.Horizontal, center.y, PathCustomization.Vertical) - center;
            if (PathCustomization.Path?.m_nodes == null)
            {
                foreach (var path in m_StationPaths)
                {
                    var nodeList = new List<Vector3>();
                    for (int i = 0; i < path.m_nodes.Count(); i++)
                    {
                        nodeList.Add(path.m_nodes[i] + diff);
                    }
                    path.m_nodes = nodeList.ToArray();
                }
            }
            else
            {
                var nodeList = new List<Vector3>();
                for (int i = 0; i < PathCustomization.Path.m_nodes.Count(); i++)
                {
                    nodeList.Add(PathCustomization.Path.m_nodes[i] + diff);
                }
                PathCustomization.Path.m_nodes = nodeList.ToArray();
            }
        }

        private static void ResizeUndergroundStationTracks()
        {
            var linkedStationTracks = GetInterlinkedStationTracks();
            var processedConnectedPaths = new List<int>();
            for (var index = 0; index < m_StationPaths.Count(); index++)
            {
                var path = m_StationPaths[index];
                if (!path.m_netInfo.IsUndergroundMetroStationTrack())
                    continue;
                if (PathCustomization.Path?.m_nodes == null || PathCustomization.Path == path)
                {
                    //if (!linkedStationTracks.Contains(path))
                    ChangeStationTrackLength(index, processedConnectedPaths);
                }
                else
                {

                }
            }
        }

        private static void ChangeStationDepthAndRotation()
        {
            var pathList = new List<BuildingInfo.PathInfo>();
            var highestStation = float.MinValue;
            var totalNode = Vector3.zero;
            if (PathCustomization.Path?.m_nodes != null)
            {
                highestStation = PathCustomization.Path.m_nodes.OrderByDescending(n => n.y).FirstOrDefault().y;
            }
            else
            {
                foreach (var path in m_Info.m_paths)
                {
                    if (path.m_netInfo?.m_netAI == null || !path.m_netInfo.IsUndergroundMetroStationTrack())
                        continue;

                    var highest = path.m_nodes.OrderByDescending(n => n.y).FirstOrDefault().y;
                    highestStation = Math.Max(highest, highestStation);
                }
            }

            var offsetDepthDist = PathCustomization.Depth + highestStation;

            if (LowestHighPaths.Count > 0)
            {
                m_AngleDelta = CalculateAngleDelta();
                for (var i = 0; i < m_Info.m_paths.Count(); i++)
                {

                    var path = m_Info.m_paths[i];
                    if (AllNodesUnderGround(path))
                    {
                        if (LowestHighPaths.Contains(path))
                        {
                            path.AssignNetInfo("Pedestrian Connection Underground");
                            path.m_forbidLaneConnection = new[] { false, true };
                        }
                        else
                        {
                            if (path.IsPedestrianPath())
                            {
                                pathList.Add(path);
                                continue;
                            }
                            if (IsAlterOfType(MetroStationTrackAlterType.Rotation))
                                ChangePathRotation(path, m_AngleDelta);
                            if (IsAlterOfType(MetroStationTrackAlterType.Depth) && IsATargetPath(path))
                                DipPath(path, offsetDepthDist);
                        }
                    }
                    pathList.Add(path);
                }
                m_Info.m_paths = pathList.ToArray();
            }
        }

        private static Vector3 GetMeanVector(List<Vector3> vectorList)
        {
            if (vectorList.Count > 0)
            {
                float x = 0f;
                float y = 0f;
                float z = 0f;
                foreach (Vector3 v in vectorList)
                {
                    x += v.x;
                    y += v.y;
                    z += v.z;
                }
                return new Vector3(x / vectorList.Count, y / vectorList.Count, z / vectorList.Count);
            }
            return Vector3.zero;


        }
        private static void DipPath(BuildingInfo.PathInfo path, float depthOffsetDist)
        {
            ShiftPath(path, new Vector3(0, -depthOffsetDist, 0));
        }

        private static void ShiftPath(BuildingInfo.PathInfo path, Vector3 offset)
        {
            for (var i = 0; i < path.m_nodes.Length; i++)
            {
                path.m_nodes[i] = path.m_nodes[i] + offset;
            }
            for (var i = 0; i < path.m_curveTargets.Length; i++)
            {
                path.m_curveTargets[i] = path.m_curveTargets[i] + offset;
            }
        }

        //private static IEnumerable<BuildingInfo.PathInfo> GenerateSteps(BuildingInfo.PathInfo path, float depth, double angle, bool isBound = true)
        //{
        //    var newPath = path.ShallowClone();
        //    var pathList = new List<BuildingInfo.PathInfo>();
        //    Vector3 lastCoords;
        //    Vector3 currCoords;
        //    if (AllNodesUnderGround(newPath))
        //    {
        //        lastCoords = newPath.m_nodes.OrderBy(n => n.z).FirstOrDefault();
        //        currCoords = newPath.m_nodes.OrderBy(n => n.z).LastOrDefault();
        //    }
        //    else
        //    {
        //        lastCoords = newPath.m_nodes.OrderBy(n => n.y).LastOrDefault();
        //        currCoords = newPath.m_nodes.OrderBy(n => n.y).FirstOrDefault();
        //    }
        //    var dir = new Vector3();

        //    MarkPathGenerated(newPath);
        //    newPath.AssignNetInfo("Pedestrian Connection Underground");
        //    var steps = (float)Math.Floor((depth + 4) / 12) * 4;
        //    if (steps == 0)
        //    {
        //        currCoords.y -= depth;
        //        newPath.m_nodes = new[] { lastCoords, currCoords };
        //        SetCurveTargets(newPath);
        //        ChangePathRotation(newPath, angle);
        //        pathList.Add(newPath);
        //    }
        //    else
        //    {
        //        float binder = 8;
        //        float stepDepth = binder / 2;
        //        var depthLeft = depth;
        //        if (isBound)
        //        {
        //            dir.x = Math.Min(currCoords.x - lastCoords.x, binder);
        //            dir.z = Math.Min(currCoords.z - lastCoords.z, binder);
        //        }
        //        else
        //        {
        //            dir.x = currCoords.x - lastCoords.x;
        //            dir.z = currCoords.z - lastCoords.z;
        //        }
        //        dir.y = currCoords.y - lastCoords.y;
        //        for (var j = 0; j < steps; j++)
        //        {
        //            var newZ = currCoords.z + dir.x;
        //            var newX = currCoords.x - dir.z;
        //            lastCoords = currCoords;
        //            currCoords = new Vector3() { x = newX, y = currCoords.y - Math.Max(0, Math.Min(stepDepth, depthLeft)), z = newZ };
        //            depthLeft -= stepDepth;
        //            var newNodes = new[] { lastCoords, currCoords };
        //            newPath = newPath.ShallowClone();
        //            newPath.m_nodes = newNodes;
        //            SetCurveTargets(newPath);
        //            ChangePathRotation(newPath, angle);
        //            pathList.Add(newPath);
        //            if (isBound)
        //            {
        //                dir.x = Math.Min(currCoords.x - lastCoords.x, binder);
        //                dir.z = Math.Min(currCoords.z - lastCoords.z, binder);
        //            }
        //            else
        //            {
        //                dir.x = currCoords.x - lastCoords.x;
        //                dir.z = currCoords.z - lastCoords.z;
        //            }
        //            dir.y = currCoords.y - lastCoords.y;
        //        }
        //    }
        //    return pathList;
        //}

        private static void ChangeStationTrackLength(int pathIndex, ICollection<int> processedConnectedPaths)
        {
            var path = m_StationPaths[pathIndex];
            if (path.m_netInfo == null || !path.m_netInfo.IsUndergroundMetroStationTrack())
            {
                return;
            }
            if (path.m_nodes.Length < 2)
            {
                return;
            }
            var beginning = path.m_nodes.First();
            var end = path.m_nodes.Last();
            var middle = path.GetMiddle();
            var length = 0.0f;
            for (var i = 1; i < path.m_nodes.Length; i++)
            {
                length += Vector3.Distance(path.m_nodes[i], path.m_nodes[i - 1]);
            }
            if (Math.Abs(length) < Extensions.BuildingInfoExtensions.AUTOGEN_TOLERANCE)
            {
                return;
            }
            var adjTrackLength = PathCustomization.Length * (((2 * Math.Abs(PathCustomization.Curve) * Math.Sqrt(2)) / Math.PI) - Math.Abs(PathCustomization.Curve) + 1);
            var coefficient = (float)Math.Abs(adjTrackLength / length);
            for (var i = 0; i < path.m_nodes.Length; i++)
            {
                path.m_nodes[i] = new Vector3
                {
                    x = middle.x + ((path.m_nodes[i].x - middle.x) * coefficient),
                    y = middle.y + ((path.m_nodes[i].y - middle.y) * coefficient),
                    z = middle.z + ((path.m_nodes[i].z - middle.z) * coefficient),
                };
            }
            for (var i = 0; i < path.m_curveTargets.Length; i++)
            {
                path.m_curveTargets[i] = new Vector3
                {
                    x = middle.x + (path.m_curveTargets[i].x - middle.x) * coefficient,
                    y = middle.y + (path.m_curveTargets[i].y - middle.y) * coefficient,
                    z = middle.z + (path.m_curveTargets[i].z - middle.z) * coefficient,
                };
            }
            var newBeginning = path.m_nodes.First();
            var newEnd = path.m_nodes.Last();

            ChangeConnectedPaths(beginning, newBeginning - beginning, processedConnectedPaths);
            ChangeConnectedPaths(end, newEnd - end, processedConnectedPaths);

        }

        private static double CalculateAngleDelta()
        {
            BuildingInfo.PathInfo mainTrack = null;
            if (PathCustomization.Path?.m_nodes != null)
            {
                mainTrack = PathCustomization.Path;
            }
            else if (m_PremierPath > -1 && m_StationPaths.Count > m_PremierPath)
            {
                mainTrack = m_StationPaths[m_PremierPath];
            }
            else
            {
                mainTrack = m_StationPaths.OrderBy(s => Vector3.Magnitude(s.GetMiddle())).ThenBy(s2 => s2.GetMiddle().y).FirstOrDefault();
                m_PremierPath = m_StationPaths.IndexOf(mainTrack);
            }
            float delta = PathCustomization.Rotation - m_PrevAngle;
            m_PrevAngle = PathCustomization.Rotation;
            delta *= (float)(Math.PI / 180);
            return delta;
        }
        private static void ChangePathRotation(BuildingInfo.PathInfo path, double angle)
        {
            Debug.Log("AngleDelta is " + angle);
            var offset = Vector3.zero;
            if (PathCustomization.Path?.m_nodes != null)
            {
                if (PathCustomization.Path == path)
                {
                    offset = Vector3.Lerp(path.m_nodes.First(), path.m_nodes.Last(), 0.5f);
                }
                else
                {
                    return;
                }
            }
            Vector3 newNode = Vector3.zero;
            if (path.m_nodes != null && path.m_nodes.Length > 0)
            {
                for (var nodeIndex = 0; nodeIndex < path.m_nodes.Count(); nodeIndex++)
                {
                    var oldNode = path.m_nodes[nodeIndex] - offset;
                    newNode = new Vector3
                    {
                        x = (float)((oldNode.x * Math.Cos(angle)) - (oldNode.z * Math.Sin(angle))),
                        y = oldNode.y,
                        z = (float)((oldNode.x * Math.Sin(angle)) + (oldNode.z * Math.Cos(angle)))
                    } + offset;
                    path.m_nodes[nodeIndex] = newNode;
                }
            }

            if (path.m_curveTargets != null && path.m_curveTargets.Length > 0)
            {
                for (var curveIndex = 0; curveIndex < path.m_curveTargets.Count(); curveIndex++)
                {
                    var oldCurve = path.m_curveTargets[curveIndex] - offset;
                    var newCurve = new Vector3
                    {
                        x = (float)((oldCurve.x * Math.Cos(angle)) - (oldCurve.z * Math.Sin(angle))),
                        y = oldCurve.y,
                        z = (float)((oldCurve.x * Math.Sin(angle)) + (oldCurve.z * Math.Cos(angle))),
                    } + offset;
                    path.m_curveTargets[curveIndex] = newCurve;
                }
            }
        }
        private static void ChangeConnectedPaths(Vector3 nodePoint, Vector3 delta, ICollection<int> processedConnectedPaths)
        {
            for (var pathIndex = 0; pathIndex < m_StationPaths.Count; pathIndex++)
            {
                var path = m_StationPaths[pathIndex];
                if (path?.m_netInfo == null || path.m_netInfo.IsUndergroundMetroStationTrack())
                {
                    continue;
                }
                if (processedConnectedPaths.Contains(pathIndex) || !path.m_nodes.Where(n => n == nodePoint).Any())
                {
                    continue;
                }
                var beginning = path.m_nodes.First();
                var end = path.m_nodes.Last();
                if (path.m_netInfo.m_netAI.IsUnderground())
                {
                    for (var i = 0; i < path.m_nodes.Length; i++)
                    {
                        if (path.m_nodes[i].y <= -12f || path.m_nodes[i] == nodePoint)
                        {
                            continue;
                        }
                        path.m_nodes[i] = new Vector3
                        {
                            x = path.m_nodes[i].x,
                            y = -DEF_DEPTH,
                            z = path.m_nodes[i].z
                        };
                        path.SetCurveTargets();
                    }
                }
                ShiftPath(path, delta);
                processedConnectedPaths.Add(pathIndex);
                var newBeginning = path.m_nodes.First();
                var newEnd = path.m_nodes.Last();

                ChangeConnectedPaths(beginning, newBeginning - beginning, processedConnectedPaths);
                ChangeConnectedPaths(end, newEnd - end, processedConnectedPaths);
            }
        }

        private static BuildingInfo.PathInfo[] GetInterlinkedStationTracks()
        {
            var pairs =
                m_Info.m_paths.Where(p => p.m_netInfo.IsUndergroundMetroStationTrack()).SelectMany(p => p.m_nodes)
                    .GroupBy(n => n)
                    .Where(grp => grp.Count() > 1)
                    .Select(grp => grp.Key)
                    .ToArray();
            return m_Info.m_paths.Where(p => p.m_netInfo.IsUndergroundMetroStationTrack() && p.m_nodes.Any(n => pairs.Contains(n))).ToArray();
        }

        private static bool AllNodesUnderGround(BuildingInfo.PathInfo path)
        {
            return path.m_nodes.All(n => n.y <= -4);
        }
        private static bool BuildingHasPedestrianConnectionSurface()
        {
            return m_Info.m_paths.Count(p => p.IsPedestrianPath()) >= 1;
        }
        private static List<Vector3> GetIsolatedNodes()
        {
            return m_Info.m_paths
                .Where(p => p.IsPedestrianPath())
                .SelectMany(p => p.m_nodes).GroupBy(x => x)
                .Where(g => g.Count() == 1)
                .Select(y => y.Key)
                .ToList();
        }
        private static List<BuildingInfo.PathInfo> GetIsolatedPaths()
        {
            List<BuildingInfo.PathInfo> isolatedPaths = null;
            var query = GetIsolatedNodes();
            if (query.Count() > 0)
            {
                isolatedPaths = m_Info.m_paths.Where(p => p.m_nodes.All(n => query.Contains(n))).ToList();
            }
            return isolatedPaths;
        }
        private static void CheckPedestrianConnections()
        {
            var isolatedPaths = GetIsolatedPaths();
            if (isolatedPaths != null)
            {
                var straddlePaths = isolatedPaths.Where(p => p.m_nodes.Any(n => n.y >= 0));
                if (isolatedPaths.Count > 0 && straddlePaths.Count() == 0)
                {
                    var pathList = m_Info.m_paths.ToList();
                    foreach (var path in isolatedPaths)
                    {
                        var newStub = AddStub(path);
                        if (newStub != null)
                        {
                            pathList.Add(newStub);
                        }
                    }
                    m_Info.m_paths = pathList.ToArray();
                }
            }
            //return m_Info.m_paths.Count(p => p.m_netInfo != null && p.m_netInfo.name == "Pedestrian Connection Surface" || p.m_netInfo.name == "Pedestrian Connection Inside");
        }
        private static BuildingInfo.PathInfo AddStub(BuildingInfo.PathInfo path)
        {
            var pathLastIndex = path?.m_nodes?.Count() - 1 ?? 0;
            if (pathLastIndex < 1 || path.m_nodes.Any(n => n.y >= 0))
            {
                return null;
            }
            var newPath = path.ShallowClone();
            var newPathNodes = new List<Vector3>();
            var pathNodeDiff = path.m_nodes.Last() - path.m_nodes.First();
            //var pathLengthCoeff = Vector3.Distance(path.m_nodes.First(), path.m_nodes.Last()) / 3;
            newPathNodes.Add(new Vector3() { x = path.m_nodes.First().x - pathNodeDiff.x, y = 0, z = path.m_nodes.First().z - pathNodeDiff.z });
            newPathNodes.Add(path.m_nodes.First());
            newPath.m_nodes = newPathNodes.ToArray();
            //MarkPathGenerated(newPath);
            return newPath;
        }
    }
}

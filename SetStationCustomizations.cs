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
        private static Stack<ParentStationMetaData> m_ParentMetaDataStack = new Stack<ParentStationMetaData>();
        private static BuildingInfo m_Info;
        private static ParentStationMetaData m_ParentMetaData;
        private static bool m_ResetMainRotation = false;
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
        public static List<Vector3> m_AnchorPoints = null;
        public static List<Vector3> AnchorPoints {
            get
            {
                if (m_AnchorPoints == null)
                {
                    m_AnchorPoints = new List<Vector3>();
                    Debug.Log("Setting up AnchorPoints for " + m_Info);
                    if (m_ParentMetaData != null)
                    {
                        Debug.Log("SubBuilding " + m_Info.name + " is being checked for AnchorPoints");
                        var anchorPoints = m_Info.m_paths.Where(p => p.IsPedestrianPath() && p.m_nodes.Any(n => n.y < 0) && p.m_nodes.Any(n => n.y >= 0)).SelectMany(p => p.m_nodes).Where(n => n.y < 0);
                        if (anchorPoints != null && anchorPoints.Count() > 0)
                        {
                            m_AnchorPoints.AddRange(anchorPoints);
                        }
                        else
                        {
                            anchorPoints = m_ParentMetaData.Info.m_paths.Where(p => p.IsPedestrianPath() && p.m_nodes.Any(n => n.y == 0)).SelectMany(p => p.m_nodes).Where(n => n.y == 0).ToList();
                            m_AnchorPoints.AddRange(anchorPoints.Select(n => AdjustNodeFromParentToChild(n)));
                        }
                    }
                    else
                    {
                        Debug.Log("Building " + m_Info.name + " is being checked for AnchorPoints");
                        var anchorPaths = m_Info.m_paths.Where(p => p.IsPedestrianPath() && p.m_nodes.Any(n => n.y == 0)).ToList();
                        if (anchorPaths != null)
                        {
                            for (int i = 0; i < anchorPaths.Count(); i++)
                            {
                                var anchorNode = anchorPaths[i].m_nodes.OrderBy(p => p.y).FirstOrDefault();
                                m_AnchorPoints.Add(anchorNode);
                                if (anchorNode == default)
                                {
                                    Debug.Log("No suitable anchorNodes found on building.");
                                    foreach (var node in anchorPaths[i].m_nodes)
                                    {
                                        Debug.Log(node.ToString() + ", parent: " + AdjustNodeFromChildToParent(node).ToString());
                                    }
                                }
                                Debug.Log("Anchorpoint " + anchorNode.ToString() + " found on building" + m_Info.name);
                            }
                        }
                        else
                        {
                            Debug.Log("No nodes in Building " + m_Info.name + " that satisfies the outline criteria.");
                        }
                    }
                }
                return m_AnchorPoints;
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
            if (pathCustomization.Path.m_nodes == null && m_ResetMainRotation)
            {
                m_ResetMainRotation = false;
                m_PrevAngle = 0;
            }
            var hasPaths = m_StationPaths.Count() > 0;
            PathCustomization = pathCustomization;
            ClearAnchorPoints();
            if (PathCustomization.AlterType == MetroStationTrackAlterType.None)
                PathCustomization.AlterType = MetroStationTrackAlterType.All;

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
                ClearAnchorPoints();
                Debug.Log("info " + m_Info.name + " has " + m_StationPaths.Count() + " station paths");
                if (m_Info.HasUndergroundMetroStationTracks(false))
                    //RemovePedPaths();
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

        }

        private static void ClearAnchorPoints()
        {
            if (m_AnchorPoints != null)
            {
                m_AnchorPoints.Clear();
                m_AnchorPoints = null;
            }
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
        private static List<Vector3> m_NodeList = new List<Vector3>();
        private static List<Vector3> NodeList {
            get
            {
                if (m_NodeList == null)
                {
                    m_NodeList = new List<Vector3>();
                }
                return m_NodeList;
            }
        }
        private static void ClearNodeList()
        {
            if (m_NodeList != null)
            {
                m_NodeList.Clear();
                m_NodeList = null;
            }
        }
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
                var midPoint = Vector3.zero;
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

                if (!StairDict.ContainsKey(TrackPath))
                    StairDict.Add(TrackPath, new List<BuildingInfo.PathInfo>());

                if (connectorHalfWidths.Count() > 1)
                {
                    var newNodesCount = newNodes.Count() - 1;
                    for (int i = 0; i < newNodesCount; i++)
                    {
                        if (Vector3.Distance(newNodes[i], newNodes[i + 1]) <= 4)
                        {
                            var averageNode = AverageNode(newNodes[i], newNodes[i + 1]);
                            newNodes[i] = averageNode;
                            newNodes[i + 1] = averageNode;
                        }
                    }
                }

                for (var i = 0; i < stairNodes.Count(); i++)
                {
                    var branchPathStair = CreatePath(NewPath, newNodes[i], stairNodes[i]);
                    branchPathStair.m_forbidLaneConnection = new[] { true, false };
                    branchPathStair.AssignNetInfo(m_SpecialNetInfo);
                    StairDict[TrackPath].Add(branchPathStair);
                    retval.Add(branchPathStair);
                }

                NodeList.AddRange(newNodes);
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
            if (m_StairDict != null)
            {
                m_StairDict.Clear();
                m_StairDict = null;
            }
            ClearNodeList();
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
            m_Info.m_paths = pathList.ToArray();
            CleanPaths();

        }
        private static List<BuildingInfo.PathInfo> m_TrackPool;
        private static List<BuildingInfo.PathInfo> TrackPool {
            get
            {
                if (m_TrackPool == null)
                {
                    m_TrackPool = new List<BuildingInfo.PathInfo>();
                    m_TrackPool.AddRange(m_StationPaths);
                }
                return m_TrackPool;
            }
        }
        private static void ClearTrackPool()
        {
            if (m_TrackPool != null)
            {
                m_TrackPool.Clear();
                m_TrackPool = null;
            }
        }
        private static Vector2 ConvertToPlanar(Vector3 vector)
        {
            return new Vector2(vector.x, vector.z);
        }

        private static float GetPlanarDistance(Vector3 a, Vector3 b)
        {
            return Vector2.Distance(ConvertToPlanar(a), ConvertToPlanar(b));
        }

        private static void ConnectStations()
        {
            if (m_StationPaths.Count > 0)
            {
                var pathList = m_Info.m_paths.ToList();
                var nodePool = new List<Vector3>();
                var pathNodeList = new List<Vector3>();
                var connectorPath = NewPath.ShallowClone();
                var averageAnchorNode = Vector3.zero;
                foreach (var node in AnchorPoints)
                {
                    averageAnchorNode += node;
                }
                averageAnchorNode /= AnchorPoints.Count();
                m_NodeList = m_NodeList.OrderByDescending(n => Vector3.Distance(n, averageAnchorNode)).ToList();
                nodePool.AddRange(m_NodeList);
                nodePool.AddRange(AnchorPoints);
                for (int i = 0; i < nodePool.Count(); i++)
                {
                    Debug.Log("NodePool contains " + nodePool[i].ToString());
                }

                Debug.Log("NodeList count is " + NodeList.Count());
                for (int i = 0; i < NodeList.Count(); i++)
                {
                    var node = NodeList[i];
                    nodePool.Remove(node);
                    Debug.Log("Inspecting NodeList[" + i + "] " + node.ToString());
                    Debug.Log("NodePool has the following remaining");
                    for (int j = 0; j < nodePool.Count(); j++)
                    {
                        Debug.Log(nodePool[j].ToString());
                    }
                    var closestNode = nodePool.Where(n => GetPlanarDistance(n, node) >= 4).OrderBy(n => Vector3.Distance(n, node)).FirstOrDefault();
                    Debug.Log("Starting Node " + node.ToString());
                    if (!pathNodeList.Contains(node))
                        pathNodeList.Add(node);
                    if (!pathNodeList.Contains(closestNode))
                        pathNodeList.Add(closestNode);
                    Debug.Log("Node " + closestNode.ToString() + " added");
                    if (AnchorPoints.Contains(closestNode))
                    {
                        Debug.Log("Path done");
                        connectorPath.m_nodes = pathNodeList.ToArray();
                        var forbidLaneConnectionList = new List<bool>();
                        for (int j = 0; j < pathNodeList.Count() - 1; j++)
                        {

                            forbidLaneConnectionList.Add(true);
                        }
                        forbidLaneConnectionList.Add(false);
                        connectorPath.m_forbidLaneConnection = forbidLaneConnectionList.ToArray();
                        connectorPath.MarkPathGenerated();
                        //connectorPath.SetCurveTargets();
                        pathList.Add(connectorPath);
                        pathNodeList.Clear();
                        connectorPath = connectorPath.ShallowClone();
                    }
                }
                m_Info.m_paths = pathList.ToArray();
                //CleanPaths();
            }
        }

        public static List<Vector3> AdjustNodeFromParentToChild(Vector3[] nodes, ParentStationMetaData parentmetadata = null)
        {
            List<Vector3> retList = new List<Vector3>();
            foreach (var node in nodes)
            {
                retList.Add(AdjustNodeFromParentToChild(node, parentmetadata));
            }
            return retList;
        }

        public static Vector3 AdjustNodeFromParentToChild(Vector3 node, ParentStationMetaData parentMetaData = null)
        {
            if (parentMetaData == null)
                parentMetaData = m_ParentMetaData;
            var retNode = node;
            if (m_ParentMetaData != null)
            {
                var angle = (parentMetaData.Angle * Math.PI) / 180;
                retNode = new Vector3
                {
                    x = (float)(((node.x - parentMetaData.Position.x) * Math.Cos(angle)) - ((node.z + parentMetaData.Position.z) * Math.Sin(angle))),
                    y = node.y,
                    z = (float)(((node.x - parentMetaData.Position.x) * Math.Sin(angle)) + ((node.z + parentMetaData.Position.z) * Math.Cos(angle)))
                };
            }
            return retNode;
        }
        public static Vector3 AdjustNodeFromChildToParent(Vector3 node)
        {
            var retNode = node;
            if (m_ParentMetaData != null)
            {
                var angle = (m_ParentMetaData.Angle * Math.PI) / 180;
                retNode = new Vector3
                {
                    x = (float)(node.x * Math.Cos(angle) + node.z * Math.Sin(angle) + m_ParentMetaData.Position.x),
                    y = node.y,
                    z = (float)(node.z * Math.Cos(angle) - node.x * Math.Sin(angle) - m_ParentMetaData.Position.z)
                };
            }
            return retNode;
        }
        private static Vector3 AverageNode(params Vector3[] nodes)
        {
            var center = Vector3.zero;
            foreach (var node in nodes)
            {
                center += node;
            }
            return center / nodes.Count();
        }

        private static Dictionary<Vector3, Vector3> m_MergePathDict = null;
        private static Dictionary<Vector3, Vector3> MergePathDict {
            get
            {
                if (m_MergePathDict == null)
                {
                    m_MergePathDict = new Dictionary<Vector3, Vector3>();
                }
                return m_MergePathDict;
            }
        }
        private static void ClearMergePathDict()
        {
            if (m_MergePathDict != null)
            {
                m_MergePathDict.Clear();
                m_MergePathDict = null;
            }
        }
        private static void CleanPaths()
        {
            var pathList = m_Info.m_paths;
            List<BuildingInfo.PathInfo> parentBuildingInfo = null;
            ClearMergePathDict();
            for (int i = 0; i < pathList.Count(); i++)
            {
                var path = pathList[i];
                for (int j = 0; j < path.m_nodes.Count() - 1; j++)
                {
                    var node = path.m_nodes[j];
                    var nextNode = path.m_nodes[j + 1];
                    if (!MergePathDict.ContainsKey(node) && Vector3.Distance(node, nextNode) <= 4)
                    {
                        var average = AverageNode(node, nextNode);
                        MergePathDict.Add(node, average);
                    }
                }
            }
            ConsolidatePaths(false);
            if (m_ParentMetaData != null)
                ConsolidatePaths(true);
        }
        private static void ConsolidatePaths(bool consolidateParentNodes)
        {
            var pathList = consolidateParentNodes ? m_ParentMetaData.Info.m_paths : m_Info.m_paths;
            for (int i = 0; i < pathList.Count(); i++)
            {
                var path = pathList[i];
                List<Vector3> nodeList = path.m_nodes.Any(n => MergePathDict.ContainsKey(n)) ? new List<Vector3>() : null;
                if (nodeList != null)
                {
                    for (int j = 0; j < pathList[i].m_nodes.Count(); j++)
                    {
                        var node = path.m_nodes[j];
                        for (int k = 0; k < MergePathDict.Count(); k++)
                        {
                            var kvp = MergePathDict.ElementAt(k);
                            var nodeToMove = consolidateParentNodes ? AdjustNodeFromChildToParent(kvp.Key) : kvp.Key;
                            var moveNodeTo = consolidateParentNodes ? AdjustNodeFromChildToParent(kvp.Value) : kvp.Value;
                            if (node == nodeToMove)
                                if (!nodeList.Contains(moveNodeTo))
                                    nodeList.Add(moveNodeTo);
                                else
                                    if (!nodeList.Contains(node))
                                    nodeList.Add(node);
                        }
                    }
                    pathList[i].m_nodes = nodeList.ToArray();
                    pathList[i].SetCurveTargets();
                }
            }
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
            newPath.SetCurveTargets();
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
            if (m_ParentMetaData != null)
                m_Info.m_paths = m_Info.m_paths.Where(p => !p.IsPedestrianPath() || p.m_nodes.Any(n => n.y == 0)).ToArray();
            else
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
                            AnchorPoints = AnchorPoints,
                            Angle = subBuilding.m_angle,
                            Position = subBuilding.m_position
                        };
                        var modifiedPathCustomization = PathCustomization;
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
                if (path.m_netInfo.IsPedestrianNetwork() && !path.IsPathGenerated() && path.m_nodes.All(n => n.y <= -8))
                    continue;
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

            if (AnchorPoints.Count > 0)
            {
                m_AngleDelta = CalculateAngleDelta();
                for (var i = 0; i < m_Info.m_paths.Count(); i++)
                {

                    var path = m_Info.m_paths[i];
                    if (AllNodesUnderGround(path))
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
                    pathList.Add(path);
                }
                m_Info.m_paths = pathList.ToArray();
            }
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
                    if (angle != 0 && !m_ResetMainRotation && PathCustomizationDict.Count > 1)
                    {
                        PathCustomizationDict.FirstOrDefault(p => p.Key.m_nodes == null).Value.Rotation = 0;
                        m_ResetMainRotation = true;
                    }
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

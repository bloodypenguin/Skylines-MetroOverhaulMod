using MetroOverhaul.NEXT.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using MetroOverhaul.Extensions;
using UnityEngine;
using MetroOverhaul.UI;
using ColossalFramework;

namespace MetroOverhaul
{
    public static class SetUpdateStationCustomizations
    {
        public const int MAX_DEPTH = 36;
        public const int MIN_DEPTH = 12;
        public const int DEF_DEPTH = 15;
        public const int MAX_ANGLE = 360;
        public const int MIN_ANGLE = 0;
        public const int DEF_ANGLE = 0;
        public const int MAX_LENGTH = 192;
        public const int MIN_LENGTH = 88;
        public const int DEF_LENGTH = 144;
        public const int MAX_BEND_STRENGTH = 1;
        public const int MIN_BEND_STRENGTH = -1;
        public const int DEF_BEND_STRENGTH = 0;
        private static float StairCoeff { get { return (11f / 64f); } }
        private static float AntiStairCoeff { get { return 1 - StairCoeff; } }
        private static float GENERATED_PATH_MARKER = 0.09999f; //regular paths have snapping distance of 0.1f. This way we differentiate
        private const float TOLERANCE = 0.000001f; //equals 1/10 of difference between 0.1f and GENERATED_PATH_MARKER
        private static NetManager ninstance = Singleton<NetManager>.instance;
        private static BuildingManager binstance = Singleton<BuildingManager>.instance;
        private static List<ushort> m_Nodes;
        public static void ModifyStation(ushort buildingID, float targetDepth, float targetStationTrackLength, double angle, float bendStrength, BuildingInfo superInfo = null)
        {
            m_Nodes = new List<ushort>();
            Building building = BuildingFrom(buildingID);
            ushort nodeID = building.m_netNode;
            while (nodeID != 0)
            {
                m_Nodes.Add(nodeID);
                nodeID = NodeFrom(nodeID).m_nextBuildingNode;
            }
            if (!building.Info.HasUndergroundMetroStationTracks() || m_Nodes.Count() == 0)
            {
                return;
            }
            if (targetDepth <= 0 || targetStationTrackLength <= 0)
            {
                return;
            }
            //CleanUpPaths();
        }
        private static Building BuildingFrom(ushort buildingID)
        {
            return binstance.m_buildings.m_buffer[buildingID];
        }
        private static NetNode NodeFrom(ushort NodeID)
        {
            return ninstance.m_nodes.m_buffer[NodeID];
        }

        public static void ModifyStation(BuildingInfo info, float targetDepth, float targetStationTrackLength, double angle, float bendStrength, BuildingInfo superInfo = null)
        {
            if (!info.HasUndergroundMetroStationTracks() || info.m_paths == null || info.m_paths.Length < 1)
            {
                return;
            }
            if (targetDepth <= 0 || targetStationTrackLength <= 0)
            {
                return;
            }
            CleanUpPaths(info);

            ResizeUndergroundStationTracks(info, targetStationTrackLength);
            //if (BuildingHasPedestrianConnectionSurface(info))
            //{
            //Vector3 pivotPoint = GetMeanVector(info.m_paths.Where(p => p.m_netInfo.IsUndergroundMetroStationTrack()));
            var connectPoints = ChangeStationDepthAndRotation(info, targetDepth, angle);
            if (superInfo != null)
            {
                if (info.m_paths.Any(p => p.m_nodes.Any(n => n.y >= 0)) == false)
                {
                    var highestIsolatedPath = info.m_paths.Where(p => IsPedestrianPath(p)).OrderBy(p => p.m_nodes.Max(n => n.y)).LastOrDefault();
                    Debug.Log($"HIGHESTISOLATEDPATH NODES: {highestIsolatedPath.m_nodes.First().ToString()} and {highestIsolatedPath.m_nodes.Last().ToString()}");
                    if (highestIsolatedPath != null)
                    {
                        var rendevous = connectPoints.First();
                        var pathList = info.m_paths.ToList();
                        var closestSuperPath = superInfo.m_paths.OrderBy(p => p.m_nodes.Min(n => Vector3.Distance(n, rendevous))).FirstOrDefault();
                        var closestSuperNode = closestSuperPath.m_nodes.OrderBy(n => Vector3.Distance(n, rendevous)).FirstOrDefault();
                        var aux = ChainPath(closestSuperPath, rendevous, Array.IndexOf(closestSuperPath.m_nodes, closestSuperNode));
                        SetCurveTargets(aux);
                        pathList.Add(aux);
                        info.m_paths = pathList.ToArray();
                    }
                }
            }
            ReconfigureStationAccess(info, connectPoints, bendStrength);
            //}
            //else
            //{
            //BendStationTracks(info, targetDepth); //works for European Main Station!
            //}
            BendStationTrack(info, bendStrength);
            RecalculateSpawnPoints(info, bendStrength);
        }

        private static void BendStationTracks(BuildingInfo info, float targetDepth)
        {
            var processedConnectedPaths = new List<int>();
            for (var index = 0; index < info.m_paths.Length; index++)
            {
                var path = info.m_paths[index];
                if (!path.m_netInfo.IsUndergroundMetroStationTrack())
                {
                    continue;
                }
                //BendStationTrack(info.m_paths, index, targetDepth, processedConnectedPaths);
            }
        }
        private static void CheckBreaks(BuildingInfo info)
        {

        }
        private static void ReconfigureStationAccess(BuildingInfo info, List<Vector3> connectPoints, float bendStrength)
        {

            var pathList = new List<BuildingInfo.PathInfo>();
            var connectList = new List<Vector3[]>();
            var singleGap = 16;
            var directionList = new List<string>();
            for (int i = 0; i < info.m_paths.Length; i++)
            {
                var thePath = info.m_paths[i];
                if (thePath.m_netInfo == null || thePath.m_nodes == null || thePath.m_nodes.Count() < 2)
                    continue;
                if (thePath.m_netInfo.IsUndergroundMetroStationTrack())
                {
                    var xDirection = (thePath.m_nodes.First().x - thePath.m_nodes.Last().x);
                    var zDirection = (thePath.m_nodes.First().z - thePath.m_nodes.Last().z);
                    var direction = $"{xDirection},{zDirection}";
                    var antiDirection = $"{-xDirection},{-zDirection}";
                    if (directionList.Contains(direction))
                    {
                        continue;
                    }
                    if (directionList.Contains(antiDirection))
                    {
                        thePath.m_nodes = thePath.m_nodes.Reverse().ToArray();
                    }
                    else
                    {
                        directionList.Add(direction);
                    }
                }
            }
            for (int i = 0; i < info.m_paths.Length; i++)
            {
                var thePath = info.m_paths[i];
                if (thePath.m_netInfo == null || thePath.m_nodes == null || thePath.m_nodes.Count() < 2)
                    continue;
                if (thePath.m_netInfo.IsUndergroundSmallStationTrack())
                {
                    var xCoeff = -(thePath.m_nodes.First().x - thePath.m_nodes.Last().x) / Vector3.Distance(thePath.m_nodes.First(), thePath.m_nodes.Last());
                    var zCoeff = (thePath.m_nodes.First().z - thePath.m_nodes.Last().z) / Vector3.Distance(thePath.m_nodes.First(), thePath.m_nodes.Last());

                    var multiplier = 1;
                    var nearestTrackNode = info.m_paths.Where(p => p.m_netInfo.IsUndergroundMetroStationTrack()).SelectMany(p => p.m_nodes).Where(n => n.y == thePath.m_nodes.First().y).OrderBy(n => Vector3.Distance(n, thePath.m_nodes.First())).FirstOrDefault();
                    if (Vector3.Distance(nearestTrackNode, thePath.m_nodes.First()) <= 2 * singleGap)
                    {
                        multiplier = -1;
                    }
                    var aNewPath = thePath.ShallowClone();
                    var nodeList = new List<Vector3>();
                    nodeList.Add(new Vector3()
                    {
                        x = thePath.m_nodes.First().x + multiplier * zCoeff * singleGap,
                        y = thePath.m_nodes.First().y,
                        z = thePath.m_nodes.First().z + multiplier * xCoeff * singleGap
                    });
                    nodeList.Add(new Vector3()
                    {
                        x = thePath.m_nodes.Last().x + multiplier * zCoeff * singleGap,
                        y = thePath.m_nodes.Last().y,
                        z = thePath.m_nodes.Last().z + multiplier * xCoeff * singleGap
                    });
                    aNewPath.m_nodes = nodeList.ToArray();
                    MarkPathGenerated(aNewPath);
                    pathList.Add(aNewPath);
                }
                pathList.Add(thePath);
            }
            var specialNetInfo = FindNetInfo("Pedestrian Connection Inside");
            specialNetInfo.m_maxSlope = 100;
            specialNetInfo.m_maxTurnAngle = 180;
            specialNetInfo.m_maxTurnAngleCos = -1;
            var aPath = pathList.FirstOrDefault(p => IsPedestrianPath(p));
            for (int i = 0; i < pathList.Count; i++)
            {
                var newPath = aPath.ShallowClone();
                var trackPath = pathList[i];
                if (trackPath.m_netInfo != null && trackPath.m_netInfo.IsUndergroundMetroStationTrack())
                {
                    if (newPath != null)
                    {
                        var xCoeff = -(trackPath.m_nodes.First().x - trackPath.m_nodes.Last().x) / Vector3.Distance(trackPath.m_nodes.First(), trackPath.m_nodes.Last());
                        var zCoeff = (trackPath.m_nodes.First().z - trackPath.m_nodes.Last().z) / Vector3.Distance(trackPath.m_nodes.First(), trackPath.m_nodes.Last());
                        var stationLength = Vector3.Distance(trackPath.m_nodes.First(), trackPath.m_nodes.Last());
                        var stairsLengthX = (((0.12f * bendStrength) + 1) * (stationLength * StairCoeff)) * -xCoeff;
                        var stairsLengthZ = (((0.12f * bendStrength) + 1) * (stationLength * StairCoeff)) * zCoeff;
                        newPath.m_netInfo.m_maxSlope = 100;
                        newPath.m_netInfo.m_maxTurnAngle = 180;
                        newPath.m_netInfo.m_maxTurnAngleCos = -1;
                        if (trackPath.m_netInfo.IsUndergroundIslandPlatformStationTrack())
                        {
                            var newNodes = new List<Vector3>();
                            newNodes.Add(new Vector3()
                            {
                                x = trackPath.m_nodes.Last().x - 3 * xCoeff,
                                y = trackPath.m_nodes.Last().y + 8,
                                z = trackPath.m_nodes.Last().z + 3 * zCoeff
                            });
                            newNodes.Add(new Vector3()
                            {
                                x = newNodes.First().x + stairsLengthX,
                                y = trackPath.m_nodes.Last().y,
                                z = newNodes.First().z + stairsLengthZ,
                            });
                            newPath.m_nodes = newNodes.ToArray();
                            newPath.AssignNetInfo(specialNetInfo);
                            MarkPathGenerated(newPath);

                            ChangePathRotation(newPath, newPath.m_nodes.First(), AntiStairCoeff * bendStrength * -Math.PI / 4);
                            pathList.Add(newPath);
                            var connectNodes = new Vector3[] { newPath.m_nodes.First() };
                            if (!connectList.Contains(connectNodes))
                                connectList.Add(connectNodes);
                        }
                        else if (trackPath.m_netInfo.IsUndergroundSmallStationTrack())
                        {
                            var newNodes = new List<Vector3>();
                            newNodes.Add(new Vector3()
                            {
                                x = trackPath.m_nodes.Last().x + (5 * zCoeff) - (3 * xCoeff),
                                y = trackPath.m_nodes.Last().y + 8,
                                z = trackPath.m_nodes.Last().z + (5 * xCoeff) + (3 * zCoeff)
                            });
                            newNodes.Add(new Vector3()
                            {
                                x = newNodes.First().x + stairsLengthX,
                                y = trackPath.m_nodes.Last().y,
                                z = newNodes.First().z + stairsLengthZ,
                            });
                            newPath.m_nodes = newNodes.ToArray();
                            newPath.AssignNetInfo(specialNetInfo);
                            MarkPathGenerated(newPath);

                            ChangePathRotation(newPath, newPath.m_nodes.First(), AntiStairCoeff * bendStrength * -Math.PI / 4);
                            pathList.Add(newPath);
                            var connectNodes = new Vector3[] { newPath.m_nodes.First() };
                            if (!connectList.Contains(connectNodes))
                                connectList.Add(connectNodes);
                        }
                        else if (trackPath.m_netInfo.IsUndergroundSidePlatformMetroStationTrack())
                        {
                            var newNodes = new List<Vector3>();
                            newNodes.Add(new Vector3()
                            {
                                x = trackPath.m_nodes.Last().x - (7 * zCoeff) - (3 * xCoeff),
                                y = trackPath.m_nodes.Last().y + 8,
                                z = trackPath.m_nodes.Last().z - (7 * xCoeff) + (3 * zCoeff)
                            });
                            newNodes.Add(new Vector3()
                            {
                                x = newNodes.First().x + stairsLengthX,
                                y = trackPath.m_nodes.Last().y,
                                z = newNodes.First().z + stairsLengthZ,
                            });
                            newPath.m_nodes = newNodes.ToArray();
                            newPath.AssignNetInfo(specialNetInfo);

                            var branchVectorConnect = new Vector3()
                            {
                                x = newNodes.First().x + 14 * zCoeff,
                                y = trackPath.m_nodes.Last().y + 8,
                                z = newNodes.First().z + 14 * xCoeff
                            };

                            var branchVectorStair = new Vector3()
                            {
                                x = branchVectorConnect.x + stairsLengthX,
                                y = trackPath.m_nodes.Last().y,
                                z = branchVectorConnect.z + stairsLengthZ,
                            };
                            var branchPathConnect = ChainPath(newPath, branchVectorConnect, 0);
                            var branchPathStair = ChainPath(branchPathConnect, branchVectorStair, -1, specialNetInfo);
                            MarkPathGenerated(newPath);

                            var trackPivot = new Vector3()
                            {
                                x = trackPath.m_nodes.Last().x,
                                y = trackPath.m_nodes.Last().y + 8,
                                z = trackPath.m_nodes.Last().z
                            }
                            ;
                            ChangePathRotation(newPath, trackPivot, AntiStairCoeff * bendStrength * -Math.PI / 4);
                            ChangePathRotation(branchPathConnect, trackPivot, AntiStairCoeff * bendStrength * -Math.PI / 4);
                            ChangePathRotation(branchPathStair, trackPivot, AntiStairCoeff * bendStrength * -Math.PI / 4);
                            pathList.Add(newPath);
                            pathList.Add(branchPathConnect);
                            pathList.Add(branchPathStair);

                            if (!connectList.Contains(branchPathConnect.m_nodes))
                                connectList.Add(branchPathConnect.m_nodes);
                        }
                    }
                }
            }
            if (info.m_paths.Count() >= 2)
            {
                CheckPedestrianConnections(info);
            }
            pathList = CleanPaths(pathList);
            var currentVector = connectPoints.First();
            var pool = new List<Vector3[]>();
            var pivotPath = pathList.FirstOrDefault(p => p.m_nodes.Any(n => n == connectPoints.First()));
            pool.AddRange(connectList);
            for (var i = 0; i < connectList.Count; i++)
            {
                Debug.Log("derpAStart");
                var closestVector = pool.SelectMany(n => n).OrderBy(n => (currentVector.x - n.x) + (100 * (connectPoints.First().y - n.y)) + (currentVector.z - n.z)).FirstOrDefault();
                Debug.Log("derpAEnd");
                var closestPath = pathList.FirstOrDefault(p => p.m_nodes.Any(n => n == closestVector));
                BuildingInfo.PathInfo branch = null;
                if (currentVector == connectPoints.First())
                {
                    branch = ChainPath(pivotPath, closestVector, Array.IndexOf(pivotPath.m_nodes, connectPoints.First()), PrefabCollection<NetInfo>.FindLoaded("Pedestrian Connection Surface"));
                }
                else
                {
                    branch = ChainPath(closestPath, currentVector, Array.IndexOf(closestPath.m_nodes, closestVector));
                }
                branch.AssignNetInfo("Pedestrian Connection Underground");
                pathList.Add(branch);
                var nodeArrayToLose = pool.FirstOrDefault(na => na.Any(n => n == closestVector));
                Debug.Log("derpBStart");
                currentVector = nodeArrayToLose.OrderBy(n => Vector3.Distance(closestVector, n)).LastOrDefault();
                Debug.Log("derpBEnd");
                if (nodeArrayToLose != null)
                {
                    pool.Remove(nodeArrayToLose);
                }
            }
            if (connectPoints.Count > 1)
            {
                for (var i = 1; i < connectPoints.Count(); i++)
                {
                    Vector3 node = connectPoints[i];
                    Debug.Log("derpCStart");
                    Vector3 closestVector = connectList.SelectMany(n => n).OrderBy(n => (node.x - n.x) + (100 * (node.y - n.y)) + (node.z - n.z)).FirstOrDefault();
                    Debug.Log("derpCEnd");
                    var closestPath = pathList.FirstOrDefault(p => p.m_nodes.Any(n => n == closestVector));
                    var branch = ChainPath(closestPath, node, Array.IndexOf(closestPath.m_nodes, closestVector));
                    branch.AssignNetInfo("Pedestrian Connection Underground");
                    pathList.Add(branch);
                }
            }
            info.m_paths = CleanPaths(pathList).ToArray();
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
                SetCurveTargets(path);
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
            MarkPathGenerated(newPath);
            return newPath;
        }

        private static void BendStationTrack(BuildingInfo.PathInfo stationPath, float bendStrength)
        {

            var middle = GetMiddle(stationPath);

            var newX = (stationPath.m_nodes.First().z - stationPath.m_nodes.Last().z) / 2;
            var newY = (stationPath.m_nodes.First().y + stationPath.m_nodes.Last().y) / 2;
            var newZ = -(stationPath.m_nodes.First().x - stationPath.m_nodes.Last().x) / 2;
            var newCurve = middle + (bendStrength * new Vector3(newX, newY, newZ));
            stationPath.m_curveTargets[0] = newCurve;
        }
        private static void BendStationTrack(BuildingInfo info, float bendStrength)
        {
            var stationPaths = info.m_paths.Where(p => p != null && p.m_netInfo != null && p.m_netInfo.IsUndergroundMetroStationTrack()).ToList();
            for (var i = 0; i < stationPaths.Count(); i++)
            {
                var stationPath = stationPaths[i];
                BendStationTrack(stationPath, bendStrength);
                var stairPaths = info.m_paths.Where(p => p != null && stationPaths.Contains(p) == false && p.m_nodes.Any(n => n.y == stationPath.m_nodes.First().y)).ToList();
                for (var j = 0; j < stairPaths.Count(); j++)
                {
                    BendStationTrack(stairPaths[j], -bendStrength * StairCoeff);
                }
            }
        }


        private static void CleanUpPaths(BuildingInfo info)
        {
            List<int> RemoveInx = null;
            for (int i = 0; i < m_Nodes.Count(); i++)
            {
                //if(NodeFrom(m_Nodes[i]).
            }
            info.m_paths = info.m_paths.Where(p => !IsPathGenerated(p)).ToArray();
            foreach (var path in info.m_paths)
            {
                path.m_forbidLaneConnection = null;
            }
        }

        private static void RecalculateSpawnPoints(BuildingInfo info, float bendStrength)
        {
            var buildingAI = info?.GetComponent<DepotAI>();
            var paths = info?.m_paths;
            if (buildingAI == null || paths == null)
            {
                return;
            }
            var spawnPoints = (from path in paths
                               where IsVehicleStop(path)
                               select GetMiddle(path, bendStrength)).Distinct().ToArray();

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

        private static bool IsVehicleStop(BuildingInfo.PathInfo path)
        {
            return (path?.m_nodes?.Length ?? 0) > 1 && (path?.m_netInfo?.IsUndergroundMetroStationTrack() ?? false);
        }

        private static void ResizeUndergroundStationTracks(BuildingInfo info, float targetStationTrackLength)
        {
            var linkedStationTracks = GetInterlinkedStationTracks(info);
            var processedConnectedPaths = new List<int>();
            for (var index = 0; index < info.m_paths.Length; index++)
            {
                var path = info.m_paths[index];
                if (!path.m_netInfo.IsUndergroundMetroStationTrack())
                {
                    continue;
                }
                if (!linkedStationTracks.Contains(path))
                {
                    ChangeStationTrackLength(info.m_paths, index, targetStationTrackLength, processedConnectedPaths);
                }
            }
        }
        private static List<Vector3> ChangeStationDepthAndRotation(BuildingInfo info, float targetDepth, double angle)
        {
            var pathList = new List<BuildingInfo.PathInfo>();
            var highestStation = float.MinValue;
            var totalNode = Vector3.zero;
            foreach (var path in info.m_paths)
            {

                if (path.m_netInfo?.m_netAI == null || !path.m_netInfo.IsUndergroundMetroStationTrack())
                {
                    continue;
                }
                var highest = path.m_nodes.OrderByDescending(n => n.y).FirstOrDefault().y;
                highestStation = Math.Max(highest, highestStation);
            }
            var offsetDepthDist = targetDepth + highestStation;
            //var lhpList = new List<List<Vector3>>();
            //var pedPathList = info.m_paths.Where(p => IsPedestrianPath(p));
            //var multiNodeList = new List<Vector3>();
            //foreach (var path in pedPathList)
            //{
            //    if (lhpList.Count() == 0)
            //    {
            //        lhpList.Add(path.m_nodes.ToList());
            //    }
            //    else
            //    {
            //        bool nodeFound = false;
            //        for (var i = 0; i < lhpList.Count(); i++)
            //        {   
            //            for(var j = 0; j < path.m_nodes.Count(); j++)
            //            {
            //                if (lhpList[i].Contains(path.m_nodes[j]))
            //                {
            //                    nodeFound = true;
            //                    for (var k = 0; k < path.m_nodes.Count(); k++)
            //                    {
            //                        if (lhpList[i].Contains(path.m_nodes[k]))
            //                        {
            //                            multiNodeList.Add(path.m_nodes[k]);
            //                        }
            //                        lhpList[i].Add(path.m_nodes[k]);
            //                    }
            //                    break;
            //                }
            //            }
            //            if (nodeFound)
            //            {
            //                break;
            //            }
            //        }
            //        if (!nodeFound)
            //        {
            //            lhpList.Add(path.m_nodes.ToList());
            //        }
            //    }
            //}
            var lowestHighPaths = info.m_paths.Where(p => IsPedestrianPath(p) && p.m_nodes.Any(n => n.y > -4) && p.m_nodes.Any(nd => nd.y <= -4)).ToList();
            if (lowestHighPaths.Count == 0)
            {
                lowestHighPaths.Add(info.m_paths.Where(p => IsPedestrianPath(p))
                    .OrderByDescending(p => p.m_nodes[0].y)
                    .FirstOrDefault());
            }
            if (lowestHighPaths.Count > 0)
            {
                Vector3 pivotPoint = GetMeanVector(info.m_paths.Where(p => p.m_netInfo.IsUndergroundMetroStationTrack()));
                for (var i = 0; i < info.m_paths.Count(); i++)
                {
                    var path = info.m_paths[i];
                    if (AllNodesUnderGround(path))
                    {
                        if (!lowestHighPaths.Contains(path))
                        {
                            if (!path.m_netInfo.IsUndergroundMetroStationTrack())
                            {
                                continue;
                            }
                            ChangePathRotation(path, pivotPoint, angle);
                            DipPath(path, offsetDepthDist);
                        }
                    }
                    pathList.Add(path);
                }
                info.m_paths = pathList.ToArray();
            }
            var lowestHighNodes = new List<Vector3>();
            foreach (BuildingInfo.PathInfo p in lowestHighPaths)
            {
                lowestHighNodes.Add(p.m_nodes.OrderByDescending(n=>n.y).ThenBy(n => n.z).LastOrDefault());
            }
            return lowestHighNodes;
        }
        //private static void ChangeStationDepthAndRotationx(BuildingInfo info, float targetDepth, double angle)
        //{
        //	var highestLow = float.MinValue;
        //	var highestStation = float.MinValue;
        //	foreach (var path in info.m_paths)
        //	{

        //		if (path.m_netInfo?.m_netAI == null || !path.m_netInfo.IsUndergroundMetroStationTrack())
        //		{
        //			continue;
        //		}
        //		var highest = path.m_nodes.OrderByDescending(n => n.y).FirstOrDefault().y;
        //		highestStation = Math.Max(highest, highestStation);
        //	}
        //	var offsetDepthDist = targetDepth + highestStation;
        //	float lowestHigh = 0;
        //	var lowestHighPaths = info.m_paths.Where(p => p.m_nodes.Any(n => n.y >= 0) && p.m_nodes.Any(nd => nd.y < 0)).ToList();

        //	if (lowestHighPaths.Count == 0)
        //	{
        //		lowestHighPaths.Add(info.m_paths.Where(p => p.m_netInfo.name == "Pedestrian Connection Surface" || p.m_netInfo.name == "Pedestrian Connection Inside")
        //			.OrderByDescending(p => p.m_nodes[0].y)
        //			.FirstOrDefault());
        //	}

        //	Vector3? pivotPoint = null;
        //	if (lowestHighPaths.Count > 0)
        //	{
        //		var lowestHighNode = lowestHighPaths[0].m_nodes.OrderBy(n => n.y).FirstOrDefault();
        //		lowestHigh = lowestHighNode.y;
        //		if (lowestHighPaths.Count == 1)
        //		{
        //			pivotPoint = lowestHighPaths[0].m_nodes.Last();
        //		}
        //		highestLow = info.m_paths.Where(p => lowestHighPaths.Contains(p) == false).SelectMany(p => p.m_nodes).Where(n => n.x == lowestHighNode.x && n.z == lowestHighNode.z).OrderByDescending(o => o.y).FirstOrDefault().y;
        //	} //TODO(earalov): properly handle integrated metro station (it has no own networks)
        //	var stepDepthDist = targetDepth + lowestHigh + highestStation - highestLow;
        //	var updatedPaths = new List<BuildingInfo.PathInfo>();

        //	if (info.m_paths.Count() == 2)
        //	{
        //		CheckPedestrianConnections(info);
        //	}

        //	var tinyPaths = info.m_paths.Where(p => Vector3.Distance(p.m_nodes.First(), p.m_nodes.Last()) <= 4).ToList();
        //	var mergePathDict = new Dictionary<Vector3, Vector3>();
        //	if (tinyPaths.Count > 0)
        //	{
        //		foreach (var path in tinyPaths)
        //		{
        //			var average = (path.m_nodes.First() + path.m_nodes.Last()) / 2;

        //			if (!(mergePathDict.ContainsKey(path.m_nodes.First())))
        //			{
        //				mergePathDict.Add(path.m_nodes.First(), average);
        //			}
        //			else if (!mergePathDict.ContainsKey(path.m_nodes.Last()))
        //			{
        //				mergePathDict.Add(path.m_nodes.Last(), mergePathDict[path.m_nodes.First()]);
        //			}
        //			if (!(mergePathDict.ContainsKey(path.m_nodes.Last())))
        //			{
        //				mergePathDict.Add(path.m_nodes.Last(), average);
        //			}
        //			else if (!mergePathDict.ContainsKey(path.m_nodes.First()))
        //			{
        //				mergePathDict.Add(path.m_nodes.First(), mergePathDict[path.m_nodes.Last()]);
        //			}
        //		}
        //	}
        //	foreach (var path in info.m_paths)
        //	{
        //		if (tinyPaths.Contains(path))
        //		{
        //			continue;
        //		}
        //		var moveNodes = path.m_nodes.Where(n => mergePathDict.ContainsKey(n)).ToList();
        //		if (moveNodes.Count() > 0)
        //		{
        //			var lowestHighPathIndex = lowestHighPaths.IndexOf(path);
        //			for (var i = 0; i < moveNodes.Count(); i++)
        //			{
        //				Next.Debug.Log($"Node changed from {moveNodes[i].x},{moveNodes[i].y}, {moveNodes[i].z} to {mergePathDict[moveNodes[i]].x},{mergePathDict[moveNodes[i]].y},{mergePathDict[moveNodes[i]].z}");
        //				if (lowestHighPathIndex > -1)
        //				{
        //					lowestHighPaths[lowestHighPathIndex].m_nodes[i] = mergePathDict[moveNodes[i]];
        //				}
        //				moveNodes[i] = mergePathDict[moveNodes[i]];
        //			}
        //		}

        //		if (AllNodesUnderGround(path))
        //		{
        //			if (path.m_netInfo.IsUndergroundMetroStationTrack())
        //			{
        //				ChangePathRotation(path, pivotPoint, angle);
        //				DipPath(path, offsetDepthDist);
        //			}
        //			else if (!lowestHighPaths.Contains(path))
        //			{
        //				continue;
        //			}
        //		}
        //		updatedPaths.Add(path);
        //	}
        //	info.m_paths = updatedPaths.ToArray();
        //}
        private static Vector3 GetMeanVector(IEnumerable<BuildingInfo.PathInfo> pathList)
        {
            List<Vector3> vectorList = pathList.SelectMany(p => p.m_nodes).ToList();
            return GetMeanVector(vectorList);
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

        private static bool IsPedestrianPath(BuildingInfo.PathInfo path)
        {
            return path.m_netInfo.IsPedestrianNetwork();
        }
        private static void SetCurveTargets(BuildingInfo.PathInfo path)
        {
            if (path.m_nodes.Length < 2)
            {
                return;
            }
            var newCurveTargets = new List<Vector3>();
            if (path.m_curveTargets.Length > 0)
                newCurveTargets.AddRange(path.m_curveTargets);
            else
                newCurveTargets.Add(new Vector3());
            newCurveTargets[0] = GetMiddle(path);
            path.m_curveTargets = newCurveTargets.ToArray();
        }

        private static IEnumerable<BuildingInfo.PathInfo> GenerateSteps(BuildingInfo.PathInfo path, float depth, Vector3? pivotPoint, double angle, bool isBound = true)
        {
            var newPath = path.ShallowClone();
            var pathList = new List<BuildingInfo.PathInfo>();
            Vector3 lastCoords;
            Vector3 currCoords;
            if (AllNodesUnderGround(newPath))
            {
                lastCoords = newPath.m_nodes.OrderBy(n => n.z).FirstOrDefault();
                currCoords = newPath.m_nodes.OrderBy(n => n.z).LastOrDefault();
            }
            else
            {
                lastCoords = newPath.m_nodes.OrderBy(n => n.y).LastOrDefault();
                currCoords = newPath.m_nodes.OrderBy(n => n.y).FirstOrDefault();
            }
            var dir = new Vector3();

            MarkPathGenerated(newPath);
            newPath.AssignNetInfo("Pedestrian Connection Underground");
            var steps = (float)Math.Floor((depth + 4) / 12) * 4;
            if (steps == 0)
            {
                currCoords.y -= depth;
                newPath.m_nodes = new[] { lastCoords, currCoords };
                SetCurveTargets(newPath);
                ChangePathRotation(newPath, pivotPoint, angle);
                pathList.Add(newPath);
            }
            else
            {
                float binder = 8;
                float stepDepth = binder / 2;
                var depthLeft = depth;
                if (isBound)
                {
                    dir.x = Math.Min(currCoords.x - lastCoords.x, binder);
                    dir.z = Math.Min(currCoords.z - lastCoords.z, binder);
                }
                else
                {
                    dir.x = currCoords.x - lastCoords.x;
                    dir.z = currCoords.z - lastCoords.z;
                }
                dir.y = currCoords.y - lastCoords.y;
                for (var j = 0; j < steps; j++)
                {
                    var newZ = currCoords.z + dir.x;
                    var newX = currCoords.x - dir.z;
                    lastCoords = currCoords;
                    currCoords = new Vector3() { x = newX, y = currCoords.y - Math.Max(0, Math.Min(stepDepth, depthLeft)), z = newZ };
                    depthLeft -= stepDepth;
                    var newNodes = new[] { lastCoords, currCoords };
                    newPath = newPath.ShallowClone();
                    newPath.m_nodes = newNodes;
                    SetCurveTargets(newPath);
                    ChangePathRotation(newPath, pivotPoint, angle);
                    pathList.Add(newPath);
                    if (isBound)
                    {
                        dir.x = Math.Min(currCoords.x - lastCoords.x, binder);
                        dir.z = Math.Min(currCoords.z - lastCoords.z, binder);
                    }
                    else
                    {
                        dir.x = currCoords.x - lastCoords.x;
                        dir.z = currCoords.z - lastCoords.z;
                    }
                    dir.y = currCoords.y - lastCoords.y;
                }
            }
            return pathList;
        }

        private static void ChangeStationTrackLength(IList<BuildingInfo.PathInfo> assetPaths, int pathIndex, float newLength, ICollection<int> processedConnectedPaths)
        {
            var path = assetPaths[pathIndex];
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
            var middle = GetMiddle(path);
            var length = 0.0f;
            for (var i = 1; i < path.m_nodes.Length; i++)
            {
                length += Vector3.Distance(path.m_nodes[i], path.m_nodes[i - 1]);
            }
            if (Math.Abs(length) < TOLERANCE)
            {
                return;
            }
            var coefficient = Math.Abs(newLength / length);
            for (var i = 0; i < path.m_nodes.Length; i++)
            {
                path.m_nodes[i] = new Vector3
                {
                    x = middle.x + (path.m_nodes[i].x - middle.x) * coefficient,
                    y = middle.y + (path.m_nodes[i].y - middle.y) * coefficient,
                    z = middle.z + (path.m_nodes[i].z - middle.z) * coefficient,
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

            ChangeConnectedPaths(assetPaths, beginning, newBeginning - beginning, processedConnectedPaths);
            ChangeConnectedPaths(assetPaths, end, newEnd - end, processedConnectedPaths);

        }

        private static Vector3 ChangePathRotation(BuildingInfo.PathInfo path, Vector3? pvtPnt, double angle)
        {
            Vector3 newNode = Vector3.zero;
            if (pvtPnt != null)
            {
                Vector3 pivotPoint = (Vector3)pvtPnt;
                for (var nodeIndex = 0; nodeIndex < path.m_nodes.Count(); nodeIndex++)
                {
                    var oldNode = path.m_nodes[nodeIndex];
                    newNode = new Vector3
                    {
                        x = (float)(pivotPoint.x + (oldNode.x - pivotPoint.x) * Math.Cos(angle) - (oldNode.z - pivotPoint.z) * Math.Sin(angle)),
                        y = oldNode.y,
                        z = (float)(pivotPoint.z + (oldNode.x - pivotPoint.x) * Math.Sin(angle) + (oldNode.z - pivotPoint.z) * Math.Cos(angle))
                    };
                    path.m_nodes[nodeIndex] = newNode;
                }
                for (var curveIndex = 0; curveIndex < path.m_curveTargets.Count(); curveIndex++)
                {
                    var oldCurve = path.m_curveTargets[curveIndex];
                    var newCurve = new Vector3
                    {
                        x = (float)(pivotPoint.x + (oldCurve.x - pivotPoint.x) * Math.Cos(angle) - (oldCurve.z - pivotPoint.z) * Math.Sin(angle)),
                        y = oldCurve.y,
                        z = (float)(pivotPoint.z + (oldCurve.x - pivotPoint.x) * Math.Sin(angle) + (oldCurve.z - pivotPoint.z) * Math.Cos(angle)),
                    };
                    path.m_curveTargets[curveIndex] = newCurve;
                }
            }
            return newNode;
        }
        private static void ChangeConnectedPaths(IList<BuildingInfo.PathInfo> assetPaths, Vector3 nodePoint, Vector3 delta, ICollection<int> processedConnectedPaths)
        {
            for (var pathIndex = 0; pathIndex < assetPaths.Count; pathIndex++)
            {
                var path = assetPaths[pathIndex];
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
                        SetCurveTargets(path);
                    }
                }
                ShiftPath(path, delta);
                processedConnectedPaths.Add(pathIndex);
                var newBeginning = path.m_nodes.First();
                var newEnd = path.m_nodes.Last();

                ChangeConnectedPaths(assetPaths, beginning, newBeginning - beginning, processedConnectedPaths);
                ChangeConnectedPaths(assetPaths, end, newEnd - end, processedConnectedPaths);
            }
        }

        private static bool IsPathGenerated(BuildingInfo.PathInfo path)
        {
            return Math.Abs(path.m_maxSnapDistance - GENERATED_PATH_MARKER) < TOLERANCE;
        }

        private static void MarkPathGenerated(BuildingInfo.PathInfo newPath)
        {
            newPath.m_maxSnapDistance = GENERATED_PATH_MARKER;
        }

        private static BuildingInfo.PathInfo[] GetInterlinkedStationTracks(BuildingInfo info)
        {
            var pairs =
                info.m_paths.Where(p => p.m_netInfo.IsUndergroundMetroStationTrack()).SelectMany(p => p.m_nodes)
                    .GroupBy(n => n)
                    .Where(grp => grp.Count() > 1)
                    .Select(grp => grp.Key)
                    .ToArray();
            return info.m_paths.Where(p => p.m_netInfo.IsUndergroundMetroStationTrack() && p.m_nodes.Any(n => pairs.Contains(n))).ToArray();
        }

        private static Vector3 GetMiddle(BuildingInfo.PathInfo path, float bendStrength = 0)
        {
            var midPoint = (path.m_nodes.First() + path.m_nodes.Last()) / 2;
            var distance = Vector3.Distance(midPoint, path.m_nodes.First());
            var xCoeff = -(path.m_nodes.First().x - path.m_nodes.Last().x) / Vector3.Distance(path.m_nodes.First(), path.m_nodes.Last());
            var zCoeff = (path.m_nodes.First().z - path.m_nodes.Last().z) / Vector3.Distance(path.m_nodes.First(), path.m_nodes.Last());
            var bendCoeff = bendStrength * (Math.Sqrt(2) - 1);
            var adjMidPoint = new Vector3()
            {
                x = midPoint.x + (float)(-zCoeff * distance * bendCoeff),
                y = midPoint.y,
                z = midPoint.z + (float)(xCoeff * distance * bendCoeff)
            };
            return adjMidPoint;
        }

        private static bool AllNodesUnderGround(BuildingInfo.PathInfo path)
        {
            return path.m_nodes.All(n => n.y <= -4);
        }
        private static bool BuildingHasPedestrianConnectionSurface(BuildingInfo info)
        {
            return info.m_paths.Count(p => IsPedestrianPath(p)) >= 1;
        }
        private static List<Vector3> GetIsoltedNodes(BuildingInfo info)
        {
            return info.m_paths
                .Where(p => IsPedestrianPath(p))
                .SelectMany(p => p.m_nodes).GroupBy(x => x)
                .Where(g => g.Count() == 1)
                .Select(y => y.Key)
                .ToList();
        }
        private static List<BuildingInfo.PathInfo> GetIsolatedPaths(BuildingInfo info)
        {
            List<BuildingInfo.PathInfo> isolatedPaths = null;
            var query = GetIsoltedNodes(info);
            if (query.Count() > 0)
            {
                isolatedPaths = info.m_paths.Where(p => p.m_nodes.All(n => query.Contains(n))).ToList();
            }
            return isolatedPaths;
        }
        private static void CheckPedestrianConnections(BuildingInfo info)
        {
            var isolatedPaths = GetIsolatedPaths(info);
            if (isolatedPaths != null)
            {
                var straddlePaths = isolatedPaths.Where(p => p.m_nodes.Any(n => n.y >= 0));
                if (isolatedPaths.Count > 0 && straddlePaths.Count() == 0)
                {
                    var pathList = info.m_paths.ToList();
                    foreach (var path in isolatedPaths)
                    {
                        var newStub = AddStub(path);
                        if (newStub != null)
                        {
                            pathList.Add(newStub);
                        }
                    }
                    info.m_paths = pathList.ToArray();
                }
            }
            //return info.m_paths.Count(p => p.m_netInfo != null && p.m_netInfo.name == "Pedestrian Connection Surface" || p.m_netInfo.name == "Pedestrian Connection Inside");
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

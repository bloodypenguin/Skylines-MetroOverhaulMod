using MetroOverhaul.NEXT.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using MetroOverhaul.Extensions;
using UnityEngine;
using MetroOverhaul.UI;

namespace MetroOverhaul
{
    public static class SetStationCustomizations
    {
        public const int MAX_DEPTH = 36;
        public const int MIN_DEPTH = 12;
        public const int MAX_ANGLE = 360;
        public const int MIN_ANGLE = 0;
        public const int MAX_LENGTH = 144;
        public const int MIN_LENGTH = 88;
        public const int MAX_BEND_STRENGTH = 1;
        public const int MIN_BEND_STRENGTH = -1;

        private static float GENERATED_PATH_MARKER = 0.09999f; //regular paths have snapping distance of 0.1f. This way we differentiate
        private const float TOLERANCE = 0.000001f; //equals 1/10 of difference between 0.1f and GENERATED_PATH_MARKER

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
            var pivotPoint = ChangeStationDepthAndRotation(info, targetDepth, angle);
            if (superInfo != null)
            {
                if (info.m_paths.Any(p => p.m_nodes.Any(n => n.y >= 0)) == false)
                {
                    var highestIsolatedPath = info.m_paths.OrderBy(p => p.m_nodes.Max(n => n.y)).LastOrDefault();
                    Debug.Log($"HIGHESTISOLATEDPATH NODES: {highestIsolatedPath.m_nodes.First().ToString()} and {highestIsolatedPath.m_nodes.Last().ToString()}");
                    if (highestIsolatedPath != null)
                    {
                        var pathList = info.m_paths.ToList();
                        var closestSuperPath = superInfo.m_paths.OrderBy(p => p.m_nodes.Min(n => Vector3.Distance(n, pivotPoint))).FirstOrDefault();
                        var closestSuperNode = closestSuperPath.m_nodes.OrderBy(n => Vector3.Distance(n, pivotPoint)).FirstOrDefault();
                        var aux = ChainPath(closestSuperPath, pivotPoint, Array.IndexOf(closestSuperPath.m_nodes, closestSuperNode));
                        SetCurveTargets(aux);
                        pathList.Add(aux);
                        info.m_paths = pathList.ToArray();
                    }
                }
            }
            ReconfigureStationAccess(info, pivotPoint, bendStrength);
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
        private static void ReconfigureStationAccess(BuildingInfo info, Vector3 pivotPoint, float bendStrength)
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
            var specialNetInfo = PrefabCollection<NetInfo>.FindLoaded("Pedestrian Connection Surface");
            specialNetInfo.m_maxSlope = 100;
            specialNetInfo.m_maxTurnAngle = 180;
            specialNetInfo.m_maxTurnAngleCos = -1;
            for (int i = 0; i < pathList.Count; i++)
            {
                var newPath = pathList.FirstOrDefault(p => p.m_nodes.Any(n => n == pivotPoint)).ShallowClone();
                var trackPath = pathList[i];
                if (trackPath.m_netInfo != null && trackPath.m_netInfo.IsUndergroundMetroStationTrack())
                {
                    if (newPath != null)
                    {
                        var xCoeff = -(trackPath.m_nodes.First().x - trackPath.m_nodes.Last().x) / Vector3.Distance(trackPath.m_nodes.First(), trackPath.m_nodes.Last());
                        var zCoeff = (trackPath.m_nodes.First().z - trackPath.m_nodes.Last().z) / Vector3.Distance(trackPath.m_nodes.First(), trackPath.m_nodes.Last());
                        var stationLength = Vector3.Distance(trackPath.m_nodes.First(), trackPath.m_nodes.Last());
                        var stairsLengthX = ((5f * stationLength / 32) + (0.11f * bendStrength) * (5f * stationLength / 32)) * -xCoeff;
                        var stairsLengthZ = ((5f * stationLength / 32) + (0.11f * bendStrength) * (5f * stationLength / 32)) * zCoeff;
                        newPath.m_netInfo.m_maxSlope = 100;
                        newPath.m_netInfo.m_maxTurnAngle = 180;
                        newPath.m_netInfo.m_maxTurnAngleCos = -1;
                        if (trackPath.m_netInfo.IsUndergroundIslandPlatformStationTrack())
                        {
                            var newNodes = new List<Vector3>();
                            newNodes.Add(new Vector3()
                            {
                                x = trackPath.m_nodes.Last().x,
                                y = trackPath.m_nodes.Last().y + 8,
                                z = trackPath.m_nodes.Last().z
                            });
                            newNodes.Add(new Vector3()
                            {
                                x = newNodes.First().x + stairsLengthX,
                                y = trackPath.m_nodes.Last().y,
                                z = newNodes.First().z + stairsLengthZ,
                            });
                            newPath.m_nodes = newNodes.ToArray();
                            newPath.m_netInfo = specialNetInfo;
                            MarkPathGenerated(newPath);
                            var corridorNodes = new List<Vector3>();
                            corridorNodes.Add(new Vector3()
                            {
                                x = newNodes.First().x,
                                y = trackPath.m_nodes.Last().y + 8,
                                z = newNodes.First().z
                            });
                            corridorNodes.Add(new Vector3()
                            {
                                x = corridorNodes.First().x + (10 * xCoeff),
                                y = trackPath.m_nodes.Last().y + 8,
                                z = corridorNodes.First().z + (10 * zCoeff)
                            });
                            var corridorPath = newPath.ShallowClone();
                            corridorPath.m_nodes = corridorNodes.ToArray();
                            corridorPath.m_netInfo = specialNetInfo;
                            MarkPathGenerated(corridorPath);

                            ChangePathRotation(newPath, newPath.m_nodes.First(), (27f / 35) * bendStrength * -Math.PI / 4);
                            ChangePathRotation(corridorPath, newPath.m_nodes.First(), (27f / 35) * bendStrength * -Math.PI / 4);
                            pathList.Add(newPath);
                            pathList.Add(corridorPath);
                            var connectNodes = new Vector3[] { corridorPath.m_nodes.Last() };
                            if (!connectList.Contains(connectNodes))
                                connectList.Add(connectNodes);
                        }
                        else if (trackPath.m_netInfo.IsUndergroundSmallStationTrack())
                        {
                            var newNodes = new List<Vector3>();
                            newNodes.Add(new Vector3()
                            {
                                x = trackPath.m_nodes.Last().x + 5 * zCoeff,
                                y = trackPath.m_nodes.Last().y + 8,
                                z = trackPath.m_nodes.Last().z + 5 * xCoeff
                            });
                            newNodes.Add(new Vector3()
                            {
                                x = newNodes.First().x + stairsLengthX,
                                y = trackPath.m_nodes.Last().y,
                                z = newNodes.First().z + stairsLengthZ,
                            });
                            newPath.m_nodes = newNodes.ToArray();
                            newPath.m_netInfo = specialNetInfo;
                            MarkPathGenerated(newPath);
                            var corridorNodes = new List<Vector3>();
                            corridorNodes.Add(new Vector3()
                            {
                                x = newNodes.First().x,
                                y = trackPath.m_nodes.Last().y + 8,
                                z = newNodes.First().z
                            });
                            corridorNodes.Add(new Vector3()
                            {
                                x = corridorNodes.First().x + (10 * xCoeff),
                                y = trackPath.m_nodes.Last().y + 8,
                                z = corridorNodes.First().z + (10 * zCoeff)
                            });
                            var corridorPath = newPath.ShallowClone();
                            corridorPath.m_nodes = corridorNodes.ToArray();
                            corridorPath.m_netInfo = specialNetInfo;
                            MarkPathGenerated(corridorPath);

                            ChangePathRotation(newPath, newPath.m_nodes.First(), (27f / 35) * bendStrength * -Math.PI / 4);
                            ChangePathRotation(corridorPath, newPath.m_nodes.First(), (27f / 35) * bendStrength * -Math.PI / 4);
                            pathList.Add(newPath);
                            pathList.Add(corridorPath);
                            var connectNodes = new Vector3[] { corridorPath.m_nodes.Last() };
                            if (!connectList.Contains(connectNodes))
                                connectList.Add(connectNodes);
                        }
                        else if (trackPath.m_netInfo.IsUndergroundSidePlatformMetroStationTrack())
                        {
                            var newNodes = new List<Vector3>();
                            newNodes.Add(new Vector3()
                            {
                                x = trackPath.m_nodes.Last().x - 7 * zCoeff,
                                y = trackPath.m_nodes.Last().y + 8,
                                z = trackPath.m_nodes.Last().z - 7 * xCoeff
                            });
                            newNodes.Add(new Vector3()
                            {
                                x = newNodes.First().x + stairsLengthX,
                                y = trackPath.m_nodes.Last().y,
                                z = newNodes.First().z + stairsLengthZ,
                            });
                            newPath.m_nodes = newNodes.ToArray();
                            newPath.m_netInfo = specialNetInfo;
                            var branchVectorCorridor1 = new Vector3()
                            {
                                x = newNodes.First().x + (10 * xCoeff),
                                y = trackPath.m_nodes.Last().y + 8,
                                z = newNodes.First().z - (10 * zCoeff)
                            };
                            var branchVectorConnect = new Vector3()
                            {
                                x = branchVectorCorridor1.x + 14 * zCoeff,
                                y = trackPath.m_nodes.Last().y + 8,
                                z = branchVectorCorridor1.z + 14 * xCoeff
                            };
                            var branchVectorCorridor2 = new Vector3()
                            {
                                x = branchVectorConnect.x - (10 * xCoeff),
                                y = trackPath.m_nodes.Last().y + 8,
                                z = branchVectorConnect.z + (10 * zCoeff)
                            };
                            var branchVectorStair = new Vector3()
                            {
                                x = branchVectorCorridor2.x + stairsLengthX,
                                y = trackPath.m_nodes.Last().y,
                                z = branchVectorCorridor2.z + stairsLengthZ,
                            };
                            var branchPathCorridor1 = ChainPath(newPath, branchVectorCorridor1, 0);
                            var branchPathConnect = ChainPath(branchPathCorridor1, branchVectorConnect);
                            var branchPathCorridor2 = ChainPath(branchPathConnect, branchVectorCorridor2);
                            var branchPathStair = ChainPath(branchPathCorridor2, branchVectorStair);
                            MarkPathGenerated(newPath);

                            ChangePathRotation(newPath, newPath.m_nodes.First(), (27f / 35) * bendStrength * -Math.PI / 4);
                            ChangePathRotation(branchPathCorridor1, newPath.m_nodes.First(), (27f / 35) * bendStrength * -Math.PI / 4);
                            ChangePathRotation(branchPathConnect, newPath.m_nodes.First(), (27f / 35) * bendStrength * -Math.PI / 4);
                            ChangePathRotation(branchPathCorridor2, newPath.m_nodes.First(), (27f / 35) * bendStrength * -Math.PI / 4);
                            ChangePathRotation(branchPathStair, newPath.m_nodes.First(), (27f / 35) * bendStrength * -Math.PI / 4);
                            pathList.Add(newPath);
                            pathList.Add(branchPathCorridor1);
                            pathList.Add(branchPathConnect);
                            pathList.Add(branchPathCorridor2);
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
            var currentVector = pivotPoint;
            var pool = new List<Vector3[]>();
            pool.AddRange(connectList);
            for (var i = 0; i < connectList.Count; i++)
            {
                var closestVector = pool.SelectMany(n => n).OrderBy(n => (currentVector.x - n.x) + (100 * (pivotPoint.y - n.y)) + (currentVector.z - n.z)).FirstOrDefault();
                var closestPath = pathList.FirstOrDefault(p => p.m_nodes.Any(n => n == closestVector));
                var branch = ChainPath(closestPath, currentVector, Array.IndexOf(closestPath.m_nodes, closestVector));
                pathList.Add(branch);
                var nodeArrayToLose = pool.FirstOrDefault(na => na.Any(n => n == closestVector));
                currentVector = nodeArrayToLose.OrderBy(n => Vector3.Distance(closestVector, n)).LastOrDefault();
                if (nodeArrayToLose != null)
                {
                    pool.Remove(nodeArrayToLose);
                }
            }
            info.m_paths = CleanPaths(pathList).ToArray();
            //pathList = CleanPaths(pathList);
            //var currentVector = Vector3.zero;
            //BuildingInfo.PathInfo currentPath = null;
            //var stationCount = pathList.Where(p => p.m_netInfo != null && p.m_netInfo.IsUndergroundMetroStationTrack()).Count();
            //var pivotPath = pathList.FirstOrDefault(p => p.m_nodes.Any(n => n == pivotPoint));
            //var nearestTrack = pathDict.Keys.OrderBy(n => currentVector.x - n.x + 100 * (pivotPoint.y - n.y) + currentVector.z - n.z).FirstOrDefault();
            //var nearestPed = pathDict[nearestTrack].OrderBy(n => Vector3.Distance(pivotPoint, n)).FirstOrDefault();
            //if (vectorRotateDict.ContainsKey(nearestPed))
            //{
            //	nearestPed = vectorRotateDict[nearestPed];
            //}
            //         pathList.Add(ChainPath(pivotPath, nearestPed, Array.IndexOf(pivotPath.m_nodes, pivotPoint), PrefabCollection<NetInfo>.FindLoaded("Pedestrian Connection Surface")));
            //for (var i = 0; i < stationCount; i++)
            //{
            //	if (pathDict.Keys.Count > 0)
            //	{
            //		if (i == 0)
            //		{
            //                     var furthestTrackVector = pathDict.Keys.OrderBy(n => Vector3.Distance(pivotPoint, n)).LastOrDefault();
            //			var furthestPedVector = pathDict[furthestTrackVector].OrderBy(n => Vector3.Distance(pivotPoint, n)).LastOrDefault();
            //			var otherPedVector = furthestPedVector;
            //			if (pathDict[furthestTrackVector].Count > 1)
            //				otherPedVector = pathDict[furthestTrackVector].OrderBy(n => Vector3.Distance(pivotPoint, n)).FirstOrDefault();
            //			currentPath = pathList.FirstOrDefault(p => p.m_nodes.Any(n => n == otherPedVector));
            //			if (pathDict.ContainsKey(furthestTrackVector))
            //			{
            //				pathDict.Remove(furthestTrackVector);
            //			}
            //			currentVector = otherPedVector;
            //		}
            //		else
            //		{
            //                     var nearestTrackVector = pathDict.Keys.OrderBy(n => currentVector.x - n.x + 100 * (currentVector.y - n.y) + currentVector.z - n.z).FirstOrDefault();
            //			var nearestPedVector = pathDict[nearestTrackVector].OrderBy(n => Vector3.Distance(currentVector, n)).FirstOrDefault();
            //			var otherPedVector = nearestPedVector;
            //			if (pathDict[nearestTrackVector].Count > 1)
            //				otherPedVector = pathDict[nearestTrackVector].OrderBy(n => Vector3.Distance(currentVector, n)).LastOrDefault();
            //			currentPath = pathList.FirstOrDefault(p => p.m_nodes.Any(n => n == currentVector));
            //			var chainIndex = Array.IndexOf(currentPath.m_nodes, currentVector);
            //			var longPath = ChainPath(currentPath, nearestPedVector, chainIndex);
            //			if (vectorRotateDict.ContainsKey(nearestPedVector))
            //			{
            //				nearestPedVector = vectorRotateDict[nearestPedVector];
            //			}
            //			pathList.Add(longPath);

            //			if (pathDict.ContainsKey(nearestTrackVector))
            //			{
            //				pathDict.Remove(nearestTrackVector);
            //			}
            //			currentVector = otherPedVector;
            //		}
            //	}
            //}
            //         info.m_paths = CleanPaths(pathList).ToArray();
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

        private static BuildingInfo.PathInfo ChainPath(BuildingInfo.PathInfo startPath, Vector3 endNode, int startNodeIndex = -1, NetInfo info = null)
        {
            var newNodes = new List<Vector3>();
            var newPath = startPath.ShallowClone();
            if (startNodeIndex == -1 || startNodeIndex >= startPath.m_nodes.Count())
            {
                startNodeIndex = startPath.m_nodes.Count() - 1;
            }
            var startNode = startPath.m_nodes[startNodeIndex];
            newNodes.Add(startNode);
            newNodes.Add(endNode);

            newPath.m_nodes = newNodes.ToArray();
            if (info == null)
            {
                info = PrefabCollection<NetInfo>.FindLoaded("Pedestrian Connection Surface");
            }
            newPath.m_netInfo = info;
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
                for (var j=0; j < stairPaths.Count(); j++)
                {
                    BendStationTrack(stairPaths[j], -bendStrength * (5f / 32f));
                }
            }
        }
        //private static void BendStationTrack(BuildingInfo.PathInfo[] assetPaths, int pathIndex, float targetDepth, List<int> processedConnectedPaths)
        //{
        //var path = assetPaths[pathIndex];
        //if (path.m_netInfo == null || !path.m_netInfo.IsUndergroundMetroStationTrack())
        //{
        //    return;
        //}
        //if (path.m_nodes.Length < 2)
        //{
        //    return;
        //}
        //var beginning = path.m_nodes.First();
        //var end = path.m_nodes.Last();
        //path.m_nodes[0] = new Vector3
        //{
        //    x = path.m_nodes[0].x,
        //    y = -MIN_DEPTH,
        //    z = path.m_nodes[0].z,
        //};
        //path.m_nodes[path.m_nodes.Length - 1] = new Vector3
        //{
        //    x = path.m_nodes[path.m_nodes.Length - 1].x,
        //    y = -MIN_DEPTH,
        //    z = path.m_nodes[path.m_nodes.Length - 1].z,
        //};
        //var newBeginning = path.m_nodes.First();
        //var newEnd = path.m_nodes.Last();

        //ChangeConnectedPaths(assetPaths, beginning, newBeginning - beginning, processedConnectedPaths);
        //ChangeConnectedPaths(assetPaths, end, newEnd - end, processedConnectedPaths);
        //}

        private static void CleanUpPaths(BuildingInfo info)
        {
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
        private static Vector3 ChangeStationDepthAndRotation(BuildingInfo info, float targetDepth, double angle)
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
            var lowestHighPaths = info.m_paths.Where(p => p.m_nodes.Any(n => n.y >= 0) && p.m_nodes.Any(nd => nd.y < 0)).ToList();
            if (lowestHighPaths.Count == 0)
            {
                lowestHighPaths.Add(info.m_paths.Where(p => p.m_netInfo.name == "Pedestrian Connection Surface" || p.m_netInfo.name == "Pedestrian Connection Inside")
                    .OrderByDescending(p => p.m_nodes[0].y)
                    .FirstOrDefault());
            }
            if (lowestHighPaths.Count > 0)
            {
                for (var i = 0; i < lowestHighPaths.Count(); i++)
                {
                    var path = lowestHighPaths[i];
                    var currCoords = path.m_nodes.OrderBy(n => n.z).LastOrDefault();
                    totalNode += currCoords;
                }
                totalNode /= lowestHighPaths.Count();

                for (var i = 0; i < lowestHighPaths.Count; i++)
                {
                    var path = lowestHighPaths[i];
                    var currCoords = path.m_nodes.OrderBy(n => n.z).LastOrDefault();
                    var newPath = path.ShallowClone();
                    newPath.m_netInfo = PrefabCollection<NetInfo>.FindLoaded("Pedestrian Connection Underground");
                    var newNodes = new List<Vector3>();
                    newNodes.Add(currCoords);
                    newNodes.Add(totalNode);
                    newPath.m_nodes = newNodes.ToArray();
                    SetCurveTargets(newPath);
                    MarkPathGenerated(newPath);
                    pathList.Add(newPath);

                }
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
                            ChangePathRotation(path, totalNode, angle);
                            DipPath(path, offsetDepthDist);
                        }
                    }
                    pathList.Add(path);
                }
                info.m_paths = pathList.ToArray();
            }
            return totalNode;
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
            newPath.m_netInfo = PrefabCollection<NetInfo>.FindLoaded("Pedestrian Connection Underground");
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
                            y = -MIN_DEPTH,
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
            return path.m_nodes.All(n => n.y < 0);
        }
        private static bool BuildingHasPedestrianConnectionSurface(BuildingInfo info)
        {
            return info.m_paths.Count(p => p.m_netInfo != null && p.m_netInfo.name == "Pedestrian Connection Surface" || p.m_netInfo.name == "Pedestrian Connection Inside") >= 1;
        }
        private static List<Vector3> GetIsoltedNodes(BuildingInfo info)
        {
            return info.m_paths
                .Where(p => p.m_netInfo != null && p.m_netInfo.name == "Pedestrian Connection Surface" || p.m_netInfo.name == "Pedestrian Connection Inside")
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

using MetroOverhaul.NEXT.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using MetroOverhaul.Extensions;
using UnityEngine;

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

        private static float GENERATED_PATH_MARKER = 0.09999f; //regular paths have snapping distance of 0.1f. This way we differentiate
        private const float TOLERANCE = 0.000001f; //equals 1/10 of difference between 0.1f and GENERATED_PATH_MARKER

        public static void ModifyStation(BuildingInfo info, float targetDepth, float targetStationTrackLength, double angle)
        {
            if (!info.HasUndergroundMetroTracks() || info.m_paths == null || info.m_paths.Length < 1)
            {
                return;
            }
            if (targetDepth <= 0 || targetStationTrackLength <= 0)
            {
                return;
            }
            CleanUpPaths(info);
            ResizeUndergroundStationTracks(info, targetStationTrackLength);
            if (BuildingHasPedestrianConnectionSurface(info))
            {
                CheckPedestrianConnections(info);
                ChangeStationDepthAndRotation(info, targetDepth, angle);
            }
            else
            {
                BendStationTracks(info, targetDepth); //works for European Main Station!
            }

            RecalculateSpawnPoints(info);
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
                BendStationTrack(info.m_paths, index, targetDepth, processedConnectedPaths);
            }
        }

        private static void BendStationTrack(BuildingInfo.PathInfo[] assetPaths, int pathIndex, float targetDepth, List<int> processedConnectedPaths)
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
            path.m_nodes[0] = new Vector3
            {
                x = path.m_nodes[0].x,
                y = -MIN_DEPTH,
                z = path.m_nodes[0].z,
            };
            path.m_nodes[path.m_nodes.Length - 1] = new Vector3
            {
                x = path.m_nodes[path.m_nodes.Length - 1].x,
                y = -MIN_DEPTH,
                z = path.m_nodes[path.m_nodes.Length - 1].z,
            };
            var newBeginning = path.m_nodes.First();
            var newEnd = path.m_nodes.Last();

            ChangeConnectedPaths(assetPaths, beginning, newBeginning - beginning, processedConnectedPaths);
            ChangeConnectedPaths(assetPaths, end, newEnd - end, processedConnectedPaths);
        }

        private static void CleanUpPaths(BuildingInfo info)
        {
            info.m_paths = info.m_paths.Where(p => !IsPathGenerated(p)).ToArray();
            foreach (var path in info.m_paths)
            {
                path.m_forbidLaneConnection = null;
            }
        }

        public static void RecalculateSpawnPoints(BuildingInfo info)
        {
            var buildingAI = info?.GetComponent<TransportStationAI>();
            var paths = info?.m_paths;
            if (buildingAI == null || paths == null || buildingAI.m_transportLineInfo == null)
            {
                return;
            }
            var allSpawnPoints = (from path in paths
                    where IsVehicleStop(path)
                    select new KeyValuePair<Vector3, ItemClass>(GetMiddle(path), path.m_netInfo.m_class)).Distinct()
                .ToArray();
            Vector3[] primarySpawnPoints;
            Vector3[] secondarySpawnPoints;

            if (buildingAI.m_secondaryTransportInfo == null)
            {
                primarySpawnPoints = allSpawnPoints.Select(kvp => kvp.Key).ToArray();
                secondarySpawnPoints = new Vector3[] { };
            }
            else
            {
                primarySpawnPoints = allSpawnPoints
                    .Where(kvp => kvp.Value.m_subService == buildingAI.m_transportInfo.m_class.m_subService ||
                                  kvp.Value.m_subService == ItemClass.SubService.None &&
                                  kvp.Value.m_service == ItemClass.Service.Road &&
                                  buildingAI.m_transportInfo.m_class.m_subService ==
                                  ItemClass.SubService.PublicTransportBus)
                    .Select(kvp => kvp.Key)
                    .ToArray();
                secondarySpawnPoints = allSpawnPoints
                    .Where(kvp => kvp.Value.m_subService == buildingAI.m_secondaryTransportInfo.m_class.m_subService ||
                                  kvp.Value.m_subService == ItemClass.SubService.None &&
                                  kvp.Value.m_service == ItemClass.Service.Road &&
                                  buildingAI.m_secondaryTransportInfo.m_class.m_subService ==
                                  ItemClass.SubService.PublicTransportBus)
                    .Select(kvp => kvp.Key)
                    .ToArray();
            }


            if (!(buildingAI.m_transportLineInfo.m_class.m_service == ItemClass.Service.PublicTransport &&
                  (buildingAI.m_transportLineInfo.m_class.m_subService == ItemClass.SubService.PublicTransportShip ||
                   buildingAI.m_transportLineInfo.m_class.m_subService == ItemClass.SubService.PublicTransportPlane) &&
                  buildingAI.m_transportLineInfo.m_class.m_level == ItemClass.Level.Level2))
            {
                switch (primarySpawnPoints.Length)
                {
                    case 0:
                        buildingAI.m_spawnPosition = Vector3.zero;
                        buildingAI.m_spawnTarget = Vector3.zero;
                        buildingAI.m_spawnPoints = new DepotAI.SpawnPoint[] { };
                        break;
                    case 1:
                        buildingAI.m_spawnPosition = primarySpawnPoints[0];
                        buildingAI.m_spawnTarget = primarySpawnPoints[0];
                        buildingAI.m_spawnPoints = new[]
                        {
                            new DepotAI.SpawnPoint
                            {
                                m_position = primarySpawnPoints[0],
                                m_target = primarySpawnPoints[0]
                            }
                        };
                        break;
                    default:
                        buildingAI.m_spawnPosition = Vector3.zero;
                        buildingAI.m_spawnTarget = Vector3.zero;
                        buildingAI.m_spawnPoints = primarySpawnPoints.Select(p => new DepotAI.SpawnPoint
                            {
                                m_position = p,
                                m_target = p
                            })
                            .ToArray();
                        break;
                }
            }

            if (buildingAI.m_secondaryTransportInfo != null &&
                !(buildingAI.m_secondaryTransportInfo.m_class.m_service == ItemClass.Service.PublicTransport &&
                  (buildingAI.m_secondaryTransportInfo.m_class.m_subService == ItemClass.SubService.PublicTransportShip ||
                   buildingAI.m_secondaryTransportInfo.m_class.m_subService == ItemClass.SubService.PublicTransportPlane) &&
                  buildingAI.m_secondaryTransportInfo.m_class.m_level == ItemClass.Level.Level2))
            {
                switch (secondarySpawnPoints.Length)
                {
                    case 0:
                        buildingAI.m_spawnPosition2 = Vector3.zero;
                        buildingAI.m_spawnTarget2 = Vector3.zero;
                        buildingAI.m_spawnPoints2 = new DepotAI.SpawnPoint[] { };
                        break;
                    case 1:
                        buildingAI.m_spawnPosition2 = secondarySpawnPoints[0];
                        buildingAI.m_spawnTarget2 = secondarySpawnPoints[0];
                        buildingAI.m_spawnPoints2 = new[]
                        {
                            new DepotAI.SpawnPoint
                            {
                                m_position = secondarySpawnPoints[0],
                                m_target = secondarySpawnPoints[0]
                            }
                        };
                        break;
                    default:
                        buildingAI.m_spawnPosition2 = Vector3.zero;
                        buildingAI.m_spawnTarget2 = Vector3.zero;
                        buildingAI.m_spawnPoints2 = secondarySpawnPoints.Select(p => new DepotAI.SpawnPoint
                            {
                                m_position = p,
                                m_target = p
                            })
                            .ToArray();
                        break;
                }
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

        private static void ChangeStationDepthAndRotation(BuildingInfo info, float targetDepth, double angle)
        {
            var highestLow = float.MinValue;
            var highestLowStation = float.MinValue;
            foreach (var path in info.m_paths)
            {

                if (path.m_netInfo?.m_netAI == null || !path.m_nodes.All(n => n.y < 0))
                {
                    continue;
                }
                var highestNode = path.m_nodes.OrderBy(n => n.y).LastOrDefault().y;
                if (path.m_netInfo.IsUndergroundMetroStationTrack())
                {
                    highestLowStation = Math.Max(highestNode, highestLowStation);
                }
                else
                {
                    highestLow = Math.Max(highestNode, highestLow);
                }
            }
            var offsetDepthDist = targetDepth + highestLowStation;
            float lowestHigh = 0;
            var lowestHighPath =
                info.m_paths.FirstOrDefault(p => p.m_nodes.Any(n => n.y >= 0) && p.m_nodes.Any(nd => nd.y < 0)) ??
                info.m_paths.Where(p => p.m_netInfo.name == "Pedestrian Connection Surface" || p.m_netInfo.name == "Pedestrian Connection Inside")
                    .OrderByDescending(p => p.m_nodes[0].y)
                    .FirstOrDefault();
            //TODO(earalov): What if author used "Pedestrian Connection" instead of "Pedestrian Connection Surface"?
            if (lowestHighPath != null)
            {
                lowestHigh = lowestHighPath.m_nodes.OrderBy(n => n.y).FirstOrDefault().y;
            } //TODO(earalov): properly handle integrated metro station (it has no own networks)
            var stepDepthDist = targetDepth + lowestHigh;
            var updatedPaths = new List<BuildingInfo.PathInfo>();
            var pivotPoint = lowestHighPath.m_nodes.Last();
            foreach (var path in info.m_paths)
            {
                if (path == lowestHighPath)
                {
                    updatedPaths.AddRange(GenerateSteps(lowestHighPath, stepDepthDist, pivotPoint, angle));
                }
                else if (AllNodesUnderGround(path))
                {
                    ChangePathRotation(path, pivotPoint, angle);
                    DipPath(path, offsetDepthDist);
                }
                updatedPaths.Add(path);
            }
            info.m_paths = updatedPaths.ToArray();
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

        private static IEnumerable<BuildingInfo.PathInfo> GenerateSteps(BuildingInfo.PathInfo path, float depth, Vector3 pivotPoint, double angle)
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
            var stepDepth = 4;
            var depthLeft = depth;

            dir.x = Math.Min(currCoords.x - lastCoords.x, 8);
            dir.y = currCoords.y - lastCoords.y;
            dir.z = Math.Min(currCoords.z - lastCoords.z, 8);
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
                dir.x = Math.Min(currCoords.x - lastCoords.x, 8);
                dir.y = currCoords.y - lastCoords.y;
                dir.z = Math.Min(currCoords.z - lastCoords.z, 8);
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

        private static void ChangePathRotation(BuildingInfo.PathInfo path, Vector3 pivotPoint, double angle)
        {
            for (var nodeIndex = 0; nodeIndex < path.m_nodes.Count(); nodeIndex++)
            {
                var oldNode = path.m_nodes[nodeIndex];
                var newNode = new Vector3
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

        private static Vector3 GetMiddle(BuildingInfo.PathInfo path)
        {
            return (path.m_nodes.First() + path.m_nodes.Last()) / 2;
        }

        private static bool AllNodesUnderGround(BuildingInfo.PathInfo path)
        {
            return path.m_nodes.All(n => n.y < 0);
        }
        private static bool BuildingHasPedestrianConnectionSurface(BuildingInfo info)
        {
            return info.m_paths.Count(p => p.m_netInfo != null && p.m_netInfo.name == "Pedestrian Connection Surface" || p.m_netInfo.name == "Pedestrian Connection Inside") >= 1;
        }
        private static void CheckPedestrianConnections(BuildingInfo info)
        {
            var query = info.m_paths
                .Where(p => p.m_netInfo != null && p.m_netInfo.name == "Pedestrian Connection Surface" || p.m_netInfo.name == "Pedestrian Connection Inside")
                .SelectMany(p => p.m_nodes).GroupBy(x => x)
                  .Where(g => g.Count() == 1)
                  .Select(y => y.Key)
                  .ToList();
            if (query.Count() > 0)
            {
                var isolatedPaths = info.m_paths.Where(p => p.m_nodes.All(n => query.Contains(n))).ToList();
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

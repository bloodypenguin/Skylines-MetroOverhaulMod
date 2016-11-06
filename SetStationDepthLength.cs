using MetroOverhaul.NEXT.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetroOverhaul.Extensions;
using UnityEngine;

namespace MetroOverhaul
{
    public static class SetStationDepthLength
    {
        private static float GENERATED_PATH_MARKER = 0.09999f; //regular paths have snapping distance of 0.1f. This way we differentiate
        private const float TOLERANCE = 0.000001f; //equals 1/10 of difference between 0.1f and GENERATED_PATH_MARKER

        public static void ModifyStation(BuildingInfo info, float targetDepth, float targetStationTrackLength)
        {
            if (!info.IsUndergroundMetroStation() || info.m_paths == null || info.m_paths.Length < 1)
            {
                return;
            }
            if (targetDepth <= 0 || targetStationTrackLength <= 0)
            {
                return;
            }
            info.m_paths = info.m_paths.Where(p => !IsPathGenerated(p)).ToArray();

            var allowChangeDepth =
                info.m_paths.Count(p => p.m_netInfo != null && p.m_netInfo.name.Contains("Pedestrian Connection")) > 0 &&
                info.m_paths.Count(p => p.m_netInfo != null && p.m_netInfo.IsUndergroundMetroStationTrack()) == 1;

            if (allowChangeDepth)
            {
                var buildingAI = info.GetComponent<DepotAI>();
                if (buildingAI != null)
                {
                    //TODO(earalov): add support for multi track stations (they have multiple spawn points). Also note that different tracks may have different initial depth
                    buildingAI.m_spawnPosition = new Vector3(buildingAI.m_spawnPosition.x, -targetDepth, buildingAI.m_spawnPosition.z);
                    buildingAI.m_spawnTarget = new Vector3(buildingAI.m_spawnPosition.x, -targetDepth, buildingAI.m_spawnPosition.z);
                }
            }


            float lowestHigh = 0;
            var lowestHighPath = info.m_paths.FirstOrDefault(p => p.m_nodes.Any(n => n.y >= 0) && p.m_nodes.Any(nd => nd.y < 0)) ??
                                 info.m_paths.Where(p => p.m_netInfo.name == "Pedestrian Connection Surface").OrderByDescending(p => p.m_nodes[0].y).FirstOrDefault(); //TODO(earalov): What if author used "Pedestrian Connection" instead of "Pedestrian Connection Surface"?
            if (lowestHighPath != null)
            {
                lowestHigh = lowestHighPath.m_nodes.OrderBy(n => n.y).FirstOrDefault().y;
            } //TODO(earalov): properly handle integrated metro station (it has no own networks)

            var linkedStationTracks = GetInterlinkedStationTracks(info);

            var highestLow = float.MinValue;
            var highestLowStation = float.MinValue;
            var processedConnectedPaths = new List<int>();
            for (var index = 0; index < info.m_paths.Length; index++)
            {
                var path = info.m_paths[index];
                path.m_forbidLaneConnection = null;
                if (!path.m_nodes.All(n => n.y < 0))
                {
                    continue;
                }
                var highestNode = path.m_nodes.OrderBy(n => n.y).LastOrDefault().y;
                if (path.m_netInfo.IsUndergroundMetroStationTrack())
                {
                    highestLowStation = Math.Max(highestNode, highestLowStation);
                    if (!linkedStationTracks.Contains(path))
                    {
                        ChangeStationTrackLength(info.m_paths, index, targetStationTrackLength, processedConnectedPaths);
                    }
                }
                else
                {
                    highestLow = Math.Max(highestNode, highestLow);
                }
            }

            if (!allowChangeDepth)
            {
                return;
            }
            var offsetDepthDist = targetDepth + highestLowStation;
            var stepDepthDist = targetDepth + lowestHigh;
            var updatedPaths = new List<BuildingInfo.PathInfo>();
            foreach (var path in info.m_paths)
            {
                if (path.m_nodes.All(n => n.y < 0) && path != lowestHighPath)
                {
                    DipPath(path, offsetDepthDist);
                }
                else
                {
                    updatedPaths.AddRange(GenerateSteps(lowestHighPath, stepDepthDist));
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
            var newCurveTargets = path.m_curveTargets.Length > 0 ? path.m_curveTargets : new[] { Vector3.zero };
            newCurveTargets[0] = (path.m_nodes.First() + path.m_nodes.Last()) / 2; //TODO(earalov): Is this approrriate when path has multiple curve targets?
            path.m_curveTargets = newCurveTargets;
        }

        private static IEnumerable<BuildingInfo.PathInfo> GenerateSteps(BuildingInfo.PathInfo path, float depth)
        {
            var pathList = new List<BuildingInfo.PathInfo>();
            var lastCoords = path.m_nodes.OrderBy(n => n.z).FirstOrDefault();
            var currCoords = path.m_nodes.OrderBy(n => n.z).LastOrDefault();
            var dir = new Vector3();
            var newPath = path.ShallowClone();
            MarkPathGenerated(newPath);
            newPath.m_netInfo = PrefabCollection<NetInfo>.FindLoaded("Pedestrian Connection Underground");
            var steps = (float)Math.Floor(depth / 4) * 2;
            var stepDepth = depth / steps;

            dir.x = Math.Min(currCoords.x - lastCoords.x, 8);
            dir.y = currCoords.y - lastCoords.y;
            dir.z = Math.Min(currCoords.z - lastCoords.z, 8);
            for (var j = 0; j < steps; j++)
            {
                var newZ = currCoords.z + dir.x;
                var newX = currCoords.x - dir.z;
                lastCoords = currCoords;
                currCoords = new Vector3() { x = newX, y = currCoords.y - stepDepth, z = newZ };
                var newNodes = new[] { lastCoords, currCoords };
                newPath = newPath.ShallowClone();
                newPath.m_nodes = newNodes;
                SetCurveTargets(newPath);
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
            if (path.m_netInfo == null || !path.m_netInfo.IsUndergroundMetroStationTrack() || processedConnectedPaths.Contains(pathIndex))
            {
                return;
            }
            if (path.m_nodes.Length < 2)
            {
                return;
            }
            var beginning = path.m_nodes.First();
            var end = path.m_nodes.Last();
            var middle = (beginning + end) / 2;
            if (path.m_curveTargets.Length > 1 || (path.m_curveTargets.Length == 1 && Vector3.Distance(middle, path.m_curveTargets.FirstOrDefault()) > 0.1))
            {
                return;
            }
            var totalX = Math.Abs(beginning.x - end.x);
            var totalZ = Math.Abs(beginning.z - end.z);
            var originalLength = (float)Math.Sqrt(Math.Pow(totalX, 2) + Math.Pow(totalZ, 2));
            var scalingCoefficient = newLength / originalLength;

            for (var i = 0; i < path.m_nodes.Length; i++)
            {
                var multiplierX = path.m_nodes[i].x / Mathf.Abs(path.m_nodes[i].x);
                var multiplierZ = path.m_nodes[i].z / Mathf.Abs(path.m_nodes[i].z);
                path.m_nodes[i] = new Vector3
                {
                    x = path.m_nodes[i].x + (0.5f * multiplierX * (scalingCoefficient - 1) * totalX),
                    y = path.m_nodes[i].y,
                    z = path.m_nodes[i].z + (0.5f * multiplierZ * (scalingCoefficient - 1) * totalZ)
                };
            }
            SetCurveTargets(path);
            var newBeginning = path.m_nodes.First();
            var newEnd = path.m_nodes.Last();
            ChangeConnectedPathsLength(assetPaths, beginning, newBeginning - beginning, processedConnectedPaths);
            ChangeConnectedPathsLength(assetPaths, end, newEnd - end, processedConnectedPaths);

        }

        private static void ChangeConnectedPathsLength(IList<BuildingInfo.PathInfo> assetPaths, Vector3 nodePoint, Vector3 delta, ICollection<int> processedPaths)
        {
            for (var pathIndex = 0; pathIndex < assetPaths.Count; pathIndex++)
            {
                var path = assetPaths[pathIndex];
                if (path.m_netInfo == null || path.m_netInfo.IsUndergroundMetroStationTrack())
                {
                    processedPaths.Add(pathIndex);
                    continue;
                }
                if (processedPaths.Contains(pathIndex) || !path.m_nodes.Where(n => n == nodePoint).Any())
                {
                    continue;
                }
                var beginning = path.m_nodes.First();
                var end = path.m_nodes.Last();
                ShiftPath(path, delta);
                processedPaths.Add(pathIndex);
                var newBeginning = path.m_nodes.First();
                var newEnd = path.m_nodes.Last();
                ChangeConnectedPathsLength(assetPaths, beginning, newBeginning - beginning, processedPaths);
                ChangeConnectedPathsLength(assetPaths, end, newEnd - end, processedPaths);
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
                info.m_paths.SelectMany(p => p.m_nodes)
                    .GroupBy(n => n)
                    .Where(grp => grp.Count() > 1)
                    .Select(grp => grp.Key)
                    .ToArray(); //revisit
            return info.m_paths.Where(p => p.m_nodes.Any(n => pairs.Contains(n)) && p.m_netInfo.IsUndergroundMetroStationTrack())
                    .ToArray();
        }
    }
}

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
            ResizeUndergroundStationTracks(info, targetStationTrackLength);
            var allowChangeDepth =
                info.m_paths.Count(p => p.m_netInfo != null && p.m_netInfo.name.Contains("Pedestrian Connection")) > 0 &&
                info.m_paths.Count(p => p.m_netInfo != null && p.m_netInfo.IsUndergroundMetroStationTrack()) == 1; //TODO(earalov): don't allow to change depth if no ped. paths above metro tracks
            if (!allowChangeDepth)
            {
                return;
            }
            ChangeStationDepth(info, targetDepth);
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

        private static void ChangeStationDepth(BuildingInfo info, float targetDepth)
        {
            var highestLow = float.MinValue;
            var highestLowStation = float.MinValue;
            foreach (var path in info.m_paths)
            {
                path.m_forbidLaneConnection = null; //TODO(earalov): what is this for?
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
            var buildingAI = info.GetComponent<DepotAI>();
            if (buildingAI != null)
            {
                //TODO(earalov): add support for multi track stations (they have multiple spawn points). Also note that different tracks may have different initial depth
                buildingAI.m_spawnPosition = new Vector3(buildingAI.m_spawnPosition.x, -targetDepth,
                    buildingAI.m_spawnPosition.z);
                buildingAI.m_spawnTarget = new Vector3(buildingAI.m_spawnPosition.x, -targetDepth, buildingAI.m_spawnPosition.z);
            }

            var offsetDepthDist = targetDepth + highestLowStation;

            float lowestHigh = 0;
            var lowestHighPath =
                info.m_paths.FirstOrDefault(p => p.m_nodes.Any(n => n.y >= 0) && p.m_nodes.Any(nd => nd.y < 0)) ??
                info.m_paths.Where(p => p.m_netInfo.name == "Pedestrian Connection Surface")
                    .OrderByDescending(p => p.m_nodes[0].y)
                    .FirstOrDefault();
                //TODO(earalov): What if author used "Pedestrian Connection" instead of "Pedestrian Connection Surface"?
            if (lowestHighPath != null)
            {
                lowestHigh = lowestHighPath.m_nodes.OrderBy(n => n.y).FirstOrDefault().y;
            } //TODO(earalov): properly handle integrated metro station (it has no own networks)
            var stepDepthDist = targetDepth + lowestHigh;
            var updatedPaths = new List<BuildingInfo.PathInfo>();
            foreach (var path in info.m_paths)
            {
                if (AllNodesUnderGround(path) && path != lowestHighPath)
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
            var newCurveTargets = new List<Vector3>();
            if (path.m_curveTargets.Length > 0)
                newCurveTargets.AddRange(path.m_curveTargets);
            else
                newCurveTargets.Add(new Vector3());
            newCurveTargets[0] = GetMiddle(path);
            path.m_curveTargets = newCurveTargets.ToArray();
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
            if (path.m_curveTargets.Length > 1 || (path.m_curveTargets.Length == 1 && Vector3.Distance(middle, path.m_curveTargets.FirstOrDefault()) > 0.1))
            {
                UnityEngine.Debug.LogError("path " + pathIndex + " not resized! Too many curve points or a curve point is offset too much. Type: " + path.m_netInfo.name);
                return;
            }
            for (var i = 0; i < path.m_nodes.Length; i++)
            {
                var toStart = Vector3.Distance(path.m_nodes[i], middle);
                path.m_nodes[i] = new Vector3
                {
                    x = middle.x + newLength * 0.5f * (path.m_nodes[i].x - middle.x) / toStart,
                    y = middle.y + newLength * 0.5f * (path.m_nodes[i].y - middle.y) / toStart,
                    z = middle.z + newLength * 0.5f * (path.m_nodes[i].z - middle.z) / toStart
                };
            }
            UnityEngine.Debug.LogError("path " + pathIndex + " resized! Type: "+path.m_netInfo.name);
            SetCurveTargets(path);
            var newBeginning = path.m_nodes.First();
            var newEnd = path.m_nodes.Last();
            ChangeConnectedPaths(assetPaths, beginning, newBeginning - beginning, processedConnectedPaths);
            ChangeConnectedPaths(assetPaths, end, newEnd - end, processedConnectedPaths);

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
                ShiftPath(path, delta);
                processedConnectedPaths.Add(pathIndex);
                if (path.m_netInfo.m_netAI.IsUnderground())
                {
                    for (var i = 0; i < path.m_nodes.Length; i++)
                    {
                        if (!(path.m_nodes[i].y > -12f) || path.m_nodes[i] == nodePoint)
                        {
                            continue;
                        }
                        path.m_nodes[i] = new Vector3
                        {
                            x = path.m_nodes[i].x,
                            y = -12f,
                            z = path.m_nodes[i].z
                        };
                        SetCurveTargets(path);
                    }
                }
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
            return info.m_paths.Where(p => p.m_netInfo.IsUndergroundMetroStationTrack() &&  p.m_nodes.Any(n => pairs.Contains(n))).ToArray();
        }

        private static Vector3 GetMiddle(BuildingInfo.PathInfo path)
        {
            return (path.m_nodes.First() + path.m_nodes.Last()) / 2;
        }

        private static bool AllNodesUnderGround(BuildingInfo.PathInfo path)
        {
            return path.m_nodes.All(n => n.y < 0);
        }
    }
}

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
            var pathList1 = new List<BuildingInfo.PathInfo>();  //TODO(earalov): give meaningful name!
            foreach (var path in info.m_paths)
            {
                path.m_forbidLaneConnection = null;
                if (path.m_nodes.All(n => n.y < 0))
                {
                    var highestNode = path.m_nodes.OrderBy(n => n.y).LastOrDefault().y;
                    if (path.m_netInfo.IsUndergroundMetroStationTrack())
                    {
                        highestLowStation = Math.Max(highestNode, highestLowStation);
                        if (!linkedStationTracks.Contains(path))
                        {
                            GenStationTrack(path, targetStationTrackLength);
                        }
                    }
                    else if (IsPathGenerated(path))
                    {
                        highestLow = Math.Max(highestNode, highestLow);
                    }
                }
                pathList1.Add(path);
            }
            pathList1.RemoveAll(IsPathGenerated);

            if (!allowChangeDepth)
            {
                info.m_paths = pathList1.ToArray();
                return;
            }
            var offsetDepthDist = targetDepth + highestLowStation;
            var stepDepthDist = targetDepth + lowestHigh;
            var pathList2 = new List<BuildingInfo.PathInfo>(); //TODO(earalov): give meaningful name!
            foreach (var path in pathList1)
            {
                if (path.m_nodes.All(n => n.y < 0) && path != lowestHighPath)
                {
                    DipPath(path, offsetDepthDist);
                    if (!path.m_netInfo.IsUndergroundMetroStationTrack())
                    {
                        GenPathCurve(path);
                    }
                }
                else
                {
                    pathList2.AddRange(lowestHighPath.GenSteps(stepDepthDist));
                }
                pathList2.Add(path);
            }
            info.m_paths = pathList2.ToArray();

        }

        private static void DipPath(BuildingInfo.PathInfo path, float depthOffsetDist)
        {
            for (var i = 0; i < path.m_nodes.Length; i++)
            {
                path.m_nodes[i] = new Vector3 { x = path.m_nodes[i].x, y = path.m_nodes[i].y - depthOffsetDist, z = path.m_nodes[i].z };
            }
        }

        //TODO(earalov): curveOff is unused. Looks suspicious. Are we able to handle curved paths at all?
        private static void GenPathCurve(BuildingInfo.PathInfo path, float curveOff = 0)
        {
            if (path.m_nodes.Length <= 1)
            {
                return;
            }
            var newCurveTargets = new List<Vector3>();
            if (path.m_curveTargets.Length > 0)
                newCurveTargets.AddRange(path.m_curveTargets);
            else
                newCurveTargets.Add(new Vector3());

            newCurveTargets[0] = (path.m_nodes.First() + path.m_nodes.Last()) / 2;

            path.m_curveTargets = newCurveTargets.ToArray();
        }

        private static IEnumerable<BuildingInfo.PathInfo> GenSteps(this BuildingInfo.PathInfo path, float depth)
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
                GenPathCurve(newPath);
                pathList.Add(newPath);
                dir.x = Math.Min(currCoords.x - lastCoords.x, 8);
                dir.y = currCoords.y - lastCoords.y;
                dir.z = Math.Min(currCoords.z - lastCoords.z, 8);
            }
            return pathList;
        }

        private static void GenStationTrack(BuildingInfo.PathInfo path, float length)
        {
            var totalX = Math.Abs(path.m_nodes.First().x - path.m_nodes.Last().x);
            var totalZ = Math.Abs(path.m_nodes.First().z - path.m_nodes.Last().z);
            var trackDistance = (float)Math.Pow((Math.Pow(totalX, 2) + Math.Pow(totalZ, 2)), 0.5);

            var curveIsOriginal = path.m_curveTargets.FirstOrDefault() == (path.m_nodes.First() + path.m_nodes.Last()) / 2;
            //TODO(earalov): the following variable is unused. Looks suspicious
            var curveIsRightAngle = (Math.Abs(path.m_nodes.First().x - path.m_curveTargets.FirstOrDefault().x) < TOLERANCE && Math.Abs(path.m_nodes.Last().z - path.m_curveTargets.FirstOrDefault().z) < TOLERANCE)
                                || (Math.Abs(path.m_nodes.First().z - path.m_curveTargets.FirstOrDefault().z) < TOLERANCE && Math.Abs(path.m_nodes.Last().x - path.m_curveTargets.FirstOrDefault().x) < TOLERANCE);

            var offCoeff = length / trackDistance;

            if (!curveIsOriginal)
            {
                return;
            }
            for (var i = 0; i < path.m_nodes.Length; i++)
            {
                var multiplierX = path.m_nodes[i].x / Mathf.Abs(path.m_nodes[i].x);
                var multiplierZ = path.m_nodes[i].z / Mathf.Abs(path.m_nodes[i].z);
                path.m_nodes[i] = new Vector3() { x = path.m_nodes[i].x + (0.5f * multiplierX * (offCoeff - 1) * totalX), y = path.m_nodes[i].y, z = path.m_nodes[i].z + (0.5f * multiplierZ * (offCoeff - 1) * totalZ) };
            }
            GenPathCurve(path);
        }

        private static bool IsPathGenerated(BuildingInfo.PathInfo path)
        {
            return Math.Abs(path.m_maxSnapDistance - GENERATED_PATH_MARKER) > TOLERANCE;
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

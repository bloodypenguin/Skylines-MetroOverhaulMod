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
        private const float TOLERANCE = 0.0001f;

        public static void SetStation(this BuildingInfo info, float stationDepthDist, float stationLengthDist)
        {
            if (!info.IsUndergroundMetroStation())
            {
                return;
            }
            if (!(stationDepthDist > 0) || !(stationLengthDist > 0))
            {
                return;
            }
            var pathList1 = new List<BuildingInfo.PathInfo>();
            var pathList2 = new List<BuildingInfo.PathInfo>();
            var pairs = new List<Vector3>();
            var buildingAI = info.GetComponent<DepotAI>();
            if (buildingAI != null)
            {
                buildingAI.m_spawnPosition.y = -stationDepthDist;
                buildingAI.m_spawnTarget.y = -stationDepthDist;
            }
            var pathList0 = info.m_paths.ToList();
            pairs.AddRange(pathList0.SelectMany(p => p.m_nodes).GroupBy(n => n).Where(grp => grp.Count() > 1).Select(grp => grp.Key).ToList()); //revisit
            var linkedStationTracks = pathList0.Where(p => p.m_nodes.Any(n => pairs.Contains(n)) && p.m_netInfo.IsUndergroundMetroStationTrack()).ToList();
            float lowestHigh = 0;
            var lowestHighPath = pathList0.FirstOrDefault(p => p.m_nodes.Any(n => n.y >= 0) && p.m_nodes.Any(nd => nd.y < 0)) ??
                                 pathList0.Where(p => p.m_netInfo.name == "Pedestrian Connection Surface").OrderByDescending(p => p.m_nodes[0].y).FirstOrDefault(); //TODO(earalov): What if author used "Pedestrian Connection" instead of "Pedestrian Connection Surface"?
            if (lowestHighPath != null)
            {
                lowestHigh = lowestHighPath.m_nodes.OrderBy(n => n.y).FirstOrDefault().y;
            } //TODO(earalov): properly handle integrated metro station (it has no own networks)
            var highestLow = float.MinValue;
            var highestLowStation = float.MinValue;
            for (var i = 0; i < pathList0.Count(); i++)
            {
                var thePath = pathList0[i];
                thePath.m_forbidLaneConnection = null;
                if (thePath.m_nodes.All(n => n.y < 0))
                {
                    var highestNode = thePath.m_nodes.OrderBy(n => n.y).LastOrDefault().y;
                    if (thePath.m_netInfo.IsUndergroundMetroStationTrack())
                    {
                        highestLowStation = Math.Max(highestNode, highestLowStation);
                        thePath.GenStationTrack(linkedStationTracks, stationLengthDist);
                    }
                    else if (Math.Abs(thePath.m_maxSnapDistance - 1f) > TOLERANCE)
                    {
                        highestLow = Math.Max(highestNode, highestLow);
                    }
                }
                pathList1.Add(thePath);
            }
            var offsetDepthDist = stationDepthDist + highestLowStation;
            var stepDepthDist = stationDepthDist + lowestHigh;
            pathList1.RemoveAll(p => Math.Abs(p.m_maxSnapDistance - 1f) < TOLERANCE);
            for (var i = 0; i < pathList1.Count(); i++)
            {
                var thePath = pathList1[i];
                if (thePath.m_nodes.All(n => n.y < 0) && thePath != lowestHighPath)
                {
                    thePath.DipPath(offsetDepthDist);
                    if (!thePath.m_netInfo.IsUndergroundMetroStationTrack())
                    {
                        thePath.GenPathCurve();
                    }
                }
                else
                {
                    pathList2.AddRange(lowestHighPath.GenSteps(stepDepthDist));
                }
                pathList2.Add(thePath);
            }
            info.m_paths = pathList2.ToArray();
        }
        private static void DipPath(this BuildingInfo.PathInfo thepath, float depthOffsetDist)
        {
            for (var i = 0; i < thepath.m_nodes.Length; i++)
            {
                thepath.m_nodes[i] = new Vector3() { x = thepath.m_nodes[i].x, y = thepath.m_nodes[i].y - depthOffsetDist, z = thepath.m_nodes[i].z };
            }
        }

        private static void GenPathCurve(this BuildingInfo.PathInfo thepath, float curveOff = 0)
        {
            if (thepath.m_nodes.Length <= 1)
            {
                return;
            }
            var newCurveTargets = new List<Vector3>();
            if (thepath.m_curveTargets.Count() > 0)
                newCurveTargets.AddRange(thepath.m_curveTargets);
            else
                newCurveTargets.Add(new Vector3());

            newCurveTargets[0] = (thepath.m_nodes.First() + thepath.m_nodes.Last()) / 2;

            thepath.m_curveTargets = newCurveTargets.ToArray();
        }

        private static IEnumerable<BuildingInfo.PathInfo> GenSteps(this BuildingInfo.PathInfo path, float depth)
        {
            var pathList = new List<BuildingInfo.PathInfo>();
            var lastCoords = path.m_nodes.OrderBy(n => n.z).FirstOrDefault();
            var currCoords = path.m_nodes.OrderBy(n => n.z).LastOrDefault(); ;
            var dir = new Vector3();
            var newPath = path.ShallowClone();
            newPath.m_maxSnapDistance = .09999f;
            newPath.m_netInfo = PrefabCollection<NetInfo>.FindLoaded("Pedestrian Connection Underground");
            var steps = (float)Math.Floor(depth / 4) * 2;
            var stepDepth = depth / steps;
            const int multiplier = -1;
            dir.x = Math.Min(currCoords.x - lastCoords.x, 8);
            dir.y = currCoords.y - lastCoords.y;
            dir.z = Math.Min(currCoords.z - lastCoords.z, 8);
            var newZ = 0.0f; //currCoords.z + dir.x; //TODO(earalov): those can be initialized in the innter loop!
            var newX = 0.0f;// currCoords.x - dir.z;
            for (var j = 0; j < steps; j++)
            {
                //multiplier *= -1;
                newZ = currCoords.z + dir.x;//
                newX = currCoords.x - dir.z;//
                lastCoords = currCoords;
                currCoords = new Vector3() { x = newX, y = currCoords.y - stepDepth, z = newZ };
                var newNodes = new[] { lastCoords, currCoords };
                newPath = newPath.ShallowClone();
                newPath.m_nodes = newNodes;
                newPath.GenPathCurve();
                pathList.Add(newPath);
                dir.x = Math.Min(currCoords.x - lastCoords.x, 8);
                dir.y = currCoords.y - lastCoords.y;
                dir.z = Math.Min(currCoords.z - lastCoords.z, 8);
                newX = currCoords.x + (dir.x * multiplier); //TODO(earalov): useless initializations!
                newZ = currCoords.z + (dir.z * multiplier);
            }
            return pathList;
        }

        private static void GenStationTrack(this BuildingInfo.PathInfo path, ICollection<BuildingInfo.PathInfo> linkedStationTracks, float length)
        {
            if (linkedStationTracks.Contains(path))
            {
                return;
            }
            var totalX = (float)Math.Abs(path.m_nodes.First().x - path.m_nodes.Last().x);
            var totalZ = (float)Math.Abs(path.m_nodes.First().z - path.m_nodes.Last().z);
            var trackDistance = (float)Math.Pow((Math.Pow(totalX, 2) + Math.Pow(totalZ, 2)), 0.5);
            var curveIsOriginal = false;
            var curveIsRightAngle = false;
            curveIsOriginal = path.m_curveTargets.FirstOrDefault() == (path.m_nodes.First() + path.m_nodes.Last()) / 2;
            curveIsRightAngle = (path.m_nodes.First().x == path.m_curveTargets.FirstOrDefault().x && path.m_nodes.Last().z == path.m_curveTargets.FirstOrDefault().z)
                                || (path.m_nodes.First().z == path.m_curveTargets.FirstOrDefault().z && path.m_nodes.Last().x == path.m_curveTargets.FirstOrDefault().x);
            var offCoeff = length / trackDistance;

            //float param = 0;
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
            path.GenPathCurve();
        }
    }
}

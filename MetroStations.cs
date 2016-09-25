using MetroOverhaul.NEXT.Extensions;
using SubwayOverhaul.NEXT.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MetroOverhaul
{
    public static class MetroStations
    {
        public static void UpdateMetro(float stationDepthDist = 12, float stationLengthDist = 144)
        {
            var vanillaMetroTrack = PrefabCollection<NetInfo>.FindLoaded("Metro Track");
            vanillaMetroTrack.m_buildHeight = -12;
            var vanillaMetroStation = PrefabCollection<BuildingInfo>.FindLoaded("Metro Entrance");
            var vanillaMetroStationTrack = PrefabCollection<NetInfo>.FindLoaded("Metro Station Track");
            vanillaMetroStationTrack.m_buildHeight = -12;
            foreach (var info in Resources.FindObjectsOfTypeAll<BuildingInfo>())
            {
                if (!(info.m_buildingAI is DepotAI) || info.m_class.m_subService != ItemClass.SubService.PublicTransportMetro)
                {
                    continue;
                }

                if (info.m_buildingAI.GetType() != typeof(DepotAI))
                {
                    var transportStationAi = ((TransportStationAI)info.m_buildingAI);
                    transportStationAi.m_maxVehicleCount = 0;
                }

                info.m_UnlockMilestone = vanillaMetroStation.m_UnlockMilestone;
                ((DepotAI)info.m_buildingAI).m_createPassMilestone = ((DepotAI)vanillaMetroStation.m_buildingAI).m_createPassMilestone;
                if (info.m_paths == null)
                {
                    continue;
                }
                var metroStations = info.m_paths.Where(p => p.m_netInfo.name == "Metro Station Track");
                if (metroStations.Count() == 0)
                {
                    continue;
                }
                if (stationDepthDist > 0 && stationLengthDist > 0)
                {
                    var pathList0 = new List<BuildingInfo.PathInfo>();
                    var pathList1 = new List<BuildingInfo.PathInfo>();
                    var pathList2 = new List<BuildingInfo.PathInfo>();
                    var linkedStationTracks = new List<BuildingInfo.PathInfo>();
                    var pairs = new List<Vector3>();

                    pathList0 = info.m_paths.ToList();
                    pairs.AddRange(pathList0.SelectMany(p => p.m_nodes).GroupBy(n => n).Where(grp => grp.Count() > 1).Select(grp => grp.Key).ToList()); //revisit
                    linkedStationTracks = pathList0.Where(p => p.m_nodes.Any(n => pairs.Contains(n)) && p.m_netInfo.name == "Metro Station Track").ToList();
                    float lowestHigh = 0;
                    var lowestHighPath = pathList0.FirstOrDefault(p => p.m_nodes.Any(n => n.y >= 0) && p.m_nodes.Any(nd => nd.y < 0));
                    if (lowestHighPath == null)
                    {
                        lowestHighPath = pathList0.Where(p => p.m_netInfo.name == "Pedestrian Connection Surface").OrderByDescending(p => p.m_nodes[0].y).FirstOrDefault();
                    }
                    lowestHigh = lowestHighPath.m_nodes.OrderBy(n => n.y).FirstOrDefault().y;
                    float highestLow = float.MinValue;
                    float highestLowStation = float.MinValue;
                    for (int i = 0; i < pathList0.Count(); i++)
                    {
                        var thePath = pathList0[i];
                        pathList1.Add(thePath);
                        if (thePath.m_nodes.All(n => n.y < 0))
                        {
                            var highestNode = thePath.m_nodes.OrderBy(n => n.y).LastOrDefault().y;
                            if (thePath.m_netInfo.name == "Metro Station Track")
                            {
                                highestLowStation = Math.Max(highestNode, highestLowStation);
                                thePath.GenStationTrack(linkedStationTracks, stationLengthDist);
                            }
                            else if (thePath.m_maxSnapDistance != 0.09999f)
                            {
                                highestLow = Math.Max(highestNode, highestLow);
                            }
                        }
                    }
                    var offsetDepthDist = stationDepthDist + highestLowStation;
                    var stepDepthDist = stationDepthDist + lowestHigh;
                    pathList1.RemoveAll(p => p.m_maxSnapDistance == 0.09999f);
                    for (int i = 0; i < pathList1.Count(); i++)
                    {
                        var thePath = pathList1[i];
                        if (thePath.m_nodes.All(n => n.y < 0) && thePath != lowestHighPath)
                        {
                            thePath.DipPath(offsetDepthDist);
                            thePath.GenPathCurve();
                        }
                        else
                        {
                            pathList2.AddRange(lowestHighPath.GenSteps(stepDepthDist));
                        }
                        pathList2.Add(thePath);
                    }

                    info.m_paths = pathList2.ToArray();
                }
            }
        }

        private static void DipPath(this BuildingInfo.PathInfo thepath, float depthOffsetDist)
        {
            for (int i = 0; i < thepath.m_nodes.Length; i++)
            {
                thepath.m_nodes[i] = new Vector3() { x = thepath.m_nodes[i].x, y = thepath.m_nodes[i].y - depthOffsetDist, z = thepath.m_nodes[i].z };
            }
            thepath.GenPathCurve();
        }

        private static void GenPathCurve(this BuildingInfo.PathInfo thepath, float curveOff = 0)
        {
            if (thepath.m_curveTargets.Count() > 0 && thepath.m_nodes.Length > 1)
            {
                var newCurveTargets = new List<Vector3>();
                newCurveTargets.AddRange(thepath.m_curveTargets);
                if (curveOff == 0)
                {
                    newCurveTargets[0] = (thepath.m_nodes.First() + thepath.m_nodes.Last()) / 2;
                }
                else
                {
                    var multiplierX = (-1 * newCurveTargets[0].x) / Math.Abs(newCurveTargets[0].x);
                    var multiplierZ = (-1 * newCurveTargets[0].z) / Math.Abs(newCurveTargets[0].z);
                    var valY = (thepath.m_nodes.First().y + thepath.m_nodes.Last().y) / 2;
                    newCurveTargets[0] = new Vector3() { x = 0, y = valY, z = 0 };
                }
                thepath.m_curveTargets = newCurveTargets.ToArray();
            }
        }

        private static List<BuildingInfo.PathInfo> GenSteps(this BuildingInfo.PathInfo path, float depth)
        {
            var pathList = new List<BuildingInfo.PathInfo>();
            var lastCoords = path.m_nodes.First();
            var currCoords = path.m_nodes.Last();
            var dir = new Vector3();
            var newPath = path.ShallowClone();
            newPath.m_maxSnapDistance = .09999f;
            Vector3[] newNodes = { };
            float steps = 4 + (4 * (float)(Math.Round((depth - 4) / 4)));
            float stepDepth = depth / steps;
            for (var j = 0; j < steps; j++)
            {
                dir = currCoords - lastCoords;
                var newX = 0.0f;
                var newZ = 0.0f;
                newZ = currCoords.z + dir.x;
                newX = currCoords.x - dir.z;
                lastCoords = currCoords;
                currCoords = new Vector3() { x = newX, y = currCoords.y - stepDepth, z = newZ };
                newNodes = new[] { lastCoords, currCoords };
                newPath = newPath.ShallowClone();
                newPath.m_nodes = newNodes;
                newPath.GenPathCurve();
                pathList.Add(newPath);
            }
            return pathList;
        }

        private static void GenStationTrack(this BuildingInfo.PathInfo path, List<BuildingInfo.PathInfo> linkedStationTracks, float length)
        {
            if (!linkedStationTracks.Contains(path))
            {
                var totalX = (float)Math.Abs(path.m_nodes.First().x - path.m_nodes.Last().x);
                var totalZ = (float)Math.Abs(path.m_nodes.First().z - path.m_nodes.Last().z);
                var trackDistance = (float)Math.Pow((Math.Pow(totalX, 2) + Math.Pow(totalZ, 2)), 0.5);
                var curveIsOriginal = false;
                var curveIsRightAngle = false;
                curveIsOriginal = path.m_curveTargets.FirstOrDefault() == (path.m_nodes.First() + path.m_nodes.Last()) / 2;
                curveIsRightAngle = (path.m_nodes.First().x == path.m_curveTargets.FirstOrDefault().x && path.m_nodes.Last().z == path.m_curveTargets.FirstOrDefault().z)
                    || (path.m_nodes.First().z == path.m_curveTargets.FirstOrDefault().z && path.m_nodes.Last().x == path.m_curveTargets.FirstOrDefault().x);
                float offCoeff = length / trackDistance;
                var offset = length - trackDistance;

                float param = 0;
                if (curveIsOriginal)
                {
                    for (var i = 0; i < path.m_nodes.Length; i++)
                    {
                        var multiplierX = path.m_nodes[i].x / Mathf.Abs(path.m_nodes[i].x);
                        var multiplierZ = path.m_nodes[i].z / Mathf.Abs(path.m_nodes[i].z);
                        path.m_nodes[i] = new Vector3() { x = path.m_nodes[i].x + (0.5f * multiplierX * (offCoeff - 1) * totalX), y = path.m_nodes[i].y, z = path.m_nodes[i].z + (0.5f * multiplierZ * (offCoeff - 1) * totalZ) };
                    }
                }
                else if (curveIsRightAngle)
                {
                    for (var i = 0; i < path.m_nodes.Length; i++)
                    {
                        var valX = path.m_nodes[i].x;
                        var valZ = path.m_nodes[i].z;
                        var multiplierX = path.m_nodes[i].x / Mathf.Abs(path.m_nodes[i].x);
                        var multiplierZ = path.m_nodes[i].z / Mathf.Abs(path.m_nodes[i].z);
                        if (valX == path.m_curveTargets.FirstOrDefault().x)
                            valZ = path.m_nodes[i].z + (0.5f * multiplierZ * (offCoeff - 1) * totalZ);
                        else
                            valX = path.m_nodes[i].x + (0.5f * multiplierX * (offCoeff - 1) * totalX);
                        path.m_nodes[i] = new Vector3() { x = valX, y = path.m_nodes[i].y, z = valZ };
                    }
                    param = offCoeff;
                }
            }
        }
    }
}
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
        public static void UpdateMetro()
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
                    //if (info.name == "Metro Entrance")
                    //{

                    //}
                    //info.SetMeshes
                    //    (@"Meshes\Tunnel_Station_Cover.obj");
                }

                info.m_UnlockMilestone = vanillaMetroStation.m_UnlockMilestone;
                ((DepotAI)info.m_buildingAI).m_createPassMilestone = ((DepotAI)vanillaMetroStation.m_buildingAI).m_createPassMilestone;
                if (info.m_paths == null)
                {
                    continue;
                }
                //var processedPaths = new List<int>();

                //for (var index = 0; index < info.m_paths.Length; index++)
                //{
                //    var path = info.m_paths[index];
                //    if (path.m_netInfo != null)
                //    {
                //        if (path.m_netInfo.name.Contains("Pedestrian"))
                //        {
                //            var lowestNode = info.m_paths[index].m_nodes.OrderBy(n => n.y).FirstOrDefault();
                //            var nodeIndex = Array.IndexOf(info.m_paths[index].m_nodes, lowestNode);
                //            var step1 = new Vector3() { x = lowestNode.x, y = lowestNode.y - 4, z = lowestNode.z - 10 };
                //            var step2 = new Vector3() { x = step1.x, y = step1.y - 4, z = step1.z + 10 };
                //            var theNodes = new List<Vector3>();
                //            theNodes.AddRange(info.m_paths[index].m_nodes);
                //            theNodes.Add(step1);
                //            theNodes.Add(step2);
                //            info.m_paths[index].m_nodes = theNodes.ToArray();

                //        }
                //        else if (path.m_netInfo.name == "Metro Station Track")
                //        {
                //            //path.m_netInfo = PrefabCollection<NetInfo>.FindLoaded("Metro Station Track Tunnel");
                //            for (var i = 0; i < path.m_nodes.Length; i++)
                //            {
                //                var multiplier = path.m_nodes[i].x / Mathf.Abs(path.m_nodes[i].x);
                //                path.m_nodes[i] = new Vector3() { x = 72 * multiplier, y = path.m_nodes[i].y - 8, z = path.m_nodes[i].z };
                //            }
                //        }
                //    }
                //}
            }
        }
    }
}
//            processedPaths.Add(index);

//            var start = path.m_nodes[0];
//            var end = path.m_nodes[1];

//            if (!(Vector3.Distance(start, end) < 144.0f))
//            {
//                UnityEngine.Debug.Log(
//                    $"Station {info.name}: can't make longer tracks: track is already long enough");
//                continue;
//            }
//            var middle = new Vector3
//            {
//                x = (start.x + end.x) / 2.0f,
//                y = (start.y + end.y) / 2.0f,
//                z = (start.z + end.z) / 2.0f
//            };
//            if (path.m_curveTargets.Length > 1 ||
//                (path.m_curveTargets.Length == 1 && Vector3.Distance(middle, path.m_curveTargets[0]) > 0.1))
//            {
//                UnityEngine.Debug.Log(
//                    $"Station {info.name}: can't make longer tracks: unprocessable curve points");
//                continue;
//            }

//            var toStart = Vector3.Distance(start, middle);
//            var newStart = new Vector3
//            {
//                x = middle.x + 72.0f * (start.x - middle.x) / toStart,
//                y = middle.y + 72.0f * (start.y - middle.y) / toStart,
//                z = middle.z + 72.0f * (start.z - middle.z) / toStart
//            };
//            path.m_nodes[0] = newStart;
//            ProcessConnectedPaths(info.m_paths, start, newStart - start, processedPaths);


//            var toEnd = Vector3.Distance(end, middle);
//            var newEnd = new Vector3
//            {
//                x = middle.x - 72.0f * (middle.x - end.x) / toEnd,
//                y = middle.y - 72.0f * (middle.y - end.y) / toEnd,
//                z = middle.z - 72.0f * (middle.z - end.z) / toEnd
//            };
//            path.m_nodes[1] = newEnd;
//            ProcessConnectedPaths(info.m_paths, end, newEnd - end, processedPaths);
//        }
//    }
//}

//private static void ProcessConnectedPaths(IList<BuildingInfo.PathInfo> mPaths, Vector3 nodePoint, Vector3 delta, List<int> processedPaths)
//{
//    for (var index = 0; index < mPaths.Count; index++)
//    {
//        var path = mPaths[index];
//        if (path.m_netInfo == null || path.m_netInfo.name == "Metro Station Track" || processedPaths.Contains(index))
//        {
//            continue;
//        }
//        if (!path.m_nodes.Where(n => n == nodePoint).Any())
//        {
//            continue;
//        }
//        processedPaths.Add(index);
//        for (var i = 0; i < path.m_nodes.Length; i++)
//        {
//            path.m_nodes[i] = path.m_nodes[i] + delta;
//        }
//        for (var i = 0; i < path.m_curveTargets.Length; i++)
//        {
//            path.m_curveTargets[i] = path.m_curveTargets[i] + delta;
//        }
//    }
//}
// }
//}
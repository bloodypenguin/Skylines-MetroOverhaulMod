using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MetroOverhaul
{
    public static class MetroStations
    {
        public static void UpdateMetroStations()
        {
            foreach (var info in Resources.FindObjectsOfTypeAll<BuildingInfo>())
            {
                if (!(info.m_buildingAI is DepotAI))
                {
                    continue;
                }

                if (info.m_buildingAI.GetType() != typeof(DepotAI) && info.m_class.m_subService == ItemClass.SubService.PublicTransportMetro)
                {
                    var transportStationAi = ((TransportStationAI)info.m_buildingAI);
                    transportStationAi.m_maxVehicleCount = 0;
                }
                if (info.m_paths == null)
                {
                    continue;
                }
                var processedPaths = new List<int>();
                for (var index = 0; index < info.m_paths.Length; index++)
                {
                    var path = info.m_paths[index];
                    if (path.m_netInfo == null || path.m_netInfo.name != "Metro Station Track")
                    {
                        continue;
                    }
                    processedPaths.Add(index);

                    var start = path.m_nodes[0];
                    var end = path.m_nodes[1];

                    if (!(Vector3.Distance(start, end) < 144.0f))
                    {
                        UnityEngine.Debug.Log(
                            $"Station {info.name}: can't make longer tracks: track is already long enough");
                        continue;
                    }
                    var middle = new Vector3
                    {
                        x = (start.x + end.x) / 2.0f,
                        y = (start.y + end.y) / 2.0f,
                        z = (start.z + end.z) / 2.0f
                    };
                    if (path.m_curveTargets.Length > 1 ||
                        (path.m_curveTargets.Length == 1 && Vector3.Distance(middle, path.m_curveTargets[0]) > 0.1))
                    {
                        UnityEngine.Debug.Log(
                            $"Station {info.name}: can't make longer tracks: unprocessable curve points");
                        continue;
                    }

                    var toStart = Vector3.Distance(start, middle);
                    var newStart = new Vector3
                    {
                        x = middle.x + 72.0f * (start.x - middle.x) / toStart,
                        y = middle.y + 72.0f * (start.y - middle.y) / toStart,
                        z = middle.z + 72.0f * (start.z - middle.z) / toStart
                    };
                    path.m_nodes[0] = newStart;
                    ProcessConnectedPaths(info.m_paths, start, newStart - start, processedPaths);


                    var toEnd = Vector3.Distance(end, middle);
                    var newEnd = new Vector3
                    {
                        x = middle.x - 72.0f * (middle.x - end.x) / toEnd,
                        y = middle.y - 72.0f * (middle.y - end.y) / toEnd,
                        z = middle.z - 72.0f * (middle.z - end.z) / toEnd
                    };
                    path.m_nodes[1] = newEnd;
                    ProcessConnectedPaths(info.m_paths, end, newEnd - end, processedPaths);
                }
            }
        }

        private static void ProcessConnectedPaths(IList<BuildingInfo.PathInfo> mPaths, Vector3 nodePoint, Vector3 delta, List<int> processedPaths)
        {
            for (var index = 0; index < mPaths.Count; index++)
            {
                var path = mPaths[index];
                if (path.m_netInfo == null || path.m_netInfo.name == "Metro Station Track" || processedPaths.Contains(index))
                {
                    continue;
                }
                if (!path.m_nodes.Where(n => n == nodePoint).Any())
                {
                    continue;
                }
                processedPaths.Add(index);
                for (var i = 0; i < path.m_nodes.Length; i++)
                {
                    path.m_nodes[i] = path.m_nodes[i] + delta;
                }
                for (var i = 0; i < path.m_curveTargets.Length; i++)
                {
                    path.m_curveTargets[i] = path.m_curveTargets[i] + delta;
                }
            }
        }
    }
}
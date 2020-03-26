using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MetroOverhaul.Extensions
{
    public static partial class BuildingInfoExtensions
    {
        private const float GENERATED_PATH_MARKER = 0.09999f; //regular paths have snapping distance of 0.1f. This way we differentiate
        public const float AUTOGEN_TOLERANCE = 0.000001f; //equals 1/10 of difference between 0.1f and GENERATED_PATH_MARKER
        public static void AssignNetInfo(this BuildingInfo.PathInfo path, string netInfoName, bool includeNetInfo = true)
        {
            NetInfo netInfo = PrefabCollection<NetInfo>.FindLoaded(netInfoName);

            if (netInfo == null)
            {
                UnityEngine.Debug.Log("Cannot find NetInfo " + netInfoName);
            }
            else
            {
                path.AssignNetInfo(netInfo, includeNetInfo);
            }
        }
        public static void AssignNetInfo(this BuildingInfo.PathInfo path, NetInfo netInfo, bool includeNetInfo = true)
        {
            if (includeNetInfo)
            {
                path.m_netInfo = netInfo;
            }
            path.m_finalNetInfo = netInfo;
        }
        public static void InvertPath(this BuildingInfo.PathInfo path)
        {
            Array.Reverse(path.m_nodes);
            Array.Reverse(path.m_curveTargets);
        }
        public static bool IsPedestrianPath(this BuildingInfo.PathInfo path)
        {
            return path.m_netInfo.IsPedestrianNetwork();
        }
        public static void SetCurveTargets(this BuildingInfo.PathInfo path)
        {
            if (path.m_nodes.Length != 2 || (!IsPathGenerated(path) && (path.m_curveTargets == null || path.m_curveTargets.Length == 0)))
            {
                return;
            }
            var newCurveTargets = new List<Vector3>();
            if (path.m_curveTargets != null && path.m_curveTargets.Length > 0)
                newCurveTargets.AddRange(path.m_curveTargets);
            else
                newCurveTargets.Add(new Vector3());
            newCurveTargets[0] = GetMiddle(path, 0);
            path.m_curveTargets = newCurveTargets.ToArray();
        }
        public static bool IsPathGenerated(this BuildingInfo.PathInfo path)
        {
            return Math.Abs(path.m_maxSnapDistance - GENERATED_PATH_MARKER) < AUTOGEN_TOLERANCE;
        }

        public static void MarkPathGenerated(this BuildingInfo.PathInfo newPath)
        {
            newPath.m_maxSnapDistance = GENERATED_PATH_MARKER;
        }
        public static Vector3 GetMiddle(this BuildingInfo.PathInfo path, float curve = 0)
        {
            var midPoint = (path.m_nodes.First() + path.m_nodes.Last()) / 2;
            var distance = Vector3.Distance(midPoint, path.m_nodes.First());
            var xCoeff = -(path.m_nodes.First().x - path.m_nodes.Last().x) / Vector3.Distance(path.m_nodes.First(), path.m_nodes.Last());
            var zCoeff = (path.m_nodes.First().z - path.m_nodes.Last().z) / Vector3.Distance(path.m_nodes.First(), path.m_nodes.Last());
            var curveCoeff = curve * (Math.Sqrt(2) - 1);
            var adjMidPoint = new Vector3()
            {
                x = midPoint.x + (float)(-zCoeff * distance * curveCoeff),
                y = midPoint.y,
                z = midPoint.z + (float)(xCoeff * distance * curveCoeff)
            };
            return adjMidPoint;
        }
        public static BuildingInfo.PathInfo Clone(this BuildingInfo.PathInfo path)
        {
            BuildingInfo.PathInfo clonedPath = new BuildingInfo.PathInfo()
            {
                m_curveTargets = path.m_curveTargets != null ? new FastDeepCloner(path.m_curveTargets).Clone<Vector3[]>() : null,
                m_finalNetInfo = path.m_finalNetInfo,
                m_forbidLaneConnection = path.m_forbidLaneConnection != null ? new FastDeepCloner(path.m_forbidLaneConnection).Clone<bool[]>() : null,
                m_invertSegments = path.m_invertSegments,
                m_maxSnapDistance = path.m_maxSnapDistance,
                m_netInfo = path.m_netInfo,
                m_nodes = path.m_nodes != null ? new FastDeepCloner(path.m_nodes).Clone<Vector3[]>() : null,
                m_trafficLights = path.m_trafficLights,
                m_yieldSigns = path.m_yieldSigns != null ? new FastDeepCloner(path.m_yieldSigns).Clone<bool[]>() : null
            };
            return clonedPath;
        }
    }
}

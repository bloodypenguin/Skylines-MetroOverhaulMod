using System;

namespace MetroOverhaul.Extensions
{
    public static partial class BuildingInfoExtensions
    {
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
    }
}

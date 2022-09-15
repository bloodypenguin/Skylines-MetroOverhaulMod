using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ColossalFramework.Plugins;
using ICities;
using UnityEngine;
using Object = UnityEngine.Object;
using static ColossalFramework.Plugins.PluginManager;
using ColossalFramework.PlatformServices;
using System.IO;
using ColossalFramework.UI;
using MetroOverhaul.UI;
using MetroOverhaul.NEXT;
using static ToolBase;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace MetroOverhaul {
    public static class Util {
        private static int count = 0;
        public static IEnumerable<FieldInfo> GetAllFieldsFromType(this Type type)
        {
            if (type == null)
                return Enumerable.Empty<FieldInfo>();

            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic |
                                 BindingFlags.Static | BindingFlags.Instance |
                                 BindingFlags.DeclaredOnly;
            if (type.BaseType != null)
                return type.GetFields(flags).Concat(type.BaseType.GetAllFieldsFromType());
            else
                return type.GetFields(flags);
        }

        public static FieldInfo GetFieldByName(this Type type, string name)
        {
            return type.GetAllFieldsFromType().Where(p => p.Name == name).FirstOrDefault();
        }
        public static string PackageName(string assetName)
        {
            var publishedFileID = PluginInfo.publishedFileID.ToString();
            if (publishedFileID.Equals(PublishedFileId.invalid.ToString()))
            {
                return assetName;
            }
            return publishedFileID;
        }
        public static bool IsMetroTrackCustomizerUIVisible()
        {
            var ui = UIView.GetAView().FindUIComponent(typeof(MetroTrackCustomizerUI).ToString());

            if (ui != null)
            {
                Debug.Log("The UI is not null!");
                Debug.Log("The UI is visible? " + ((MetroTrackCustomizerUI)ui).isVisible + ". Count: " + count++);
                //Debug.Log("The UI is active? " + ((MetroTrackCustomizerUI)ui).IsActivated());
                //Debug.Log("The UI is active & enabled? " + ((MetroTrackCustomizerUI)ui).isActiveAndEnabled);
                return ((MetroTrackCustomizerUI)ui).isVisible;
            }
            else
            {
                Debug.Log("The UI is null!");
            }
                
            return false;
        }
        public static T ClonePrefab<T>(T originalPrefab, string newName, Transform parentTransform) where T : PrefabInfo
        {
            var instance = Object.Instantiate(originalPrefab.gameObject);
            instance.name = newName;
            instance.transform.SetParent(parentTransform);
            instance.transform.localPosition = new Vector3(-7500, -7500, -7500);
            var newPrefab = instance.GetComponent<T>();
            instance.SetActive(false);
            newPrefab.m_prefabInitialized = false;
            return newPrefab;
        }

        public static NetTool GetNetTool()
        {
            return Object.FindObjectOfType<NetTool>();
        }

        public static string AssemblyPath => PluginInfo.modPath;


        private static PluginInfo PluginInfo {
            get
            {
                var pluginManager = PluginManager.instance;
                var plugins = pluginManager.GetPluginsInfo();

                foreach (var item in plugins)
                {
                    try
                    {
                        var instances = item.GetInstances<IUserMod>();
                        if (!(instances.FirstOrDefault() is Mod))
                        {
                            continue;
                        }
                        return item;
                    }
                    catch
                    {

                    }
                }
                throw new Exception("Failed to find MetroOverhaul assembly!");

            }
        }

        public static string AssemblyDirectory {
            get
            {
                var pluginManager = PluginManager.instance;
                var plugins = pluginManager.GetPluginsInfo();

                foreach (var item in plugins)
                {
                    try
                    {
                        var instances = item.GetInstances<IUserMod>();
                        if (!(instances.FirstOrDefault() is Mod))
                        {
                            continue;
                        }
                        return item.modPath;
                    }
                    catch
                    {

                    }
                }
                throw new Exception("Failed to find MetroOverhaul assembly!");

            }
        }
        public static string GetMetroTrackName(NetInfoVersion version, TrackStyle style, TrackType type = TrackType.TwowayTwoLane, bool isNoBar = false)
        {
            var trackName = "Metro Track";
            if (version == NetInfoVersion.Ground)
            {
                switch (style)
                {
                    case TrackStyle.Vanilla:
                        trackName += " Ground 01";
                        break;
                    case TrackStyle.Modern:
                        trackName = string.Format("{0} Ground", trackName);
                        break;
                    case TrackStyle.Classic:
                        trackName = string.Format("Steel {0} Ground", trackName);
                        break;
                }
            }
            else if (version != NetInfoVersion.Tunnel || style != TrackStyle.Vanilla)
            {
                switch (style)
                {
                    case TrackStyle.Vanilla:
                        trackName = string.Format("{0} {1} 01", trackName, version.ToString());
                        break;
                    case TrackStyle.Modern:
                        trackName = string.Format("{0} {1}",trackName, version.ToString());
                        break;
                    case TrackStyle.Classic:
                        trackName = string.Format("Steel {0} {1}", trackName, version.ToString());
                        break;
                }
            }
            switch (type)
            {
                case TrackType.TwowayOneLane:
                    trackName += " Small Two-Way";
                    break;
                case TrackType.OnewayTwoLane:
                    trackName += " Two-Lane One-Way";
                    break;
                case TrackType.OnewayOneLane:
                    trackName += " Small";
                    break;
                case TrackType.Quad:
                    trackName += " Large";
                    break;
            }
            if (style != TrackStyle.Vanilla)
            {
                if (isNoBar)
                {
                    return trackName + " NoBar";
                }
            }
            return trackName;
        }

        public static string GetMetroStationTrackName(NetInfoVersion version, TrackStyle style, StationTrackType stationType = StationTrackType.SidePlatform)
        {
            string trackName = GetMetroTrackName(version, style).Replace("Metro Track", "Metro Station Track");
            switch (stationType)
            {
                case StationTrackType.SidePlatform:
                    return trackName;
                case StationTrackType.SinglePlatform:
                    return trackName + " Small";
                case StationTrackType.IslandPlatform:
                    return trackName + " Island";
                case StationTrackType.ExpressSidePlatform:
                    return trackName + " Large";
                case StationTrackType.DualIslandPlatform:
                    return trackName + " Large Dual Island";
            }
            return trackName;
        }
        public static bool IsModActive(string modName)
        {
            var plugins = PluginManager.instance.GetPluginsInfo();
            return (from plugin in plugins.Where(p => p.isEnabled)
                    select plugin.GetInstances<IUserMod>() into instances
                    where instances.Any()
                    select instances[0].Name into name
                    where name == modName
                    select name).Any();
        }

        public static bool TryGetWorkshopId(PrefabInfo info, out long workshopId)
        {
            workshopId = -1;
            if (info?.name == null)
            {
                return false;
            }
            if (!info.name.Contains(".")) //only for custom prefabs
            {
                return false;
            }
            var idStr = info.name.Split('.')[0];
            return long.TryParse(idStr, out workshopId);
        }
        public static bool IsGameMode()
        {
            return ToolManager.instance.m_properties.m_mode == ItemClass.Availability.Game;
        }
    }
}
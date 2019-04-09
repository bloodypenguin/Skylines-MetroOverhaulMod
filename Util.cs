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

namespace MetroOverhaul {
    public static class Util
    {
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
        public static string AssemblyPath => PluginInfo.modPath;


        private static PluginInfo PluginInfo
        {
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

        public static string AssemblyDirectory
        {
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
        public static bool IsHooked()
        {
            foreach (PluginInfo current in PluginManager.instance.GetPluginsInfo())
            {
                if (current.publishedFileID.AsUInt64 == 530771650uL) return true;
            }
            return false;
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
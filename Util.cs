using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.Plugins;
using ICities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace OneWayTrainTrack
{
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

        public static NetInfo ClonePrefab(NetInfo originalPrefab, string newName, Transform parentTransform)
        {
            var instance = Object.Instantiate(originalPrefab.gameObject);
            instance.name = newName;
            instance.transform.SetParent(parentTransform);
            instance.transform.localPosition = new Vector3(-7500, -7500, -7500);
            var newPrefab = instance.GetComponent<NetInfo>();
            instance.SetActive(false);
            newPrefab.m_prefabInitialized = false;
            return newPrefab;
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
                throw new Exception("Failed to find OneWayTrainTrack assembly!");

            }
        }

        public static void AddLocale(string idBase, string key, string title, string description)
        {
            var localeField = typeof(LocaleManager).GetField("m_Locale", BindingFlags.NonPublic | BindingFlags.Instance);
            var locale = (Locale)localeField.GetValue(SingletonLite<LocaleManager>.instance);
            var localeKey = new Locale.Key() { m_Identifier = $"{idBase}_TITLE", m_Key = key };
            if (!locale.Exists(localeKey))
            {
                locale.AddLocalizedString(localeKey, title);
            }
            localeKey = new Locale.Key() { m_Identifier = $"{idBase}_DESC", m_Key = key };
            if (!locale.Exists(localeKey))
            {
                locale.AddLocalizedString(localeKey, description);
            }
            localeKey = new Locale.Key() { m_Identifier = $"{idBase}", m_Key = key };
            if (!locale.Exists(localeKey))
            {
                locale.AddLocalizedString(localeKey, description);
            }
        }
    }
}
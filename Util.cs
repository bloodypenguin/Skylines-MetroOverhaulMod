using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ColossalFramework.Plugins;
using ICities;
using UnityEngine;
using Object = UnityEngine.Object;
using static ColossalFramework.Plugins.PluginManager;

namespace MetroOverhaul
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

//        public static Mesh LoadMesh(string fullPath, string meshName)
//        {
//            var mesh = new Mesh();
//            using (var fileStream = File.Open(fullPath, FileMode.Open))
//            {
//                mesh.LoadOBJ(OBJLoader.LoadOBJ(fileStream));
//            }
//            mesh.Optimize();
//            mesh.name = meshName;
//
//            return mesh;
//        }

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
                throw new Exception("Failed to find SingleTrainTrack assembly!");

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
                throw new Exception("Failed to find ElevatedTrainStationTrack assembly!");

            }
        }
    }
}
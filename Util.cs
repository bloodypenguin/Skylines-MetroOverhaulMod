using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ColossalFramework.Plugins;
using ICities;
using ObjUnity3D;
using UnityEngine;

namespace ElevatedTrainStationTrack
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

        public static Mesh LoadMesh(string fullPath, string meshName)
        {
            var mesh = new Mesh();
            using (var fileStream = File.Open(fullPath, FileMode.Open))
            {
                mesh.LoadOBJ(OBJLoader.LoadOBJ(fileStream));
            }
            mesh.Optimize();
            mesh.name = meshName;

            return mesh;
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
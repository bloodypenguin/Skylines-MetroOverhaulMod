using System;
using System.IO;
using System.Linq;
using ColossalFramework.Plugins;
using ICities;
using ObjUnity3D;
using UnityEngine;

namespace ElevatedTrainStationTrack
{
    public class Util
    {
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
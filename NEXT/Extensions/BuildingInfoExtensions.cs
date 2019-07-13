using MetroOverhaul.NEXT.Texturing;
using System.Collections.Generic;
using UnityEngine;

namespace MetroOverhaul.NEXT.Extensions {
    public static partial class BuildingInfoExtensions {
        public static BuildingInfo SetMeshes(this BuildingInfo info, string newMeshPath, string newLODMeshPath = null)
        {
            info.m_mesh = AssetManager.instance.GetMesh(newMeshPath);

            if (newLODMeshPath != null)
            {
                info.m_lodMesh = AssetManager.instance.GetMesh(newLODMeshPath);
            }

            return info;
        }

        public static BuildingInfo SetConsistentUVs(this BuildingInfo info, bool lodOnly = false)
        {
            var colors = new List<Color>();
            var colors32 = new List<Color32>();
            if (lodOnly == false)
            {
                for (int i = 0; i < info.m_mesh.vertexCount; i++)
                {
                    colors.Add(new Color(255, 0, 255, 255));
                    colors32.Add(new Color32(255, 0, 255, 255));
                }

                info.m_mesh.colors = colors.ToArray();
                info.m_mesh.colors32 = colors32.ToArray();
                colors = new List<Color>();
                colors32 = new List<Color32>();
            }

            for (int i = 0; i < info.m_lodMesh.vertexCount; i++)
            {
                colors.Add(new Color(255, 0, 255, 255));
                colors32.Add(new Color32(255, 0, 255, 255));
            }

            info.m_lodMesh.colors = colors.ToArray();
            info.m_lodMesh.colors32 = colors32.ToArray();

            return info;
        }

        public static BuildingInfo SetTextures(this BuildingInfo info, TextureSet newTextures,
            LODTextureSet newLODTextures = null)
        {
            if (newTextures != null)
            {
                if (info.m_material != null)
                {
                    info.m_material = newTextures.CreateRoadMaterial(info.m_material);
                }
            }

            if (newLODTextures != null)
            {
                if (info.m_lodMaterial != null)
                {
                    info.m_lodMaterial = newLODTextures.CreateRoadMaterial(info.m_lodMaterial);
                }
            }

            return info;
        }
    }
}

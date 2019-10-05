using MetroOverhaul.NEXT.Texturing;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MetroOverhaul.NEXT.Extensions
{
    public static partial class NetInfoExtensions
    {
        public static NetInfo SetAllNodesTexture(this NetInfo info, TextureSet newTextures, LODTextureSet newLODTextures = null)
        {
            foreach (var node in info.m_nodes)
            {
                node.SetTextures(newTextures, newLODTextures);
            }

            return info;
        }

        public static NetInfo.Node SetTextures(this NetInfo.Node node, TextureSet newTextures, LODTextureSet newLODTextures = null)
        {
            if (newTextures != null)
            {
                if (node.m_material != null)
                {
                    node.m_material = newTextures.CreateRoadMaterial(node.m_material);
                    node.m_nodeMaterial = newTextures.CreateRoadMaterial(node.m_material);
                }
            }

            if (newLODTextures != null)
            {
                if (node.m_lodMaterial != null)
                {
                    node.m_lodMaterial = newLODTextures.CreateRoadMaterial(node.m_lodMaterial);
                }
            }

            return node;
        }

        public static NetInfo.Node[] AddNodes(this NetInfo info, params NetInfo.Node[] nodesToAdd)
        {
            var thenodes = info.m_nodes.ToList();
            thenodes.AddRange(nodesToAdd);
            return thenodes.ToArray();
        }

        public static NetInfo.Node SetMeshes(this NetInfo.Node node, string newMeshPath, string newLODMeshPath = null)
        {
            node.m_mesh = AssetManager.instance.GetMesh(newMeshPath);
            node.m_nodeMesh = AssetManager.instance.GetMesh(newMeshPath);
            if (newLODMeshPath != null)
            {
                node.m_lodMesh = AssetManager.instance.GetMesh(newLODMeshPath);
            }

            return node;
        }


        public static NetInfo.Node SetFlags(this NetInfo.Node node, NetNode.Flags required, NetNode.Flags forbidden)
        {
            node.m_flagsRequired = required;
            node.m_flagsForbidden = forbidden;

            return node;
        }
        public static NetInfo.Node SetFlagsDefault(this NetInfo.Node node)
        {
            node.m_flagsRequired = NetNode.Flags.None;
            node.m_flagsForbidden = NetNode.Flags.None;

            return node;
        }
        public static NetInfo.Node SetConsistentUVs(this NetInfo.Node node, bool lodOnly = false)
        {
            var colors = new List<Color>();
            var colors32 = new List<Color32>();
            if (lodOnly == false)
            {
                for (int i = 0; i < node.m_mesh.vertexCount; i++)
                {
                    colors.Add(new Color(255, 0, 255, 255));
                    colors32.Add(new Color32(255, 0, 255, 255));
                }
                node.m_mesh.colors = colors.ToArray();
                node.m_mesh.colors32 = colors32.ToArray();
                colors = new List<Color>();
                colors32 = new List<Color32>();
            }
            for (int i = 0; i < node.m_lodMesh.vertexCount; i++)
            {
                colors.Add(new Color(255, 0, 255, 255));
                colors32.Add(new Color32(255, 0, 255, 255));
            }
            node.m_lodMesh.colors = colors.ToArray();
            node.m_lodMesh.colors32 = colors32.ToArray();

            return node;
        }
    }
}

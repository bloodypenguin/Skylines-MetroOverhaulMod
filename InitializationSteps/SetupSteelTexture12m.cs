﻿using MetroOverhaul.NEXT;
using MetroOverhaul.NEXT.Extensions;
using MetroOverhaul.NEXT.Texturing;

namespace MetroOverhaul.InitializationSteps
{
    public static class SetupSteelTexture
    {
        public static void Setup12mSteelTexture(NetInfo info, NetInfoVersion version)
        {
            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        for (var i = 0; i < info.m_segments.Length; i++)
                        {
                            if (info.m_segments[i].m_mesh.name.Contains("Pavement_Steel"))
                            {
                                info.m_segments[i].SetTextures(
                                    new TextureSet
                                        (@"Textures\Ground_Segment_Pavement_Steel__MainTex.png",
                                        @"Textures\Ground_Segment_Pavement_Steel__AlphaMap.png",
                                        @"Textures\Ground_Segment_Pavement_Steel__XYSMap.png"),
                                    new LODTextureSet
                                        (@"Textures\Ground_Segment_Pavement_Steel__MainTex_LOD.png",
                                        @"Textures\Ground_Segment_Pavement_Steel__AlphaMap_LOD.png",
                                        @"Textures\Ground_Segment_Pavement_Steel__XYSMap_LOD.png"));
                                        
                            }
                            else
                            {
                                info.m_segments[i].SetTextures(
                                    new TextureSet
                                        (@"Textures\Ground_Segment_Rail__MainTex.png",
                                        @"Textures\Ground_Segment_Rail__AlphaMap.png",
                                        @"Textures\Ground_Segment_Rail__XYSMap.png"));
                            }
                        }
                        for (var i = 0; i < info.m_nodes.Length; i++)
                        {
                            if (info.m_nodes[i].m_mesh.name.Contains("Pavement_Steel"))
                            {
                                info.m_nodes[i].SetTextures(
                                    new TextureSet
                                        (@"Textures\Ground_Node_Pavement_Steel__MainTex.png",
                                        @"Textures\Ground_Node_Pavement_Steel__AlphaMap.png",
                                        @"Textures\Ground_Segment_Pavement_Steel__XYSMap.png"),
                                    new LODTextureSet
                                        (@"Textures\Ground_Node_Pavement_Steel__MainTex_LOD.png",
                                        @"Textures\Ground_Node_Pavement_Steel__AlphaMap_LOD.png",
                                        @"Textures\Ground_Segment_Pavement_Steel__XYSMap_LOD.png"));
                            }
                            else if (info.m_nodes[i].m_mesh.name.Contains("Ground_Level_Crossing_Rail"))
                            {
                                info.m_nodes[i].SetTextures(
                                    new TextureSet
                                        (@"Textures\Ground_Level_Crossing_Rail__MainTex.png",
                                        @"Textures\Ground_Segment_Rail__AlphaMap.png",
                                        @"Textures\Ground_Level_Crossing_Rail__XYSMap.png"));
                            }
                            else if(info.m_nodes[i].m_mesh.name.Contains("Ground_Level_Crossing"))
                            {
                                info.m_nodes[i].SetTextures(
                                    new TextureSet
                                        (@"Textures\Ground_Level_Crossing__MainTex.png",
                                        @"Textures\Ground_Level_Crossing__AlphaMap.png",
                                        @"Textures\Ground_Segment_Rail__XYSMap.png"));
                            }
                            else
                            {
                                info.m_nodes[i].SetTextures(
                                    new TextureSet
                                        (@"Textures\Ground_Segment_Rail__MainTex.png",
                                        @"Textures\Ground_Segment_Rail__AlphaMap.png",
                                        @"Textures\Ground_Segment_Rail__XYSMap.png"));
                            }
                        }
                        break;
                    }
                case NetInfoVersion.Elevated:
                //case NetInfoVersion.Bridge:
                    {
                        for (var i = 0; i < info.m_segments.Length; i++)
                        {
                            if (info.m_segments[i].m_mesh.name.Contains("Pavement_Steel"))
                            {
                                var isElevated = info.m_segments[i].m_mesh.name.Contains("Elevated_Pavement_Steel");
                                    
                                info.m_segments[i].SetTextures(
                                    new TextureSet
                                        (@"Textures\Elevated_Segment_Pavement_Steel__MainTex.png",
                                        @"Textures\Elevated_Segment_Pavement_Steel__AlphaMap.png",
                                        @"Textures\Elevated_Segment_Pavement_Steel__XYSMap.png"),
                                    new LODTextureSet
                                        (@"Textures\Elevated_Segment_Pavement_Steel__MainTex_LOD.png",
                                        (isElevated?@"Textures\Elevated_Segment_Pavement_Steel__AlphaMap_LOD.png"
                                                   :@"Textures\Bridge_Pavement_Steel__AlphaMap_LOD.png"),
                                        @"Textures\Elevated_Segment_Pavement_Steel__XYSMap_LOD.png"));
                            }
                            else
                            {
                                info.m_segments[i].SetTextures(
                                    new TextureSet
                                        (@"Textures\Ground_Segment_Rail__MainTex.png",
                                        @"Textures\Ground_Segment_Rail__AlphaMap.png",
                                        @"Textures\Ground_Segment_Rail__XYSMap.png"));
                            }
                        }
                        for (var i = 0; i < info.m_nodes.Length; i++)
                        {
                            if (info.m_nodes[i].m_mesh.name.Contains("Elevated_Node_Pavement_Steel"))
                            {
                                info.m_nodes[i].SetTextures(
                                    new TextureSet
                                        (@"Textures\Elevated_Node_Pavement_Steel__MainTex.png",
                                        @"Textures\Elevated_Node_Pavement_Steel__AlphaMap.png",
                                        @"Textures\Ground_Segment_Pavement_Steel__XYSMap.png"),
                                    new LODTextureSet
                                        (@"Textures\Elevated_Node_Pavement_Steel__MainTex_LOD.png",
                                        @"Textures\Elevated_Node_Pavement_Steel__AlphaMap_LOD.png",
                                        @"Textures\Ground_Segment_Pavement_Steel__XYSMap_LOD.png"));
                            }
                            else if (info.m_nodes[i].m_mesh.name.Contains("Bridge_Node_Pavement_Steel"))
                            {
                                info.m_nodes[i].SetTextures(
                                    new TextureSet
                                        (@"Textures\Bridge_Node_Pavement_Steel__MainTex.png",
                                        @"Textures\Elevated_Node_Pavement_Steel__AlphaMap.png",
                                        @"Textures\Bridge_Node_Pavement_Steel__XYSMap.png"),
                                    new LODTextureSet
                                        (@"Textures\Bridge_Node_Pavement_Steel__MainTex_LOD.png",
                                        @"Textures\Elevated_Node_Pavement_Steel__AlphaMap_LOD.png",
                                        @"Textures\Bridge_Node_Pavement_Steel__XYSMap_LOD.png"));
                    }
                            else
                            {
                                info.m_nodes[i].SetTextures(
                                    new TextureSet
                                        (@"Textures\Ground_Segment_Rail__MainTex.png",
                                        @"Textures\Ground_Segment_Rail__AlphaMap.png",
                                        @"Textures\Ground_Segment_Rail__XYSMap.png"));
                            }
                        }
                        break;
                    }
                case NetInfoVersion.Slope:
                    //case NetInfoVersion.Tunnel:
                    {
                        for (var i = 0; i < info.m_segments.Length; i++)
                        {
                            if (info.m_segments[i].m_mesh.name.Contains("Pavement_Steel"))
                            {
                                info.m_segments[i].SetTextures(
                                    new TextureSet
                                        (@"Textures\Tunnel_Segment_Pavement_Steel__MainTex.png",
                                        @"Textures\Elevated_Segment_Pavement_Steel__AlphaMap.png",
                                        @"Textures\Tunnel_Segment_Pavement_Steel__XYSMap.png"),
                                    new LODTextureSet
                                        (@"Textures\Tunnel_Segment_Pavement_Steel__MainTex_LOD.png",
                                        @"Textures\Elevated_Segment_Pavement_Steel__AlphaMap_LOD.png",
                                        @"Textures\Tunnel_Segment_Pavement_Steel__XYSMap_LOD.png"));
                            }
                            else
                            {
                                info.m_segments[i].SetTextures(
                                    new TextureSet
                                        (@"Textures\Ground_Segment_Rail__MainTex.png",
                                        @"Textures\Ground_Segment_Rail__AlphaMap.png",
                                        @"Textures\Ground_Segment_Rail__XYSMap.png"));
                            }
                        }
                        for (var i = 0; i < info.m_nodes.Length; i++)
                        {
                            if (info.m_nodes[i].m_mesh.name.Contains("Node_Pavement_Steel"))
                            {
                                info.m_nodes[i].SetTextures(
                                    new TextureSet
                                        (@"Textures\Tunnel_Node_Pavement_Steel__MainTex.png",
                                        @"Textures\Elevated_Node_Pavement_Steel__AlphaMap.png",
                                        @"Textures\Tunnel_Segment_Pavement_Steel__XYSMap.png"),
                                    new LODTextureSet
                                        (@"Textures\Tunnel_Node_Pavement_Steel__MainTex_LOD.png",
                                        @"Textures\Elevated_Node_Pavement_Steel__AlphaMap_LOD.png",
                                        @"Textures\Tunnel_Segment_Pavement_Steel__XYSMap_LOD.png"));
                            }
                            else
                            {
                                info.m_nodes[i].SetTextures(
                                    new TextureSet
                                        (@"Textures\Ground_Segment_Rail__MainTex.png",
                                        @"Textures\Ground_Segment_Rail__AlphaMap.png",
                                        @"Textures\Ground_Segment_Rail__XYSMap.png"));
                            }
                        }
                        break;
                    }
            }
        }
    }
}
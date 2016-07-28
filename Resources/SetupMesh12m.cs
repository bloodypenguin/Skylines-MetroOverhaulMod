using SubwayOverhaul.NEXT;
using SubwayOverhaul.NEXT.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroOverhaul.SetupPrefab
{
    public class SetupMesh
    {
        public static void Setup12mMesh(NetInfo info, NetInfoVersion version, NetInfo elevatedInfo = null, NetInfo trainTrackInfo = null)
        {

            var elevatedMaterial = elevatedInfo?.m_segments[0].m_material;
            switch (version)
            {

                case NetInfoVersion.Ground:
                    {
                        var segment0 = info.m_segments[0];
                        var segment1 = info.m_segments[1];
                        var node0 = info.m_nodes[0];
                        var node1 = info.m_nodes[1];
                        var node2 = info.m_nodes[2];

                        segment0
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\Ground_Pavement.obj");

                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\Ground_Rail.obj")
                            .SetConsistentUVs();

                        node0
                            .SetMeshes
                            (@"Meshes\Ground_Node_Pavement.obj")
                            .SetConsistentUVs();
                        node1
                            .SetMeshes
                            (@"Meshes\Elevated_Rail.obj")
                            .SetConsistentUVs();

                        if (elevatedMaterial != null)
                        {
                            segment0.m_material = elevatedMaterial;
                            //segment1.m_material = railMaterial;
                            //node1.m_material = railMaterial;
                        }

                        info.m_segments = new[] { segment0, segment1 };
                        info.m_nodes = new[] { node0, node1, node2 };
                        break;
                    }
                case NetInfoVersion.Elevated:
                    {
                        var segment0 = info.m_segments[0];
                        var segment1 = info.m_segments[1];
                        var node0 = info.m_nodes[0];
                        var node1 = info.m_nodes[1];
                        var node3 = info.m_nodes[3];

                        segment0
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\Elevated_Pavement.obj");

                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\Elevated_Rail.obj")
                            .SetConsistentUVs();

                        node0
                            .SetMeshes
                            (@"Meshes\Elevated_Node_Pavement.obj")
                            .SetConsistentUVs();
                        node1
                            .SetMeshes
                            (@"Meshes\Elevated_Rail.obj")
                            .SetConsistentUVs();

                        if (elevatedMaterial != null)
                        {
                            segment0.m_material = elevatedMaterial;
                            node0.m_material = elevatedMaterial;
                            //segment1.m_material = railMaterial;
                            //node1.m_material = railMaterial;
                        }

                        info.m_segments = new[] { segment0, segment1 };
                        info.m_nodes = new[] { node0, node1, node3 };
                        break;
                    }
                case NetInfoVersion.Slope:
                    {
                        var segment0 = info.m_segments[0];
                        var segment1 = info.m_segments[1];
                        var segment3 = info.m_segments[3];
                        var node0 = info.m_nodes[0];
                        var node1 = info.m_nodes[1];
                        var node3 = info.m_nodes[3];
                        var node4 = info.m_nodes[4];
                        var node5 = info.m_nodes[0].ShallowClone();

                        segment1
                            .SetMeshes
                            (@"Meshes\Elevated_Rail.obj")
                            .SetConsistentUVs();
                        segment3
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\Slope_Pavement.obj")
                            .SetConsistentUVs();
                        node1
                            .SetMeshes
                            (@"Meshes\Elevated_Rail.obj")
                            .SetConsistentUVs();
                        node3
                            .SetMeshes
                            (@"Meshes\Slope_Node_Pavement.obj")
                            .SetConsistentUVs();
                        node5
                            .SetMeshes
                            (@"Meshes\Tunnel_Node_Pavement.obj")
                            .SetConsistentUVs();
                        if (elevatedMaterial != null)
                        {
                            segment3.m_material = elevatedMaterial;
                            node3.m_material = elevatedMaterial;
                            node5.m_material = elevatedMaterial;
                        }
                        info.m_segments = new[] { segment0, segment1, segment3 };
                        info.m_nodes = new[] { node0, node1, node3, node4, node5 };
                        break;
                    }
                case NetInfoVersion.Tunnel:
                    {
                        var segment0 = info.m_segments[0];
                        var segment1 = info.m_segments[0].ShallowClone();
                        var segment2 = trainTrackInfo.m_segments[1].ShallowClone();
                        var node0 = info.m_nodes[0];
                        var node1 = info.m_nodes[0].ShallowClone();
                        var node2 = trainTrackInfo.m_nodes[1].ShallowClone();

                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\Tunnel_Pavement.obj")
                            .SetConsistentUVs();
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\Elevated_Rail.obj")
                            .SetConsistentUVs();
                        node1
                            .SetMeshes
                            (@"Meshes\Tunnel_Node_Pavement.obj")
                            .SetConsistentUVs();
                        node2
                            .SetMeshes
                            (@"Meshes\Elevated_Rail.obj")
                            .SetConsistentUVs();
                        if (elevatedMaterial != null)
                        {
                            segment1.m_material = elevatedMaterial;
                            node1.m_material = elevatedMaterial;
                            segment2.m_material = elevatedMaterial;
                            node2.m_material = elevatedMaterial;
                        }
                        info.m_segments = new[] { segment0, segment1,segment2 };
                        info.m_nodes = new[] { node0, node1, node2 };
                        break;
                    }
            }

        }
    }
}

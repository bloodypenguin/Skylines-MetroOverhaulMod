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
        public static void Setup12mMesh(NetInfo info, NetInfoVersion version, NetInfo trainTrackInfo = null)
        {

            var railMaterial = trainTrackInfo?.m_nodes[1].m_material;
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
                            (@"Meshes\Ground_Rail.obj")
                            .SetConsistentUVs();

                        if (railMaterial != null)
                        {
                            segment1.m_material = railMaterial;
                            node1.m_material = railMaterial;
                        }

                        info.m_segments = new[] { segment0, segment1 };
                        info.m_nodes = new[] { node0, node1, node2 };
                        break;
                    }
            }

        }
    }
}

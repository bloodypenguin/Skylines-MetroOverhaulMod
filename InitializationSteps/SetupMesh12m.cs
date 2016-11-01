using MetroOverhaul.NEXT;
using MetroOverhaul.NEXT.Extensions;

namespace MetroOverhaul.InitializationSteps
{
    public static class SetupMesh
    {
        public static void Setup12mMesh(NetInfo info, NetInfoVersion version, NetInfo elevatedInfo,
            NetInfo trainTrackInfo)
        {
            var elevatedMaterial = elevatedInfo.m_segments[0].m_material;
            var elevatedLODMaterial = elevatedInfo.m_segments[0].m_lodMaterial;
            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        var segment0 = info.m_segments[0];
                        var segment1 = info.m_segments[1];
                        var node0 = info.m_nodes[0];
                        var node1 = info.m_nodes[1];
                        var node2 = info.m_nodes[2];
                        var node3 = info.m_nodes[1].ShallowClone();

                        segment0
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\Ground_Pavement.obj",
                                @"Meshes\Ground_Pavement_LOD.obj");

                        node0
                            .SetMeshes
                            (@"Meshes\Ground_Node_Pavement.obj",
                                @"Meshes\Ground_Node_Pavement_LOD.obj")
                            .SetConsistentUVs(true);


                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\Ground_Rail.obj")
                            .SetConsistentUVs();

                        node1
                            .SetMeshes
                            (@"Meshes\Elevated_Rail.obj")
                            .SetConsistentUVs();
                        node2
                            .SetMeshes
                            (@"Meshes\Ground_Level_Crossing.obj")
                            .SetConsistentUVs();
                        node3
                            .SetMeshes
                            (@"Meshes\Ground_Level_Crossing_Rail.obj")
                            .SetConsistentUVs();
                        node1.m_flagsForbidden = NetNode.Flags.LevelCrossing;
                        node3.m_flagsRequired = NetNode.Flags.LevelCrossing;

                        //

                        info.m_segments = new[] { segment0, segment1 };
                        info.m_nodes = new[] { node0, node1, node2, node3 };
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
                            (@"Meshes\Elevated_Pavement.obj",
                                @"Meshes\Elevated_Pavement_LOD.obj")
                            .SetConsistentUVs(); //ehem

                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\Elevated_Rail.obj");

                        node0
                            .SetMeshes
                            (@"Meshes\Elevated_Node_Pavement.obj",
                                @"Meshes\Elevated_Node_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        node1
                            .SetMeshes
                            (@"Meshes\Elevated_Rail.obj");


                        segment0.m_material = elevatedMaterial;
                        segment0.m_lodMaterial = elevatedLODMaterial;
                        node0.m_material = elevatedMaterial;


                        info.m_segments = new[] { segment0, segment1 };
                        info.m_nodes = new[] { node0, node1, node3 };
                        break;
                    }
                case NetInfoVersion.Bridge:
                    {
                        var segment0 = info.m_segments[0];
                        var segment1 = info.m_segments[1];
                        var node0 = info.m_nodes[0];
                        var node1 = info.m_nodes[1];
                        var node3 = info.m_nodes[3];

                        segment0
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\Bridge_Pavement.obj",
                                @"Meshes\Bridge_Pavement_LOD.obj")
                            .SetConsistentUVs();

                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\Elevated_Rail.obj");

                        node0
                            .SetMeshes
                            (@"Meshes\Bridge_Node_Pavement.obj",
                                @"Meshes\Bridge_Node_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        node1
                            .SetMeshes
                            (@"Meshes\Elevated_Rail.obj");

                        segment0.m_material = elevatedMaterial;
                        segment0.m_lodMaterial = elevatedLODMaterial;
                        node0.m_material = elevatedMaterial;
                        //segment1.m_material = railMaterial;
                        //node1.m_material = railMaterial;


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
                            (@"Meshes\Elevated_Rail.obj");
                        segment3
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\Slope_Pavement.obj",
                                @"Meshes\Slope_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        node1
                            .SetMeshes
                            (@"Meshes\Elevated_Rail.obj");
                        node3
                            .SetMeshes
                            (@"Meshes\Slope_Node_Pavement.obj",
                                @"Meshes\Slope_Node_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        node5
                            .SetMeshes
                            (@"Meshes\Tunnel_Node_Pavement.obj",
                                @"Meshes\Tunnel_Node_Pavement_LOD.obj")
                            .SetConsistentUVs();

                        segment3.m_material = elevatedMaterial;
                        node3.m_material = elevatedMaterial;
                        node5.m_material = elevatedMaterial;

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
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\Elevated_Rail.obj");
                        node2
                            .SetMeshes
                            (@"Meshes\Elevated_Rail.obj");
                        segment1.m_material = elevatedMaterial;
                        node1.m_material = elevatedMaterial;
                        segment2.m_material = elevatedMaterial;
                        node2.m_material = elevatedMaterial;

                        info.m_segments = new[] { segment0, segment1, segment2 };
                        info.m_nodes = new[] { node0, node1, node2 };
                        break;
                    }
            }

        }

        //mind changed segment and node indices! (after Setup12mMesh)
        public static void Setup12mMeshNonAlt(NetInfo info, NetInfoVersion version, NetInfo elevatedInfo)
        {

            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        var elevatedMaterial = elevatedInfo.m_segments[0].m_material;
                        var elevatedLODMaterial = elevatedInfo.m_segments[0].m_lodMaterial;

                        var segment0 = info.m_segments[0];
                        var node0 = info.m_nodes[0];
                        var node2 = info.m_nodes[2];
                        segment0.m_material = elevatedMaterial;
                        segment0.m_lodMaterial = elevatedLODMaterial;
                        node0.m_material = elevatedMaterial;
                        node2.m_material = elevatedMaterial;
                        //segment1.m_material = railMaterial;
                        //node1.m_material = railMaterial;
                        break;
                    }
                case NetInfoVersion.Tunnel:
                    {
                        var segment1 = info.m_segments[1];
                        var node1 = info.m_nodes[1];
                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\Tunnel_Pavement.obj",
                                @"Meshes\Tunnel_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        node1
                            .SetMeshes
                            (@"Meshes\Tunnel_Node_Pavement.obj",
                                @"Meshes\Tunnel_Node_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        break;
                    }

            }
        }

        //mind changed segment and node indices! (after Setup12mMesh)
        public static void Setup12mMeshAlt(NetInfo info, NetInfoVersion version, NetInfo elevatedInfo, NetInfo trainTrackInfo)
        {
            var trainTrackMaterial = trainTrackInfo.m_segments[0].m_material;
            var trainTrackLODMaterial = elevatedInfo.m_segments[0].m_lodMaterial;
            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        var segment0 = info.m_segments[0];
                        var node0 = info.m_nodes[0];
                        segment0
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\Ground_NoBar_Pavement.obj",
                                @"Meshes\Ground_NoBar_Pavement_LOD.obj");

                        node0
                            .SetMeshes
                            (@"Meshes\Ground_NoBar_Node_Pavement.obj",
                                @"Meshes\Ground_NoBar_Node_Pavement_LOD.obj")
                            .SetConsistentUVs(true);
                        break;
                    }
                case NetInfoVersion.Tunnel:
                    {
                        var segment0 = info.m_segments[0];
                        var segment2 = info.m_segments[2];
                        var node0 = info.m_nodes[0];
                        var node2 = info.m_nodes[2];
                        segment0
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\Ground_NoBar_Pavement.obj",
                                @"Meshes\Ground_NoBar_Pavement_LOD.obj");

                        node0
                            .SetMeshes
                            (@"Meshes\Ground_NoBar_Node_Pavement.obj",
                                @"Meshes\Ground_NoBar_Node_Pavement_LOD.obj")
                            .SetConsistentUVs(true);


                        segment0.m_material = trainTrackMaterial;
                        segment0.m_lodMaterial = trainTrackLODMaterial;
                        node2.m_material = trainTrackMaterial;


                        info.m_segments = new[] { segment0, segment2 };
                        info.m_nodes = new[] { node0, node2 };
                        break;
                    }
            }
        }

        public static void Setup12mMeshStation(NetInfo prefab, NetInfoVersion version, NetInfo tunnelInfo)
        {
            switch (version)
            {
                case NetInfoVersion.Elevated:
                    {
                        var segment0 = prefab.m_segments[0].ShallowClone();
                        var segment1 = prefab.m_segments[1];
                        var node0 = prefab.m_nodes[0].ShallowClone();
                        var node1 = prefab.m_nodes[1];

                        segment0
                            .SetMeshes
                            (@"Meshes\Elevated_Station_Pavement.obj",
                                @"Meshes\Elevated_Station_Pavement_LOD.obj");
                        node0
                            .SetMeshes
                            (@"Meshes\Elevated_Station_Node_Pavement.obj",
                                @"Meshes\Elevated_Node_Pavement_LOD.obj");

                        prefab.m_segments = new[] { segment0, segment1 };
                        prefab.m_nodes = new[] { node0, node1 };
                        break;
                    }
                case NetInfoVersion.Tunnel:
                    {
                        var segment0 = tunnelInfo.m_segments[0];
                        var segment1 = prefab.m_segments[1].ShallowClone();
                        var segment2 = prefab.m_segments[2];
                        var node0 = tunnelInfo.m_nodes[0];
                        var node1 = prefab.m_nodes[1].ShallowClone();
                        var node2 = prefab.m_nodes[2];

                        segment1
                            .SetMeshes
                            (@"Meshes\Tunnel_Station_Pavement.obj",
                                @"Meshes\Ground_NoBar_Pavement_LOD.obj");
                        node1
                            .SetMeshes
                            (@"Meshes\Tunnel_Station_Node_Pavement.obj",
                                @"Meshes\Tunnel_Node_Pavement_LOD.obj");
                        prefab.m_segments = new[] { segment0, segment1, segment2 };
                        prefab.m_nodes = new[] { node0, node1, node2 };
                        break;
                    }
                case NetInfoVersion.Ground:
                    {
                        var segment0 = prefab.m_segments[0].ShallowClone();
                        var segment1 = prefab.m_segments[1];
                        var node0 = prefab.m_nodes[0].ShallowClone();
                        var node1 = prefab.m_nodes[1];
                        var node2 = prefab.m_nodes[2];
                        var node3 = prefab.m_nodes[3];

                        segment0
                            .SetMeshes
                            (@"Meshes\Ground_Station_Pavement.obj",
                                @"Meshes\Ground_NoBar_Pavement_LOD.obj");
                        node0
                            .SetMeshes
                            (@"Meshes\Ground_NoBar_Node_Pavement.obj",
                                @"Meshes\Ground_NoBar_Node_Pavement_LOD.obj")
                            .SetConsistentUVs(true);

                        prefab.m_segments = new[] { segment0, segment1 };
                        prefab.m_nodes = new[] { node0, node1, node2, node3 };
                        break;
                    }
            }
        }
    }
}

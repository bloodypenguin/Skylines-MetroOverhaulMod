using MetroOverhaul.NEXT;
using MetroOverhaul.NEXT.Extensions;

namespace MetroOverhaul.InitializationSteps
{
    public static class SetupSteelMesh
    {

        public static void Setup12mSteelMesh(NetInfo info, NetInfoVersion version, NetInfo elevatedInfo, NetInfo trainTrackInfo)
        {
            var elevatedMaterial = elevatedInfo.m_segments[0].m_material;
            var elevatedLODMaterial = elevatedInfo.m_segments[0].m_lodMaterial;
            var trainTrackMaterial = trainTrackInfo.m_segments[0].m_material;
            var trainTrackLODMaterial = elevatedInfo.m_segments[0].m_lodMaterial;
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
                            (@"Meshes\Ground_Pavement_Steel.obj",
                                @"Meshes\Ground_Pavement_LOD.obj");

                        node0
                            .SetMeshes
                            (@"Meshes\Ground_Node_Pavement_Steel.obj",
                                @"Meshes\Ground_Node_Pavement_LOD.obj")
                            .SetConsistentUVs(true);

                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\Rail.obj")
                            .SetConsistentUVs();

                        node1
                            .SetMeshes
                            (@"Meshes\Boosted_Rail.obj")
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
                        var node2 = info.m_nodes[0].ShallowClone();
                        var node3 = info.m_nodes[3];

                        segment0
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\Elevated_Pavement_Steel.obj",
                                @"Meshes\Elevated_Pavement_Steel_LOD.obj")
                            .SetConsistentUVs(); //ehem

                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\Rail.obj");

                        node0
                            .SetMeshes
                            (@"Meshes\Elevated_Node_Pavement_Steel.obj",
                                @"Meshes\Elevated_Node_Pavement_Steel_LOD.obj")
                            .SetConsistentUVs();
                        node1
                            .SetMeshes
                            (@"Meshes\Rail.obj");
                        node2
                            .SetMeshes
                            (@"Meshes\Boosted_Rail_Steel.obj",
                            @"Meshes\Boosted_Rail_Steel_LOD.obj")
                            .SetConsistentUVs();

                        segment0.m_material = elevatedMaterial;
                        segment0.m_lodMaterial = elevatedLODMaterial;
                        node0.m_material = elevatedMaterial;
                        node2.m_material = elevatedMaterial;
                        node2.m_directConnect = true;
                        info.m_segments = new[] { segment0, segment1 };
                        info.m_nodes = new[] { node0, node1, node2, node3 };
                        break;
                    }
                case NetInfoVersion.Bridge:
                    {
                        var segment0 = info.m_segments[0];
                        var segment1 = info.m_segments[1];
                        var node0 = info.m_nodes[0];
                        var node1 = info.m_nodes[1];
                        var node2 = info.m_nodes[0].ShallowClone();
                        segment0
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\Bridge_Pavement_Steel.obj",
                            @"Meshes\Bridge_Pavement_Steel_LOD.obj")
                            .SetConsistentUVs();

                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\Rail.obj");

                        node0
                            .SetMeshes
                            (@"Meshes\Bridge_Node_Pavement_Steel.obj",
                            @"Meshes\Bridge_Node_Pavement_Steel_LOD.obj")
                            .SetConsistentUVs();

                        node1
                            .SetMeshes
                            (@"Meshes\Rail.obj");

                        node2
                            .SetMeshes
                            (@"Meshes\Boosted_Rail_Steel.obj",
                            @"Meshes\Boosted_Rail_Steel_LOD.obj")
                            .SetConsistentUVs();

                        if (elevatedMaterial != null)
                        {
                            segment0.m_material = elevatedMaterial;
                            segment0.m_lodMaterial = elevatedLODMaterial;
                            node0.m_material = elevatedMaterial;
                            //segment1.m_material = railMaterial;
                            //node1.m_material = railMaterial;
                        }

                        info.m_segments = new[] { segment0, segment1 };
                        info.m_nodes = new[] { node0, node1, node2 };
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
                            (@"Meshes\Rail.obj");
                        segment3
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\Slope_Pavement_Steel.obj",
                                @"Meshes\Slope_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        node1
                            .SetMeshes
                            (@"Meshes\Rail.obj");
                        node3
                            .SetMeshes
                            (@"Meshes\Slope_Node_Pavement_Steel.obj",
                                @"Meshes\Slope_Node_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        node5
                            .SetMeshes
                            (@"Meshes\Tunnel_Node_Pavement_Steel.obj",
                                @"Meshes\Tunnel_Node_Pavement_LOD.obj")
                            .SetConsistentUVs();

                        segment3.m_material = elevatedMaterial;
                        node3.m_material = elevatedMaterial;
                        node5.m_material = elevatedMaterial;

                        info.m_segments = new[] { segment0, segment1, segment3 };
                        info.m_nodes = new[] { node0, node1, node3, node4, node5 };
                        break;
                    }
                    //case NetInfoVersion.Tunnel:
                    //    {
                    //        var segment0 = info.m_segments[0];
                    //        var segment1 = info.m_segments[0].ShallowClone();
                    //        var segment2 = trainTrackInfo.m_segments[1].ShallowClone();
                    //        var node0 = info.m_nodes[0];
                    //        var node1 = info.m_nodes[0].ShallowClone();
                    //        var node2 = trainTrackInfo.m_nodes[1].ShallowClone();
                    //        segment2
                    //            .SetFlagsDefault()
                    //            .SetMeshes
                    //            (@"Meshes\Rail.obj");
                    //        node2
                    //            .SetMeshes
                    //            (@"Meshes\Rail.obj");

                    //        if (isAlt)
                    //        {
                    //            segment0
                    //                .SetFlagsDefault()
                    //                .SetMeshes
                    //                (@"Meshes\Ground_NoBar_Pavement_Steel.obj",
                    //                @"Meshes\Ground_NoBar_Pavement_LOD.obj");

                    //            node0
                    //                .SetMeshes
                    //                (@"Meshes\Ground_NoBar_Node_Pavement_Steel.obj",
                    //                @"Meshes\Ground_NoBar_Node_Pavement_LOD.obj")
                    //                .SetConsistentUVs(true);


                    //            if (trainTrackMaterial != null)
                    //            {
                    //                segment0.m_material = trainTrackMaterial;
                    //                segment0.m_lodMaterial = trainTrackLODMaterial;
                    //                node2.m_material = trainTrackMaterial;
                    //            }

                    //            info.m_segments = new[] { segment0, segment2 };
                    //            info.m_nodes = new[] { node0, node2 };
                    //        }
                    //        else
                    //        {
                    //            segment1
                    //                .SetFlagsDefault()
                    //                .SetMeshes
                    //                (@"Meshes\Tunnel_Pavement_Steel.obj",
                    //                @"Meshes\Tunnel_Pavement_LOD.obj")
                    //                .SetConsistentUVs();
                    //            node1
                    //                .SetMeshes
                    //                (@"Meshes\Tunnel_Node_Pavement_Steel.obj",
                    //                @"Meshes\Tunnel_Node_Pavement_LOD.obj")
                    //                .SetConsistentUVs();

                    //        }
                    //        if (elevatedMaterial != null)
                    //        {
                    //            segment1.m_material = elevatedMaterial;
                    //            node1.m_material = elevatedMaterial;
                    //            segment2.m_material = elevatedMaterial;
                    //            node2.m_material = elevatedMaterial;
                    //        }
                    //        info.m_segments = new[] { segment0, segment1,segment2 };
                    //        info.m_nodes = new[] { node0, node1, node2 };
                    //        break;
                    //    }
            }

        }

        //mind changed indices! (after Setup12mSteelMesh)
        public static void Setup12mSteelMeshBar(NetInfo info, NetInfoVersion version, NetInfo elevatedInfo)
        {
            if (version != NetInfoVersion.Ground)
            {
                return;
            }
            var segment0 = info.m_segments[0];
            var node0 = info.m_nodes[0];
            var node2 = info.m_nodes[2];

            var elevatedMaterial = elevatedInfo?.m_segments[0].m_material;
            var elevatedLODMaterial = elevatedInfo?.m_segments[0].m_lodMaterial;
            segment0.m_material = elevatedMaterial;
            segment0.m_lodMaterial = elevatedLODMaterial;
            node0.m_material = elevatedMaterial;
            node2.m_material = elevatedMaterial;
            //segment1.m_material = railMaterial;
            //node1.m_material = railMaterial;
        }

        //mind changed indices! (after Setup12mSteelMesh)
        public static void Setup12mSteelMeshNoBar(NetInfo info, NetInfoVersion version, NetInfo elevatedInfo, NetInfo trainTrackInfo)
        {
            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        var segment0 = info.m_segments[0];
                        var node0 = info.m_nodes[0];
                        segment0
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\Ground_NoBar_Pavement_Steel.obj",
                                @"Meshes\Ground_NoBar_Pavement_LOD.obj");
                        node0
                            .SetMeshes
                            (@"Meshes\Ground_NoBar_Node_Pavement_Steel.obj",
                                @"Meshes\Ground_NoBar_Node_Pavement_LOD.obj")
                            .SetConsistentUVs(true);
                        break;
                    }
                case NetInfoVersion.Elevated:
                    {
                        var segment0 = info.m_segments[0];
                        var node0 = info.m_nodes[0];
                        segment0
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\Elevated_NoBar_Pavement_Steel.obj",
                                @"Meshes\Elevated_Pavement_LOD.obj")
                                .SetConsistentUVs();
                        node0
                            .SetMeshes
                            (@"Meshes\Elevated_NoBar_Node_Pavement_Steel.obj",
                                @"Meshes\Elevated_Node_Pavement_LOD.obj")
                            .SetConsistentUVs(true);
                        break;
                    }
                case NetInfoVersion.Bridge:
                    {
                        var segment0 = info.m_segments[0];
                        var segment1 = info.m_segments[1];
                        var node0 = info.m_nodes[0];
                        var node1 = info.m_nodes[1];
                        var node2 = info.m_nodes[0].ShallowClone();
                        segment0
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\Bridge_Pavement_Steel.obj",
                            @"Meshes\Bridge_Pavement_Steel_LOD.obj")
                            .SetConsistentUVs();

                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\Rail.obj");

                        node0
                            .SetMeshes
                            (@"Meshes\Bridge_Node_Pavement_Steel.obj",
                            @"Meshes\Bridge_Node_Pavement_Steel_LOD.obj")
                            .SetConsistentUVs();
                        node1
                            .SetMeshes
                            (@"Meshes\Rail.obj");

                        node2
                            .SetMeshes
                            (@"Meshes\Boosted_Rail_Steel.obj",
                            @"Meshes\Boosted_Rail_Steel_LOD.obj")
                            .SetConsistentUVs();

                        var elevatedMaterial = elevatedInfo?.m_segments[0].m_material;
                        var elevatedLODMaterial = elevatedInfo?.m_segments[0].m_lodMaterial;
                        segment0.m_material = elevatedMaterial;
                        segment0.m_lodMaterial = elevatedLODMaterial;
                        node0.m_material = elevatedMaterial;

                        info.m_segments = new[] { segment0, segment1 };
                        info.m_nodes = new[] { node0, node1,node2 };
                        break;
                    }
            }
        }
        public static void Setup12mStationSteelMesh(NetInfo prefab, NetInfoVersion version, NetInfo elevatedInfo, NetInfo metroStationInfo)
        {
            var elevatedMaterial = elevatedInfo.m_segments[0].m_material;
            var elevatedLODMaterial = elevatedInfo.m_segments[0].m_lodMaterial;
            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        var segment0 = prefab.m_segments[0].ShallowClone();
                        var segment1 = prefab.m_segments[1];
                        var node0 = prefab.m_nodes[0].ShallowClone();
                        var node1 = prefab.m_nodes[1];
                        var node2 = prefab.m_nodes[2];
                        var node3 = prefab.m_nodes[1].ShallowClone();

                        segment0
                            .SetMeshes
                            (@"Meshes\Ground_Station_Pavement_Steel.obj",
                                @"Meshes\Ground_NoBar_Pavement_LOD.obj");
                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\Rail.obj")
                            .SetConsistentUVs();
                        node0
                            .SetMeshes
                            (@"Meshes\Ground_Station_Node_Pavement_Steel.obj",
                                @"Meshes\Ground_Station_Node_Pavement_LOD.obj");
                        node1
                            .SetMeshes
                            (@"Meshes\Elevated_Station_Rail_Node.obj")
                            .SetConsistentUVs();
                        node2
                            .SetMeshes
                            (@"Meshes\Ground_Level_Crossing.obj",
                                @"Meshes\Ground_Level_Crossing_LOD.obj")
                            .SetConsistentUVs();
                        node3
                            .SetMeshes
                            (@"Meshes\Ground_Level_Crossing_Rail_Station.obj")
                            .SetConsistentUVs();

                        segment0.m_material = elevatedMaterial;
                        segment0.m_lodMaterial = elevatedLODMaterial;
                        node0.m_material = elevatedMaterial;
                        node0.m_lodMaterial = elevatedLODMaterial;
                        node2.m_lodMaterial = elevatedLODMaterial;

                        node1.m_flagsForbidden = NetNode.Flags.LevelCrossing;
                        node2.m_flagsRequired = NetNode.Flags.LevelCrossing;
                        node3.m_flagsRequired = NetNode.Flags.LevelCrossing;

                        prefab.m_segments = new[] { segment0, segment1 };
                        prefab.m_nodes = new[] { node0, node1, node2, node3 };
                        break;
                    }
                case NetInfoVersion.Elevated:
                    {
                        var segment0 = prefab.m_segments[0].ShallowClone();
                        var segment1 = prefab.m_segments[1].ShallowClone();
                        var node0 = prefab.m_nodes[0].ShallowClone();
                        var node1 = prefab.m_nodes[1].ShallowClone();
                        var node2 = prefab.m_nodes[0].ShallowClone();
                        segment0
                            .SetMeshes
                            (@"Meshes\Elevated_Station_Pavement_Steel.obj",
                                @"Meshes\Elevated_Station_Pavement_LOD.obj");
                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\Rail.obj")
                            .SetConsistentUVs();
                        node0
                            .SetMeshes
                            (@"Meshes\Elevated_Station_Node_Pavement_Steel.obj",
                                @"Meshes\Elevated_Station_Node_Pavement_LOD.obj")
                                .SetConsistentUVs();
                        node1
                            .SetMeshes
                            (@"Meshes\Rail.obj")
                            .SetConsistentUVs();

                        node2
                            .SetMeshes
                            (@"Meshes\Boosted_Rail_Steel.obj",
                            @"Meshes\Boosted_Rail_Steel_LOD.obj")
                            .SetConsistentUVs();
                         
                        segment0.m_material = elevatedMaterial;
                        segment0.m_lodMaterial = elevatedLODMaterial;
                        node0.m_material = elevatedMaterial;
                        node0.m_lodMaterial = elevatedLODMaterial;

                        prefab.m_segments = new[] { segment0, segment1 };
                        prefab.m_nodes = new[] { node0, node1, node2 };
                        break;
                    }
            }
        }
        public static void Setup12mStationSteelMeshNoBar(NetInfo prefab, NetInfoVersion version, NetInfo elevatedInfo, NetInfo metroStationInfo)
        {
            var elevatedMaterial = elevatedInfo.m_segments[0].m_material;
            var elevatedLODMaterial = elevatedInfo.m_segments[0].m_lodMaterial;
            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        var segment0 = prefab.m_segments[0].ShallowClone();
                        var segment1 = prefab.m_segments[1];
                        var node0 = prefab.m_nodes[0].ShallowClone();
                        var node1 = prefab.m_nodes[1];
                        var node2 = prefab.m_nodes[2];
                        var node3 = prefab.m_nodes[1].ShallowClone();

                        segment0
                            .SetMeshes
                            (@"Meshes\Ground_Station_Pavement_Steel.obj",
                                @"Meshes\Ground_NoBar_Pavement_LOD.obj");
                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\Rail.obj")
                            .SetConsistentUVs();
                        node0
                            .SetMeshes
                            (@"Meshes\Ground_Nobar_Node_Pavement_Steel.obj",
                                @"Meshes\Ground_Station_Node_Pavement_LOD.obj");
                        node1
                            .SetMeshes
                            (@"Meshes\Elevated_Station_Rail_Node.obj")
                            .SetConsistentUVs();
                        node2
                            .SetMeshes
                            (@"Meshes\Ground_Level_Crossing.obj",
                                @"Meshes\Ground_Level_Crossing_LOD.obj")
                            .SetConsistentUVs();
                        node3
                            .SetMeshes
                            (@"Meshes\Ground_Level_Crossing_Rail_Station.obj")
                            .SetConsistentUVs();

                        segment0.m_material = elevatedMaterial;
                        segment0.m_lodMaterial = elevatedLODMaterial;
                        node0.m_material = elevatedMaterial;
                        node0.m_lodMaterial = elevatedLODMaterial;
                        node2.m_lodMaterial = elevatedLODMaterial;

                        node1.m_flagsForbidden = NetNode.Flags.LevelCrossing;
                        node2.m_flagsRequired = NetNode.Flags.LevelCrossing;
                        node3.m_flagsRequired = NetNode.Flags.LevelCrossing;

                        prefab.m_segments = new[] { segment0, segment1 };
                        prefab.m_nodes = new[] { node0, node1, node2, node3 };
                        break;
                    }
                case NetInfoVersion.Elevated:
                    {
                        var segment0 = prefab.m_segments[0].ShallowClone();
                        var segment1 = prefab.m_segments[1];
                        var node0 = prefab.m_nodes[0].ShallowClone();
                        var node1 = prefab.m_nodes[1];
                        var node2 = prefab.m_nodes[0].ShallowClone();

                        segment0
                            .SetMeshes
                            (@"Meshes\Elevated_Station_Pavement_Steel.obj",
                                @"Meshes\Elevated_Station_Pavement_LOD.obj");
                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\Rail.obj")
                            .SetConsistentUVs();
                        node0
                            .SetMeshes
                            (@"Meshes\Elevated_Nobar_Station_Node_Pavement_Steel.obj",
                                @"Meshes\Elevated_Station_Node_Pavement_LOD.obj");
                        node1
                            .SetMeshes
                            (@"Meshes\Elevated_Station_Rail_Node.obj")
                            .SetConsistentUVs();
                        node2
                            .SetMeshes
                            (@"Meshes\Boosted_Rail_Steel.obj",
                            @"Meshes\Boosted_Rail_Steel_LOD.obj")
                            .SetConsistentUVs();
                        segment0.m_material = elevatedMaterial;
                        segment0.m_lodMaterial = elevatedLODMaterial;
                        node0.m_material = elevatedMaterial;
                        node0.m_lodMaterial = elevatedLODMaterial;

                        prefab.m_segments = new[] { segment0, segment1 };
                        prefab.m_nodes = new[] { node0, node1 };
                        break;
                    }
            }
        }
    }
}

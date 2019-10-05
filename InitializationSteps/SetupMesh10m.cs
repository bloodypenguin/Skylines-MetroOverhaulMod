using MetroOverhaul.NEXT;
using MetroOverhaul.NEXT.Extensions;
using System.Collections.Generic;

namespace MetroOverhaul.InitializationSteps {
    public static partial class SetupMesh
    {
        public static void Setup10mMesh(NetInfo info, NetInfoVersion version, NetInfo elevatedInfo, NetInfo metroInfo)
        {
            var isOneWay = info.name.Contains("One-Way");
            var isLarge = info.name.Contains("Large");
            var width = isLarge ? "18m" : "10m";
            var brElInfo = Prefabs.Find<NetInfo>("Basic Road Elevated");
            var trainTrackInfo = Prefabs.Find<NetInfo>("Train Track");
            var elevatedbrMaterial = brElInfo.m_segments[0].m_lodMaterial;
            var elevatedMaterial = elevatedInfo.m_segments[0].m_material;
            var elevatedLODMaterial = elevatedInfo.m_segments[0].m_lodMaterial;
            var railMaterial = trainTrackInfo.m_segments[1].m_material;
            var railLODMaterial = trainTrackInfo.m_segments[1].m_lodMaterial;
            var nodeList = new List<NetInfo.Node>();
            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        var segment0 = info.m_segments[0].ShallowClone();
                        var segment1 = info.m_segments[1].ShallowClone();
                        var segment2 = info.m_segments[0].ShallowClone();

                        var node0 = info.m_nodes[0].ShallowClone();
                        var node4 = info.m_nodes[0].ShallowClone();

                        nodeList.Add(node0);
                        nodeList.Add(node4);

                        segment0
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Ground_Pavement.obj",
                                $@"Meshes\{width}\Ground_Pavement_LOD.obj");
                        //if (isLarge)
                        //{
                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Rail.obj",
                             $@"Meshes\{width}\Rail_LOD.obj")
                            .SetConsistentUVs();
                        //}
                        //else
                        //{
                        //    segment1
                        //        .SetFlagsDefault()
                        //        .SetMeshes
                        //        ($@"Meshes\{width}\Rail.obj")
                        //        .SetConsistentUVs();
                        //}

                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail.obj",
                            $@"Meshes\{width}\Rail_LOD.obj")
                            .SetConsistentUVs();
                        node0
                            .SetMeshes
                            ($@"Meshes\{width}\Ground_Node_Pavement.obj",
                                $@"Meshes\{width}\Ground_Node_Pavement_LOD.obj");
                        node4
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail_Node.obj", $@"Meshes\{width}\Rail_LOD.obj")
                            .SetConsistentUVs();
                        segment2.m_material = railMaterial;
                        segment2.m_lodMaterial = railLODMaterial;
                        node4.m_material = railMaterial;
                        node4.m_lodMaterial = railLODMaterial;
                        info.m_segments = new[] { segment0, segment1, segment2 };
                        break;
                    }
                case NetInfoVersion.Elevated:
                    {
                        var segment0 = info.m_segments[0].ShallowClone();
                        var segment1 = info.m_segments[0].ShallowClone();
                        var segment2 = info.m_segments[1].ShallowClone();
                        var segment3 = info.m_segments[0].ShallowClone();
                        var segment4 = info.m_segments[1].ShallowClone();
                        var node0 = info.m_nodes[0].ShallowClone();

                        var node2 = info.m_nodes[0].ShallowClone();
                        var node4 = info.m_nodes[0].ShallowClone();

                        nodeList.Add(node0);
                        nodeList.Add(node2);
                        nodeList.Add(node4);

                        segment0
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Elevated_Pavement.obj",
                             $@"Meshes\{width}\Elevated_Pavement_LOD.obj");

                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Boosted_Rail.obj",
                             $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();
                        segment4
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Rail.obj",
                             $@"Meshes\{width}\Rail_LOD.obj")
                            .SetConsistentUVs();

                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail.obj",
                            $@"Meshes\{width}\Rail_LOD.obj")
                            .SetConsistentUVs();
                        segment3
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Elevated_RailGuards.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();
                        node0
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.Transition)
                            .SetMeshes
                            ($@"Meshes\{width}\Elevated_Node_Pavement.obj",
                                $@"Meshes\{width}\Elevated_Node_Pavement_LOD.obj");
                        node2
                            .SetFlags(NetNode.Flags.Transition, NetNode.Flags.None)
                            .SetMeshes
                            ($@"Meshes\{width}\Elevated_Trans_Pavement.obj",
                                $@"Meshes\{width}\Elevated_Node_Pavement_LOD.obj");
                        node4
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail_Node.obj", $@"Meshes\{width}\Rail_LOD.obj")
                            .SetConsistentUVs();
                        segment0.m_material = elevatedMaterial;
                        segment0.m_lodMaterial = elevatedbrMaterial;
                        segment2.m_material = railMaterial;
                        segment2.m_lodMaterial = railLODMaterial;
                        node0.m_material = elevatedMaterial;
                        node0.m_lodMaterial = elevatedbrMaterial;
                        node2.m_material = elevatedMaterial;
                        node2.m_lodMaterial = elevatedLODMaterial;
                        node4.m_material = railMaterial;
                        node4.m_lodMaterial = railLODMaterial;
                        info.m_segments = new[] { segment0, segment1, segment2, segment3, segment4 };
                        break;
                    }
                case NetInfoVersion.Bridge:
                    {
                        var segment0 = info.m_segments[0].ShallowClone();
                        var segment1 = info.m_segments[0].ShallowClone();
                        var segment2 = info.m_segments[1].ShallowClone();
                        var segment3 = info.m_segments[0].ShallowClone();
                        var segment4 = info.m_segments[1].ShallowClone();
                        var node0 = info.m_nodes[0].ShallowClone();

                        var node2 = info.m_nodes[0].ShallowClone();
                        var node4 = info.m_nodes[0].ShallowClone();

                        nodeList.Add(node0);
                        nodeList.Add(node2);
                        nodeList.Add(node4);
                        segment0
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Bridge_Pavement.obj",
                            $@"Meshes\{width}\Bridge_Pavement_LOD.obj")
                            .SetConsistentUVs();

                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Boosted_Rail.obj",
                             $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();

                        segment4
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Rail.obj",
                             $@"Meshes\{width}\Rail_LOD.obj")
                            .SetConsistentUVs();

                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail.obj", 
                            $@"Meshes\{width}\Rail_LOD.obj")
                            .SetConsistentUVs();
                        segment3
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Elevated_RailGuards.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();
                        node0
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.Transition)
                            .SetMeshes
                            ($@"Meshes\{width}\Bridge_Node_Pavement.obj",
                            $@"Meshes\{width}\Bridge_Node_Pavement_LOD.obj")
                            .SetConsistentUVs();

                        node2
                            .SetFlags(NetNode.Flags.Transition, NetNode.Flags.None)
                            .SetMeshes
                            ($@"Meshes\{width}\Bridge_Trans_Pavement.obj",
                            $@"Meshes\{width}\Bridge_Node_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        node4
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail_Node.obj", $@"Meshes\{width}\Rail_LOD.obj")
                            .SetConsistentUVs();
                        segment0.m_material = elevatedMaterial;
                        segment0.m_lodMaterial = elevatedbrMaterial;
                        segment2.m_material = railMaterial;
                        segment2.m_lodMaterial = railLODMaterial;
                        segment3.m_material = elevatedMaterial;
                        segment3.m_lodMaterial = elevatedbrMaterial;
                        node0.m_material = elevatedMaterial;
                        node0.m_lodMaterial = elevatedbrMaterial;
                        node2.m_material = elevatedMaterial;
                        node2.m_lodMaterial = elevatedLODMaterial;
                        node4.m_material = railMaterial;
                        node4.m_lodMaterial = railLODMaterial;
                        info.m_segments = new[] { segment0, segment1, segment2, segment3, segment4 };
                        break;
                    }
                case NetInfoVersion.Slope:
                    {
                        var segment0 = metroInfo.m_segments[0].ShallowClone();
                        var segment1 = info.m_segments[1].ShallowClone();
                        var segment2 = info.m_segments[1].ShallowClone();
                        var segment3 = info.m_segments[0].ShallowClone();
                        var segment4 = info.m_segments[1].ShallowClone();
                        var node0 = metroInfo.m_nodes[0].ShallowClone();

                        var node2 = info.m_nodes[2].ShallowClone();
                        var node5 = info.m_nodes[0].ShallowClone();
                        var node7 = info.m_nodes[0].ShallowClone();

                        nodeList.Add(node0);

                        nodeList.Add(node2);
                        nodeList.Add(node5);
                        nodeList.Add(node7);
                        segment0
                            .SetMeshes
                            ($@"Meshes\{width}\Tunnel_Pavement_Gray.obj",
                            $@"Meshes\{width}\Tunnel_Pavement_Gray_LOD.obj");

                            segment1
                                .SetFlagsDefault()
                                .SetMeshes
                                ($@"Meshes\{width}\Boosted_Rail.obj",
                                 $@"Meshes\{width}\Blank.obj")
                                .SetConsistentUVs();
                            segment4
                                .SetFlagsDefault()
                                .SetMeshes
                                ($@"Meshes\{width}\Rail.obj",
                                 $@"Meshes\{width}\Rail_LOD.obj")
                                .SetConsistentUVs();

                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Slope_Pavement.obj",
                                $@"Meshes\{width}\Slope_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        segment3
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail.obj",
                            $@"Meshes\{width}\Rail_LOD.obj")
                            .SetConsistentUVs();
                        node0
                            .SetMeshes
                            ($@"Meshes\{width}\Tunnel_Pavement_Gray.obj",
                            $@"Meshes\{width}\Tunnel_Pavement_Gray_LOD.obj");
                        node2
                            .SetMeshes
                            ($@"Meshes\{width}\Slope_Node_Pavement.obj",
                                $@"Meshes\{width}\Slope_Node_Pavement_LOD.obj")
                            .SetConsistentUVs();

                        node5
                            .SetMeshes
                            ($@"Meshes\{width}\Slope_U_Node_Pavement.obj",
                                $@"Meshes\{width}\Tunnel_Node_Pavement_LOD.obj")
                            .SetConsistentUVs();

                        node7
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail_Node.obj", $@"Meshes\{width}\Rail_LOD.obj")
                            .SetConsistentUVs();
                        segment2.m_material = elevatedMaterial;
                        segment2.m_lodMaterial = elevatedLODMaterial;
                        segment3.m_material = railMaterial;
                        segment3.m_lodMaterial = railLODMaterial;
                        node2.m_material = elevatedMaterial;
                        node2.m_lodMaterial = elevatedLODMaterial;
                        node5.m_material = elevatedMaterial;
                        node5.m_lodMaterial = elevatedLODMaterial;
                        node7.m_material = railMaterial;
                        node7.m_lodMaterial = railLODMaterial;
                        //node6.m_connectGroup = info.m_connectGroup;
                        info.m_segments = new[] { segment0, segment1, segment2, segment3, segment4 };
                        break;
                    }
                case NetInfoVersion.Tunnel:
                    {
                        var segment0 = metroInfo.m_segments[0].ShallowClone();
                        var segment1 = info.m_segments[0].ShallowClone();
                        var segment2 = info.m_segments[0].ShallowClone();
                        var segment3 = info.m_segments[0].ShallowClone();
                        var segment4 = info.m_segments[0].ShallowClone();
                        var node0 = metroInfo.m_nodes[0].ShallowClone();
                        var node1 = info.m_nodes[0].ShallowClone();
                        var node3 = info.m_nodes[0].ShallowClone();

                        nodeList.Add(node0);
                        nodeList.Add(node1);
                        nodeList.Add(node3);
                        segment0
                            .SetMeshes
                            ($@"Meshes\{width}\Tunnel_Pavement_Gray.obj",
                            $@"Meshes\{width}\Tunnel_Pavement_Gray_LOD.obj");
                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Tunnel_Pavement.obj",
                                $@"Meshes\{width}\Tunnel_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Boosted_Rail.obj");
                        segment3
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail.obj",
                            $@"Meshes\{width}\Rail_LOD.obj")
                            .SetConsistentUVs();
                        segment4
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Rail.obj");
                        node0
                            .SetMeshes
                            ($@"Meshes\{width}\Tunnel_Pavement_Gray.obj",
                            $@"Meshes\{width}\Tunnel_Pavement_Gray_LOD.obj");
                        node1
                            .SetMeshes
                            ($@"Meshes\{width}\Tunnel_Node_Pavement.obj",
                                $@"Meshes\{width}\Tunnel_Node_Pavement_LOD.obj")
                            .SetConsistentUVs();

                        node3
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail_Node.obj", $@"Meshes\{width}\Rail_LOD.obj")
                            .SetConsistentUVs();

                        RoadHelper.HandleAsymSegmentFlags(segment1);
                        segment1.m_material = elevatedbrMaterial;
                        segment1.m_lodMaterial = elevatedLODMaterial;
                        segment2.m_material = elevatedbrMaterial;
                        segment2.m_lodMaterial = elevatedLODMaterial;
                        segment3.m_material = railMaterial;
                        segment3.m_lodMaterial = railLODMaterial;
                        segment4.m_material = railMaterial;
                        segment4.m_lodMaterial = railLODMaterial;
                        node1.m_material = elevatedMaterial;
                        node1.m_lodMaterial = elevatedLODMaterial;
                        node3.m_material = railMaterial;
                        node3.m_lodMaterial = railLODMaterial;
                        info.m_segments = new[] { segment0, segment1, segment2, segment3, segment4 };
                        break;
                    }
            }
            for (int i = 0; i < nodeList.Count; i++)
            {
                nodeList[i].m_flagsForbidden |= NetNode.Flags.LevelCrossing;
            }
            nodeList.AddRange(SetupMeshUtil.GenerateSplitTracksAndLevelCrossings(info, version));
            info.m_nodes = nodeList.ToArray();
        }

        //mind changed segment and node indices! (after Setup10mMesh)
        public static void Setup10mBarMesh(NetInfo info, NetInfoVersion version, NetInfo elevatedInfo)
        {
            var width = info.name.Contains("Large") ? "18m" : "10m";
            var elevatedMaterial = elevatedInfo.m_segments[0].m_material;
            var elevatedLODMaterial = elevatedInfo.m_segments[0].m_lodMaterial;

            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        var segment = info.m_segments[0].ShallowClone();
                        var node = info.m_nodes[0].ShallowClone();
                        segment
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Ground_Fence.obj",
                            $@"Meshes\{width}\Ground_Fence_LOD.obj");
                        node
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.LevelCrossing)
                            .SetMeshes
                            ($@"Meshes\{width}\Ground_Node_Fence.obj",
                            $@"Meshes\{width}\Ground_Node_Fence_LOD.obj");

                        segment.m_material = elevatedMaterial;
                        segment.m_lodMaterial = elevatedLODMaterial;
                        node.m_material = elevatedMaterial;
                        node.m_lodMaterial = elevatedLODMaterial;

                        info.m_segments = info.AddSegments(segment);
                        info.m_nodes = info.AddNodes(node);
                        break;
                    }
                case NetInfoVersion.Elevated:
                case NetInfoVersion.Bridge:
                    {
                        var segment = info.m_segments[0].ShallowClone();
                        var node = info.m_nodes[0].ShallowClone();
                        segment
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Elevated_Fence.obj",
                            $@"Meshes\{width}\Blank.obj");
                        node
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.None)
                            .SetMeshes
                            ($@"Meshes\{width}\Elevated_Node_Fence.obj",
                            $@"Meshes\{width}\Blank.obj");

                        segment.m_material = elevatedMaterial;
                        segment.m_lodMaterial = elevatedLODMaterial;
                        node.m_material = elevatedMaterial;
                        node.m_lodMaterial = elevatedLODMaterial;

                        info.m_segments = info.AddSegments(segment);
                        info.m_nodes = info.AddNodes(node);
                        break;
                    }
                case NetInfoVersion.Slope:
                    {
                        var node = info.m_nodes[0].ShallowClone();
                        node
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.Underground)
                            .SetMeshes
                            ($@"Meshes\{width}\Slope_Node_Fence.obj",
                            $@"Meshes\{width}\Slope_Node_Fence_LOD.obj");

                        node.m_material = elevatedMaterial;
                        node.m_lodMaterial = elevatedLODMaterial;

                        info.m_nodes = info.AddNodes(node);
                        break;
                    }
            }
        }

        public static void Setup10mStationMesh(NetInfo prefab, NetInfoVersion version, NetInfo elevatedInfo, NetInfo metroStationInfo)
        {
            var trainTrackInfo = Prefabs.Find<NetInfo>("Train Track");
            var elevatedMaterial = elevatedInfo.m_segments[0].m_material;
            var elevatedLODMaterial = elevatedInfo.m_segments[0].m_lodMaterial;
            var railMaterial = trainTrackInfo.m_segments[1].m_material;
            var railLODMaterial = trainTrackInfo.m_segments[1].m_lodMaterial;
            var width = prefab.name.Contains("Large") ? "18m" : "10m";
            var nodeList = new List<NetInfo.Node>();
            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        var segment0 = prefab.m_segments[0].ShallowClone();
                        var segment1 = prefab.m_segments[1].ShallowClone();
                        var segment2 = prefab.m_segments[0].ShallowClone();
                        var node0 = prefab.m_nodes[0].ShallowClone();
                        var node4 = prefab.m_nodes[0].ShallowClone();

                        nodeList.Add(node0);
                        nodeList.Add(node4);
                        segment0
                            .SetMeshes
                            ($@"Meshes\{width}\Ground_Station_Pavement.obj",
                                $@"Meshes\{width}\Ground_Pavement_LOD.obj");
                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Rail.obj",
                             $@"Meshes\{width}\Rail_LOD.obj")
                            .SetConsistentUVs();
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail.obj",
                            $@"Meshes\{width}\Rail_LOD.obj")
                            .SetConsistentUVs();
                        node0
                            .SetMeshes
                            ($@"Meshes\{width}\Ground_Node_Pavement.obj",
                                $@"Meshes\{width}\Ground_Node_Pavement_LOD.obj");

                        node4
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail_Node.obj",
                            $@"Meshes\{width}\Rail_LOD.obj")
                            .SetConsistentUVs();

                        segment0.m_material = elevatedMaterial;
                        segment0.m_lodMaterial = elevatedLODMaterial;
                        segment2.m_material = railMaterial;
                        segment2.m_lodMaterial = railLODMaterial;
                        node0.m_material = elevatedMaterial;
                        node0.m_lodMaterial = elevatedLODMaterial;
                        node4.m_material = railMaterial;
                        node4.m_lodMaterial = railLODMaterial;

                        prefab.m_segments = new[] { segment0, segment1, segment2 };
                        break;
                    }
                case NetInfoVersion.Elevated:
                    {
                        var segment0 = prefab.m_segments[0].ShallowClone();
                        var segment1 = prefab.m_segments[0].ShallowClone();
                        var segment2 = prefab.m_segments[0].ShallowClone();
                        var segment3 = prefab.m_segments[1].ShallowClone();
                        var node0 = prefab.m_nodes[0].ShallowClone();
                        var node2 = prefab.m_nodes[0].ShallowClone();

                        nodeList.Add(node0);
                        nodeList.Add(node2);

                        segment0
                            .SetMeshes
                            ($@"Meshes\{width}\Elevated_Station_Pavement.obj",
                             $@"Meshes\{width}\Elevated_Station_Pavement_LOD.obj");
                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Boosted_Station_Rail.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail.obj",
                            $@"Meshes\{width}\Rail_LOD.obj")
                            .SetConsistentUVs();
                        segment3
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Rail.obj",
                            $@"Meshes\{width}\Rail_LOD.obj")
                            .SetConsistentUVs();
                        node0
                            .SetMeshes
                            ($@"Meshes\{width}\Elevated_Station_Node_Pavement.obj",
                             $@"Meshes\{width}\Elevated_Station_Node_Pavement_LOD.obj");

                        node2
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail_Node.obj", $@"Meshes\{width}\Rail_LOD.obj")
                            .SetConsistentUVs();
                        segment0.m_material = elevatedMaterial;
                        segment0.m_lodMaterial = elevatedLODMaterial;
                        segment1.m_material = elevatedMaterial;
                        segment1.m_lodMaterial = elevatedLODMaterial;
                        segment2.m_material = railMaterial;
                        segment2.m_lodMaterial = railLODMaterial;
                        node0.m_material = elevatedMaterial;
                        node0.m_lodMaterial = elevatedLODMaterial;
                        node2.m_material = railMaterial;
                        node2.m_lodMaterial = railLODMaterial;
                        prefab.m_segments = new[] { segment0, segment1, segment2, segment3 };
                        break;
                    }
                case NetInfoVersion.Tunnel:
                    {
                        var segment0 = metroStationInfo.m_segments[0].ShallowClone();
                        var segment1 = metroStationInfo.m_segments[0].ShallowClone();
                        var segment2 = metroStationInfo.m_segments[0].ShallowClone();
                        var segment3 = metroStationInfo.m_segments[0].ShallowClone();
                        var segment4 = metroStationInfo.m_segments[0].ShallowClone();
                        var node0 = metroStationInfo.m_nodes[0].ShallowClone();
                        var node1 = metroStationInfo.m_nodes[0].ShallowClone();
                        var node3 = metroStationInfo.m_nodes[0].ShallowClone();

                        nodeList.Add(node0);
                        nodeList.Add(node1);
                        nodeList.Add(node3);
                        segment0
                            .SetMeshes
                            ($@"Meshes\{width}\Tunnel_Station_Pavement_Gray.obj",
                            $@"Meshes\{width}\Tunnel_Station_Pavement_Gray_LOD.obj");
                        segment1
                            .SetMeshes
                            ($@"Meshes\{width}\Tunnel_Station_Pavement.obj",
                                $@"Meshes\{width}\Tunnel_Pavement_LOD.obj")
                                .SetConsistentUVs();
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Boosted_Station_Rail.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();
                        segment3
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail.obj",
                            $@"Meshes\{width}\Rail_LOD.obj")
                            .SetConsistentUVs();
                        segment4
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Station_Rail.obj",
                            $@"Meshes\{width}\Rail_LOD.obj")
                            .SetConsistentUVs();
                        node0
                            .SetMeshes
                            ($@"Meshes\{width}\Tunnel_Station_Pavement_Gray.obj",
                            $@"Meshes\{width}\Tunnel_Station_Pavement_Gray_LOD.obj");
                        node1
                            .SetMeshes
                            ($@"Meshes\{width}\Tunnel_Station_Node_Pavement.obj",
                                $@"Meshes\{width}\Tunnel_Node_Pavement_LOD.obj")
                                .SetConsistentUVs();
                        node3
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail_Node.obj", $@"Meshes\{width}\Rail_LOD.obj")
                            .SetConsistentUVs();
                        segment1.m_material = elevatedMaterial;
                        segment1.m_lodMaterial = elevatedLODMaterial;
                        segment2.m_material = elevatedMaterial;
                        segment2.m_lodMaterial = elevatedLODMaterial;
                        segment3.m_material = railMaterial;
                        segment3.m_lodMaterial = railLODMaterial;
                        segment4.m_material = railMaterial;
                        segment4.m_lodMaterial = railLODMaterial;
                        node1.m_material = elevatedMaterial;
                        node1.m_lodMaterial = elevatedLODMaterial;

                        node3.m_material = railMaterial;
                        node3.m_lodMaterial = railLODMaterial;
                        prefab.m_segments = new[] { segment0, segment1, segment2, segment3, segment4 };
                        break;
                    }
            }
            
            //if (version == NetInfoVersion.Ground || version == NetInfoVersion.Elevated) {
            //    nodeList[0].m_flagsForbidden |= NetNode.Flags.End;
            //}
            for (int i = 0; i < nodeList.Count; i++)
            {
                nodeList[i].m_flagsForbidden |= NetNode.Flags.LevelCrossing;
            }
            nodeList.AddRange(SetupMeshUtil.GenerateSplitTracksAndLevelCrossings(prefab, version));
            prefab.m_nodes = nodeList.ToArray();
        }
    }
}

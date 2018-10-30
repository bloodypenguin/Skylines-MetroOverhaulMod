using MetroOverhaul.NEXT;
using MetroOverhaul.NEXT.Extensions;
using Next;
using System.Collections.Generic;
using System.Linq;

namespace MetroOverhaul.InitializationSteps
{
    public static partial class SetupMesh
    {
        public static void Setup10mMesh(NetInfo info, NetInfoVersion version, NetInfo elevatedInfo, NetInfo metroInfo)
        {
            var isOneWay = info.name.Contains("One-Way");
            var isLarge = info.name.Contains("Large");
            var width = isLarge ? "18m" : "10m";
            var brElInfo = Prefabs.Find<NetInfo>("Basic Road Elevated");
            var elevatedbrMaterial = brElInfo.m_segments[0].m_lodMaterial;
            var elevatedMaterial = elevatedInfo.m_segments[0].m_material;
            var elevatedLODMaterial = elevatedInfo.m_segments[0].m_lodMaterial;
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
                        if (isLarge)
                        {
                            segment1
                                .SetFlagsDefault()
                                .SetMeshes
                                ($@"Meshes\{width}\Rail.obj",
                                 $@"Meshes\{width}\Rail_LOD.obj")
                                .SetConsistentUVs();
                        }
                        else
                        {
                            segment1
                                .SetFlagsDefault()
                                .SetMeshes
                                ($@"Meshes\{width}\Rail.obj")
                                .SetConsistentUVs();
                        }

                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();
                        node0
                            .SetMeshes
                            ($@"Meshes\{width}\Ground_Node_Pavement.obj",
                                $@"Meshes\{width}\Ground_Node_Pavement_LOD.obj");
                        node4
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.None)
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail_Node.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();

                        info.m_segments = new[] { segment0, segment1, segment2 };
                        break;
                    }
                case NetInfoVersion.Elevated:
                    {
                        var segment0 = info.m_segments[0].ShallowClone();
                        var segment1 = info.m_segments[1].ShallowClone();
                        var segment2 = info.m_segments[0].ShallowClone();
                        var segment3 = info.m_segments[0].ShallowClone();
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
                                $@"Meshes\{width}\Elevated_Pavement_LOD.obj")
                            .SetConsistentUVs(); //ehem
                        if (isLarge)
                        {
                            segment1
                                .SetFlagsDefault()
                                .SetMeshes
                                ($@"Meshes\{width}\Boosted_Rail.obj",
                                 $@"Meshes\{width}\Rail_LOD.obj")
                                .SetConsistentUVs();
                        }
                        else
                        {
                            segment1
                                .SetFlagsDefault()
                                .SetMeshes
                                ($@"Meshes\{width}\Boosted_Rail.obj")
                                .SetConsistentUVs();
                        }
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail.obj", $@"Meshes\{width}\Blank.obj")
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
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail_Node.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();

                        segment0.m_material = elevatedMaterial;
                        segment0.m_lodMaterial = elevatedbrMaterial;
                        segment3.m_material = elevatedMaterial;
                        segment3.m_lodMaterial = elevatedLODMaterial;

                        node0.m_material = elevatedMaterial;
                        node0.m_lodMaterial = elevatedbrMaterial;
                        node2.m_material = elevatedMaterial;
                        node2.m_lodMaterial = elevatedLODMaterial;

                        info.m_segments = new[] { segment0, segment1, segment2, segment3 };
                        break;
                    }
                case NetInfoVersion.Bridge:
                    {
                        var segment0 = info.m_segments[0].ShallowClone();
                        var segment1 = info.m_segments[1].ShallowClone();
                        var segment2 = info.m_segments[0].ShallowClone();
                        var segment3 = info.m_segments[0].ShallowClone();
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
                        if (isLarge)
                        {
                            segment1
                                .SetFlagsDefault()
                                .SetMeshes
                                ($@"Meshes\{width}\Boosted_Rail.obj",
                                 $@"Meshes\{width}\Rail_LOD.obj")
                                .SetConsistentUVs();
                        }
                        else
                        {
                            segment1
                                .SetFlagsDefault()
                                .SetMeshes
                                ($@"Meshes\{width}\Boosted_Rail.obj")
                                .SetConsistentUVs();
                        }
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail.obj", $@"Meshes\{width}\Blank.obj")
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
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail_Node.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();
                        segment0.m_material = elevatedMaterial;
                        segment0.m_lodMaterial = elevatedbrMaterial;
                        segment3.m_material = elevatedMaterial;
                        segment3.m_lodMaterial = elevatedLODMaterial;
                        node0.m_material = elevatedMaterial;
                        node0.m_lodMaterial = elevatedbrMaterial;
                        node2.m_material = elevatedMaterial;
                        node2.m_lodMaterial = elevatedLODMaterial;
                        info.m_segments = new[] { segment0, segment1, segment2, segment3 };
                        break;
                    }
                case NetInfoVersion.Slope:
                    {
                        var segment0 = metroInfo.m_segments[0].ShallowClone();
                        var segment1 = info.m_segments[1].ShallowClone();
                        var segment2 = info.m_segments[3].ShallowClone();
                        var segment3 = info.m_segments[1].ShallowClone();
                        var node0 = metroInfo.m_nodes[0].ShallowClone();
                        
                        var node2 = info.m_nodes[3].ShallowClone();
                        var node5 = info.m_nodes[0].ShallowClone();
                        var node6 = info.m_nodes[1].ShallowClone();
                        var node7 = info.m_nodes[3].ShallowClone();

                        nodeList.Add(node0);

                        nodeList.Add(node2);
                        nodeList.Add(node5);
                        nodeList.Add(node6);
                        nodeList.Add(node7);
                        if (isLarge)
                        {
                            segment1
                                .SetFlagsDefault()
                                .SetMeshes
                                ($@"Meshes\{width}\Boosted_Rail.obj",
                                 $@"Meshes\{width}\Rail_LOD.obj")
                                .SetConsistentUVs();
                        }
                        else
                        {
                            segment1
                                .SetFlagsDefault()
                                .SetMeshes
                                ($@"Meshes\{width}\Boosted_Rail.obj")
                                .SetConsistentUVs();
                        }
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\Slope_Pavement.obj",
                                $@"Meshes\{width}\Slope_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        segment3
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();

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
                        node6
                            .SetFlags(NetNode.Flags.Underground, NetNode.Flags.None)
                            .SetMeshes
                            ($@"Meshes\{width}\Boosted_Rail.obj")
                            .SetConsistentUVs();
                        node7
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail_Node.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();
                        segment2.m_material = elevatedMaterial;
                        segment2.m_lodMaterial = elevatedLODMaterial;
                        segment3.m_material = elevatedMaterial;
                        segment3.m_lodMaterial = elevatedLODMaterial;
                        node2.m_material = elevatedMaterial;
                        node2.m_lodMaterial = elevatedLODMaterial;
                        node5.m_material = elevatedMaterial;
                        node5.m_lodMaterial = elevatedLODMaterial;
                        node6.m_connectGroup = info.m_connectGroup;
                        info.m_segments = new[] { segment0, segment1, segment2, segment3 };
                        break;
                    }
                case NetInfoVersion.Tunnel:
                    {
                        var segment0 = metroInfo.m_segments[0].ShallowClone();
                        var segment1 = info.m_segments[0].ShallowClone();
                        var segment2 = info.m_segments[0].ShallowClone();
                        var segment3 = info.m_segments[0].ShallowClone();
                        var node0 = metroInfo.m_nodes[0].ShallowClone();
                        var node1 = info.m_nodes[0].ShallowClone();
                        var node3 = info.m_nodes[0].ShallowClone();

                        nodeList.Add(node0);
                        nodeList.Add(node1);
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
                            ($@"Meshes\{width}\ThirdRail.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();
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
                            ($@"Meshes\{width}\ThirdRail_Node.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();
                        segment1.m_material = elevatedbrMaterial;
                        segment1.m_lodMaterial = elevatedLODMaterial;
                        RoadHelper.HandleAsymSegmentFlags(segment1);
                        segment2.m_material = elevatedbrMaterial;
                        segment2.m_lodMaterial = elevatedLODMaterial;
                        segment3.m_material = elevatedbrMaterial;
                        segment3.m_lodMaterial = elevatedLODMaterial;
                        node1.m_material = elevatedMaterial;
                        node1.m_lodMaterial = elevatedLODMaterial;
                        node3.m_material = elevatedMaterial;
                        node3.m_lodMaterial = elevatedLODMaterial;
                        info.m_segments = new[] { segment0, segment1, segment2, segment3 };
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
            var elevatedMaterial = elevatedInfo.m_segments[0].m_material;
            var elevatedLODMaterial = elevatedInfo.m_segments[0].m_lodMaterial;
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
                            ($@"Meshes\{width}\Rail.obj")
                            .SetConsistentUVs();
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();
                        node0
                            .SetMeshes
                            ($@"Meshes\{width}\Ground_Node_Pavement.obj",
                                $@"Meshes\{width}\Ground_Node_Pavement_LOD.obj");

                        node4
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail_Node.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();
                        segment0.m_material = elevatedMaterial;
                        segment0.m_lodMaterial = elevatedLODMaterial;
                        node0.m_material = elevatedMaterial;
                        node0.m_lodMaterial = elevatedLODMaterial;

                        prefab.m_segments = new[] { segment0, segment1, segment2 };
                        break;
                    }
                case NetInfoVersion.Elevated:
                    {
                        var segment0 = prefab.m_segments[0].ShallowClone();
                        var segment1 = prefab.m_segments[1].ShallowClone();
                        var segment2 = prefab.m_segments[0].ShallowClone();
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
                            ($@"Meshes\10m\Station_Boosted_Rail.obj")
                            .SetConsistentUVs();
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\10m\Station_ThirdRail.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();
                        node0
                            .SetMeshes
                            ($@"Meshes\{width}\Elevated_Station_Node_Pavement.obj",
                             $@"Meshes\{width}\Elevated_Station_Node_Pavement_LOD.obj");

                        node2
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail_Node.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();
                        segment0.m_material = elevatedMaterial;
                        segment0.m_lodMaterial = elevatedLODMaterial;
                        node0.m_material = elevatedMaterial;
                        node0.m_lodMaterial = elevatedLODMaterial;
                        prefab.m_segments = new[] { segment0, segment1, segment2 };
                        break;
                    }
                case NetInfoVersion.Tunnel:
                    {
                        var segment0 = metroStationInfo.m_segments[0].ShallowClone();
                        var segment1 = metroStationInfo.m_segments[0].ShallowClone();
                        var segment2 = metroStationInfo.m_segments[0].ShallowClone();
                        var segment3 = metroStationInfo.m_segments[0].ShallowClone();
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
                            ($@"Meshes\{width}\Boosted_Station_Rail.obj")
                            .SetConsistentUVs();
                        segment3
                            .SetFlagsDefault()
                            .SetMeshes
                            ($@"Meshes\{width}\ThirdRail.obj", $@"Meshes\{width}\Blank.obj")
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
                            ($@"Meshes\{width}\ThirdRail_Node.obj", $@"Meshes\{width}\Blank.obj")
                            .SetConsistentUVs();
                        segment1.m_material = elevatedMaterial;
                        segment1.m_lodMaterial = elevatedLODMaterial;
                        segment2.m_material = elevatedMaterial;
                        segment2.m_lodMaterial = elevatedLODMaterial;
                        segment3.m_material = elevatedMaterial;
                        segment3.m_lodMaterial = elevatedLODMaterial;
                        node1.m_material = elevatedMaterial;
                        node1.m_lodMaterial = elevatedLODMaterial;

                        node3.m_material = elevatedMaterial;
                        node3.m_lodMaterial = elevatedLODMaterial;
                        prefab.m_segments = new[] { segment0, segment1, segment2, segment3 };
                        break;
                    }
            }
            for (int i = 0; i < nodeList.Count; i++)
            {
                nodeList[i].m_flagsForbidden |= NetNode.Flags.LevelCrossing;
            }
            nodeList.AddRange(SetupMeshUtil.GenerateSplitTracksAndLevelCrossings(prefab, version));
            prefab.m_nodes = nodeList.ToArray();
        }
    }
}

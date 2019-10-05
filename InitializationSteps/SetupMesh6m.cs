using System;
using System.Collections.Generic;
using MetroOverhaul.NEXT;
using MetroOverhaul.NEXT.Extensions;

namespace MetroOverhaul.InitializationSteps {
    public static partial class SetupMesh
    {
        public static void Setup6mMesh(this NetInfo info, NetInfoVersion version)
        {
            var ttInfo = Prefabs.Find<NetInfo>("Train Track");
            var brElInfo = Prefabs.Find<NetInfo>("Basic Road Elevated");
            //var ttElInfo = Prefabs.Find<NetInfo>("Train Track Elevated");
            var defaultMaterial = brElInfo.m_segments[0].m_material;
            var defaultLODMaterial = brElInfo.m_segments[0].m_lodMaterial;
            var railMaterial = ttInfo.m_segments[1].m_material;
            var railLODMaterial = ttInfo.m_segments[1].m_lodMaterial;
            var isTwoWay = info.name.Contains("Two-Way");
            var nodeList = new List<NetInfo.Node>();
            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        var segments0 = info.m_segments[0].ShallowClone();
                        var segments1 = info.m_segments[1].ShallowClone();
                        var segments3 = info.m_segments[1].ShallowClone();
                        var nodes0 = info.m_nodes[0].ShallowClone();
                        nodeList.Add(nodes0);

                        segments0
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Ground_Pavement.obj",
                            @"Meshes\6m\Ground_Pavement_LOD.obj");

                        segments1
                            .SetFlagsDefault()
                           .SetMeshes
                           (@"Meshes\6m\Ground_Rail.obj",
                           @"Meshes\6m\Rail_LOD.obj");

                        segments3
                            .SetFlagsDefault()
                           .SetMeshes
                           (@"Meshes\6m\ThirdRail.obj",
                           $@"Meshes\6m\Rail_LOD.obj")
                           .SetConsistentUVs();

                        nodes0
                            .SetMeshes
                           (@"Meshes\6m\Ground_Node_Pavement.obj",
                           @"Meshes\6m\Ground_Node_Pavement_LOD.obj");

                        RoadHelper.HandleAsymSegmentFlags(segments3);
                        info.m_segments = new[] { segments0, segments1, /*segments2*/segments3 };
                        break;
                    }

                case NetInfoVersion.Elevated:
                    {
                        var segments0 = info.m_segments[0].ShallowClone();
                        var segments1 = info.m_segments[0].ShallowClone();
                        var segments2 = info.m_segments[1].ShallowClone();
                        var segments3 = info.m_segments[0].ShallowClone();
                        var segments4 = info.m_segments[0].ShallowClone();
                        var nodes0 = info.m_nodes[0].ShallowClone();
                        var nodes2 = info.m_nodes[0].ShallowClone();
                        nodeList.Add(nodes0);
                        nodeList.Add(nodes2);
                        segments0
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Elevated_Pavement.obj",
                            @"Meshes\6m\Elevated_Pavement_LOD.obj");

                        segments1
                            .SetFlagsDefault()
                              .SetMeshes
                              (@"Meshes\6m\Boosted_Rail.obj", 
                               @"Meshes\10m\Blank.obj");
                        segments2
                            .SetFlagsDefault()
                              .SetMeshes
                              (@"Meshes\6m\Rail.obj",
                              @"Meshes\6m\Rail_LOD.obj");
                        segments3
                            .SetFlagsDefault()
                              .SetMeshes
                              (@"Meshes\6m\ThirdRail.obj",
                              $@"Meshes\6m\Rail_LOD.obj")
                              .SetConsistentUVs();
                        segments4
                            .SetFlagsDefault()
                              .SetMeshes
                              (@"Meshes\6m\Elevated_RailGuards.obj",
                              @"Meshes\10m\Blank.obj")
                              .SetConsistentUVs();
                        nodes0
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.Transition)
                            .SetMeshes
                            (@"Meshes\6m\Elevated_Pavement.obj",
                            @"Meshes\6m\Elevated_Node_Pavement_LOD.obj");

                        nodes2
                            .SetFlags(NetNode.Flags.Transition, NetNode.Flags.None)
                            .SetMeshes
                            (@"Meshes\6m\Elevated_Trans_Pavement.obj",
                            @"Meshes\6m\Elevated_Node_Pavement_LOD.obj");
                        RoadHelper.HandleAsymSegmentFlags(segments3);
                        segments0.m_material = defaultMaterial;
                        segments0.m_lodMaterial = defaultLODMaterial;
                        segments3.m_material = railMaterial;
                        segments3.m_lodMaterial = railLODMaterial;
                        segments4.m_material = defaultMaterial;
                        segments4.m_lodMaterial = defaultLODMaterial;
                        nodes0.m_material = defaultMaterial;
                        nodes0.m_lodMaterial = defaultLODMaterial;
                        nodes2.m_material = defaultMaterial;
                        nodes2.m_lodMaterial = defaultLODMaterial;
                        info.m_segments = new[] { segments0, segments1,segments2, segments3, segments4 };
                        break;
                    }
                case NetInfoVersion.Bridge:
                    {
                        var segments0 = info.m_segments[0].ShallowClone();
                        var segments1 = info.m_segments[0].ShallowClone();
                        var segments2 = info.m_segments[1].ShallowClone();
                        var segments3 = info.m_segments[1].ShallowClone();
                        var segments4 = info.m_segments[0].ShallowClone();
                        var nodes0 = info.m_nodes[0].ShallowClone();
                        var nodes2 = info.m_nodes[0].ShallowClone();
                        nodeList.Add(nodes0);
                        nodeList.Add(nodes2);
                        segments0
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Bridge_Pavement.obj",
                            @"Meshes\6m\Bridge_Pavement_LOD.obj");

                        segments1
                            .SetFlagsDefault()
                              .SetMeshes
                              (@"Meshes\6m\Boosted_Rail.obj",
                               @"Meshes\10m\Blank.obj");
                        segments2
                            .SetFlagsDefault()
                              .SetMeshes
                              (@"Meshes\6m\Rail.obj",
                              @"Meshes\6m\Rail_LOD.obj");
                        segments3
                            .SetFlagsDefault()
                              .SetMeshes
                              (@"Meshes\6m\ThirdRail.obj",
                              $@"Meshes\6m\Rail_LOD.obj")
                              .SetConsistentUVs();
                        segments4
                            .SetFlagsDefault()
                              .SetMeshes
                              (@"Meshes\6m\Elevated_RailGuards.obj",
                              @"Meshes\10m\Blank.obj")
                              .SetConsistentUVs();
                        nodes0
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.Transition)
                            .SetMeshes
                              (@"Meshes\6m\Bridge_Node_Pavement.obj",
                              @"Meshes\6m\Elevated_Node_Pavement_LOD.obj")
                              .SetConsistentUVs();

                        nodes2
                            .SetFlags(NetNode.Flags.Transition, NetNode.Flags.None)
                            .SetMeshes
                              (@"Meshes\6m\Bridge_Trans_Pavement.obj",
                              @"Meshes\6m\Elevated_Node_Pavement_LOD.obj")
                              .SetConsistentUVs();
                        var segmentNormals = segments0.m_mesh.normals;
                        var segmentVertices = segments0.m_mesh.vertices;
                        var colors = new List<UnityEngine.Color>();
                        var colors32 = new List<UnityEngine.Color32>();

                        for (int i = 0; i < segments0.m_mesh.vertexCount; i++)
                        {
                            if (segmentNormals[i].y == 1 && segmentVertices[i].y == 0)
                            {
                                colors.Add(new UnityEngine.Color(255, 255, 255, 255));
                                colors32.Add(new UnityEngine.Color32(255, 255, 255, 255));
                            }
                            else
                            {
                                colors.Add(new UnityEngine.Color(255, 0, 255, 255));
                                colors32.Add(new UnityEngine.Color32(255, 0, 255, 255));
                            }
                        }
                        segments0.m_mesh.colors = colors.ToArray();
                        segments0.m_mesh.colors32 = colors32.ToArray();

                        segmentNormals = segments0.m_lodMesh.normals;
                        segmentVertices = segments0.m_lodMesh.vertices;
                        colors = new List<UnityEngine.Color>();
                        colors32 = new List<UnityEngine.Color32>();

                        for (int i = 0; i < segments0.m_lodMesh.vertexCount; i++)
                        {
                            if (segmentNormals[i].y == 1 && Math.Abs(segmentVertices[i].x) <= 3 && segmentVertices[i].y < 3)
                            {
                                colors.Add(new UnityEngine.Color(255, 255, 255, 255));
                                colors32.Add(new UnityEngine.Color32(255, 255, 255, 255));
                            }
                            else
                            {
                                colors.Add(new UnityEngine.Color(255, 0, 255, 255));
                                colors32.Add(new UnityEngine.Color32(255, 0, 255, 255));
                            }
                        }
                        segments0.m_lodMesh.colors = colors.ToArray();
                        segments0.m_lodMesh.colors32 = colors32.ToArray();
                        RoadHelper.HandleAsymSegmentFlags(segments3);
                        segments0.m_material = defaultMaterial;
                        segments0.m_lodMaterial = defaultLODMaterial;
                        segments3.m_material = railMaterial;
                        segments3.m_lodMaterial = railLODMaterial;
                        segments4.m_material = defaultMaterial;
                        segments4.m_lodMaterial = defaultLODMaterial;
                        nodes0.m_material = defaultMaterial;
                        nodes0.m_lodMaterial = defaultLODMaterial;
                        nodes2.m_material = defaultMaterial;
                        nodes2.m_lodMaterial = defaultLODMaterial;
                        info.m_segments = new[] { segments0, segments1, segments2, segments3, segments4 };
                        break;
                    }

                case NetInfoVersion.Slope:
                    {
                        var segments0 = info.m_segments[0].ShallowClone();
                        var segments1 = info.m_segments[0].ShallowClone();
                        var segments2 = info.m_segments[1].ShallowClone();
                        var segments3 = info.m_segments[0].ShallowClone();
                        var segments4 = info.m_segments[1].ShallowClone();
                        var nodes0 = info.m_nodes[0].ShallowClone();
                        //var nodes2 wires
                        var nodes3 = info.m_nodes[2].ShallowClone();
                        var nodes9 = info.m_nodes[0].ShallowClone();

                        //var nodes10 = info.m_nodes[1].ShallowClone();
                        nodeList.Add(nodes0);
                        nodeList.Add(nodes3);
                        nodeList.Add(nodes9);
                        segments0
                            .SetMeshes
                            ($@"Meshes\6m\Tunnel_Pavement_Gray.obj",
                            $@"Meshes\6m\Tunnel_Pavement_Gray_LOD.obj");
                        segments1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Boosted_Rail.obj",
                            @"Meshes\10m\Blank.obj")
                            .SetConsistentUVs();
                        segments2
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Rail.obj",
                            @"Meshes\6m\Rail_LOD.obj")
                            .SetConsistentUVs();
                        segments3
                            .SetFlagsDefault()
                              .SetMeshes
                              (@"Meshes\6m\Slope_Pavement.obj",
                              @"Meshes\6m\Slope_Pavement_LOD.obj")
                              .SetConsistentUVs();

                        segments4
                            .SetFlagsDefault()
                              .SetMeshes
                              (@"Meshes\6m\ThirdRail.obj",
                              $@"Meshes\6m\Rail_LOD.obj")
                              .SetConsistentUVs();
                        nodes0
                            .SetMeshes
                            (@"Meshes\6m\Tunnel_Pavement_Gray.obj",
                            @"Meshes\6m\Tunnel_Pavement_Gray_LOD.obj");

                        nodes3
                            .SetMeshes
                              (@"Meshes\6m\Slope_Node_Pavement.obj",
                              @"Meshes\6m\Ground_Node_Pavement_LOD.obj");
                        nodes9
                            .SetMeshes
                            (@"Meshes\6m\Slope_U_Node_Pavement.obj",
                                @"Meshes\6m\Tunnel_Node_Pavement_LOD.obj")
                            .SetConsistentUVs();

                        RoadHelper.HandleAsymSegmentFlags(segments4);
                        segments1.m_material = defaultMaterial;
                        segments1.m_lodMaterial = defaultLODMaterial;
                        segments3.m_material = defaultMaterial;
                        segments3.m_lodMaterial = defaultLODMaterial;
                        segments4.m_material = railMaterial;
                        segments4.m_lodMaterial = railLODMaterial;
                        nodes3.m_material = defaultMaterial;
                        nodes3.m_lodMaterial = defaultLODMaterial;
                        nodes9.m_material = defaultMaterial;
                        nodes9.m_lodMaterial = defaultLODMaterial;

                        info.m_segments = new[] { segments0, segments1, segments2, segments3, segments4 };
                        break;
                    }
                case NetInfoVersion.Tunnel:
                    {
                        var segment0 = info.m_segments[0].ShallowClone();
                        var segment1 = info.m_segments[0].ShallowClone();
                        var segment2 = info.m_segments[0].ShallowClone();
                        var segment3 = info.m_segments[0].ShallowClone();
                        var segment4 = info.m_segments[0].ShallowClone();
                        var node0 = info.m_nodes[0].ShallowClone();
                        var node1 = info.m_nodes[0].ShallowClone();

                        nodeList.Add(node0);
                        nodeList.Add(node1);
                        segment0
                            .SetMeshes
                            (@"Meshes\6m\Tunnel_Pavement_Gray.obj",
                            @"Meshes\6m\Tunnel_Pavement_Gray_LOD.obj");
                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Tunnel_Pavement.obj")
                            .SetConsistentUVs();
                        segment2
                            .SetFlagsDefault()
                              .SetMeshes
                              (@"Meshes\6m\Boosted_Rail.obj",
                              @"Meshes\10m\Blank.obj");
                        segment3
                            .SetFlagsDefault()
                              .SetMeshes
                              (@"Meshes\6m\ThirdRail.obj",
                               @"Meshes\6m\Rail_LOD.obj")
                              .SetConsistentUVs();
                        segment4
                            .SetFlagsDefault()
                              .SetMeshes
                              (@"Meshes\6m\Rail.obj",
                               @"Meshes\6m\Rail_LOD.obj")
                              .SetConsistentUVs();
                        node0
                            .SetMeshes
                            (@"Meshes\6m\Tunnel_Pavement_Gray.obj",
                             @"Meshes\6m\Tunnel_Pavement_Gray_LOD.obj");
                        node1
                            .SetMeshes
                              (@"Meshes\6m\Tunnel_Node_Pavement.obj",
                               @"Meshes\6m\Tunnel_Node_Pavement_LOD.obj")
                              .SetConsistentUVs();

                        segment1.m_material = defaultMaterial;
                        segment1.m_lodMaterial = defaultLODMaterial;
                        segment2.m_material = defaultMaterial;
                        segment2.m_lodMaterial = defaultLODMaterial;
                        segment3.m_material = defaultMaterial;
                        segment3.m_lodMaterial = defaultLODMaterial;
                        segment4.m_material = railMaterial;
                        segment4.m_lodMaterial = railLODMaterial;
                        RoadHelper.HandleAsymSegmentFlags(segment1);
                        RoadHelper.HandleAsymSegmentFlags(segment3);
                        node1.m_material = defaultMaterial;
                        node1.m_lodMaterial = defaultLODMaterial;

                        info.m_segments = new[] { segment0, segment1, segment2, segment3,segment4 };
                    }
                    break;
            }
            for (int i = 0; i < nodeList.Count; i++)
            {
                nodeList[i].m_flagsForbidden |= NetNode.Flags.LevelCrossing;
            }
            nodeList.AddRange(SetupMeshUtil.GenerateSplitTracksAndLevelCrossings(info, version));
            info.m_nodes = nodeList.ToArray();
        }
        public static void Setup6mMeshBar(NetInfo info, NetInfoVersion version)
        {
            var elevatedInfo = Prefabs.Find<NetInfo>("Train Track Elevated");
            var elevatedRInfo = Prefabs.Find<NetInfo>("Basic Road Elevated");
            var metroInfo = Prefabs.Find<NetInfo>(Util.GetMetroTrackName(NetInfoVersion.Tunnel, TrackStyle.Modern));
            var elevatedRMaterial = elevatedRInfo.m_nodes[0].m_material;
            var elevatedRLODMaterial = elevatedRInfo.m_nodes[0].m_lodMaterial;
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
                            (@"Meshes\6m\Ground_Fence.obj",
                            @"Meshes\6m\Ground_Fence_LOD.obj");
                        node
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.LevelCrossing)
                            .SetMeshes
                            (@"Meshes\6m\Ground_Node_Fence.obj",
                            @"Meshes\6m\Ground_Node_Fence_LOD.obj");

                        segment.m_material = elevatedRMaterial;
                        segment.m_lodMaterial = elevatedRLODMaterial;
                        node.m_material = elevatedRMaterial;
                        node.m_lodMaterial = elevatedRLODMaterial;

                        info.m_segments = info.AddSegments(segment);
                        info.m_nodes = info.AddNodes(node);
                        break;
                    }
                case NetInfoVersion.Elevated:
                    {
                        var segment = info.m_segments[0].ShallowClone();
                        var node = info.m_nodes[0].ShallowClone();
                        segment
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Elevated_Fence.obj",
                            @"Meshes\6m\Blank.obj");
                        node
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.LevelCrossing)
                            .SetMeshes
                            (@"Meshes\6m\Elevated_Node_Fence.obj",
                            @"Meshes\6m\Blank.obj");

                        segment.m_material = elevatedRMaterial;
                        segment.m_lodMaterial = elevatedRLODMaterial;
                        node.m_material = elevatedRMaterial;
                        node.m_lodMaterial = elevatedRLODMaterial;

                        info.m_segments = info.AddSegments(segment);
                        info.m_nodes = info.AddNodes(node);
                        break;
                    }
                case NetInfoVersion.Bridge:
                    {

                        var segment = info.m_segments[0].ShallowClone();
                        var node = info.m_nodes[0].ShallowClone();
                        segment
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Elevated_Fence.obj",
                            @"Meshes\6m\Blank.obj");
                        node
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.LevelCrossing)
                            .SetMeshes
                            (@"Meshes\6m\Elevated_Node_Fence.obj",
                            @"Meshes\6m\Blank.obj");

                        segment.m_material = elevatedRMaterial;
                        segment.m_lodMaterial = elevatedRLODMaterial;
                        node.m_material = elevatedRMaterial;
                        node.m_lodMaterial = elevatedRLODMaterial;

                        info.m_segments = info.AddSegments(segment);
                        info.m_nodes = info.AddNodes(node);
                        break;
                    }
                case NetInfoVersion.Slope:
                    {
                        var node = info.m_nodes[0].ShallowClone();
                        node
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.Underground | NetNode.Flags.LevelCrossing)
                            .SetMeshes
                            (@"Meshes\6m\Slope_Node_Fence.obj",
                            @"Meshes\6m\Slope_Node_Fence_LOD.obj");

                        node.m_material = elevatedMaterial;
                        node.m_lodMaterial = elevatedLODMaterial;

                        info.m_nodes = info.AddNodes(node);
                        break;
                    }
            }
        }
        public static void Setup6mStationMesh(NetInfo prefab, NetInfoVersion version, NetInfo elevatedInfo, NetInfo metroStationInfo)
        {
            var trainTrackInfo = PrefabCollection<NetInfo>.FindLoaded("Train Track");
            var elevatedMaterial = elevatedInfo.m_segments[0].m_material;
            var elevatedLODMaterial = elevatedInfo.m_segments[0].m_lodMaterial;
            var railMaterial = trainTrackInfo.m_segments[1].m_material;
            var railLODMaterial = trainTrackInfo.m_segments[1].m_lodMaterial;
            var nodeList = new List<NetInfo.Node>();
            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        var segment0 = prefab.m_segments[0].ShallowClone();
                        var segment1 = prefab.m_segments[1].ShallowClone();
                        var segment2 = prefab.m_segments[1].ShallowClone();
                        var node0 = prefab.m_nodes[0].ShallowClone();
                        nodeList.Add(node0);

                        segment0
                            .SetMeshes
                            (@"Meshes\6m\Ground_Station_Pavement.obj",
                                @"Meshes\6m\Ground_Station_Pavement_LOD.obj");
                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Station_Rail.obj",
                             @"Meshes\6m\Rail_LOD.obj")
                            .SetConsistentUVs();
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Station_ThirdRail.obj",
                             @"Meshes\6m\Rail_LOD.obj")
                            .SetConsistentUVs();

                        node0
                            .SetMeshes
                            (@"Meshes\6m\Ground_Node_Pavement.obj",
                                @"Meshes\6m\Ground_Node_Pavement_LOD.obj");

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
                        var segment1 = prefab.m_segments[0].ShallowClone();
                        var segment2 = prefab.m_segments[1].ShallowClone();
                        var segment3 = prefab.m_segments[1].ShallowClone();
                        var node0 = prefab.m_nodes[0].ShallowClone();
                        nodeList.Add(node0);

                        segment0
                            .SetMeshes
                            (@"Meshes\6m\Elevated_Station_Pavement.obj",
                             @"Meshes\6m\Elevated_Station_Pavement_LOD.obj");
                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Station_Boosted_Rail.obj",
                            @"Meshes\6m\Blank.obj")
                            .SetConsistentUVs();
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Station_ThirdRail.obj",
                            @"Meshes\6m\Rail_LOD.obj")
                            .SetConsistentUVs();
                        segment3
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Station_Rail.obj",
                            @"Meshes\6m\Rail_LOD.obj")
                            .SetConsistentUVs();
                        node0
                            .SetMeshes
                            (@"Meshes\6m\Elevated_Station_Node_Pavement.obj",
                             @"Meshes\6m\Elevated_Station_Node_Pavement_LOD.obj");

                        segment0.m_material = elevatedMaterial;
                        segment0.m_lodMaterial = elevatedLODMaterial;
                        node0.m_material = elevatedMaterial;
                        node0.m_lodMaterial = elevatedLODMaterial;
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

                        nodeList.Add(node0);
                        nodeList.Add(node1);

                        segment0
                            .SetMeshes
                            (@"Meshes\6m\Tunnel_Station_Pavement_Gray.obj",
                            @"Meshes\6m\Tunnel_Station_Pavement_Gray_LOD.obj");
                        segment1
                            .SetMeshes
                            (@"Meshes\6m\Tunnel_Station_Pavement.obj",
                                @"Meshes\6m\Tunnel_Pavement_LOD.obj")
                                .SetConsistentUVs();
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Station_Boosted_Rail.obj",
                            @"Meshes\6m\Blank.obj")
                            .SetConsistentUVs();
                        segment3
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Station_ThirdRail.obj",
                            @"Meshes\6m\Rail_LOD.obj")
                            .SetConsistentUVs();
                        segment4
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Station_Rail.obj",
                            @"Meshes\6m\Rail_LOD.obj")
                            .SetConsistentUVs();
                        node0
                            .SetMeshes
                            (@"Meshes\6m\Tunnel_Station_Pavement_Gray.obj",
                            @"Meshes\6m\Tunnel_Station_Pavement_Gray_LOD.obj");
                        node1
                            .SetMeshes
                            (@"Meshes\6m\Tunnel_Station_Node_Pavement.obj",
                                @"Meshes\6m\Tunnel_Node_Pavement_LOD.obj")
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

                        prefab.m_segments = new[] { segment0, segment1, segment2, segment3,segment4 };

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
using System;
using System.Collections.Generic;
using MetroOverhaul.NEXT;
using MetroOverhaul.NEXT.Extensions;

namespace MetroOverhaul.InitializationSteps
{
    public static partial class SetupMesh
    {
        public static List<NetInfo.Node> GenerateSplitTracks(NetInfo info, NetInfoVersion version)
        {
            var ttInfo = PrefabCollection<NetInfo>.FindLoaded("Train Track");
            var defaultMaterial = ttInfo.m_nodes[0].m_material;
            var defaultLODMaterial = ttInfo.m_nodes[0].m_lodMaterial;
            var nodeList = new List<NetInfo.Node>();
            var is10m = info.name.Contains("Two-Lane");
            var isMerge = info.name.Contains("Two-Way") || info.name.Contains("Station") || is10m;
            var mergeName = isMerge ? "Merge_" : "";
            var widthName = is10m ? "10m" : "6m";
            NetInfo.Node nodes6 = null;
            NetInfo.Node nodes7 = null;
            NetInfo.Node nodes8 = null;
            NetInfo.Node nodes9 = null;
            NetInfo.Node nodes10 = null;
            NetInfo.Node nodes11 = null;
            if (version == NetInfoVersion.Tunnel)
            {
                nodes6 = info.m_nodes[0].ShallowClone();
                nodes7 = info.m_nodes[0].ShallowClone();
                nodes8 = info.m_nodes[0].ShallowClone();
                nodes9 = info.m_nodes[0].ShallowClone();
                nodes10 = info.m_nodes[0].ShallowClone();
                nodes11 = info.m_nodes[0].ShallowClone();
                nodes6.m_material = defaultMaterial;
                nodes6.m_lodMaterial = defaultLODMaterial;
                nodes6.m_directConnect = true;
                nodes7.m_material = defaultMaterial;
                nodes7.m_lodMaterial = defaultLODMaterial;
                nodes7.m_directConnect = true;
                nodes8.m_material = defaultMaterial;
                nodes8.m_lodMaterial = defaultLODMaterial;
                nodes8.m_directConnect = true;
                nodes9.m_material = defaultMaterial;
                nodes9.m_lodMaterial = defaultLODMaterial;
                nodes9.m_directConnect = true;
                nodes10.m_material = defaultMaterial;
                nodes10.m_lodMaterial = defaultLODMaterial;
                nodes10.m_directConnect = true;
                nodes11.m_material = defaultMaterial;
                nodes11.m_lodMaterial = defaultLODMaterial;
                nodes11.m_directConnect = true;
            }
            else
            {
                nodes6 = info.m_nodes[1].ShallowClone();
                nodes7 = info.m_nodes[1].ShallowClone();
                nodes8 = info.m_nodes[1].ShallowClone();
                nodes9 = info.m_nodes[1].ShallowClone();
                nodes10 = info.m_nodes[1].ShallowClone();
                nodes11 = info.m_nodes[1].ShallowClone();
            }
            nodeList.Add(nodes6);
            nodeList.Add(nodes7);
            nodes6
                .SetMeshes
                   ($@"Meshes\{widthName}\Boosted_Rail_{mergeName}Start.obj",
                  @"Meshes\6m\Ground_Rail_Start_LOD.obj")
            .SetConsistentUVs();
            nodes7
                .SetMeshes
                   ($@"Meshes\{widthName}\Boosted_Rail_{mergeName}End.obj",
                   @"Meshes\6m\Ground_Rail_End_LOD.obj")
            .SetConsistentUVs();

            nodes6.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayStart;
            nodes7.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayEnd;

            if (is10m)
            {
                nodeList.Add(nodes8);
                nodeList.Add(nodes9);
                nodeList.Add(nodes10);
                nodeList.Add(nodes11);
                nodes8
                    .SetMeshes
                       ($@"Meshes\6m\Boosted_Rail_Merge_Start.obj",
                      @"Meshes\6m\Ground_Rail_Start_LOD.obj")
                     .SetConsistentUVs();
                nodes9
                    .SetMeshes
                       ($@"Meshes\6m\Boosted_Rail_Merge_End.obj",
                       @"Meshes\6m\Ground_Rail_End_LOD.obj")
                      .SetConsistentUVs();
                nodes10
                    .SetMeshes
                       ($@"Meshes\6m\Boosted_Rail_Merge_Start.obj",
                      @"Meshes\6m\Ground_Rail_Start_LOD.obj")
                     .SetConsistentUVs();
                nodes11
                    .SetMeshes
                       ($@"Meshes\6m\Boosted_Rail_Merge_End.obj",
                       @"Meshes\6m\Ground_Rail_End_LOD.obj")
                      .SetConsistentUVs();
                nodes8.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.OnewayEnd;
                nodes9.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.OnewayStart;
                nodes10.m_connectGroup = (NetInfo.ConnectGroup)16 | NetInfo.ConnectGroup.OnewayEnd;
                nodes11.m_connectGroup = (NetInfo.ConnectGroup)16 | NetInfo.ConnectGroup.OnewayStart;
            }

            return nodeList;
        }
        public static List<NetInfo.Node> GenerateLevelCrossing(NetInfo info)
        {
            var is10m = info.name.Contains("Two-Lane");
            var isTwoWay = info.name.Contains("Two-Way") || info.name.Contains("Station") || is10m;
            var mergeName = isTwoWay ? "Merge_" : "";
            var lcName = isTwoWay ? "" : "Level_Crossing_";
            var nodeList = new List<NetInfo.Node>();
            var pavementIndex = -1;
            for(var i = 0; i < info.m_nodes.Length; i++)
            {
                if (info.m_nodes[i].m_flagsRequired == NetNode.Flags.LevelCrossing)
                {
                    if (info.m_nodes[i].m_directConnect == false)
                    {
                        pavementIndex = i;
                        break;
                    }
                }
            }
            var nodes0 = info.m_nodes[pavementIndex].ShallowClone();
            var nodes1 = info.m_nodes[1].ShallowClone();
            var nodes2 = info.m_nodes[pavementIndex].ShallowClone();
            var nodes3 = info.m_nodes[1].ShallowClone();
            var nodes4 = info.m_nodes[1].ShallowClone();
            var nodes5 = info.m_nodes[pavementIndex].ShallowClone();
            var nodes6 = info.m_nodes[pavementIndex].ShallowClone();

            nodeList.Add(nodes0);
            nodeList.Add(nodes1);
            nodeList.Add(nodes2);
            nodeList.Add(nodes3);
            nodeList.Add(nodes4);
            nodeList.Add(nodes5);
            nodeList.Add(nodes6);
            if (is10m)
            {
                var nodes7 = info.m_nodes[1].ShallowClone();
                var nodes8 = info.m_nodes[1].ShallowClone();
                var nodes9 = info.m_nodes[pavementIndex].ShallowClone();
                var nodes10 = info.m_nodes[pavementIndex].ShallowClone();
                var nodes11 = info.m_nodes[1].ShallowClone();
                var nodes12 = info.m_nodes[1].ShallowClone();
                var nodes13 = info.m_nodes[pavementIndex].ShallowClone();
                var nodes14 = info.m_nodes[pavementIndex].ShallowClone();
                nodeList.Add(nodes7);
                nodeList.Add(nodes8);
                nodeList.Add(nodes9);
                nodeList.Add(nodes10);
                nodeList.Add(nodes11);
                nodeList.Add(nodes12);
                nodeList.Add(nodes13);
                nodeList.Add(nodes14);
                nodes0
                    .SetMeshes
                    (@"Meshes\10m\LevelCrossing_Pavement.obj",
                    @"Meshes\10m\LevelCrossing_Pavement_LOD.obj");
                nodes1
                    .SetFlags(NetNode.Flags.LevelCrossing, NetNode.Flags.None)
                    .SetMeshes
                    (@"Meshes\10m\LevelCrossing_Rail.obj", @"Meshes\10m\Blank.obj")
                    .SetConsistentUVs();
                nodes2
                    .SetMeshes
                    (@"Meshes\10m\LevelCrossing_Rail_Insert.obj", @"Meshes\10m\Blank.obj")
                    .SetConsistentUVs();
                nodes3
                    .SetFlags(NetNode.Flags.LevelCrossing, NetNode.Flags.None)
                    .SetMeshes
                    ($@"Meshes\10m\LevelCrossing_Rail_{mergeName}Start.obj", @"Meshes\10m\Blank.obj")
                    .SetConsistentUVs();
                nodes4
                    .SetFlags(NetNode.Flags.LevelCrossing, NetNode.Flags.None)
                    .SetMeshes
                    ($@"Meshes\10m\LevelCrossing_Rail_{mergeName}End.obj", @"Meshes\10m\Blank.obj")
                    .SetConsistentUVs();
                nodes5
                    .SetMeshes
                    ($@"Meshes\10m\LevelCrossing_Rail_Insert_{mergeName}Start.obj", @"Meshes\10m\Blank.obj")
                    .SetConsistentUVs();
                nodes6
                    .SetMeshes
                    ($@"Meshes\10m\LevelCrossing_Rail_Insert_{mergeName}End.obj", @"Meshes\10m\Blank.obj")
                    .SetConsistentUVs();

                nodes7
                    .SetFlags(NetNode.Flags.LevelCrossing, NetNode.Flags.None)
                    .SetMeshes
                    ($@"Meshes\6m\Ground_{lcName}Rail_Merge_Start.obj", @"Meshes\10m\Blank.obj")
                    .SetConsistentUVs();
                nodes8
                    .SetFlags(NetNode.Flags.LevelCrossing, NetNode.Flags.None)
                    .SetMeshes
                    ($@"Meshes\6m\Ground_{lcName}Rail_Merge_End.obj", @"Meshes\10m\Blank.obj")
                    .SetConsistentUVs();
                nodes9
                    .SetMeshes
                    ($@"Meshes\6m\Ground_Level_Crossing_Rail_Insert_Merge_Start.obj", @"Meshes\10m\Blank.obj")
                    .SetConsistentUVs();
                nodes10
                    .SetMeshes
                    ($@"Meshes\6m\Ground_Level_Crossing_Rail_Insert_Merge_End.obj", @"Meshes\10m\Blank.obj")
                    .SetConsistentUVs();

                nodes11
                    .SetFlags(NetNode.Flags.LevelCrossing, NetNode.Flags.None)
                    .SetMeshes
                    ($@"Meshes\6m\Ground_{lcName}Rail_Merge_Start.obj", @"Meshes\10m\Blank.obj")
                    .SetConsistentUVs();
                nodes12
                    .SetFlags(NetNode.Flags.LevelCrossing, NetNode.Flags.None)
                    .SetMeshes
                    ($@"Meshes\6m\Ground_{lcName}Rail_Merge_End.obj", @"Meshes\10m\Blank.obj")
                    .SetConsistentUVs();
                nodes13
                    .SetMeshes
                    ($@"Meshes\6m\Ground_Level_Crossing_Rail_Insert_Merge_Start.obj", @"Meshes\10m\Blank.obj")
                    .SetConsistentUVs();
                nodes14
                    .SetMeshes
                    ($@"Meshes\6m\Ground_Level_Crossing_Rail_Insert_Merge_End.obj", @"Meshes\10m\Blank.obj")
                    .SetConsistentUVs();
                nodes7.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.OnewayEnd;
                nodes8.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.OnewayStart;
                nodes9.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.OnewayEnd;
                nodes10.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.OnewayStart;
                nodes11.m_connectGroup = (NetInfo.ConnectGroup)16 | NetInfo.ConnectGroup.OnewayEnd;
                nodes12.m_connectGroup = (NetInfo.ConnectGroup)16 | NetInfo.ConnectGroup.OnewayStart;
                nodes13.m_connectGroup = (NetInfo.ConnectGroup)16 | NetInfo.ConnectGroup.OnewayEnd;
                nodes14.m_connectGroup = (NetInfo.ConnectGroup)16 | NetInfo.ConnectGroup.OnewayStart;
                nodes9.m_directConnect = true;
                nodes10.m_directConnect = true;
                nodes13.m_directConnect = true;
                nodes14.m_directConnect = true;
            }
            else
            {
                nodes0
                .SetMeshes
                (@"Meshes\6m\Ground_Level_Crossing.obj",
                @"Meshes\6m\Ground_Level_Crossing_LOD.obj");
            nodes1
                .SetFlags(NetNode.Flags.LevelCrossing, NetNode.Flags.None)
                .SetMeshes
                (@"Meshes\6m\Ground_Level_Crossing_Rail.obj", @"Meshes\10m\Blank.obj")
                .SetConsistentUVs();
            nodes2
                .SetMeshes
                (@"Meshes\6m\Ground_Level_Crossing_Rail_Insert.obj", @"Meshes\10m\Blank.obj")
                .SetConsistentUVs();
            nodes3
                .SetFlags(NetNode.Flags.LevelCrossing, NetNode.Flags.None)
                .SetMeshes
                ($@"Meshes\6m\Ground_{lcName}Rail_{mergeName}Start.obj", @"Meshes\10m\Blank.obj")
                .SetConsistentUVs();
            nodes4
                .SetFlags(NetNode.Flags.LevelCrossing, NetNode.Flags.None)
                .SetMeshes
                ($@"Meshes\6m\Ground_{lcName}Rail_{mergeName}End.obj", @"Meshes\10m\Blank.obj")
                .SetConsistentUVs();
            nodes5
                .SetMeshes
                ($@"Meshes\6m\Ground_Level_Crossing_Rail_Insert_{mergeName}Start.obj", @"Meshes\10m\Blank.obj")
                .SetConsistentUVs();
            nodes6
                .SetMeshes
                ($@"Meshes\6m\Ground_Level_Crossing_Rail_Insert_{mergeName}End.obj", @"Meshes\10m\Blank.obj")
                .SetConsistentUVs();
            }


            nodes1.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.Oneway;
            nodes2.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.Oneway;
            nodes3.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayStart;
            nodes4.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayEnd;
            nodes5.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayStart;
            nodes6.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayEnd;
            nodes2.m_directConnect = true;
            nodes5.m_directConnect = true;
            nodes6.m_directConnect = true;
            return nodeList;
        }
        public static void Setup6mMesh(this NetInfo info, NetInfoVersion version)
        {
            var ttInfo = Prefabs.Find<NetInfo>("Train Track");
            var brElInfo = Prefabs.Find<NetInfo>("Basic Road Elevated");
            var ttElInfo = Prefabs.Find<NetInfo>("Train Track Elevated");
            var defaultMaterial = ttInfo.m_nodes[0].m_material;
            var defaultElMaterial = ttElInfo.m_nodes[0].m_material;
            var defaultBrElMaterial = brElInfo.m_segments[0].m_material;
            var defaultLODMaterial = ttInfo.m_nodes[0].m_lodMaterial;
            var defaultElLODMaterial = ttInfo.m_nodes[0].m_lodMaterial;
            var defaultBrElLodMaterial = brElInfo.m_segments[0].m_lodMaterial;
            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        var segments0 = info.m_segments[0].ShallowClone();
                        var segments1 = info.m_segments[1].ShallowClone();
                        var segments3 = info.m_segments[0].ShallowClone();
                        var nodes0 = info.m_nodes[0].ShallowClone();
                        var nodes1 = info.m_nodes[1].ShallowClone();

                        var nodeList = new List<NetInfo.Node>();
                        nodeList.Add(nodes0);
                        nodeList.Add(nodes1);
                        nodes1.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.Oneway;

                        segments0
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Ground_Pavement.obj",
                            @"Meshes\6m\Ground_Pavement_LOD.obj");

                        segments1
                            .SetFlagsDefault()
                           .SetMeshes
                           (@"Meshes\6m\Ground_Rail.obj",
                           @"Meshes\6m\Ground_Rail_LOD.obj");

                        segments3
                            .SetFlagsDefault()
                           .SetMeshes
                           (@"Meshes\6m\ThirdRail.obj", @"Meshes\10m\Blank.obj")
                           .SetConsistentUVs();

                        nodes0
                            .SetMeshes
                           (@"Meshes\6m\Ground_Node_Pavement.obj",
                           @"Meshes\6m\Ground_Node_Pavement_LOD.obj");

                        nodes1
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.LevelCrossing)
                            .SetMeshes
                            (@"Meshes\6m\Boosted_Rail.obj",
                            @"Meshes\6m\Ground_Rail_Node_LOD.obj");

                        RoadHelper.HandleAsymSegmentFlags(segments3);

                        nodes1.m_flagsForbidden = NetNode.Flags.LevelCrossing;

                        nodeList.AddRange(GenerateSplitTracks(info, version));
                        nodeList.AddRange(GenerateLevelCrossing(info));

                        info.m_segments = new[] { segments0, segments1, /*segments2*/segments3 };
                        info.m_nodes = nodeList.ToArray();
                        break;
                    }

                case NetInfoVersion.Elevated:
                    {
                        var segments0 = info.m_segments[0].ShallowClone();
                        var segments1 = info.m_segments[1].ShallowClone();

                        var segments3 = info.m_segments[1].ShallowClone();
                        var segments4 = info.m_segments[0].ShallowClone();
                        var nodes0 = info.m_nodes[0].ShallowClone();
                        var nodes1 = info.m_nodes[1].ShallowClone();
                        var nodes2 = info.m_nodes[0].ShallowClone();
                        var nodeList = new List<NetInfo.Node>();
                        nodeList.Add(nodes0);
                        nodeList.Add(nodes1);
                        nodeList.Add(nodes2);
                        nodes1.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.Oneway;


                        segments0
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Elevated_Pavement.obj",
                            @"Meshes\6m\Elevated_Pavement_LOD.obj");

                        segments1
                            .SetFlagsDefault()
                              .SetMeshes
                              (@"Meshes\6m\Boosted_Rail.obj",
                              @"Meshes\6m\Ground_Rail_LOD.obj");

                        segments3
                            .SetFlagsDefault()
                              .SetMeshes
                              (@"Meshes\6m\ThirdRail.obj", @"Meshes\10m\Blank.obj")
                              .SetConsistentUVs();
                        segments4
                            .SetFlagsDefault()
                              .SetMeshes
                              (@"Meshes\6m\Elevated_RailGuards.obj", @"Meshes\10m\Blank.obj")
                              .SetConsistentUVs();
                        nodes0
                            .SetMeshes
                            (@"Meshes\6m\Elevated_Pavement.obj",
                            @"Meshes\6m\Elevated_Node_Pavement_LOD.obj");

                        nodes1
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.LevelCrossing)
                            .SetMeshes
                            (@"Meshes\6m\Boosted_Rail.obj",
                            @"Meshes\6m\Ground_Rail_Node_LOD.obj");
                        nodes2
                            .SetMeshes
                            (@"Meshes\6m\Elevated_Node_Pavement_Trans.obj",
                            @"Meshes\6m\Elevated_Node_Pavement_LOD.obj");

                        RoadHelper.HandleAsymSegmentFlags(segments3);
                        segments0.m_material = defaultMaterial;
                        segments0.m_lodMaterial = defaultLODMaterial;
                        segments3.m_material = defaultMaterial;
                        segments3.m_lodMaterial = defaultLODMaterial;
                        nodes0.m_material = defaultMaterial;
                        nodes0.m_lodMaterial = defaultLODMaterial;
                        nodes2.m_material = defaultMaterial;
                        nodes2.m_lodMaterial = defaultLODMaterial;
                        nodeList.AddRange(GenerateSplitTracks(info, version));
                        nodeList.AddRange(GenerateLevelCrossing(info));
                        info.m_segments = new[] { segments0, segments1 /*,segments2*/, segments3, segments4 };
                        info.m_nodes = nodeList.ToArray();
                        break;
                    }
                case NetInfoVersion.Bridge:
                    {
                        var segments0 = info.m_segments[0].ShallowClone();
                        var segments1 = info.m_segments[1].ShallowClone();
                        var segments3 = info.m_segments[1].ShallowClone();
                        var segments4 = info.m_segments[0].ShallowClone();
                        var nodes0 = info.m_nodes[0].ShallowClone();
                        var nodes1 = info.m_nodes[1].ShallowClone();
                        var nodes2 = info.m_nodes[0].ShallowClone();
                        nodes1.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.Oneway;
                        var nodeList = new List<NetInfo.Node>();
                        nodeList.Add(nodes0);
                        nodeList.Add(nodes1);
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
                              @"Meshes\6m\Ground_Rail_LOD.obj");

                        segments3
                            .SetFlagsDefault()
                              .SetMeshes
                              (@"Meshes\6m\ThirdRail.obj", @"Meshes\10m\Blank.obj")
                              .SetConsistentUVs();
                        segments4
                            .SetFlagsDefault()
                              .SetMeshes
                              (@"Meshes\6m\Elevated_RailGuards.obj", @"Meshes\10m\Blank.obj")
                              .SetConsistentUVs();
                        nodes0
                            .SetMeshes
                              (@"Meshes\6m\Bridge_Node_Pavement.obj",
                              @"Meshes\6m\Elevated_Node_Pavement_LOD.obj")
                              .SetConsistentUVs();

                        nodes1
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.LevelCrossing)
                            .SetMeshes
                              (@"Meshes\6m\Boosted_Rail.obj",
                              @"Meshes\6m\Ground_Rail_Node_LOD.obj");
                        nodes2
                            .SetMeshes
                            (@"Meshes\6m\Elevated_Node_Pavement_Trans.obj",
                            @"Meshes\6m\Elevated_Node_Pavement_LOD.obj");

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
                        nodes0.m_material = defaultMaterial;
                        nodes0.m_lodMaterial = defaultLODMaterial;
                        nodes2.m_material = defaultMaterial;
                        nodes2.m_lodMaterial = defaultLODMaterial;
                        nodeList.AddRange(GenerateLevelCrossing(info));
                        nodeList.AddRange(GenerateSplitTracks(info, version));
                        info.m_segments = new[] { segments0, segments1/*, segments2*/, segments3, segments4 };
                        info.m_nodes = nodeList.ToArray();
                        break;
                    }

                case NetInfoVersion.Slope:
                    {
                        var segments0 = info.m_segments[0].ShallowClone();
                        var segments1 = info.m_segments[1].ShallowClone();
                        var segments3 = info.m_segments[1].ShallowClone();
                        var segments4 = info.m_segments[1].ShallowClone();
                        var nodes0 = info.m_nodes[0].ShallowClone();
                        var nodes1 = info.m_nodes[1].ShallowClone();
                        var nodes2 = info.m_nodes[2].ShallowClone();
                        var nodes3 = info.m_nodes[3].ShallowClone();
                        //var nodes4 = info.m_nodes[4].ShallowClone();
                        var nodes9 = info.m_nodes[0].ShallowClone();

                        //var nodes10 = info.m_nodes[1].ShallowClone();
                        var nodeList = new List<NetInfo.Node>();
                        nodeList.Add(nodes0);
                        nodeList.Add(nodes1);
                        nodeList.Add(nodes2);
                        nodeList.Add(nodes3);
                        //nodeList.Add(nodes4);
                        nodeList.Add(nodes9);
                        //nodeList.Add(nodes10);

                        nodes1.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.Oneway;
                        nodes2.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.Oneway;


                        segments0
                            .SetMeshes
                            (@"Meshes\6m\Tunnel_Pavement.obj");
                        segments1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Boosted_Rail.obj")
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
                              (@"Meshes\6m\ThirdRail.obj", @"Meshes\10m\Blank.obj")
                              .SetConsistentUVs();
                        nodes0
                          .SetMeshes
                            (@"Meshes\6m\Tunnel_Node_Pavement.obj",
                            @"Meshes\6m\Tunnel_Node_Pavement_LOD.obj");

                        nodes1
                            .SetMeshes
                            (@"Meshes\6m\Boosted_Rail.obj")
                            .SetConsistentUVs();

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

                        segments3.m_material = defaultBrElMaterial;
                        segments3.m_lodMaterial = defaultBrElLodMaterial;
                        nodes9.m_material = defaultBrElMaterial;
                        nodes9.m_lodMaterial = defaultBrElLodMaterial;

                        info.m_segments = new[] { segments0, segments1, segments3, segments4 };
                        info.m_nodes = nodeList.ToArray();
                        break;
                    }
                case NetInfoVersion.Tunnel:
                    {
                        var segment0 = info.m_segments[0].ShallowClone();
                        var segment1 = info.m_segments[0].ShallowClone();
                        var segment2 = info.m_segments[0].ShallowClone();
                        var segment3 = info.m_segments[0].ShallowClone();
                        var node0 = info.m_nodes[0].ShallowClone();
                        var node1 = info.m_nodes[0].ShallowClone();
                        var node2 = info.m_nodes[0].ShallowClone();

                        var nodeList = new List<NetInfo.Node>();
                        nodeList.Add(node0);
                        nodeList.Add(node1);
                        nodeList.Add(node2);
                        node2.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.Oneway;

                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Tunnel_Pavement.obj")
                            .SetConsistentUVs();
                        segment2
                            .SetFlagsDefault()
                  .SetMeshes
                  (@"Meshes\6m\Boosted_Rail.obj");
                        segment3
                            .SetFlagsDefault()
                  .SetMeshes
                  (@"Meshes\6m\ThirdRail.obj", @"Meshes\10m\Blank.obj")
                  .SetConsistentUVs();
                        node1
                            .SetMeshes
                  (@"Meshes\6m\Tunnel_Node_Pavement.obj",
                      @"Meshes\6m\Tunnel_Node_Pavement_LOD.obj")
                  .SetConsistentUVs();
                        node2
                            .SetMeshes
                  (@"Meshes\6m\Boosted_Rail.obj")
                  .SetConsistentUVs();
                        
                        segment1.m_material = defaultElMaterial;
                        segment1.m_lodMaterial = defaultElLODMaterial;
                        segment2.m_material = defaultElMaterial;
                        segment2.m_lodMaterial = defaultElLODMaterial;
                        segment3.m_material = defaultElMaterial;
                        segment3.m_lodMaterial = defaultElLODMaterial;
                        RoadHelper.HandleAsymSegmentFlags(segment1);
                        RoadHelper.HandleAsymSegmentFlags(segment3);
                        node1.m_material = defaultBrElMaterial;
                        node1.m_lodMaterial = defaultBrElLodMaterial;
                        node2.m_material = defaultMaterial;
                        node2.m_lodMaterial = defaultLODMaterial;
                        
                        node2.m_directConnect = true;
                        nodeList.AddRange(GenerateSplitTracks(info, version));
                        info.m_segments = new[] { segment0, segment1, segment2, segment3 };
                        info.m_nodes = nodeList.ToArray();
                    }
                    break;
            }
        }
        public static void Setup6mMeshBar(NetInfo info, NetInfoVersion version)
        {
            var elevatedInfo = Prefabs.Find<NetInfo>("Train Track Elevated");
            var elevatedRInfo = Prefabs.Find<NetInfo>("Basic Road Elevated");
            var metroInfo = Prefabs.Find<NetInfo>("Metro Track");
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
                            .SetFlags(NetNode.Flags.None, NetNode.Flags.Underground)
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
            var trainTrackMaterial = trainTrackInfo.m_nodes[0].m_material;
            var trainTrackLODMaterial = trainTrackInfo.m_nodes[0].m_lodMaterial;
            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        var segment0 = prefab.m_segments[0].ShallowClone();
                        var segment1 = prefab.m_segments[1].ShallowClone();
                        var segment2 = prefab.m_segments[0].ShallowClone();
                        var node0 = prefab.m_nodes[0].ShallowClone();
                        var node1 = prefab.m_nodes[1].ShallowClone();
                        var nodeList = new List<NetInfo.Node>();
                        nodeList.Add(node0);
                        nodeList.Add(node1);

                        node1.m_connectGroup = NetInfo.ConnectGroup.CenterTram;
                        segment0
                            .SetMeshes
                            (@"Meshes\6m\Ground_Station_Pavement.obj",
                                @"Meshes\6m\Ground_Station_Pavement_LOD.obj");
                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Ground_Rail.obj")
                            .SetConsistentUVs();
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\ThirdRail.obj", @"Meshes\6m\Blank.obj")
                            .SetConsistentUVs();
                        node0
                            .SetMeshes
                            (@"Meshes\6m\Ground_Node_Pavement.obj",
                                @"Meshes\6m\Ground_Node_Pavement_LOD.obj");
                        node1
                            .SetMeshes
                            (@"Meshes\6m\Boosted_Rail.obj")
                            .SetConsistentUVs();

                        segment0.m_material = elevatedMaterial;
                        segment0.m_lodMaterial = elevatedLODMaterial;
                        node0.m_material = elevatedMaterial;
                        node0.m_lodMaterial = elevatedLODMaterial;
                        node1.m_flagsForbidden = NetNode.Flags.LevelCrossing;
                        nodeList.AddRange(GenerateSplitTracks(prefab, version));
                        nodeList.AddRange(GenerateLevelCrossing(prefab));
                        prefab.m_segments = new[] { segment0, segment1, segment2 };
                        prefab.m_nodes = nodeList.ToArray();
                        break;
                    }
                case NetInfoVersion.Elevated:
                    {
                        var segment0 = prefab.m_segments[0].ShallowClone();
                        var segment1 = prefab.m_segments[1].ShallowClone();
                        var segment2 = prefab.m_segments[0].ShallowClone();
                        var node0 = prefab.m_nodes[0].ShallowClone();
                        var node1 = prefab.m_nodes[1].ShallowClone();
                        var nodeList = new List<NetInfo.Node>();
                        nodeList.Add(node0);
                        nodeList.Add(node1);
                        node1.m_connectGroup = NetInfo.ConnectGroup.CenterTram;

                        segment0
                            .SetMeshes
                            (@"Meshes\6m\Elevated_Station_Pavement.obj",
                             @"Meshes\6m\Elevated_Station_Pavement_LOD.obj");
                        segment1
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Boosted_Rail.obj")
                            .SetConsistentUVs();
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\ThirdRail.obj", @"Meshes\6m\Blank.obj")
                            .SetConsistentUVs();
                        node0
                            .SetMeshes
                            (@"Meshes\6m\Elevated_Station_Node_Pavement.obj",
                             @"Meshes\6m\Elevated_Station_Node_Pavement_LOD.obj");
                        node1
                            .SetMeshes
                            (@"Meshes\6m\Boosted_Rail.obj")
                            .SetConsistentUVs();
                        
                        segment0.m_material = elevatedMaterial;
                        segment0.m_lodMaterial = elevatedLODMaterial;
                        node0.m_material = elevatedMaterial;
                        node0.m_lodMaterial = elevatedLODMaterial;
                        nodeList.AddRange(GenerateSplitTracks(prefab, version));
                        nodeList.AddRange(GenerateLevelCrossing(prefab));
                        prefab.m_segments = new[] { segment0, segment1, segment2 };
                        prefab.m_nodes = nodeList.ToArray();
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
                        var node2 = metroStationInfo.m_nodes[0].ShallowClone();

                        var nodeList = new List<NetInfo.Node>();
                        nodeList.Add(node0);
                        nodeList.Add(node1);
                        nodeList.Add(node2);

                        node2.m_connectGroup = NetInfo.ConnectGroup.CenterTram;
                        segment1
                            .SetMeshes
                            (@"Meshes\6m\Tunnel_Station_Pavement.obj",
                                @"Meshes\6m\Tunnel_Pavement_LOD.obj")
                                .SetConsistentUVs();
                        segment2
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\Boosted_Rail.obj")
                            .SetConsistentUVs();
                        segment3
                            .SetFlagsDefault()
                            .SetMeshes
                            (@"Meshes\6m\ThirdRail.obj", @"Meshes\6m\Blank.obj")
                            .SetConsistentUVs();
                        node1
                            .SetMeshes
                            (@"Meshes\6m\Tunnel_Station_Node_Pavement.obj",
                                @"Meshes\6m\Tunnel_Node_Pavement_LOD.obj")
                                .SetConsistentUVs();
                        node2
                            .SetMeshes
                            (@"Meshes\6m\Boosted_Rail.obj")
                            .SetConsistentUVs();

                        segment1.m_material = elevatedMaterial;
                        segment1.m_lodMaterial = elevatedLODMaterial;
                        segment2.m_material = elevatedMaterial;
                        segment2.m_lodMaterial = elevatedLODMaterial;
                        segment3.m_material = elevatedMaterial;
                        segment3.m_lodMaterial = elevatedLODMaterial;
                        node1.m_material = elevatedMaterial;
                        node1.m_lodMaterial = elevatedLODMaterial;
                        node2.m_material = elevatedMaterial;
                        node2.m_lodMaterial = elevatedLODMaterial;
                        node2.m_directConnect = true;
                        nodeList.AddRange(GenerateSplitTracks(prefab,version));
                        prefab.m_segments = new[] { segment0, segment1, segment2, segment3 };
                        prefab.m_nodes = nodeList.ToArray();
                        break;
                    }
            }
        }
    }
}
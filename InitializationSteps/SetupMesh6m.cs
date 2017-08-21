using System;
using System.Collections.Generic;
using MetroOverhaul.NEXT;
using MetroOverhaul.NEXT.Extensions;

namespace MetroOverhaul.InitializationSteps
{
    public static partial class SetupMesh
    {
        public static void Setup6mMesh(this NetInfo info, NetInfoVersion version)
        {
            ///////////////////////////
            // Template              //
            ///////////////////////////
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
                        var nodes2 = info.m_nodes[2].ShallowClone();
                        var nodes3 = info.m_nodes[2].ShallowClone();
                        var nodes4 = info.m_nodes[1].ShallowClone();
                        var nodes5 = info.m_nodes[1].ShallowClone();
                        var nodes6 = info.m_nodes[2].ShallowClone();

                        nodes1.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.Oneway;
                        nodes4.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayStart;
                        nodes5.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayEnd;

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
                            .SetMeshes
                            (@"Meshes\6m\Boosted_Rail.obj",
                            @"Meshes\6m\Ground_Rail_Node_LOD.obj");

                        nodes2
                            .SetMeshes
                            (@"Meshes\6m\Ground_Level_Crossing.obj",
                            @"Meshes\6m\Ground_Level_Crossing_LOD.obj");
                        nodes3
                            .SetMeshes
                            (@"Meshes\6m\Ground_Level_Crossing_Rail.obj", @"Meshes\10m\Blank.obj")
                            .SetConsistentUVs();
                        nodes4
                            .SetMeshes
                            (@"Meshes\6m\Boosted_Rail_Start.obj",
                            @"Meshes\6m\Ground_Rail_Start_LOD.obj");
                        nodes5
                            .SetMeshes
                            (@"Meshes\6m\Boosted_Rail_End.obj",
                            @"Meshes\6m\Ground_Rail_End_LOD.obj");
                        nodes6
                            .SetMeshes
                            (@"Meshes\6m\Ground_Level_Crossing_Rail_Insert.obj", @"Meshes\10m\Blank.obj")
                            .SetConsistentUVs();

                        RoadHelper.HandleAsymSegmentFlags(segments3);

                        nodes1.m_flagsForbidden = NetNode.Flags.LevelCrossing;
                        nodes3.m_material = defaultMaterial;
                        nodes3.m_lodMaterial = defaultLODMaterial;
                        nodes3.m_directConnect = true;
                        nodes6.m_directConnect = true;
                        info.m_segments = new[] { segments0, segments1, /*segments2*/segments3 };
                        info.m_nodes = new[] { nodes0, nodes1, nodes2, nodes3, nodes4, nodes5, nodes6 };
                        break;
                    }

                case NetInfoVersion.Elevated:
                    {
                        var segments0 = info.m_segments[0].ShallowClone();
                        var segments1 = info.m_segments[1].ShallowClone();

                        var segments3 = info.m_segments[1].ShallowClone();
                        var segments4 = info.m_segments[1].ShallowClone();
                        var nodes0 = info.m_nodes[0].ShallowClone();
                        var nodes1 = info.m_nodes[1].ShallowClone();

                        var nodes3 = info.m_nodes[3].ShallowClone();
                        var nodes4 = info.m_nodes[1].ShallowClone();
                        var nodes5 = info.m_nodes[1].ShallowClone();

                        nodes1.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.Oneway;
                        nodes4.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayStart;
                        nodes5.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayEnd;

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

                        //segments2
                        //    .SetMeshes
                        //    (@"Meshes\6m\Ground_Power.obj",
                        //    @"Meshes\6m\Ground_Power.obj")
                        //    .SetConsistentUVs();
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
                            .SetMeshes
                            (@"Meshes\6m\Boosted_Rail.obj",
                            @"Meshes\6m\Ground_Rail_Node_LOD.obj");
                        //nodes2
                        //    .SetMeshes
                        //    (@"Meshes\6m\Ground_Power.obj",
                        //    @"Meshes\6m\Ground_Power.obj")
                        //    .SetConsistentUVs();
                        nodes4
                            .SetMeshes
                            (@"Meshes\6m\Boosted_Rail_Start.obj",
                            @"Meshes\6m\Ground_Rail_Start_LOD.obj");
                        nodes5
                            .SetMeshes
                            (@"Meshes\6m\Boosted_Rail_End.obj",
                            @"Meshes\6m\Ground_Rail_End_LOD.obj");
                        //nodes6
                        //    .SetMeshes
                        //    (@"Meshes\6m\Ground_Power_Start.obj",
                        //    @"Meshes\6m\Ground_Power_Start.obj")
                        //    .SetConsistentUVs();
                        //nodes7
                        //    .SetMeshes
                        //    (@"Meshes\6m\Ground_Power_End.obj",
                        //    @"Meshes\6m\Ground_Power_End.obj")
                        //    .SetConsistentUVs();
                        RoadHelper.HandleAsymSegmentFlags(segments3);
                        segments0.m_material = defaultBrElMaterial;
                        segments0.m_lodMaterial = defaultBrElLodMaterial;
                        segments3.m_material = defaultBrElMaterial;
                        segments3.m_lodMaterial = defaultBrElLodMaterial;
                        nodes0.m_material = defaultBrElMaterial;
                        nodes0.m_lodMaterial = defaultBrElLodMaterial;
                        info.m_segments = new[] { segments0, segments1 /*,segments2*/, segments3, segments4 };
                        info.m_nodes = new[] { nodes0, nodes1/*, nodes2*/, nodes3, nodes4, nodes5/*, nodes6, nodes7*/ };
                        break;
                    }
                case NetInfoVersion.Bridge:
                    {
                        var segments0 = info.m_segments[0].ShallowClone();
                        var segments1 = info.m_segments[1].ShallowClone();
                        //var segments2 = info.m_segments[2].ShallowClone();
                        var segments3 = info.m_segments[1].ShallowClone();
                        var segments4 = info.m_segments[1].ShallowClone();
                        var nodes0 = info.m_nodes[0].ShallowClone();
                        var nodes1 = info.m_nodes[1].ShallowClone();
                        var nodes2 = info.m_nodes[2].ShallowClone();
                        var nodes3 = info.m_nodes[3].ShallowClone();
                        var nodes4 = info.m_nodes[1].ShallowClone();
                        var nodes5 = info.m_nodes[1].ShallowClone();
                        //var nodes6 = info.m_nodes[2].ShallowClone();
                        //var nodes7 = info.m_nodes[2].ShallowClone();

                        nodes1.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.Oneway;
                        nodes2.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.Oneway;
                        nodes4.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayStart;
                        nodes5.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayEnd;
                        //nodes6.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayStart;
                        //nodes7.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayEnd;

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

                        //segments2
                        //    .SetMeshes
                        //    (@"Meshes\6m\Ground_Power.obj",
                        //    @"Meshes\6m\Ground_Power.obj")
                        //    .SetConsistentUVs();
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
                            .SetMeshes
                            (@"Meshes\6m\Boosted_Rail.obj",
                            @"Meshes\6m\Ground_Rail_Node_LOD.obj");
                        nodes2
                            .SetMeshes
                            (@"Meshes\6m\Ground_Power.obj",
                            @"Meshes\6m\Ground_Power.obj")
                            .SetConsistentUVs();
                        nodes4
                            .SetMeshes
                            (@"Meshes\6m\Boosted_Rail_Start.obj",
                            @"Meshes\6m\Ground_Rail_Start_LOD.obj");
                        nodes5
                            .SetMeshes
                            (@"Meshes\6m\Boosted_Rail_End.obj",
                            @"Meshes\6m\Ground_Rail_End_LOD.obj");
                        //nodes6
                        //    .SetMeshes
                        //    (@"Meshes\6m\Ground_Power_Start.obj",
                        //    @"Meshes\6m\Ground_Power_Start.obj")
                        //    .SetConsistentUVs();
                        //nodes7
                        //    .SetMeshes
                        //    (@"Meshes\6m\Ground_Power_End.obj",
                        //    @"Meshes\6m\Ground_Power_End.obj")
                        //    .SetConsistentUVs();

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
                        segments0.m_material = defaultBrElMaterial;
                        segments0.m_lodMaterial = defaultBrElLodMaterial;
                        nodes0.m_material = defaultBrElMaterial;
                        nodes0.m_lodMaterial = defaultBrElLodMaterial;
                        info.m_segments = new[] { segments0, segments1/*, segments2*/, segments3,segments4 };
                        info.m_nodes = new[] { nodes0, nodes1, nodes2, nodes3, nodes4, nodes5/*, nodes6, nodes7*/ };
                        break;
                    }

                case NetInfoVersion.Slope:
                    {
                        var segments0 = info.m_segments[0].ShallowClone();
                        var segments1 = info.m_segments[1].ShallowClone();
                        //var segments2 = info.m_segments[2].ShallowClone();
                        var segments3 = info.m_segments[1].ShallowClone();
                        var segments4 = info.m_segments[1].ShallowClone();
                        var nodes0 = info.m_nodes[0].ShallowClone();
                        var nodes1 = info.m_nodes[1].ShallowClone();
                        var nodes2 = info.m_nodes[2].ShallowClone();
                        var nodes3 = info.m_nodes[3].ShallowClone();
                        var nodes4 = info.m_nodes[4].ShallowClone();
                        var nodes5 = info.m_nodes[1].ShallowClone();
                        var nodes6 = info.m_nodes[1].ShallowClone();
                        //var nodes7 = info.m_nodes[2].ShallowClone();
                        //var nodes8 = info.m_nodes[2].ShallowClone();
                        var nodes9 = info.m_nodes[0].ShallowClone();
                        var nodes10 = info.m_nodes[1].ShallowClone();
                        //var nodes11 = info.m_nodes[3].ShallowClone();
                        nodes1.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.Oneway;
                        nodes2.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.Oneway;
                        nodes5.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayStart;
                        nodes6.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayEnd;
                        //nodes7.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayStart;
                        //nodes8.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayEnd;

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
                        //segments2
                        //    .SetMeshes
                        //    (@"Meshes\6m\Ground_Power.obj",
                        //    @"Meshes\6m\Ground_Power.obj")
                        //    .SetConsistentUVs();
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
                        //nodes2
                        //    .SetMeshes
                        //    (@"Meshes\6m\Ground_Power.obj",
                        //    @"Meshes\6m\Ground_Power.obj")
                        //    .SetConsistentUVs();
                        nodes3
                            .SetMeshes
                            (@"Meshes\6m\Slope_Node_Pavement.obj",
                            @"Meshes\6m\Ground_Node_Pavement_LOD.obj");
                        nodes5
                            .SetMeshes
                            (@"Meshes\6m\Boosted_Rail_Start.obj",
                            @"Meshes\6m\Ground_Rail_Start_LOD.obj")
                            .SetConsistentUVs();
                        nodes6
                            .SetMeshes
                            (@"Meshes\6m\Boosted_Rail_End.obj",
                            @"Meshes\6m\Ground_Rail_End_LOD.obj")
                            .SetConsistentUVs();
                        //nodes7
                        //    .SetMeshes
                        //    (@"Meshes\6m\Ground_Power_Start.obj",
                        //    @"Meshes\6m\Ground_Power_Start.obj")
                        //    .SetConsistentUVs();
                        //nodes8
                        //    .SetMeshes
                        //    (@"Meshes\6m\Ground_Power_End.obj",
                        //    @"Meshes\6m\Ground_Power_End.obj")
                        //    .SetConsistentUVs();
                        nodes9
                            .SetMeshes
                            (@"Meshes\6m\Slope_U_Node_Pavement.obj",
                                @"Meshes\6m\Tunnel_Node_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        nodes10
                            .SetFlags(NetNode.Flags.Underground, NetNode.Flags.None)
                            .SetMeshes
                            (@"Meshes\6m\Boosted_Rail.obj")
                            .SetConsistentUVs();
                        //nodes11
                        //    .SetFlags(NetNode.Flags.Underground, NetNode.Flags.None)
                        //    .SetMeshes
                        //    (@"Meshes\6m\ThirdRail_Node.obj", @"Meshes\10m\Blank.obj")
                        //    .SetConsistentUVs();
                        //var colors = new List<UnityEngine.Color>();
                        //var colors32 = new List<UnityEngine.Color32>();
                        //var tangents = new List<UnityEngine.Vector4>();

                        //for (int i = 0; i < segments2.m_mesh.vertexCount; i++)
                        //{
                        //    colors.Add(new UnityEngine.Color(0, 0, 0, 255));
                        //    colors32.Add(new UnityEngine.Color32(0, 0, 0, 255));
                        //    tangents.Add(new UnityEngine.Vector4(0, 0, 1, -1));
                        //}

                        //segments2.m_mesh.colors = colors.ToArray();
                        //segments2.m_mesh.colors32 = colors32.ToArray();
                        //segments2.m_mesh.tangents = tangents.ToArray();
                        RoadHelper.HandleAsymSegmentFlags(segments4);
                        //nodes2.m_mesh.colors = colors.ToArray();
                        //nodes2.m_mesh.colors32 = colors32.ToArray();
                        //nodes2.m_mesh.tangents = tangents.ToArray();

                        //nodes7.m_mesh.colors = colors.ToArray();
                        //nodes7.m_mesh.colors32 = colors32.ToArray();
                        //nodes7.m_mesh.tangents = tangents.ToArray();

                        //nodes8.m_mesh.colors = colors.ToArray();
                        //nodes8.m_mesh.colors32 = colors32.ToArray();
                        //nodes8.m_mesh.tangents = tangents.ToArray();
                        segments3.m_material = defaultBrElMaterial;
                        segments3.m_lodMaterial = defaultBrElLodMaterial;
                        nodes9.m_material = defaultBrElMaterial;
                        nodes9.m_lodMaterial = defaultBrElLodMaterial;

                        info.m_segments = new[] { segments0, segments1, /*segments2,*/ segments3, segments4 };
                        info.m_nodes = new[] { nodes0, nodes1, nodes2, nodes3, nodes4, nodes5, nodes6/*, nodes7, nodes8*/, nodes9, nodes10, /*nodes11*/ };
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
                        //var node3 = info.m_nodes[0].ShallowClone();
                        //var node4 = info.m_nodes[0].ShallowClone();
                        var node5 = info.m_nodes[0].ShallowClone();
                        var node6 = info.m_nodes[0].ShallowClone();
                        node2.m_connectGroup = NetInfo.ConnectGroup.CenterTram | NetInfo.ConnectGroup.Oneway;
                        //node3.m_flagsRequired = NetNode.Flags.OneWayOut;
                        //node4.m_flagsRequired = NetNode.Flags.OneWayIn;
                        node5.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayStart;
                        node6.m_connectGroup = NetInfo.ConnectGroup.NarrowTram | NetInfo.ConnectGroup.OnewayEnd;
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
                        //node3
                        //    .SetMeshes
                        //    (@"Meshes\6m\ThirdRail_Node.obj", @"Meshes\10m\Blank.obj")
                        //    .SetConsistentUVs();
                        //node4
                        //    .SetMeshes
                        //    (@"Meshes\6m\ThirdRail_Node_Inv.obj", @"Meshes\10m\Blank.obj")
                        //    .SetConsistentUVs();
                        node5
                            .SetMeshes
                            (@"Meshes\6m\Boosted_Rail_Start.obj",
                            @"Meshes\6m\Ground_Rail_Start_LOD.obj")
                            .SetConsistentUVs();
                        node6
                            .SetMeshes
                            (@"Meshes\6m\Boosted_Rail_End.obj",
                            @"Meshes\6m\Ground_Rail_End_LOD.obj")
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
                        //node3.m_material = defaultMaterial;
                        //node3.m_lodMaterial = defaultLODMaterial;
                        //node4.m_material = defaultMaterial;
                        //node4.m_lodMaterial = defaultLODMaterial;
                        node5.m_material = defaultMaterial;
                        node5.m_lodMaterial = defaultLODMaterial;
                        node5.m_directConnect = true;
                        node6.m_material = defaultMaterial;
                        node6.m_lodMaterial = defaultLODMaterial;
                        node6.m_directConnect = true;
                        info.m_segments = new[] { segment0, segment1, segment2, segment3 };
                        info.m_nodes = new[] { node0, node1, node2, /*node3, node4,*/ node5, node6 };
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
            var elevatedMaterial = elevatedInfo.m_segments[0].m_material;
            var elevatedLODMaterial = elevatedInfo.m_segments[0].m_lodMaterial;

            switch (version)
            {
                case NetInfoVersion.Ground:
                    {
                        var segment0 = prefab.m_segments[0].ShallowClone();
                        var segment1 = prefab.m_segments[1].ShallowClone();
                        var segment2 = prefab.m_segments[0].ShallowClone();
                        var node0 = prefab.m_nodes[0].ShallowClone();
                        var node1 = prefab.m_nodes[1].ShallowClone();
                        var node2 = prefab.m_nodes[2].ShallowClone();
                        var node3 = prefab.m_nodes[1].ShallowClone();
                        var node4 = prefab.m_nodes[0].ShallowClone();

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
                            (@"Meshes\6m\Ground_Rail.obj")
                            .SetConsistentUVs();
                        node2
                            .SetMeshes
                            (@"Meshes\6m\LevelCrossing_Pavement.obj",
                                @"Meshes\6m\LevelCrossing_Pavement_LOD.obj")
                            .SetConsistentUVs();
                        node3
                            .SetMeshes
                            (@"Meshes\6m\Ground_Rail.obj")
                            .SetConsistentUVs();
                        node4
                            .SetMeshes
                            (@"Meshes\6m\ThirdRail_Node.obj", @"Meshes\6m\Blank.obj")
                            .SetConsistentUVs();
                        segment0.m_material = elevatedMaterial;
                        segment0.m_lodMaterial = elevatedLODMaterial;
                        node0.m_material = elevatedMaterial;
                        node0.m_lodMaterial = elevatedLODMaterial;
                        node2.m_lodMaterial = elevatedLODMaterial;

                        node1.m_flagsForbidden = NetNode.Flags.LevelCrossing;
                        node2.m_flagsRequired = NetNode.Flags.LevelCrossing;
                        node3.m_flagsRequired = NetNode.Flags.LevelCrossing;

                        prefab.m_segments = new[] { segment0, segment1, segment2 };
                        prefab.m_nodes = new[] { node0, node1, node2, node3, node4 };
                        break;
                    }
                case NetInfoVersion.Elevated:
                    {
                        var segment0 = prefab.m_segments[0].ShallowClone();
                        var segment1 = prefab.m_segments[1].ShallowClone();
                        var segment2 = prefab.m_segments[0].ShallowClone();
                        var node0 = prefab.m_nodes[0].ShallowClone();
                        var node1 = prefab.m_nodes[1].ShallowClone();
                        var node2 = prefab.m_nodes[0].ShallowClone();
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
                            (@"Meshes\6m\Ground_Rail.obj")
                            .SetConsistentUVs();
                        node2
                            .SetMeshes
                            (@"Meshes\6m\ThirdRail_Node.obj", @"Meshes\6m\Blank.obj")
                            .SetConsistentUVs();
                        segment0.m_material = elevatedMaterial;
                        segment0.m_lodMaterial = elevatedLODMaterial;
                        node0.m_material = elevatedMaterial;
                        node0.m_lodMaterial = elevatedLODMaterial;

                        prefab.m_segments = new[] { segment0, segment1, segment2 };
                        prefab.m_nodes = new[] { node0, node1, node2 };
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
                        var node3 = metroStationInfo.m_nodes[0].ShallowClone();

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
                            (@"Meshes\6m\Ground_Rail.obj")
                            .SetConsistentUVs();
                        node3
                            .SetMeshes
                            (@"Meshes\6m\ThirdRail_Node.obj", @"Meshes\6m\Blank.obj")
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
                        node3.m_material = elevatedMaterial;
                        node3.m_lodMaterial = elevatedLODMaterial;
                        prefab.m_segments = new[] { segment0, segment1, segment2, segment3 };
                        prefab.m_nodes = new[] { node0, node1, node2, node3 };
                        break;
                    }
            }
        }
    }
}
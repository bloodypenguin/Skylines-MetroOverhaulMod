using MetroOverhaul.NEXT;
using MetroOverhaul.NEXT.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MetroOverhaul.InitializationSteps
{
	public static class SetupMeshUtil
	{
		public static string WidthName = null;
		public static string[] Variations = null;
		public static NetInfo.ConnectGroup[] Groups = null;
		public static List<NetInfo.ConnectGroup> LikenessGroups = null;
		public static List<NetInfo.Node> NodeList = null;
		public static Material DefaultMaterial = null;
		public static Material DefaultLODMaterial = null;
		public static Material DefaultElMaterial = null;
		public static Material DefaultLodElMaterial = null;
		private static int m_Till = -1;
		private static bool m_IsOneWay = false;
		public static List<NetInfo.Node> GenerateSplitTracksAndLevelCrossings(NetInfo info, NetInfoVersion version)
		{
			var ttInfo = PrefabCollection<NetInfo>.FindLoaded("Train Track");
			var isStation = info.name.Contains("Station");
			var brElInfo = Prefabs.Find<NetInfo>("Basic Road Elevated");
			var ttElInfo = Prefabs.Find<NetInfo>("Train Track Elevated");
			DefaultElMaterial = brElInfo.m_segments[0].m_material;
			DefaultLodElMaterial = brElInfo.m_segments[0].m_lodMaterial;
			DefaultMaterial = ttInfo.m_nodes[0].m_material;
			DefaultLODMaterial = ttInfo.m_nodes[0].m_lodMaterial;
			NodeList = new List<NetInfo.Node>();
			m_IsOneWay = (info.m_connectGroup & (NetInfo.ConnectGroup.MonorailStation | NetInfo.ConnectGroup.CenterTram)) != NetInfo.ConnectGroup.None;
			var isSteel = info.name.Contains("Steel");
			PopulateMetaData(info);
			CreateBaseLevelCrossing(info, version);
			if (LikenessGroups != null && (info.m_connectGroup & (NetInfo.ConnectGroup.DoubleMonorail | (NetInfo.ConnectGroup)32768)) == NetInfo.ConnectGroup.None)
			{
				NetInfo.Node node0 = null;
				NetInfo.Node node1 = null;
				NetInfo.Node node2 = null;
				NetInfo.Node node3 = null;
				if (version == NetInfoVersion.Tunnel)
				{
					node0 = info.m_nodes[0].ShallowClone();
					node0.m_material = DefaultElMaterial;
					node0.m_lodMaterial = DefaultLodElMaterial;
					node0.m_connectGroup = info.m_connectGroup;
					node0.m_directConnect = true;
					node3 = info.m_nodes[0].ShallowClone();
					node3.m_material = DefaultElMaterial;
					node3.m_lodMaterial = DefaultLodElMaterial;
					node3.m_connectGroup = info.m_connectGroup;
					node3.m_directConnect = true;
					if (isSteel && (version & (NetInfoVersion.Elevated | NetInfoVersion.Bridge)) != NetInfoVersion.None)
					{
						node1 = info.m_nodes[0].ShallowClone();
						node1.m_material = DefaultElMaterial;
						node1.m_lodMaterial = DefaultLodElMaterial;
						node1.m_connectGroup = info.m_connectGroup;

						NodeList.Add(node1);
						node2 = info.m_nodes[0].ShallowClone();
						node2.m_material = DefaultElMaterial;
						node2.m_lodMaterial = DefaultLodElMaterial;
						node2.m_connectGroup = info.m_connectGroup;
						node1.m_directConnect = true;
						node2.m_directConnect = true;
						NodeList.Add(node2);
					}
				}
				else
				{
					node0 = info.m_nodes[1].ShallowClone();
					node0.m_directConnect = true;
					node3 = info.m_nodes[0].ShallowClone();
					node3.m_directConnect = true;
					if (isSteel && (version & (NetInfoVersion.Elevated | NetInfoVersion.Bridge)) != NetInfoVersion.None)
					{
						node1 = info.m_nodes[0].ShallowClone();
						node1.m_material = DefaultElMaterial;
						node1.m_lodMaterial = DefaultLodElMaterial;
						node1.m_directConnect = true;
						NodeList.Add(node1);
						node2 = info.m_nodes[0].ShallowClone();
						node2.m_material = DefaultElMaterial;
						node2.m_lodMaterial = DefaultLodElMaterial;
						node2.m_directConnect = true;
						NodeList.Add(node2);
					}
				}
				NodeList.Add(node0);
				if (isSteel)
				{
					if ((version & (NetInfoVersion.Elevated | NetInfoVersion.Bridge)) != NetInfoVersion.None)
					{
						if (isStation && WidthName != "6m")
						{
							node3.m_directConnect = true;
							NodeList.Add(node3);
							if (WidthName == "18m")
							{
								node0
								.SetMeshes
								($@"Meshes\{WidthName}\Station_Node_Rail.obj",
								$@"Meshes\{WidthName}\Rail_LOD.obj")
								.SetConsistentUVs();
								node3
								.SetMeshes
								($@"Meshes\{WidthName}\Boosted_Station_Node_Rail.obj",
								$@"Meshes\{WidthName}\Rail_LOD.obj")
								.SetConsistentUVs();
							}
							else
							{
								node0
									.SetMeshes
									($@"Meshes\{WidthName}\Station_Node_Rail.obj")
									.SetConsistentUVs();
								node3
									.SetMeshes
									($@"Meshes\{WidthName}\Boosted_Station_Node_Rail.obj")
									.SetConsistentUVs();
							}
						}
						else
						{
							if (WidthName == "18m")
							{
								node0
									.SetMeshes
									($@"Meshes\{WidthName}\Rail.obj",
									$@"Meshes\{WidthName}\Rail_LOD.obj")
									.SetConsistentUVs();
							}
							else
							{
								node0
									.SetMeshes
									($@"Meshes\{WidthName}\Rail.obj")
									.SetConsistentUVs();
							}
						}
						node1
							.SetMeshes
							($@"Meshes\{WidthName}\Boosted_Rail_Steel_Insert.obj", @"Meshes\6m\Blank.obj")
							.SetConsistentUVs();
						node2
							.SetMeshes
							($@"Meshes\{WidthName}\Elevated_Node_Pavement_Steel_Insert.obj", @"Meshes\6m\Blank.obj")
							.SetConsistentUVs();
					}
					else
					{
						node3.m_directConnect = true;
						NodeList.Add(node3);
						if (WidthName == "18m")
						{
							node0
								.SetMeshes
								($@"Meshes\{WidthName}\{(isStation && WidthName != "6m" ? "Station_Node_" : "")}Rail.obj",
								$@"Meshes\{WidthName}\Rail_LOD.obj")
								.SetConsistentUVs();
							node3
								.SetMeshes
								($@"Meshes\{WidthName}\Boosted_{(isStation && WidthName != "6m" ? "Station_Node_" : "")}Rail.obj",
								$@"Meshes\{WidthName}\Rail_LOD.obj")
								.SetConsistentUVs();
						}
						else
						{
							node0
								.SetMeshes
								($@"Meshes\{WidthName}\{(isStation && WidthName != "6m" ? "Station_Node_" : "")}Rail.obj")
								.SetConsistentUVs();
							node3
								.SetMeshes
								($@"Meshes\{WidthName}\Boosted_{(isStation && WidthName != "6m" ? "Station_Node_" : "")}Rail.obj")
								.SetConsistentUVs();
						}
					}
				}

				else
				{
					node3.m_directConnect = true;
					NodeList.Add(node3);
					if (WidthName == "18m")
					{
						node0
							.SetMeshes
							($@"Meshes\{WidthName}\{(isStation && WidthName != "6m" ? "Station_Node_" : "")}Rail.obj",
							 $@"Meshes\{WidthName}\Rail_LOD.obj")
							.SetConsistentUVs();
						node3
							.SetMeshes
							($@"Meshes\{WidthName}\Boosted_{(isStation && WidthName != "6m" ? "Station_Node_" : "")}Rail.obj",
							 $@"Meshes\{WidthName}\Rail_LOD.obj")
							.SetConsistentUVs();
					}
					else
					{
						node0
							.SetMeshes
							($@"Meshes\{WidthName}\{(isStation && WidthName != "6m" ? "Station_Node_" : "")}Rail.obj")
							.SetConsistentUVs();
						node3
							.SetMeshes
							($@"Meshes\{WidthName}\Boosted_{(isStation && WidthName != "6m" ? "Station_Node_" : "")}Rail.obj")
							.SetConsistentUVs();
					}

				}

				for (var i = 0; i < LikenessGroups.Count; i++)
				{
					if (i == 0)
					{
						node0.m_connectGroup = LikenessGroups[i];
						node3.m_connectGroup = LikenessGroups[i];
						if (isSteel && (version & (NetInfoVersion.Elevated | NetInfoVersion.Bridge)) != NetInfoVersion.None)
						{
							node1.m_connectGroup = LikenessGroups[i];
							node2.m_connectGroup = LikenessGroups[i];
						}
					}
					else
					{
						node0.m_connectGroup |= LikenessGroups[i];
						node3.m_connectGroup |= LikenessGroups[i];
						if (isSteel && (version & (NetInfoVersion.Elevated | NetInfoVersion.Bridge)) != NetInfoVersion.None)
						{
							node1.m_connectGroup |= LikenessGroups[i];
							node2.m_connectGroup |= LikenessGroups[i];
						}
					}
				}
			}
			if ((info.m_connectGroup & ((NetInfo.ConnectGroup)2048 | NetInfo.ConnectGroup.NarrowTram)) == NetInfo.ConnectGroup.None)
			{
				if (Variations != null)
				{
					m_Till = -1;
					for (var i = 0; i < Variations.Length; i++)
					{
						if (i > m_Till)
						{
							for (var j = i; j < Variations.Length; j++)
							{
								if (Variations[j] == Variations[i])
								{
									m_Till = j;
								}
								else
								{
									break;
								}
							}
							if (info.name.Contains("Steel"))
							{
								CreateClassicSplitTracks(info, version, i);
							}
							else
							{
								CreateModernSplitTracks(info, version, i);
							}
							if (version != NetInfoVersion.Tunnel)
							{
								CreateSplitLevelCrossings(info, i);
							}
						}

					}
				}
			}
			return NodeList;
		}
		public static void PopulateMetaData(NetInfo info)
		{
			LikenessGroups = null;
			LikenessGroups = new List<NetInfo.ConnectGroup>();
			LikenessGroups.Add(info.m_connectGroup);
			info.m_nodeConnectGroups = info.m_connectGroup;
			switch (info.m_connectGroup)
			{
				case NetInfo.ConnectGroup.SingleMonorail:
					Variations = new[] { "_Merge", "_Merge", "_Merge" };
					Groups = new[] { NetInfo.ConnectGroup.NarrowTram, NetInfo.ConnectGroup.MonorailStation, (NetInfo.ConnectGroup)2048 };
					WidthName = "6m";
					break;
				case NetInfo.ConnectGroup.CenterTram:
					LikenessGroups.Add(NetInfo.ConnectGroup.SingleMonorail);
					Variations = new[] { "", "_Merge", "_Merge", "_Quad", "_Quad" };
					Groups = new[] { NetInfo.ConnectGroup.NarrowTram, NetInfo.ConnectGroup.MonorailStation, (NetInfo.ConnectGroup)2048, NetInfo.ConnectGroup.WideTram, (NetInfo.ConnectGroup)128 };
					WidthName = "6m";
					break;
				case NetInfo.ConnectGroup.MonorailStation:
					Variations = new[] { "_Merge", "", "" };
					Groups = new[] { NetInfo.ConnectGroup.NarrowTram, NetInfo.ConnectGroup.WideTram, (NetInfo.ConnectGroup)128 };
					WidthName = "10m";
					break;
				case NetInfo.ConnectGroup.NarrowTram:
					WidthName = "10m";
					break;
				case (NetInfo.ConnectGroup)2048:
					WidthName = "10m";
					LikenessGroups.Add(NetInfo.ConnectGroup.NarrowTram);
					LikenessGroups.Add(NetInfo.ConnectGroup.MonorailStation);
					break;
				case NetInfo.ConnectGroup.WideTram:
					Variations = new[] { "_Merge", "_Merge", "_Single_Merge" };
					Groups = new[] { NetInfo.ConnectGroup.NarrowTram, (NetInfo.ConnectGroup)2048, NetInfo.ConnectGroup.SingleMonorail };
					WidthName = "18m";
					break;
				case (NetInfo.ConnectGroup)128:
					WidthName = "18m";
					LikenessGroups.Add(NetInfo.ConnectGroup.WideTram);
					Variations = new[] { "_Merge", "_Merge", "_Single_Merge" };
					Groups = new[] { NetInfo.ConnectGroup.NarrowTram, (NetInfo.ConnectGroup)2048, NetInfo.ConnectGroup.SingleMonorail };
					break;
				case NetInfo.ConnectGroup.DoubleMonorail:
					WidthName = "19m";
					Variations = new[] { "_Merge", "_Merge", "_Single_Merge", "_Single_Merge", "_Quad" };
					Groups = new[] { NetInfo.ConnectGroup.NarrowTram, NetInfo.ConnectGroup.MonorailStation, NetInfo.ConnectGroup.CenterTram, NetInfo.ConnectGroup.SingleMonorail, NetInfo.ConnectGroup.WideTram };
					break;
                case (NetInfo.ConnectGroup)32768:
                    WidthName = "27m";
                    Variations = new[] { "_Merge", "_Merge", "_Single_Merge", "_Single_Merge", "_Quad" };
                    Groups = new[] { NetInfo.ConnectGroup.NarrowTram, NetInfo.ConnectGroup.MonorailStation, NetInfo.ConnectGroup.CenterTram, NetInfo.ConnectGroup.SingleMonorail, NetInfo.ConnectGroup.WideTram };
                    break;
            }
			if (Groups != null)
			{
				for (var i = 0; i < Groups.Length; i++)
				{
					info.m_nodeConnectGroups |= Groups[i];
				}
			}
			if (LikenessGroups.Count > 1)
			{
				for (var i = 1; i < LikenessGroups.Count; i++)
				{
					if (Groups == null || Array.IndexOf(Groups, LikenessGroups[i]) == -1)
						info.m_nodeConnectGroups |= LikenessGroups[i];
				}
			}

		}

		public static void CreateBaseLevelCrossing(NetInfo info, NetInfoVersion version)
		{
			var isStation = info.name.Contains("Station");
			if (version != NetInfoVersion.Tunnel && (!isStation || LikenessGroups.Count > 1))
			{
				var pavementIndex = -1;
				var railIndex = -1;
				for (var i = 0; i < info.m_nodes.Length; i++)
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
				for (var i = 0; i < info.m_nodes.Length; i++)
				{
					if (info.m_nodes[i].m_directConnect == true)
					{
						railIndex = i;
						break;
					}
				}
				var nodes0 = info.m_nodes[pavementIndex].ShallowClone();
				var nodes1 = info.m_nodes[railIndex].ShallowClone();
				var nodes2 = info.m_nodes[pavementIndex].ShallowClone();

				NodeList.Add(nodes0);
				NodeList.Add(nodes1);
				NodeList.Add(nodes2);

				var isSteel = info.name.Contains("Steel");
				var prefix = "";
				if (version == NetInfoVersion.Elevated || version == NetInfoVersion.Bridge)
				{
					prefix = "Elevated";
				}
				else
				{
					prefix = version.ToString();
				}
				nodes0
					.SetMeshes
					($@"Meshes\{WidthName}\{prefix}_LevelCrossing_Pavement.obj",
					$@"Meshes\{WidthName}\LevelCrossing_Pavement_LOD.obj")
					.SetConsistentUVs();
				if (WidthName != "10m")
				{
					nodes1
						.SetFlags(NetNode.Flags.LevelCrossing, NetNode.Flags.None)
						.SetMeshes
						($@"Meshes\{WidthName}\LevelCrossing_{(isStation && WidthName != "6m" ? "Station_" : "")}Rail.obj",
						$@"Meshes\{WidthName}\LevelCrossing_Rail_LOD.obj")
						.SetConsistentUVs();
				}
				else
				{
					nodes1
						.SetFlags(NetNode.Flags.LevelCrossing, NetNode.Flags.None)
						.SetMeshes
						($@"Meshes\{WidthName}\LevelCrossing_{(isStation && WidthName != "6m" ? "Station_" : "")}Rail.obj")
						.SetConsistentUVs();
				}


				nodes2
					.SetMeshes
					($@"Meshes\{WidthName}\LevelCrossing_{(isStation && WidthName != "6m" ? "Station_" : "")}Rail_Insert.obj", @"Meshes\10m\Blank.obj")
					.SetConsistentUVs();

				if (!isSteel && (version == NetInfoVersion.Elevated || version == NetInfoVersion.Bridge))
				{
					var nodes3 = info.m_nodes[pavementIndex].ShallowClone();
					NodeList.Add(nodes3);
					nodes3
					.SetMeshes
					($@"Meshes\{WidthName}\{prefix}_Node_Pavement_Short.obj",
					$@"Meshes\{WidthName}\LevelCrossing_Pavement_LOD.obj")
					.SetConsistentUVs();
				}

				nodes1.m_directConnect = true;
				nodes2.m_directConnect = true;

				for (var i = 0; i < LikenessGroups.Count; i++)
				{
					if (i == 0)
					{
						nodes1.m_connectGroup = LikenessGroups[i];
						nodes2.m_connectGroup = LikenessGroups[i];
					}
					else
					{
						nodes1.m_connectGroup |= LikenessGroups[i];
						nodes2.m_connectGroup |= LikenessGroups[i];
					}
				}
			}
		}
		public static void CreateClassicSplitTracks(NetInfo info, NetInfoVersion version, int index)
		{
			NetInfo.Node node1 = null;
			NetInfo.Node node1a = null;
			NetInfo.Node node2 = null;
			NetInfo.Node node2a = null;

			if (version == NetInfoVersion.Elevated || version == NetInfoVersion.Bridge)
			{
				node1 = info.m_nodes[1].ShallowClone();
				node1a = info.m_nodes[0].ShallowClone();
				node1a.m_directConnect = true;
				NetInfo.Node node3 = info.m_nodes[0].ShallowClone();
				NetInfo.Node node4 = null;
				NetInfo.Node node5 = info.m_nodes[0].ShallowClone();
				NetInfo.Node node6 = null;

				NodeList.Add(node1);
				NodeList.Add(node1a);
				NodeList.Add(node3);
				NodeList.Add(node5);
				if (m_IsOneWay)
				{
					node2 = info.m_nodes[1].ShallowClone();
					node2a = info.m_nodes[0].ShallowClone();
					node4 = info.m_nodes[0].ShallowClone();
					node6 = info.m_nodes[0].ShallowClone();

					NodeList.Add(node2);
					NodeList.Add(node2a);
					NodeList.Add(node4);
					NodeList.Add(node6);

					node1
						.SetFlags(NetNode.Flags.None, NetNode.Flags.None)
						.SetMeshes
						   ($@"Meshes\{WidthName}\Rail{Variations[index]}_Start.obj",
						  @"Meshes\6m\Ground_Rail_Start_LOD.obj")
						 .SetConsistentUVs();
					node1a
						.SetFlags(NetNode.Flags.Transition, NetNode.Flags.None)
						.SetMeshes
						   ($@"Meshes\{WidthName}\Boosted_Rail{Variations[index]}_Start.obj",
						   @"Meshes\6m\Ground_Rail_Start_LOD.obj")
						  .SetConsistentUVs();
					node2
						.SetFlags(NetNode.Flags.None, NetNode.Flags.None)
						.SetMeshes
						   ($@"Meshes\{WidthName}\Rail{Variations[index]}_End.obj",
						  @"Meshes\6m\Ground_Rail_End_LOD.obj")
						 .SetConsistentUVs();
					node2a
						.SetFlags(NetNode.Flags.Transition, NetNode.Flags.None)
						.SetMeshes
							($@"Meshes\{WidthName}\Boosted_Rail{Variations[index]}_End.obj",
							@"Meshes\6m\Ground_Rail_End_LOD.obj")
							.SetConsistentUVs();
					node3
						.SetFlags(NetNode.Flags.None, NetNode.Flags.Transition)
						.SetMeshes
						   ($@"Meshes\{WidthName}\Boosted_Rail_Steel_Insert{Variations[index]}_Start.obj",
						  @"Meshes\6m\Ground_Rail_Start_LOD.obj")
						 .SetConsistentUVs();
					node4
						.SetFlags(NetNode.Flags.None, NetNode.Flags.Transition)
						.SetMeshes
						   ($@"Meshes\{WidthName}\Boosted_Rail_Steel_Insert{Variations[index]}_End.obj",
						   @"Meshes\6m\Ground_Rail_End_LOD.obj")
						  .SetConsistentUVs();
					node5
						.SetFlags(NetNode.Flags.None, NetNode.Flags.Transition)
						.SetMeshes
						   ($@"Meshes\{WidthName}\Elevated_Node_Pavement_Steel_Insert{Variations[index]}_Start.obj",
						  @"Meshes\6m\Ground_Rail_Start_LOD.obj")
						 .SetConsistentUVs();
					node6
						.SetFlags(NetNode.Flags.None, NetNode.Flags.Transition)
						.SetMeshes
						   ($@"Meshes\{WidthName}\Elevated_Node_Pavement_Steel_Insert{Variations[index]}_End.obj",
						   @"Meshes\6m\Ground_Rail_End_LOD.obj")
						  .SetConsistentUVs();

					node2.m_directConnect = true;
					node2a.m_directConnect = true;
					node3.m_directConnect = true;
					node4.m_directConnect = true;
					node5.m_directConnect = true;
					node6.m_directConnect = true;

					node3.m_material = DefaultElMaterial;
					node3.m_lodMaterial = DefaultLodElMaterial;
					node4.m_material = DefaultElMaterial;
					node4.m_lodMaterial = DefaultLodElMaterial;
					node5.m_material = DefaultElMaterial;
					node5.m_lodMaterial = DefaultLodElMaterial;
					node6.m_material = DefaultElMaterial;
					node6.m_lodMaterial = DefaultLodElMaterial;

					node1.m_connectGroup = NetInfo.ConnectGroup.OnewayStart;
					node1a.m_connectGroup = NetInfo.ConnectGroup.OnewayStart;
					node2.m_connectGroup = NetInfo.ConnectGroup.OnewayEnd;
					node2a.m_connectGroup = NetInfo.ConnectGroup.OnewayEnd;
					node3.m_connectGroup = NetInfo.ConnectGroup.OnewayStart;
					node4.m_connectGroup = NetInfo.ConnectGroup.OnewayEnd;
					node5.m_connectGroup = NetInfo.ConnectGroup.OnewayStart;
					node6.m_connectGroup = NetInfo.ConnectGroup.OnewayEnd;

					for (var j = index; j <= m_Till; j++)
					{
						node1.m_connectGroup |= Groups[j];
						node1a.m_connectGroup |= Groups[j];
						node2.m_connectGroup |= Groups[j];
						node2a.m_connectGroup |= Groups[j];
						node3.m_connectGroup |= Groups[j];
						node4.m_connectGroup |= Groups[j];
						node5.m_connectGroup |= Groups[j];
						node6.m_connectGroup |= Groups[j];
					}
				}
				else
				{
					node1
						.SetFlags(NetNode.Flags.None, NetNode.Flags.Transition)
						.SetMeshes
						   ($@"Meshes\{WidthName}\Rail{Variations[index]}.obj",
						  @"Meshes\6m\Ground_Rail_Start_LOD.obj")
						 .SetConsistentUVs();
					node1a
						.SetFlags(NetNode.Flags.Transition, NetNode.Flags.None)
						.SetMeshes
						   ($@"Meshes\{WidthName}\Boosted_Rail{Variations[index]}.obj",
						   @"Meshes\6m\Ground_Rail_Start_LOD.obj")
						  .SetConsistentUVs();
					node3
						.SetFlags(NetNode.Flags.None, NetNode.Flags.Transition)
						.SetMeshes
						   ($@"Meshes\{WidthName}\Boosted_Rail_Steel_Insert{Variations[index]}.obj",
						  @"Meshes\6m\Ground_Rail_Start_LOD.obj")
						 .SetConsistentUVs();
					node5
						.SetFlags(NetNode.Flags.None, NetNode.Flags.Transition)
						.SetMeshes
						   ($@"Meshes\{WidthName}\Elevated_Node_Pavement_Steel_Insert{Variations[index]}.obj",
						  @"Meshes\6m\Ground_Rail_Start_LOD.obj")
						 .SetConsistentUVs();

					node3.m_directConnect = true;
					node5.m_directConnect = true;

					node3.m_material = DefaultElMaterial;
					node3.m_lodMaterial = DefaultLodElMaterial;
					node5.m_material = DefaultElMaterial;
					node5.m_lodMaterial = DefaultLodElMaterial;
					for (var j = index; j <= m_Till; j++)
					{
						if (j == index)
						{
							node1.m_connectGroup = Groups[j];
							node1a.m_connectGroup = Groups[j];
							node3.m_connectGroup = Groups[j];
							node5.m_connectGroup = Groups[j];
						}
						else
						{
							node1.m_connectGroup |= Groups[j];
							node1a.m_connectGroup |= Groups[j];
							node3.m_connectGroup |= Groups[j];
							node5.m_connectGroup |= Groups[j];
						}
					}
				}
			}
			else
			{
				if (version == NetInfoVersion.Tunnel)
				{
					node1 = info.m_nodes[0].ShallowClone();
					node1.m_material = DefaultMaterial;
					node1.m_lodMaterial = DefaultLODMaterial;
					node1.m_directConnect = true;
					node1a = info.m_nodes[0].ShallowClone();
					node1a.m_material = DefaultMaterial;
					node1a.m_lodMaterial = DefaultLODMaterial;
					node1a.m_directConnect = true;
					if (m_IsOneWay)
					{
						node2 = info.m_nodes[0].ShallowClone();
						node2.m_material = DefaultMaterial;
						node2.m_lodMaterial = DefaultLODMaterial;
						node2.m_directConnect = true;
						node2a = info.m_nodes[0].ShallowClone();
						node2a.m_material = DefaultMaterial;
						node2a.m_lodMaterial = DefaultLODMaterial;
						node2a.m_directConnect = true;
					}
				}
				else
				{
					node1 = info.m_nodes[1].ShallowClone();
					node1a = info.m_nodes[0].ShallowClone();
					if (m_IsOneWay)
					{
						node2 = info.m_nodes[1].ShallowClone();
						node2a = info.m_nodes[0].ShallowClone();
					}

				}

				NodeList.Add(node1);
				NodeList.Add(node1a);
				node1.m_directConnect = true;
				node1a.m_directConnect = true;

				if (m_IsOneWay)
				{
					NodeList.Add(node2);
					NodeList.Add(node2a);
					node2.m_directConnect = true;
					node2a.m_directConnect = true;

					node1
						.SetMeshes
						   ($@"Meshes\{WidthName}\Rail{Variations[index]}_Start.obj",
						  @"Meshes\6m\Ground_Rail_Start_LOD.obj")
						 .SetConsistentUVs();
					node2
						.SetMeshes
						   ($@"Meshes\{WidthName}\Rail{Variations[index]}_End.obj",
						   @"Meshes\6m\Ground_Rail_End_LOD.obj")
						  .SetConsistentUVs();
					node1a
						.SetMeshes
						   ($@"Meshes\{WidthName}\Boosted_Rail{Variations[index]}_Start.obj",
						  @"Meshes\6m\Ground_Rail_Start_LOD.obj")
						 .SetConsistentUVs();
					node2a
						.SetMeshes
						   ($@"Meshes\{WidthName}\Boosted_Rail{Variations[index]}_End.obj",
						   @"Meshes\6m\Ground_Rail_End_LOD.obj")
						  .SetConsistentUVs();
					node1.m_connectGroup = NetInfo.ConnectGroup.OnewayStart;
					node1a.m_connectGroup = NetInfo.ConnectGroup.OnewayStart;
					node2.m_connectGroup = NetInfo.ConnectGroup.OnewayEnd;
					node2a.m_connectGroup = NetInfo.ConnectGroup.OnewayEnd;
					for (var j = index; j <= m_Till; j++)
					{
						node1.m_connectGroup |= Groups[j];
						node1a.m_connectGroup |= Groups[j];
						node2.m_connectGroup |= Groups[j];
						node2a.m_connectGroup |= Groups[j];
					}
				}
				else
				{
					node1
						.SetMeshes
						   ($@"Meshes\{WidthName}\Rail{Variations[index]}.obj",
						  @"Meshes\6m\Ground_Rail_Start_LOD.obj")
						 .SetConsistentUVs();
					node1a
						.SetMeshes
						   ($@"Meshes\{WidthName}\Boosted_Rail{Variations[index]}.obj",
						  @"Meshes\6m\Ground_Rail_Start_LOD.obj")
						 .SetConsistentUVs();
					for (var j = index; j <= m_Till; j++)
					{
						if (j == index)
						{
							node1.m_connectGroup = Groups[j];
							node1a.m_connectGroup = Groups[j];
						}
						else
						{
							node1.m_connectGroup |= Groups[j];
							node1a.m_connectGroup |= Groups[j];
						}
					}
				}
			}
		}
		public static void CreateModernSplitTracks(NetInfo info, NetInfoVersion version, int index)
		{
			NetInfo.Node node1 = null;
			NetInfo.Node node2 = null;
			NetInfo.Node node3 = null;
			NetInfo.Node node4 = null;
			if (version == NetInfoVersion.Tunnel)
			{
				node1 = info.m_nodes[0].ShallowClone();
				node1.m_material = DefaultMaterial;
				node1.m_lodMaterial = DefaultLODMaterial;


				node3 = info.m_nodes[0].ShallowClone();
				node3.m_material = DefaultMaterial;
				node3.m_lodMaterial = DefaultLODMaterial;

				if (m_IsOneWay)
				{
					node2 = info.m_nodes[0].ShallowClone();
					node2.m_material = DefaultMaterial;
					node2.m_lodMaterial = DefaultLODMaterial;
					node2.m_directConnect = true;

					node4 = info.m_nodes[0].ShallowClone();
					node4.m_material = DefaultMaterial;
					node4.m_lodMaterial = DefaultLODMaterial;
					node4.m_directConnect = true;
				}
			}
			else
			{
				node1 = info.m_nodes[1].ShallowClone();
				node3 = info.m_nodes[0].ShallowClone();
				if (m_IsOneWay)
				{
					node2 = info.m_nodes[1].ShallowClone();
					node4 = info.m_nodes[0].ShallowClone();
					node2.m_directConnect = true;
					node4.m_directConnect = true;
				}

			}
			node1.m_directConnect = true;
			node3.m_directConnect = true;

			NodeList.Add(node1);
			NodeList.Add(node3);
			if (m_IsOneWay)
			{
				NodeList.Add(node2);
				NodeList.Add(node4);
				node1
					.SetMeshes
					   ($@"Meshes\{WidthName}\Rail{Variations[index]}_Start.obj",
					  @"Meshes\6m\Ground_Rail_Start_LOD.obj")
					 .SetConsistentUVs();


				node2
					.SetMeshes
					   ($@"Meshes\{WidthName}\Rail{Variations[index]}_End.obj",
					   @"Meshes\6m\Ground_Rail_End_LOD.obj")
					  .SetConsistentUVs();
				node3
					.SetMeshes
					   ($@"Meshes\{WidthName}\Boosted_Rail{Variations[index]}_Start.obj",
					  @"Meshes\6m\Ground_Rail_Start_LOD.obj")
					 .SetConsistentUVs();

				node4
					.SetMeshes
					   ($@"Meshes\{WidthName}\Boosted_Rail{Variations[index]}_End.obj",
					   @"Meshes\6m\Ground_Rail_End_LOD.obj")
					  .SetConsistentUVs();

				node1.m_flagsForbidden = NetNode.Flags.LevelCrossing;
				node2.m_flagsForbidden = NetNode.Flags.LevelCrossing;
				node3.m_flagsForbidden = NetNode.Flags.LevelCrossing;
				node4.m_flagsForbidden = NetNode.Flags.LevelCrossing;

				node1.m_connectGroup = NetInfo.ConnectGroup.OnewayStart;
				node2.m_connectGroup = NetInfo.ConnectGroup.OnewayEnd;
				node3.m_connectGroup = NetInfo.ConnectGroup.OnewayStart;
				node4.m_connectGroup = NetInfo.ConnectGroup.OnewayEnd;
			}
			else
			{
				node1
					.SetMeshes
					   ($@"Meshes\{WidthName}\Rail{Variations[index]}.obj",
					  @"Meshes\6m\Ground_Rail_Start_LOD.obj")
					 .SetConsistentUVs();
				node3
					.SetMeshes
					   ($@"Meshes\{WidthName}\Boosted_Rail{Variations[index]}.obj",
					  @"Meshes\6m\Ground_Rail_Start_LOD.obj")
					 .SetConsistentUVs();
				node1.m_flagsForbidden = NetNode.Flags.LevelCrossing;
				node3.m_flagsForbidden = NetNode.Flags.LevelCrossing;
			}

			for (var j = index; j <= m_Till; j++)
			{
				if (m_IsOneWay)
				{
					node1.m_connectGroup |= Groups[j];
					node2.m_connectGroup |= Groups[j];
					node3.m_connectGroup |= Groups[j];
					node4.m_connectGroup |= Groups[j];
				}
				else
				{
					if (j == index)
					{
						node1.m_connectGroup = Groups[j];
						node3.m_connectGroup = Groups[j];
					}
					else
					{
						node1.m_connectGroup |= Groups[j];
						node3.m_connectGroup |= Groups[j];
					}
				}

			}
		}

		public static void CreateSplitLevelCrossings(NetInfo info, int index)
		{
			NetInfo.Node node5 = info.m_nodes[1].ShallowClone();
			NetInfo.Node node7 = info.m_nodes[1].ShallowClone();
			NodeList.Add(node5);
			NodeList.Add(node7);

			if (m_IsOneWay)
			{
				NetInfo.Node node6 = info.m_nodes[1].ShallowClone();
				NetInfo.Node node8 = info.m_nodes[1].ShallowClone();
				NodeList.Add(node6);
				NodeList.Add(node8);
				node5
					.SetFlags(NetNode.Flags.LevelCrossing, NetNode.Flags.None)
					.SetMeshes
					($@"Meshes\{WidthName}\LevelCrossing_Rail{Variations[index]}_Start.obj", @"Meshes\10m\Blank.obj")
					.SetConsistentUVs();
				node6
					.SetFlags(NetNode.Flags.LevelCrossing, NetNode.Flags.None)
					.SetMeshes
					($@"Meshes\{WidthName}\LevelCrossing_Rail{Variations[index]}_End.obj", @"Meshes\10m\Blank.obj")
					.SetConsistentUVs();
				node7
					.SetFlags(NetNode.Flags.LevelCrossing, NetNode.Flags.None)
					.SetMeshes
					($@"Meshes\{WidthName}\LevelCrossing_Rail_Insert{Variations[index]}_Start.obj", @"Meshes\10m\Blank.obj")
					.SetConsistentUVs();
				node8
					.SetFlags(NetNode.Flags.LevelCrossing, NetNode.Flags.None)
					.SetMeshes
					($@"Meshes\{WidthName}\LevelCrossing_Rail_Insert{Variations[index]}_End.obj", @"Meshes\10m\Blank.obj")
					.SetConsistentUVs();

				node8.m_directConnect = true;
				node8.m_material = DefaultMaterial;
				node8.m_lodMaterial = DefaultLODMaterial;

				node5.m_connectGroup = NetInfo.ConnectGroup.OnewayStart;
				node6.m_connectGroup = NetInfo.ConnectGroup.OnewayEnd;
				node7.m_connectGroup = NetInfo.ConnectGroup.OnewayStart;
				node8.m_connectGroup = NetInfo.ConnectGroup.OnewayEnd;
				for (var j = index; j <= m_Till; j++)
				{
					node5.m_connectGroup |= Groups[j];
					node6.m_connectGroup |= Groups[j];
					node7.m_connectGroup |= Groups[j];
					node8.m_connectGroup |= Groups[j];
				}
			}
			else
			{
				node5
					.SetFlags(NetNode.Flags.LevelCrossing, NetNode.Flags.None)
					.SetMeshes
					($@"Meshes\{WidthName}\LevelCrossing_Rail{Variations[index]}.obj", @"Meshes\10m\Blank.obj")
					.SetConsistentUVs();
				node7
					.SetFlags(NetNode.Flags.LevelCrossing, NetNode.Flags.None)
					.SetMeshes
					($@"Meshes\{WidthName}\LevelCrossing_Rail_Insert{Variations[index]}.obj", @"Meshes\10m\Blank.obj")
					.SetConsistentUVs();
				for (var j = index; j <= m_Till; j++)
				{
					if (j == index)
					{
						node5.m_connectGroup = Groups[j];
						node7.m_connectGroup = Groups[j];
					}
					else
					{
						node5.m_connectGroup |= Groups[j];
						node7.m_connectGroup |= Groups[j];
					}
				}
			}

			node7.m_directConnect = true;
			node7.m_material = DefaultMaterial;
			node7.m_lodMaterial = DefaultLODMaterial;
		}

		public enum TrackType
		{
			Modern,
			Classic
		}
	}
}

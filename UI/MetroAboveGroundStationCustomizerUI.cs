using System;
using ColossalFramework.Threading;
using ColossalFramework.UI;
using MetroOverhaul.Extensions;
using UnityEngine;

namespace MetroOverhaul.UI
{
	public class MetroAboveGroundStationCustomizerUI : MetroCustomizerBase
	{

		public override void Update()
		{
			if (m_buildingTool == null)
			{
				return;
			}
			try
			{
				var toolInfo = m_buildingTool.enabled ? m_buildingTool.m_prefab : null;
				if (toolInfo == m_currentBuilding)
				{
					return;
				}
				BuildingInfo finalInfo = null;
				if (toolInfo != null)
				{
					if (toolInfo.HasAbovegroundMetroStationTracks())
					{
						finalInfo = toolInfo;
					}
					else if (toolInfo.m_subBuildings != null)
					{
						foreach (var subInfo in toolInfo.m_subBuildings)
						{
							if (subInfo.m_buildingInfo == null || !subInfo.m_buildingInfo.HasAbovegroundMetroStationTracks())
							{
								continue;
							}
							finalInfo = subInfo.m_buildingInfo;
							break;
						}
					}
				}
				if (finalInfo == m_currentBuilding)
				{
					return;
				}
				if (finalInfo != null)
				{
					Activate(finalInfo);
				}
				else
				{
					Deactivate();
				}
			}
			catch (Exception e)
			{
				UnityEngine.Debug.LogException(e);
				Deactivate();
			}

		}
		public override void Start()
		{
			m_buildingTool = FindObjectOfType<BuildingTool>();
			if (m_buildingTool == null)
			{
#if DEBUG
				Next.Debug.Log("BuildingTool Not Found");
#endif
				enabled = false;
				return;
			}

			m_bulldozeTool = FindObjectOfType<BulldozeTool>();
			if (m_bulldozeTool == null)
			{
#if DEBUG
				Next.Debug.Log("BulldozeTool Not Found");
#endif
				enabled = false;
				return;
			}
			m_netTool = FindObjectOfType<NetTool>();
			if (m_netTool == null)
			{
#if DEBUG
				Next.Debug.Log("NetTool Not Found");
#endif
				enabled = false;
				return;
			}

			try
			{
				m_upgradeButtonTemplate = GameObject.Find("RoadsSmallPanel").GetComponent<GeneratedScrollPanel>().m_OptionsBar.Find<UIButton>("Upgrade");
			}
			catch
			{
#if DEBUG
				Next.Debug.Log("Upgrade button template not found");
#endif
			}

			CreateUI();
			trackStyle = 0;
		}

		private void CreateUI()
		{
#if DEBUG
			Next.Debug.Log("MOM ABOVE GROUND STATION TRACK GUI Created");
#endif

			backgroundSprite = "GenericPanel";
			color = new Color32(68, 84, 68, 170);
			width = 200;
			height = 100;
			opacity = 60;
			position = Vector2.zero;
			isVisible = false;
			isInteractive = true;
			padding = new RectOffset() { bottom = 8, left = 8, right = 8, top = 8 };

            CreateDragHandle("Surface Station Options");

			btnModernStyle = CreateButton("Modern", 2, (c, v) =>
			{
				trackStyle = 0;
				SetNetToolPrefab();
			});

			btnClassicStyle = CreateButton("Classic", 2, (c, v) =>
			{
				trackStyle = 1;
				SetNetToolPrefab();
			});
		}

		private void SetNetToolPrefab()
		{
			if (trackStyle == 0)
			{
				btnModernStyle.color = new Color32(163, 255, 16, 255);
				btnModernStyle.normalBgSprite = "ButtonMenuFocused";
				btnModernStyle.useDropShadow = true;
				btnModernStyle.opacity = 95;
				btnClassicStyle.color = new Color32(150, 150, 150, 255);
				btnClassicStyle.normalBgSprite = "ButtonMenu";
				btnClassicStyle.useDropShadow = false;
				btnClassicStyle.opacity = 75;
			}
			else if (trackStyle == 1)
			{
				btnClassicStyle.color = new Color32(163, 255, 16, 255);
				btnClassicStyle.normalBgSprite = "ButtonMenuFocused";
				btnClassicStyle.useDropShadow = true;
				btnClassicStyle.opacity = 95;
				btnModernStyle.color = new Color32(150, 150, 150, 255);
				btnModernStyle.normalBgSprite = "ButtonMenu";
				btnModernStyle.useDropShadow = false;
				btnModernStyle.opacity = 75;
			}

			var paths = m_buildingTool.m_prefab.m_paths;
			for (var i = 0; i < paths.Length; i++)
			{
				if (paths[i] != null && paths[i].m_netInfo != null && paths[i].m_netInfo.IsAbovegroundMetroStationTrack())
				{
					var trackName = paths[i].m_netInfo.name;
					if (trackStyle == 0)
					{
						if (trackName.ToLower().StartsWith("steel"))
						{
							trackName = trackName.Substring(6);
						}
                        paths[i].AssignNetInfo(trackName);
					}
					else if (trackStyle == 1)
					{
						if (trackName.ToLower().StartsWith("steel") == false)
						{
							trackName = "Steel " + trackName;
						}
                        paths[i].AssignNetInfo(trackName);
                    }
				}

			}
		}
		private void Activate(BuildingInfo bInfo)
		{
			m_activated = true;
			m_currentBuilding = bInfo;
			isVisible = true;
			foreach (var path in bInfo.m_paths)
			{
				if (path.m_netInfo.IsAbovegroundMetroStationTrack())
				{
					trackStyle = path.m_netInfo.name.ToLower().StartsWith("steel") ? 1 : 0;
					break;
				}
			}
			SetNetToolPrefab();
		}
		private void Deactivate()
		{
			if (!m_activated)
			{
				return;
			}
			m_currentBuilding = null;
			isVisible = false;
			m_activated = false;

		}
	}
}

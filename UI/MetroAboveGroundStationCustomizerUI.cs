using System;
using ColossalFramework.Threading;
using ColossalFramework.UI;
using MetroOverhaul.Extensions;
using UnityEngine;

namespace MetroOverhaul.UI
{
	public class MetroAboveGroundStationCustomizerUI : UIPanel
	{
		public int trackStyle = 0;
		private BulldozeTool m_bulldozeTool;
		private BuildingTool m_buildingTool;
		private NetTool m_netTool;
		private UIButton m_upgradeButtonTemplate;
		private BuildingInfo m_currentBuilding;
		private bool m_activated = false;
		private UIButton btnModernStyle;
		private UIButton btnClassicStyle;

		public static MetroAboveGroundStationCustomizerUI instance;

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

			UIPanel dragHandlePanel = AddUIComponent<UIPanel>();
			dragHandlePanel.atlas = atlas;
			dragHandlePanel.backgroundSprite = "GenericPanel";
			dragHandlePanel.width = width;
			dragHandlePanel.height = 20;
			dragHandlePanel.opacity = 100;
			dragHandlePanel.color = new Color32(21, 140, 34, 255);
			dragHandlePanel.relativePosition = Vector3.zero;

			UIDragHandle dragHandle = dragHandlePanel.AddUIComponent<UIDragHandle>();
			dragHandle.width = width;
			dragHandle.height = 20;
			dragHandle.relativePosition = Vector3.zero;
			dragHandle.target = this;

			UILabel titleLabel = dragHandlePanel.AddUIComponent<UILabel>();
			titleLabel.relativePosition = new Vector3() { x = 5, y = 3, z = 0 };
			titleLabel.textAlignment = UIHorizontalAlignment.Center;
			titleLabel.text = "Station Attributes";
			titleLabel.isInteractive = false;

			btnModernStyle = CreateButton("Modern", new Vector3(8, 50), (c, v) =>
			{
				trackStyle = 0;
				SetNetToolPrefab();
			});

			btnClassicStyle = CreateButton("Classic", new Vector3(8 + (0.5f * width) - 16, 50), (c, v) =>
			{
				trackStyle = 1;
				SetNetToolPrefab();
			});
		}

		private UIButton CreateButton(string text, Vector3 pos, MouseEventHandler eventClick)
		{
			var button = this.AddUIComponent<UIButton>();
			button.width = 80;
			button.height = 30;
			button.normalBgSprite = "ButtonMenu";
			button.color = new Color32(150, 150, 150, 255);
			button.disabledBgSprite = "ButtonMenuDisabled";
			button.hoveredBgSprite = "ButtonMenuHovered";
			button.hoveredColor = new Color32(163, 255, 16, 255);
			button.focusedBgSprite = "ButtonMenu";
			button.focusedColor = new Color32(163, 255, 16, 255);
			button.pressedBgSprite = "ButtonMenuPressed";
			button.pressedColor = new Color32(163, 255, 16, 255);
			button.textColor = new Color32(255, 255, 255, 255);
			button.normalBgSprite = "ButtonMenu";
			button.focusedBgSprite = "ButtonMenuFocused";
			button.playAudioEvents = true;
			button.opacity = 75;
			button.dropShadowColor = new Color32(0, 0, 0, 255);
			button.dropShadowOffset = new Vector2(-1, -1);
			button.text = text;
			button.relativePosition = pos;
			button.eventClick += eventClick;

			return button;
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
							trackName = trackName.Substring(5);
						}
						paths[i].m_netInfo = PrefabCollection<NetInfo>.FindLoaded(trackName);
					}
					else if (trackStyle == 1)
					{
						if (trackName.ToLower().StartsWith("steel") == false)
						{
							trackName = "Steel " + trackName;
						}
						paths[i].m_netInfo = PrefabCollection<NetInfo>.FindLoaded(trackName);
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

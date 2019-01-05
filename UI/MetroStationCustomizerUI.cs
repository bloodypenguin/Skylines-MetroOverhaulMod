using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ColossalFramework.Threading;
using ColossalFramework.UI;
using MetroOverhaul.Extensions;
using UnityEngine;

namespace MetroOverhaul.UI
{
	public class MetroStationCustomizerUI : MetroCustomizerBase
	{
		private const int DEPTH_STEP = 3;
		private const int LENGTH_STEP = 8;
		private const int ANGLE_STEP = 15;
		private const float BEND_STRENGTH_STEP = 0.5f;

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
				BuildingInfo superFinalInfo = null;
				if (toolInfo != null)
				{
					
					if (toolInfo.HasUndergroundMetroStationTracks())
					{
						finalInfo = toolInfo;
					}
					else if (toolInfo.m_subBuildings != null)
					{
						foreach (var subInfo in toolInfo.m_subBuildings)
						{
							if (subInfo.m_buildingInfo == null || !subInfo.m_buildingInfo.HasUndergroundMetroStationTracks())
							{
								continue;
							}
							finalInfo = subInfo.m_buildingInfo;
							superFinalInfo = toolInfo;
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
					Activate(finalInfo, superFinalInfo);
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
			SetDict[ToggleType.Depth] = SetStationCustomizations.DEF_DEPTH;
			SetDict[ToggleType.Length] = SetStationCustomizations.DEF_LENGTH;
			SetDict[ToggleType.Angle] = SetStationCustomizations.DEF_ANGLE;
			SetDict[ToggleType.Bend] = SetStationCustomizations.DEF_BEND_STRENGTH;
		}

		protected override void OnToggleKeyDown(UIComponent c, UIKeyEventParameter e)
		{
			if (SliderDataDict != null && m_Toggle != ToggleType.None)
			{
				SliderData sData = SliderDataDict[m_Toggle];

				switch (e.keycode)
				{
					case KeyCode.LeftArrow:
						SliderDict[m_Toggle].value = Math.Max(sData.Min, SetDict[m_Toggle] - sData.Step);
						break;
					case KeyCode.RightArrow:
						SliderDict[m_Toggle].value = Math.Min(sData.Max, SetDict[m_Toggle] + sData.Step);
						break;
					case KeyCode.UpArrow:
						SliderDict[m_Toggle].value = sData.Max;
						break;
					case KeyCode.DownArrow:
						SliderDict[m_Toggle].value = sData.Min;
						break;
					case KeyCode.RightControl:
						SliderDict[m_Toggle].value = sData.Def;
						break;
				}

				m_T.Run();
			}
		}
		protected override void OnToggleMouseDown(UIComponent c, UIMouseEventParameter e, ToggleType type)
		{
			m_Toggle = type;
			if (SliderDataDict != null && type != ToggleType.None)
			{
				foreach (UIPanel pnl in PanelDict.Values)
				{
					pnl.color = new Color32(150, 150, 150, 210);
				}
				PanelDict[type].color = new Color32(30, 200, 50, 255);
				SliderDict[type].Focus();
				m_T.Run();
			}
		}
		
		private void CreateUI()
		{
#if DEBUG
			Next.Debug.Log("MOM UNDERGROUND STATION TRACK GUI Created");
#endif
			Action stationMechanicsTask = DoStationMechanics;
			Task t = Task.Create(stationMechanicsTask);
			m_T = t;
			backgroundSprite = "GenericPanel";
			color = new Color32(68, 84, 68, 170);
			width = 280;
			height = 300;
			opacity = 60;
			position = Vector2.zero;
			isVisible = false;
			isInteractive = true;
			padding = new RectOffset() { bottom = 8, left = 8, right = 8, top = 8 };

            CreateDragHandle("Subway Station Options");

			SliderDataDict = new Dictionary<ToggleType, SliderData>();
			SliderDataDict.Add(ToggleType.Depth, new SliderData()
			{
				Def = SetStationCustomizations.DEF_DEPTH,
				Max = SetStationCustomizations.MAX_DEPTH,
				Min = SetStationCustomizations.MIN_DEPTH,
				SetVal = SetStationCustomizations.DEF_DEPTH,
				Step = DEPTH_STEP
			});
			SliderDataDict.Add(ToggleType.Length, new SliderData()
			{
				Def = SetStationCustomizations.DEF_LENGTH,
				Max = SetStationCustomizations.MAX_LENGTH,
				Min = SetStationCustomizations.MIN_LENGTH,
				SetVal = SetStationCustomizations.DEF_LENGTH,
				Step = LENGTH_STEP
			});
			SliderDataDict.Add(ToggleType.Angle, new SliderData()
			{
				Def = SetStationCustomizations.DEF_ANGLE,
				Max = SetStationCustomizations.MAX_ANGLE,
				Min = SetStationCustomizations.MIN_ANGLE,
				SetVal = SetStationCustomizations.DEF_ANGLE,
				Step = ANGLE_STEP
			});
			SliderDataDict.Add(ToggleType.Bend, new SliderData()
			{
				Def = SetStationCustomizations.DEF_BEND_STRENGTH,
				Max = SetStationCustomizations.MAX_BEND_STRENGTH,
				Min = SetStationCustomizations.MIN_BEND_STRENGTH,
				SetVal = SetStationCustomizations.DEF_BEND_STRENGTH,
				Step = BEND_STRENGTH_STEP
			});
			SliderDict = new Dictionary<ToggleType, UISlider>();
			PanelDict = new Dictionary<ToggleType, UIPanel>();
			CheckboxDict = new Dictionary<StationTrackType, UICheckBox>();

			CreateSlider(ToggleType.Length);
			CreateSlider(ToggleType.Depth);
			CreateSlider(ToggleType.Angle);
			CreateSlider(ToggleType.Bend);

			CreateCheckbox(StationTrackType.SidePlatform);
			CreateCheckbox(StationTrackType.IslandPlatform);
			CreateCheckbox(StationTrackType.SingleTrack);
			CreateCheckbox(StationTrackType.QuadSidePlatform);
			CreateCheckbox(StationTrackType.QuadIslandPlatform);
		}

		protected override void TunnelStationTrackToggleStyles(BuildingInfo info, StationTrackType type)
		{
			if (info?.m_paths == null)
			{
				return;
			}
			for (var i = 0; i < info.m_paths.Length; i++)
			{
				var path = info.m_paths[i];
				if (path?.m_netInfo?.name == null || !path.m_netInfo.IsUndergroundMetroStationTrack())
				{
					continue;
				}

				foreach (var cb in CheckboxDict.Values)
				{
					var cbSprite = cb.GetComponentInChildren<UISprite>();
					if (cbSprite != null)
					{
						cbSprite.spriteName = "check-unchecked";
					}
				}
				var activeCbSprite = CheckboxDict[type].GetComponentInChildren<UISprite>();
				if (activeCbSprite != null)
				{
					activeCbSprite.spriteName = "check-checked";
				}
				else
				{
					Debug.Log("NOOOO");
				}
					switch (m_TrackType)
					{
						case StationTrackType.SidePlatform:
							path.AssignNetInfo("Metro Station Track Tunnel");
							break;
						case StationTrackType.IslandPlatform:
							path.AssignNetInfo("Metro Station Track Tunnel Island");
							break;
						case StationTrackType.SingleTrack:
							path.AssignNetInfo("Metro Station Track Tunnel Small");
							break;
						case StationTrackType.QuadSidePlatform:
							path.AssignNetInfo("Metro Station Track Tunnel Large");
							break;
						case StationTrackType.QuadIslandPlatform:
							path.AssignNetInfo("Metro Station Track Tunnel Large Island");
							break;
					}
			}
		}
		private void Activate(BuildingInfo bInfo, BuildingInfo superInfo = null)
		{
			m_activated = true;
			DoStationMechanicsResetAngles();
			m_currentBuilding = bInfo;
			m_currentSuperBuilding = superInfo;
			isVisible = true;
			TunnelStationTrackToggleStyles(bInfo, m_TrackType);
			DoStationMechanics();
		}
		private void Deactivate()
		{
			if (!m_activated)
			{
				return;
			}
			foreach (UIPanel pnl in PanelDict.Values)
			{
				pnl.color = new Color32(150, 150, 150, 210);
			}
			DoStationMechanicsResetAngles();
			SetStationCustomizations.m_PremierPath = -1;
			m_currentBuilding = null;
			m_currentSuperBuilding = null;
			isVisible = false;
			m_activated = false;

		}

		private void DoStationMechanics()
		{
			SetStationCustomizations.ModifyStation(m_currentBuilding, SetDict[ToggleType.Depth], SetDict[ToggleType.Length], (int)SetDict[ToggleType.Angle], SetDict[ToggleType.Bend], m_currentSuperBuilding);
		}
		private void DoStationMechanicsResetAngles()
		{
			SetStationCustomizations.ModifyStation(m_currentBuilding, SetDict[ToggleType.Depth], SetDict[ToggleType.Length], 0, SetDict[ToggleType.Bend], m_currentSuperBuilding);
		}
	}
	public struct SliderData
	{
		public float Max { get; set; }
		public float Min { get; set; }
		public float Def { get; set; }
		public float Step { get; set; }
		public float SetVal { get; set; }
		public void SetTheVal(float val)
		{
			SetVal = val;
		}
	}
}

using System;
using System.Collections.Generic;
using ColossalFramework.Threading;
using ColossalFramework.UI;
using MetroOverhaul.Extensions;
using UnityEngine;

namespace MetroOverhaul.UI {
    public class MetroStationCustomizerUI : MetroCustomizerBaseUI
    {
        private const int DEPTH_STEP = 3;
        private const int LENGTH_STEP = 8;
        private const int ANGLE_STEP = 15;
        private const float BEND_STRENGTH_STEP = 0.5f;

        protected override bool SatisfiesTrackSpecs(PrefabInfo info)
        {
            return ((BuildingInfo)info).HasUndergroundMetroStationTracks();
        }

        protected override ToolBase GetTheTool()
        {
            return m_buildingTool;
        }

        protected override PrefabInfo GetToolPrefab()
        {
            return ((BuildingTool)GetTheTool())?.m_prefab;
        }

        protected override PrefabInfo CurrentInfo { get => m_currentBuilding; set => m_currentBuilding = (BuildingInfo)value; }

        protected override void SubStart()
        {
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

        protected override void CreateUI()
        {
#if DEBUG
            Next.Debug.Log("MOM UNDERGROUND STATION TRACK GUI Created");
#endif
            base.CreateUI();
            Action stationMechanicsTask = DoStationMechanics;
            Task t = Task.Create(stationMechanicsTask);
            m_T = t;
            width = 280;

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
            CheckboxStationDict = new Dictionary<StationTrackType, UICheckBox>();

            CreateSlider(ToggleType.Length);
            CreateSlider(ToggleType.Depth);
            CreateSlider(ToggleType.Angle);
            CreateSlider(ToggleType.Bend);

            CreateCheckbox(StationTrackType.SidePlatform);
            CreateCheckbox(StationTrackType.IslandPlatform);
            CreateCheckbox(StationTrackType.SingleTrack);
            CreateCheckbox(StationTrackType.QuadSidePlatform);
            CreateCheckbox(StationTrackType.QuadDualIslandPlatform);

            height = m_height;
        }

        protected override void TunnelStationTrackToggleStyles(StationTrackType type)
        {
            if (type == StationTrackType.None)
            {
                m_TrackType = StationTrackType.SidePlatform;
            }
            else if (m_TrackType != type)
            {
                m_TrackType = type;
            }

            foreach (var kvp in CheckboxStationDict)
            {
                var cbSprite = kvp.Value.GetComponentInChildren<UISprite>();
                if (cbSprite != null)
                {
                    if (kvp.Key == m_TrackType)
                    {
                        cbSprite.spriteName = "check-checked";
                    }
                    else
                    {
                        cbSprite.spriteName = "check-unchecked";
                    }
                }
            }
            if (m_currentBuilding.HasUndergroundMetroStationTracks())
            {
                SetTunnelInfoByType();
            }
            if (m_currentBuilding?.m_subBuildings != null)
            {
                for (int i = 0; i < m_currentBuilding.m_subBuildings.Length; i++)
                {
                    var subBuildingInfo = m_currentBuilding.m_subBuildings[i]?.m_buildingInfo;
                    if (subBuildingInfo != null && subBuildingInfo.HasUndergroundMetroStationTracks())
                    {
                        SetTunnelInfoByType(subBuildingInfo);
                    }
                }
            }
            m_T.Run();
            

        }
        private void SetTunnelInfoByType(BuildingInfo info = null)
        {
            if (info == null)
            {
                info = m_currentBuilding;
            }
            for (int i = 0; i < info.m_paths.Length; i++)
            {
                var path = info.m_paths[i];
                if (path.m_netInfo.IsUndergroundMetroStationTrack())
                {
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
                        //case StationTrackType.QuadSideIslandPlatform:
                        //    path.AssignNetInfo("Metro Station Track Tunnel Large Side Island");
                        //    break;
                        case StationTrackType.QuadDualIslandPlatform:
                            path.AssignNetInfo("Metro Station Track Tunnel Large Dual Island");
                            break;
                    }
                }
            }
        }
        protected override void Activate(PrefabInfo info)
        {
            base.Activate(info);
            DoStationMechanicsResetAngles();
            TunnelStationTrackToggleStyles(m_TrackType);
            m_T.Run();
        }
        protected override void SubDeactivate()
        {
            foreach (UIPanel pnl in PanelDict.Values)
            {
                pnl.color = new Color32(150, 150, 150, 210);
            }
            if (m_currentBuilding != null)
            {
                DoStationMechanicsResetAngles();
            }
            SetStationCustomizations.m_PremierPath = -1;
        }

        private void DoStationMechanics()
        {
            if (m_currentBuilding != null)
            {
                SetStationCustomizations.ModifyStation(m_currentBuilding, SetDict[ToggleType.Depth], SetDict[ToggleType.Length], (int)SetDict[ToggleType.Angle], SetDict[ToggleType.Bend]);
            }

        }
        private void DoStationMechanicsResetAngles()
        {
            if (m_currentBuilding != null)
            {
                SetStationCustomizations.ModifyStation(m_currentBuilding, SetDict[ToggleType.Depth], SetDict[ToggleType.Length], 0, SetDict[ToggleType.Bend]);
            }
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

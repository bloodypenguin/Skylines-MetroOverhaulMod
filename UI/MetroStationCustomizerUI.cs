using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using ColossalFramework.Threading;
using ColossalFramework.UI;
using Harmony;
using MetroOverhaul.Extensions;
using MetroOverhaul.NEXT;
using UnityEngine;

namespace MetroOverhaul.UI
{
    public class MetroStationCustomizerUI : MetroCustomizerBaseUI
    {
        private const int DEPTH_STEP = 3;
        private const int LENGTH_STEP = 8;
        private const int ANGLE_STEP = 15;
        private const float BEND_STRENGTH_STEP = 15;
        private bool m_IsRelocate = false;
        private int m_OrderedTrackIndex = -1;
        private ReverseEngineerStationCustomizations m_Customizations;
        protected override bool SatisfiesTrackSpecs(PrefabInfo info)
        {
            return ((BuildingInfo)info).HasUndergroundMetroStationTracks() || m_IsRelocate;
        }

        protected override ToolBase GetTheTool()
        {
            return m_buildingTool;
        }

        protected override PrefabInfo GetToolPrefab()
        {
            if (GetTheTool() != null)
            {
                if (IsRelocate())
                {
                    return m_Customizations.BuildingPrefab;
                }
                return ((BuildingTool)GetTheTool())?.m_prefab;
            }
            return null;
        }
        private List<BuildingInfo.PathInfo> GetNestedStationPaths(BuildingInfo info)
        {
            var trackList = new List<BuildingInfo.PathInfo>();
            if (info?.m_subBuildings != null)
            {
                foreach (var subBuilding in info.m_subBuildings)
                {
                    if (subBuilding?.m_buildingInfo != null && subBuilding.m_buildingInfo.HasUndergroundMetroStationTracks())
                    {
                        if (subBuilding.m_buildingInfo.m_subBuildings != null)
                        {
                            trackList.AddRange(GetNestedStationPaths(subBuilding.m_buildingInfo));
                        }
                    }
                }
                foreach (var path in info.m_paths)
                {
                    if (path.m_netInfo.IsUndergroundMetroStationTrack())
                    {
                        trackList.Add(path);
                    }
                }
            }
            return trackList;
        }
        private List<BuildingInfo.PathInfo> OrderedStationTrackList()
        {
            if (GetToolPrefab() != null)
            {
                var trackList = new List<BuildingInfo.PathInfo>();
                trackList.AddRange(GetNestedStationPaths((BuildingInfo)GetToolPrefab()));
                if (((BuildingInfo)GetToolPrefab()).m_subBuildings != null && ((BuildingInfo)GetToolPrefab()).m_subBuildings.Count() > 0)
                {
                    foreach (var subBuilding in ((BuildingInfo)GetToolPrefab()).m_subBuildings)
                    {
                        if (subBuilding?.m_buildingInfo != null && subBuilding.m_buildingInfo.HasUndergroundMetroStationTracks())
                            trackList.AddRange(GetNestedStationPaths(subBuilding.m_buildingInfo));
                    }
                }
                return trackList.OrderBy(y => y.m_nodes.First().y).ThenBy(z => z.m_nodes.First().z).ThenBy(x => x.m_nodes.First().x).ToList();
            }
            return null;
        }
        protected override PrefabInfo CurrentInfo { get => m_currentBuilding; set => m_currentBuilding = (BuildingInfo)value; }

        protected override void SubStart()
        {
            SetDict[MetroStationTrackAlterType.Depth] = m_IsRelocate ? m_Customizations.Depth : SetStationCustomizations.DEF_DEPTH;
            SetDict[MetroStationTrackAlterType.Length] = m_IsRelocate ? m_Customizations.Depth : SetStationCustomizations.DEF_LENGTH;
            SetDict[MetroStationTrackAlterType.Rotation] = m_IsRelocate ? m_Customizations.Angle : SetStationCustomizations.DEF_ANGLE;
            SetDict[MetroStationTrackAlterType.Bend] = m_IsRelocate ? m_Customizations.Bend : SetStationCustomizations.DEF_BEND_STRENGTH;
        }

        protected override void OnToggleKeyDown(UIComponent c, UIKeyEventParameter e)
        {
            if (SliderDataDict != null && m_Toggle != MetroStationTrackAlterType.None)
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
        protected override void OnToggleMouseDown(UIComponent c, UIMouseEventParameter e, MetroStationTrackAlterType type)
        {
            m_Toggle = type;
            if (SliderDataDict != null && type != MetroStationTrackAlterType.None)
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
            width = 450;

            CreateDragHandle("Subway Station Options");

            SliderDataDict = new Dictionary<MetroStationTrackAlterType, SliderData>();
            SliderDataDict.Add(MetroStationTrackAlterType.Depth, new SliderData()
            {
                Def = SetStationCustomizations.DEF_DEPTH,
                Max = SetStationCustomizations.MAX_DEPTH,
                Min = SetStationCustomizations.MIN_DEPTH,
                Step = DEPTH_STEP
            });
            SliderDataDict.Add(MetroStationTrackAlterType.Length, new SliderData()
            {
                Def = SetStationCustomizations.DEF_LENGTH,
                Max = SetStationCustomizations.MAX_LENGTH,
                Min = SetStationCustomizations.MIN_LENGTH,
                Step = LENGTH_STEP
            });
            SliderDataDict.Add(MetroStationTrackAlterType.Rotation, new SliderData()
            {
                Def = SetStationCustomizations.DEF_ANGLE,
                Max = SetStationCustomizations.MAX_ANGLE,
                Min = SetStationCustomizations.MIN_ANGLE,
                Step = ANGLE_STEP
            });
            SliderDataDict.Add(MetroStationTrackAlterType.Bend, new SliderData()
            {
                Def = SetStationCustomizations.DEF_BEND_STRENGTH,
                Max = SetStationCustomizations.MAX_BEND_STRENGTH,
                Min = SetStationCustomizations.MIN_BEND_STRENGTH,
                Step = BEND_STRENGTH_STEP
            });
            SliderDict = new Dictionary<MetroStationTrackAlterType, UISlider>();
            PanelDict = new Dictionary<MetroStationTrackAlterType, UIPanel>();
            CheckboxStationDict = new Dictionary<StationTrackType, UICheckBox>();
            var stationLengthPanel = CreatePanel(new UIPanelParamProps()
            {
                Name = "stationLengthPanel",
                Margins = new Vector4(8, 0, 8, 8),
                ColumnCount = 1
            });
            //var ttpStationLengthSelectorTooltip = CreateToolTipPanel(new UIToolTipPanelParamProps()
            //{
            //    Name = "ttpStationLengthSelectorTooltip",
            //    ParentComponent = stationLengthPanel,
            //    ToolTipPanelText = Station_Length_Info,
            //});
            var lengthSlider = CreateSlider(new UISliderParamProps()
            {
                Name = "lengthSlider",
                ParentComponent = stationLengthPanel,
                TrackAlterType = MetroStationTrackAlterType.Length,
                //TooltipComponent = ttpStationLengthSelectorTooltip,
                ColumnCount = 1
            });

            var stationDepthPanel = CreatePanel(new UIPanelParamProps()
            {
                Name = "stationDepthPanel",
                Margins = new Vector4(8, 0, 8, 8),
                ColumnCount = 1
            });
            //var ttpStationDepthSelectorTooltip = CreateToolTipPanel(new UIToolTipPanelParamProps()
            //{
            //    Name = "ttpStationDepthSelectorTooltip",
            //    ToolTipPanelText = Station_Depth_Info
            //});
            var depththSlider = CreateSlider(new UISliderParamProps()
            {
                Name = "depththSlider",
                ParentComponent = stationDepthPanel,
                TrackAlterType = MetroStationTrackAlterType.Depth,
                //TooltipComponent = ttpStationDepthSelectorTooltip,
                ColumnCount = 1
            });

            var stationAnglePanel = CreatePanel(new UIPanelParamProps()
            {
                Name = "stationAnglePanel",
                Margins = new Vector4(8, 0, 8, 8),
                ColumnCount = 1
            });
            //var ttpStationAngleSelectorTooltip = CreateToolTipPanel(new UIToolTipPanelParamProps()
            //{
            //    Name = "ttpStationAngleSelectorTooltip",
            //    ToolTipPanelText = Station_Angle_Info
            //});
            var angleSlider = CreateSlider(new UISliderParamProps()
            {
                Name = "angleSlider",
                ParentComponent = stationAnglePanel,
                TrackAlterType = MetroStationTrackAlterType.Rotation,
                //TooltipComponent = ttpStationAngleSelectorTooltip,
                ColumnCount = 1
            });

            var stationBendPanel = CreatePanel(new UIPanelParamProps()
            {
                Name = "stationBendPanel",
                Margins = new Vector4(8, 0, 8, 8),
                ColumnCount = 1
            });
            //var ttpStationBendSelectorTooltip = CreateToolTipPanel(new UIToolTipPanelParamProps()
            //{
            //    Name = "ttpStationBendSelectorTooltip",
            //    ToolTipPanelText = Station_Bend_Info
            //});
            var bendSlider = CreateSlider(new UISliderParamProps()
            {
                Name = "bendSlider",
                ParentComponent = stationBendPanel,
                TrackAlterType = MetroStationTrackAlterType.Bend,
                //TooltipComponent = ttpStationBendSelectorTooltip,
                ColumnCount = 1
            });

            var pnlStationTrackChooser = CreatePanel(new UIPanelParamProps()
            {
                Name = "pnlStationTrackChooser",
                Margins = new Vector4(28, 4, 8, 16),
                ColumnCount = 1
            });
            var lblStationTrackChooser = CreateLabel(new UILabelParamProps()
            {
                Name = "lblStationTrackChooser",
                Text = "Subway Station Track Layout",
                ParentComponent = pnlStationTrackChooser,
                ColumnCount = 1
            });
            var tsStationTrackChooser = CreateTabStrip(new UITabstripParamProps()
            {
                Name = "tsStationTrackChooser",
                ColumnCount = 1,
                ParentComponent = pnlStationTrackChooser
            });
            var btnSidePlatformStationTrack = CreateButton(new UIButtonParamProps()
            {
                Name = "btnSidePlatformStationTrack",
                ToolTip = "Side Platform Station",
                ParentComponent = tsStationTrackChooser,
                ColumnCount = 5,
                Width = 79,
                Height = 69,
                Atlas = UIHelper.GenerateLinearAtlas("MOM_SidePlatformStationTrackAtlas", UIHelper.SidePlatformStationTrack),
                EventClick = (c, v) =>
                {
                    TunnelStationTrackToggleStyles(StationTrackType.SidePlatform);
                }
            });
            var btnIslandPlatformStationTrack = CreateButton(new UIButtonParamProps()
            {
                Name = "btnIslandPlatformStationTrack",
                ToolTip = "Island Platform Station",
                ParentComponent = tsStationTrackChooser,
                ColumnCount = 5,
                Width = 79,
                Height = 69,
                Atlas = UIHelper.GenerateLinearAtlas("MOM_IslandPlatformStationTrackAtlas", UIHelper.IslandPlatformStationTrack),
                EventClick = (c, v) =>
                {
                    TunnelStationTrackToggleStyles(StationTrackType.IslandPlatform);
                }
            });
            var btnSinglePlatformStationTrack = CreateButton(new UIButtonParamProps()
            {
                Name = "btnSinglePlatformStationTrack",
                ToolTip = "Single Platform Station",
                ParentComponent = tsStationTrackChooser,
                ColumnCount = 5,
                Width = 79,
                Height = 69,
                Atlas = UIHelper.GenerateLinearAtlas("MOM_SinglePlatformStationTrackAtlas", UIHelper.SinglePlatformStationTrack),
                EventClick = (c, v) =>
                {
                    TunnelStationTrackToggleStyles(StationTrackType.SinglePlatform);
                }
            });
            var btnExpressSidePlatformStationTrack = CreateButton(new UIButtonParamProps()
            {
                Name = "btnExpressSidePlatformStationTrack",
                ToolTip = "Express Side Platform Station",
                ParentComponent = tsStationTrackChooser,
                ColumnCount = 5,
                Width = 79,
                Height = 69,
                Atlas = UIHelper.GenerateLinearAtlas("MOM_ExpressSidePlatformStationTrackAtlas", UIHelper.ExpressSidePlatformStationTrack),
                EventClick = (c, v) =>
                {
                    TunnelStationTrackToggleStyles(StationTrackType.ExpressSidePlatform);
                }
            });
            var btnDualIslandPlatformStationTrack = CreateButton(new UIButtonParamProps()
            {
                Name = "btnDualIslandPlatformStationTrack",
                ToolTip = "Dual Island Platform Station",
                ParentComponent = tsStationTrackChooser,
                ColumnCount = 5,
                Width = 79,
                Height = 69,
                Atlas = UIHelper.GenerateLinearAtlas("MOM_DualIslandPlatformStationTrackAtlas", UIHelper.DualIslandPlatformStationTrack),
                EventClick = (c, v) =>
                {
                    TunnelStationTrackToggleStyles(StationTrackType.DualIslandPlatform);
                }
            });
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
                            Debug.Log(Util.GetMetroStationTrackName(NetInfoVersion.Tunnel, TrackStyle.Modern));
                            path.AssignNetInfo(Util.GetMetroStationTrackName(NetInfoVersion.Tunnel, TrackStyle.Modern));
                            break;
                        case StationTrackType.IslandPlatform:
                            path.AssignNetInfo(Util.GetMetroStationTrackName(NetInfoVersion.Tunnel, TrackStyle.Modern, StationTrackType.IslandPlatform));
                            break;
                        case StationTrackType.SinglePlatform:
                            path.AssignNetInfo(Util.GetMetroStationTrackName(NetInfoVersion.Tunnel, TrackStyle.Modern, StationTrackType.SinglePlatform));
                            break;
                        case StationTrackType.ExpressSidePlatform:
                            path.AssignNetInfo(Util.GetMetroStationTrackName(NetInfoVersion.Tunnel, TrackStyle.Modern, StationTrackType.ExpressSidePlatform));
                            break;
                        //case StationTrackType.QuadSideIslandPlatform:
                        //    path.AssignNetInfo("Metro Station Track Tunnel Large Side Island");
                        //    break;
                        case StationTrackType.DualIslandPlatform:
                            path.AssignNetInfo(Util.GetMetroStationTrackName(NetInfoVersion.Tunnel, TrackStyle.Modern, StationTrackType.DualIslandPlatform));
                            break;
                    }
                }
                else
                {
                    Debug.Log("Not a station track " + path.m_netInfo.name);
                }
            }
        }
        private bool IsRelocate()
        {
            m_IsRelocate = m_buildingTool.m_relocate > 0;
            if (m_IsRelocate)
            {
                m_Customizations = new ReverseEngineerStationCustomizations(this, m_buildingTool.m_relocate, 0);
                SliderDict[MetroStationTrackAlterType.Length].value = m_Customizations.Length;
                SliderDict[MetroStationTrackAlterType.Depth].value = m_Customizations.Depth;
                SliderDict[MetroStationTrackAlterType.Rotation].value = m_Customizations.Angle;
                SliderDict[MetroStationTrackAlterType.Bend].value = m_Customizations.Bend;
                m_TrackType = m_Customizations.StationTrackType;
            }
            return m_IsRelocate;
        }

        protected override void Activate(PrefabInfo info)
        {
            base.Activate(info);
            if (SetDict != null && SetDict.Count > 0)
            {
                DoStationMechanicsResetAngles();
                TunnelStationTrackToggleStyles(m_TrackType);
                m_T.Run();
            }
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
                SetStationCustomizations.ModifyStation(m_currentBuilding, SetDict[MetroStationTrackAlterType.Depth], SetDict[MetroStationTrackAlterType.Length], (int)SetDict[MetroStationTrackAlterType.Rotation], SetDict[MetroStationTrackAlterType.Bend]);
            }

        }
        private void DoStationMechanicsResetAngles()
        {
            if (m_currentBuilding != null)
            {
                SetStationCustomizations.ModifyStation(m_currentBuilding, SetDict[MetroStationTrackAlterType.Depth], SetDict[MetroStationTrackAlterType.Length], 0, SetDict[MetroStationTrackAlterType.Bend]);
            }
        }
    }
    public struct SliderData
    {
        public float Max { get; set; }
        public float Min { get; set; }
        public float Def { get; set; }
        public float Step { get; set; }
    }
}

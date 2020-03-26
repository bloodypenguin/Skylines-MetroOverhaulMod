using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using ColossalFramework.Threading;
using ColossalFramework.UI;
using Harmony;
using MetroOverhaul.Extensions;
using MetroOverhaul.NEXT;
using MetroOverhaul.NEXT.Extensions;
using UnityEngine;
using static MetroOverhaul.StationTrackCustomizations;

namespace MetroOverhaul.UI
{
    public class MetroStationCustomizerUI : MetroCustomizerBaseUI
    {
        private const int HORIZONTAL_STEP = 1;
        private const int VERTICAL_STEP = 1;
        private const int DEPTH_STEP = 3;
        private const int LENGTH_STEP = 8;
        private const int ANGLE_STEP = 15;
        private const float CURVE_STRENGTH_STEP = 15;
        private bool m_IsRelocate = false;
        private int m_PathIndex = -1;
        private UIPanel pnlIndividualTrackManipulator;
        private UIPanel pnlIndividualTrackChooser;
        private UIPanel pnlIndividualTrackChooserWrapper;
        //private UITabstrip pnlIndividualTrackChooser;
        //private UITabstrip pnlIndividualTrackManipulator;
        private UIButton btnSidePlatformStationTrack;
        private UIButton btnSelectAllTracks;
        private UIButton btnCloneSelectedTrack;
        private UIButton btnRemoveSelectedTrack;
        private UIButton btnRevertAllTracks;
        private UIPanel stationXPanel;
        private UIPanel stationYPanel;
        private ReverseEngineerStationCustomizations m_Customizations;
        private List<BuildingInfo.PathInfo> m_StationPaths;

        public static bool InPropagation = false;
        private List<BuildingInfo.PathInfo> StationPaths {
            get
            {
                if (m_StationPaths == null)
                {
                    m_StationPaths = ((BuildingInfo)GetToolPrefab()).UndergroundStationPaths();
                }
                return m_StationPaths;
            }
            set
            {
                m_StationPaths = value;
            }
        }
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
        private bool PathIndexIsSpecified()
        {
            return m_PathIndex > -1 && m_PathIndex < StationPaths.Count();
        }
        private BuildingInfo.PathInfo ActiveStationPath {
            get
            {
                BuildingInfo.PathInfo path = null;
                if (PathIndexIsSpecified())
                {
                    path = StationPaths[m_PathIndex];
                }
                return path;
            }
        }
        private BuildingInfo.PathInfo m_DefaultPath = null;
        private BuildingInfo.PathInfo DefaultPath {
            get
            {
                if (m_DefaultPath == null)
                {
                    m_DefaultPath = new BuildingInfo.PathInfo();
                }
                return m_DefaultPath;
            }
        }
        private StationTrackCustomizations ActiveStationCustomizations {
            get
            {
                StationTrackCustomizations customizations;
                if (ActiveStationPath == null)
                {
                    if (SetStationCustomizations.PathCustomizationDict.ContainsKey(DefaultPath))
                        customizations = SetStationCustomizations.PathCustomizationDict[DefaultPath];
                    else
                        customizations = PopulateStationCustomizations();
                }
                else
                {
                    if (SetStationCustomizations.PathCustomizationDict.ContainsKey(ActiveStationPath))
                        customizations = SetStationCustomizations.PathCustomizationDict[ActiveStationPath];
                    else
                        customizations = PopulateStationCustomizations();
                }
                return customizations;
            }
        }
        private bool EnsureActiveStationCustomizations()
        {
            return ActiveStationCustomizations != null;
        }
        private StationTrackCustomizations PopulateStationCustomizations()
        {
            StationTrackCustomizations customizations = new StationTrackCustomizations()
            {
                Horizontal = SliderDict[MetroStationTrackAlterType.Horizontal].value,
                Vertical = SliderDict[MetroStationTrackAlterType.Vertical].value,
                Length = SliderDict[MetroStationTrackAlterType.Length].value,
                Depth = SliderDict[MetroStationTrackAlterType.Depth].value,
                Rotation = SliderDict[MetroStationTrackAlterType.Rotation].value,
                Curve = SliderDict[MetroStationTrackAlterType.Curve].value / 90,
                Path = ActiveStationPath,
                TrackType = m_TrackType,
                AlterType = m_Toggle
            };

            if (ActiveStationPath == null)
            {
                customizations.Path = DefaultPath;
                if (!SetStationCustomizations.PathCustomizationDict.ContainsKey(DefaultPath))
                {
                    Debug.Log("Adding default path");
                    SetStationCustomizations.PathCustomizationDict.Add(DefaultPath, customizations);
                }
                else
                {
                    SetStationCustomizations.PathCustomizationDict[DefaultPath] = customizations;
                }
            }
            else
            {
                if (!SetStationCustomizations.PathCustomizationDict.ContainsKey(ActiveStationPath))
                {
                    Debug.Log("ActivePath not added.  Adding now");
                    SetStationCustomizations.PathCustomizationDict.Add(ActiveStationPath, customizations);
                }
                else
                {
                    SetStationCustomizations.PathCustomizationDict[ActiveStationPath] = customizations;
                }
            }
            for (var i = 0; i < StationPaths.Count(); i++)
            {
                var pathCustomizations = customizations.ShallowClone();
                var path = StationPaths[i];
                pathCustomizations.Path = path;
                if (!SetStationCustomizations.PathCustomizationDict.ContainsKey(path))
                {
                    Debug.Log("Path " + i + " was not found adding path");
                    SetStationCustomizations.PathCustomizationDict.Add(path, pathCustomizations);
                }
                else
                {
                    Debug.Log("Path " + i + " already exists.  Adding new pathCustomization");
                    SetStationCustomizations.PathCustomizationDict[path] = pathCustomizations;
                }
            }
            return customizations;
        }
        protected override PrefabInfo CurrentInfo { get => m_currentBuilding; set => m_currentBuilding = (BuildingInfo)value; }

        protected override void SubStart()
        {
            SetDict[MetroStationTrackAlterType.Horizontal] = m_IsRelocate ? m_Customizations.TrackCustomization.Horizontal : SetStationCustomizations.defHorizontal;
            SetDict[MetroStationTrackAlterType.Vertical] = m_IsRelocate ? m_Customizations.TrackCustomization.Vertical : SetStationCustomizations.defVertical;
            SetDict[MetroStationTrackAlterType.Depth] = m_IsRelocate ? m_Customizations.TrackCustomization.Depth : SetStationCustomizations.DEF_DEPTH;
            SetDict[MetroStationTrackAlterType.Length] = m_IsRelocate ? m_Customizations.TrackCustomization.Length : SetStationCustomizations.DEF_LENGTH;
            SetDict[MetroStationTrackAlterType.Rotation] = m_IsRelocate ? m_Customizations.TrackCustomization.Rotation : SetStationCustomizations.DEF_ROTATION;
            SetDict[MetroStationTrackAlterType.Curve] = m_IsRelocate ? m_Customizations.TrackCustomization.Curve : SetStationCustomizations.DEF_CURVE;
        }

        protected override void OnToggleKeyDown(UIComponent c, UIKeyEventParameter e)
        {
            if (SliderDataDict != null && SliderDataDict.ContainsKey(m_Toggle) && (m_Toggle & MetroStationTrackAlterType.All) != MetroStationTrackAlterType.All && m_Toggle != MetroStationTrackAlterType.None)
            {
                SliderData sData = SliderDataDict[m_Toggle];

                switch (e.keycode)
                {
                    case KeyCode.LeftArrow:
                        if (m_Toggle == MetroStationTrackAlterType.Vertical)
                        {
                            SliderDict[MetroStationTrackAlterType.Horizontal].value = Math.Max(sData.Min, SetDict[MetroStationTrackAlterType.Horizontal] - sData.Step);
                        }
                        else
                        {
                            SliderDict[m_Toggle].value = Math.Max(sData.Min, SetDict[m_Toggle] - sData.Step);
                        }
                        break;
                    case KeyCode.RightArrow:
                        if (m_Toggle == MetroStationTrackAlterType.Vertical)
                        {
                            SliderDict[MetroStationTrackAlterType.Horizontal].value = Math.Min(sData.Max, SetDict[MetroStationTrackAlterType.Horizontal] + sData.Step);
                        }
                        else
                        {
                            SliderDict[m_Toggle].value = Math.Max(sData.Min, SetDict[m_Toggle] + sData.Step);
                        }
                        break;
                    case KeyCode.UpArrow:
                        if (m_Toggle == MetroStationTrackAlterType.Horizontal || m_Toggle == MetroStationTrackAlterType.Vertical)
                        {
                            SliderDict[MetroStationTrackAlterType.Vertical].value = Math.Min(sData.Max, SetDict[MetroStationTrackAlterType.Vertical] + sData.Step);
                        }
                        else
                        {
                            SliderDict[m_Toggle].value = sData.Max;
                        }
                        break;
                    case KeyCode.DownArrow:
                        if (m_Toggle == MetroStationTrackAlterType.Horizontal || m_Toggle == MetroStationTrackAlterType.Vertical)
                        {
                            SliderDict[MetroStationTrackAlterType.Vertical].value = Math.Max(sData.Min, SetDict[MetroStationTrackAlterType.Vertical] - sData.Step);
                        }
                        else
                        {
                            SliderDict[m_Toggle].value = sData.Max;
                        }
                        break;
                    case KeyCode.RightControl:
                        if (m_Toggle == MetroStationTrackAlterType.Horizontal || m_Toggle == MetroStationTrackAlterType.Vertical)
                        {
                            SliderDict[MetroStationTrackAlterType.Horizontal].value = SetStationCustomizations.defHorizontal;
                            SliderDict[MetroStationTrackAlterType.Vertical].value = SetStationCustomizations.defVertical;
                        }
                        else
                        {
                            SliderDict[m_Toggle].value = sData.Def;
                        }
                        break;
                }
                if (ActiveStationPath?.m_nodes == null)
                {
                    if (m_Toggle == MetroStationTrackAlterType.Rotation)
                    {
                        var average = m_currentBuilding.FindAverageNode(true);
                        SliderDict[MetroStationTrackAlterType.Horizontal].value = average.x;
                        SliderDict[MetroStationTrackAlterType.Vertical].value = average.z;
                    }
                }
                SetActiveStationCustomizations();
                m_T.Run();
            }
        }
        protected override void OnToggleKeyUp(UIComponent c, UIKeyEventParameter e)
        {

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
                if (type == MetroStationTrackAlterType.Horizontal)
                {
                    PanelDict[MetroStationTrackAlterType.Vertical].color = new Color32(30, 200, 50, 255);
                }
                else if (type == MetroStationTrackAlterType.Vertical)
                {
                    PanelDict[MetroStationTrackAlterType.Horizontal].color = new Color32(30, 200, 50, 255);
                }

                SliderDict[type].Focus();
                if (ActiveStationPath?.m_nodes == null)
                {
                    if (m_Toggle == MetroStationTrackAlterType.Rotation)
                    {
                        var average = m_currentBuilding.FindAverageNode(true);
                        SliderDict[MetroStationTrackAlterType.Horizontal].value = average.x;
                        SliderDict[MetroStationTrackAlterType.Vertical].value = average.z;
                    }
                }
            }
        }
        protected override void OnToggleMouseUp(UIComponent c, UIMouseEventParameter e, MetroStationTrackAlterType type)
        {
            SetActiveStationCustomizations();
            m_T.Run();
        }
        private void SetActiveStationCustomizations()
        {
            switch (m_Toggle)
            {
                case MetroStationTrackAlterType.Horizontal:
                case MetroStationTrackAlterType.Vertical:
                    if (PathIndexIsSpecified())
                    {
                        ActiveStationCustomizations.Horizontal = SliderDict[MetroStationTrackAlterType.Horizontal].value;
                        ActiveStationCustomizations.Vertical = SliderDict[MetroStationTrackAlterType.Vertical].value;
                        ActiveStationCustomizations.AlterType = m_Toggle;
                    }
                    else
                    {
                        foreach (var customization in SetStationCustomizations.PathCustomizationDict.Values)
                        {
                            customization.Horizontal = SliderDict[MetroStationTrackAlterType.Horizontal].value;
                            customization.Vertical = SliderDict[MetroStationTrackAlterType.Vertical].value;
                            customization.AlterType = m_Toggle;
                        }
                    }

                    break;
                case MetroStationTrackAlterType.Length:
                    if (PathIndexIsSpecified())
                    {
                        ActiveStationCustomizations.Length = SliderDict[MetroStationTrackAlterType.Length].value;
                        ActiveStationCustomizations.AlterType = m_Toggle;
                    }
                    else
                    {
                        foreach (var customization in SetStationCustomizations.PathCustomizationDict.Values)
                        {
                            customization.Length = SliderDict[MetroStationTrackAlterType.Length].value;
                            customization.AlterType = m_Toggle;
                        }
                    }

                    break;
                case MetroStationTrackAlterType.Depth:
                    if (PathIndexIsSpecified())
                    {
                        ActiveStationCustomizations.Depth = SliderDict[MetroStationTrackAlterType.Depth].value;
                        ActiveStationCustomizations.AlterType = m_Toggle;
                    }
                    else
                    {
                        foreach (var customization in SetStationCustomizations.PathCustomizationDict.Values)
                        {
                            customization.Depth = SliderDict[MetroStationTrackAlterType.Depth].value;
                            customization.AlterType = m_Toggle;
                        }
                    }

                    break;
                case MetroStationTrackAlterType.Rotation:
                    if (PathIndexIsSpecified())
                    {
                        ActiveStationCustomizations.Rotation = SliderDict[MetroStationTrackAlterType.Rotation].value;
                        ActiveStationCustomizations.AlterType = m_Toggle;
                    }
                    else
                    {
                        foreach (var customization in SetStationCustomizations.PathCustomizationDict.Values)
                        {
                            customization.Rotation = SliderDict[MetroStationTrackAlterType.Rotation].value;
                            customization.AlterType = m_Toggle;
                        }
                    }

                    break;
                case MetroStationTrackAlterType.Curve:
                    if (PathIndexIsSpecified())
                    {
                        ActiveStationCustomizations.Curve = SliderDict[MetroStationTrackAlterType.Curve].value / 90;
                        ActiveStationCustomizations.AlterType = m_Toggle;
                    }
                    else
                    {
                        foreach (var customization in SetStationCustomizations.PathCustomizationDict.Values)
                        {
                            customization.Curve = SliderDict[MetroStationTrackAlterType.Curve].value / 90;
                            customization.AlterType = m_Toggle;
                        }
                    }

                    break;
            }
        }
        protected override void CreateUI()
        {
            base.CreateUI();
            Action stationMechanicsTask = DoStationMechanics;
            Task t = Task.Create(stationMechanicsTask);

            m_T = t;
            width = 450;

            CreateDragHandle("Subway Station Options");

            SliderDataDict = new Dictionary<MetroStationTrackAlterType, SliderData>();
            SliderDataDict.Add(MetroStationTrackAlterType.Horizontal, new SliderData()
            {
                Def = SetStationCustomizations.defHorizontal,
                Max = SetStationCustomizations.maxHorizontal,
                Min = SetStationCustomizations.minHorizontal,
                Step = HORIZONTAL_STEP
            });
            SliderDataDict.Add(MetroStationTrackAlterType.Vertical, new SliderData()
            {
                Def = SetStationCustomizations.defVertical,
                Max = SetStationCustomizations.maxVertical,
                Min = SetStationCustomizations.minVertical,
                Step = VERTICAL_STEP
            });
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
                Def = SetStationCustomizations.DEF_ROTATION,
                Max = SetStationCustomizations.MAX_ROTATION,
                Min = SetStationCustomizations.MIN_ROTATION,
                Step = ANGLE_STEP
            });
            SliderDataDict.Add(MetroStationTrackAlterType.Curve, new SliderData()
            {
                Def = SetStationCustomizations.DEF_CURVE,
                Max = SetStationCustomizations.MAX_CURVE,
                Min = SetStationCustomizations.MIN_CURVE,
                Step = CURVE_STRENGTH_STEP
            });
            SliderDict = new Dictionary<MetroStationTrackAlterType, UISlider>();
            PanelDict = new Dictionary<MetroStationTrackAlterType, UIPanel>();
            CheckboxStationDict = new Dictionary<StationTrackType, UICheckBox>();
            pnlIndividualTrackManipulator = CreatePanel(new UIPanelParamProps()
            {
                Name = "pnlIndividualTrackManipulator",
                ColShare = 3
            });
            pnlIndividualTrackChooserWrapper = CreatePanel(new UIPanelParamProps()
            {
                Name = "pnlIndividualTrackChooserWrapper",
                ColOffset = 1,
                ColShare = 8
            });

            var lblIndividualTrackChooser = CreateLabel(new UILabelParamProps()
            {
                Name = "lblIndividualTrackChooser",
                Text = "Choose a track to modify",
                ParentComponent = pnlIndividualTrackChooserWrapper,
                ColumnCount = 1
            });

            pnlIndividualTrackChooser = CreatePanel(new UIPanelParamProps()
            {
                Name = "pnlIndividualTrackChooser",
                ParentComponent = pnlIndividualTrackChooserWrapper,
                ColumnCount = 1
            });

            //pnlIndividualTrackChooser = CreateTabStrip(new UITabstripParamProps()
            //{
            //    Name = "pnlIndividualTrackChooser",
            //    ColumnCount = 1,
            //    ParentComponent = pnlIndividualTrackChooser
            //});

            var lblIndividualTrackManipulator = CreateLabel(new UILabelParamProps()
            {
                Name = "lblIndividualTrackManipulator",
                Text = "Modify Selections",
                ParentComponent = pnlIndividualTrackManipulator,
                ColumnCount = 1
            });

            //pnlIndividualTrackManipulator = CreateTabStrip(new UITabstripParamProps()
            //{
            //    Name = "pnlIndividualTrackManipulator",
            //    ColumnCount = 1,
            //    ParentComponent = pnlIndividualTrackManipulator
            //});

            CreateSelectionTools();
            stationXPanel = CreatePanel(new UIPanelParamProps()
            {
                Name = "stationXPanel",
                Margins = new Vector4(8, 0, 8, 8),
                ColumnCount = 1
            });

            var xSlider = CreateSlider(new UISliderParamProps()
            {
                Name = "xSlider",
                ParentComponent = stationXPanel,
                TrackAlterType = MetroStationTrackAlterType.Horizontal,
                //TooltipComponent = ttpStationLengthSelectorTooltip,
                ColumnCount = 1
            });

            stationYPanel = CreatePanel(new UIPanelParamProps()
            {
                Name = "stationYPanel",
                Margins = new Vector4(8, 0, 8, 8),
                ColumnCount = 1
            });

            var ySlider = CreateSlider(new UISliderParamProps()
            {
                Name = "ySlider",
                ParentComponent = stationYPanel,
                TrackAlterType = MetroStationTrackAlterType.Vertical,
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

            var stationCurvePanel = CreatePanel(new UIPanelParamProps()
            {
                Name = "stationCurvePanel",
                Margins = new Vector4(8, 0, 8, 8),
                ColumnCount = 1
            });
            //var ttpStationCurveSelectorTooltip = CreateToolTipPanel(new UIToolTipPanelParamProps()
            //{
            //    Name = "ttpStationCurveSelectorTooltip",
            //    ToolTipPanelText = Station_Curve_Info
            //});
            var curveSlider = CreateSlider(new UISliderParamProps()
            {
                Name = "curveSlider",
                ParentComponent = stationCurvePanel,
                TrackAlterType = MetroStationTrackAlterType.Curve,
                //TooltipComponent = ttpStationCurveSelectorTooltip,
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
            //var tsStationTrackChooser = CreateTabStrip(new UITabstripParamProps()
            //{
            //    Name = "tsStationTrackChooser",
            //    ColumnCount = 1,
            //    ParentComponent = pnlStationTrackChooser
            //});
            btnSidePlatformStationTrack = CreateButton(new UIButtonParamProps()
            {
                Name = "btnSidePlatformStationTrack",
                ToolTip = "Side Platform Station",
                ParentComponent = pnlStationTrackChooser,
                ColumnCount = 5,
                Width = 79,
                Height = 69,
                Atlas = UIHelper.GenerateLinearAtlas("MOM_SidePlatformStationTrackAtlas", UIHelper.SidePlatformStationTrack),
                EventClick = (c, v) =>
                {
                    m_TrackType = StationTrackType.SidePlatform;
                    TunnelStationTrackToggleStyles();
                }
            });
            var btnIslandPlatformStationTrack = CreateButton(new UIButtonParamProps()
            {
                Name = "btnIslandPlatformStationTrack",
                ToolTip = "Island Platform Station",
                ParentComponent = pnlStationTrackChooser,
                ColumnCount = 5,
                Width = 79,
                Height = 69,
                Atlas = UIHelper.GenerateLinearAtlas("MOM_IslandPlatformStationTrackAtlas", UIHelper.IslandPlatformStationTrack),
                EventClick = (c, v) =>
                {
                    m_TrackType = StationTrackType.IslandPlatform;
                    TunnelStationTrackToggleStyles();
                }
            });
            var btnSinglePlatformStationTrack = CreateButton(new UIButtonParamProps()
            {
                Name = "btnSinglePlatformStationTrack",
                ToolTip = "Single Platform Station",
                ParentComponent = pnlStationTrackChooser,
                ColumnCount = 5,
                Width = 79,
                Height = 69,
                Atlas = UIHelper.GenerateLinearAtlas("MOM_SinglePlatformStationTrackAtlas", UIHelper.SinglePlatformStationTrack),
                EventClick = (c, v) =>
                {
                    m_TrackType = StationTrackType.SinglePlatform;
                    TunnelStationTrackToggleStyles();
                }
            });
            var btnExpressSidePlatformStationTrack = CreateButton(new UIButtonParamProps()
            {
                Name = "btnExpressSidePlatformStationTrack",
                ToolTip = "Express Side Platform Station",
                ParentComponent = pnlStationTrackChooser,
                ColumnCount = 5,
                Width = 79,
                Height = 69,
                Atlas = UIHelper.GenerateLinearAtlas("MOM_ExpressSidePlatformStationTrackAtlas", UIHelper.ExpressSidePlatformStationTrack),
                EventClick = (c, v) =>
                {
                    m_TrackType = StationTrackType.ExpressSidePlatform;
                    TunnelStationTrackToggleStyles();
                }
            });
            var btnDualIslandPlatformStationTrack = CreateButton(new UIButtonParamProps()
            {
                Name = "btnDualIslandPlatformStationTrack",
                ToolTip = "Dual Island Platform Station",
                ParentComponent = pnlStationTrackChooser,
                ColumnCount = 5,
                Width = 79,
                Height = 69,
                Atlas = UIHelper.GenerateLinearAtlas("MOM_DualIslandPlatformStationTrackAtlas", UIHelper.DualIslandPlatformStationTrack),
                EventClick = (c, v) =>
                {
                    m_TrackType = StationTrackType.DualIslandPlatform;
                    TunnelStationTrackToggleStyles();
                }
            });
        }

        protected override void TunnelStationTrackToggleStyles()
        {
            if (m_TrackType == StationTrackType.None)
            {
                m_TrackType = StationTrackType.SidePlatform;
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

            if (PathIndexIsSpecified())
            {
                Debug.Log("Track " + m_PathIndex + " will be set from " + ActiveStationCustomizations.TrackType.ToString() + " to " + m_TrackType);
                ActiveStationCustomizations.TrackType = m_TrackType;
                foreach (var kvp in SetStationCustomizations.PathCustomizationDict)
                {
                    Debug.Log("Track " + StationPaths.IndexOf(kvp.Key) + " has a tracktype of " + kvp.Value.TrackType.ToString());
                }
                RefreshSprites(ButtonArray[m_PathIndex], GetAtlasFromStationTrackType(m_TrackType), false);
            }
            else
            {
                Debug.Log("PathIndex was not specified so just going to blanket with " + m_TrackType.ToString());
                for (int i = 0; i < StationPaths.Count(); i++)
                {
                    if (ButtonArray[i] != null && ButtonArray[i].height > 1)
                    {
                        RefreshSprites(ButtonArray[i], GetAtlasFromStationTrackType(m_TrackType), false);
                    }
                    if (StationPaths[i] != null && SetStationCustomizations.PathCustomizationDict.ContainsKey(StationPaths[i]))
                    {
                        SetStationCustomizations.PathCustomizationDict[StationPaths[i]].TrackType = m_TrackType;
                    }
                }
            }
            m_T.Run();
        }

        private UITextureAtlas GetAtlasFromStationTrackType(StationTrackType trackType)
        {
            switch (trackType)
            {
                case StationTrackType.SidePlatform:
                    return UIHelper.GenerateLinearAtlas("MOM_SidePlatformStationTrackAtlas", UIHelper.SidePlatformStationTrack);
                case StationTrackType.IslandPlatform:
                    return UIHelper.GenerateLinearAtlas("MOM_IslandPlatformStationTrackAtlas", UIHelper.IslandPlatformStationTrack);
                case StationTrackType.SinglePlatform:
                    return UIHelper.GenerateLinearAtlas("MOM_SinglePlatformStationTrackAtlas", UIHelper.SinglePlatformStationTrack);
                case StationTrackType.ExpressSidePlatform:
                    return UIHelper.GenerateLinearAtlas("MOM_ExpressSidePlatformStationTrackAtlas", UIHelper.ExpressSidePlatformStationTrack);
                case StationTrackType.DualIslandPlatform:
                    return UIHelper.GenerateLinearAtlas("MOM_DualIslandPlatformStationTrackAtlas", UIHelper.DualIslandPlatformStationTrack);
            }
            return null;
        }

        private void SetTunnelInfoByType(BuildingInfo info = null)
        {
            if (info == null)
                info = m_currentBuilding;
            for (int i = 0; i < info.m_paths.Length; i++)
            {
                var path = info.m_paths[i];
                if (path != null && (!PathIndexIsSpecified() || path == ActiveStationPath))
                {
                    if (path.m_netInfo.IsUndergroundMetroStationTrack())
                    {
                        switch (m_TrackType)
                        {
                            case StationTrackType.SidePlatform:
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
                }
            }
        }
        private bool IsRelocate()
        {
            m_IsRelocate = m_buildingTool.m_relocate > 0;
            if (m_IsRelocate)
            {
                m_Customizations = new ReverseEngineerStationCustomizations(this, m_buildingTool.m_relocate, 0);
                SliderDict[MetroStationTrackAlterType.Horizontal].value = m_Customizations.TrackCustomization.Horizontal;
                SliderDict[MetroStationTrackAlterType.Vertical].value = m_Customizations.TrackCustomization.Vertical;
                SliderDict[MetroStationTrackAlterType.Length].value = m_Customizations.TrackCustomization.Length;
                SliderDict[MetroStationTrackAlterType.Depth].value = m_Customizations.TrackCustomization.Depth;
                SliderDict[MetroStationTrackAlterType.Rotation].value = m_Customizations.TrackCustomization.Rotation;
                SliderDict[MetroStationTrackAlterType.Curve].value = m_Customizations.TrackCustomization.Curve;
                m_TrackType = m_Customizations.TrackCustomization.TrackType;
            }
            return m_IsRelocate;
        }

        private void CreateIndividualTrackSelector(int index, bool selectOnCreate = true)
        {
            StationTrackType trackType = m_TrackType;
            if (SetStationCustomizations.PathCustomizationDict.ContainsKey(StationPaths[index]))
            {
                trackType = SetStationCustomizations.PathCustomizationDict[StationPaths[index]].TrackType;
            }
            ButtonArray.Add(CreateButton(new UIButtonParamProps()
            {
                Name = $"btnIndividualTrack{index}",
                ToolTip = $"Track {index + 1}",
                ParentComponent = pnlIndividualTrackChooser,
                ColumnCount = 6,
                Width = 40,
                Height = 35,
                Margins = new Vector2(2, 2),
                SelectOnCreate = selectOnCreate,
                Atlas = GetAtlasFromStationTrackType(trackType),
                EventClick = (c, v) =>
                {
                    btnIndividualTrackOnClick(ButtonArray.IndexOf((UIButton)c));
                }
            }));
        }
        private void CreateSelectionTools()
        {
            btnCloneSelectedTrack = CreateButton(new UIButtonParamProps()
            {
                Name = "btnCloneSelectedTrack",
                ToolTip = "Clone Selected Track",
                Atlas = UIHelper.GenerateLinearAtlas("MOM_CloneSelectedTracks", UIHelper.CloneSelected),
                ColumnCount = 2,
                ParentComponent = pnlIndividualTrackManipulator,
                Width = 40,
                Height = 35,
                Margins = new Vector2(2, 2),
                //DisableState = true,
                EventClick = (c, v) =>
                {
                    btnCloneSelectedTrackOnClick();
                }
            });
            btnRemoveSelectedTrack = CreateButton(new UIButtonParamProps()
            {
                Name = "btnRemoveSelectedTrack",
                ToolTip = "Remove Selected Track",
                Atlas = UIHelper.GenerateLinearAtlas("MOM_RemoveSelectedTracks", UIHelper.RemoveSelected),
                ColumnCount = 2,
                ParentComponent = pnlIndividualTrackManipulator,
                Width = 40,
                Height = 35,
                Margins = new Vector2(2, 2),
                EventClick = (c, v) =>
                {
                    btnRemoveSelectedTrackOnClick();
                }
            });
            btnSelectAllTracks = CreateButton(new UIButtonParamProps()
            {
                Name = "btnSelectAllTracks",
                Text = StationPaths.Count().ToString(),
                ToolTip = "Select All Tracks",
                Atlas = UIHelper.GenerateLinearAtlas("MOM_SelectAllTracksAtlas", UIHelper.SelectAll),
                ColumnCount = 2,
                ParentComponent = pnlIndividualTrackManipulator,
                Width = 40,
                Height = 35,
                Margins = new Vector2(2, 2),
                EventClick = (c, v) =>
                {
                    btnSelectAllTracksOnClick();
                }
            });
            btnRevertAllTracks = CreateButton(new UIButtonParamProps()
            {
                Name = "btnRevertAllTracks",
                ToolTip = "Revert Station to Original Layout",
                Atlas = UIHelper.GenerateLinearAtlas("MOM_RevertAllTracks", UIHelper.RevertAll),
                ColumnCount = 2,
                ParentComponent = pnlIndividualTrackManipulator,
                Width = 40,
                Height = 35,
                Margins = new Vector2(2, 2),
                EventClick = (c, v) =>
                {
                    btnRevertAllTracksOnClick();
                }
            });
        }
        private void btnIndividualTrackOnClick(int inx)
        {
            m_PathIndex = inx;

            if (PathIndexIsSpecified() && SetStationCustomizations.PathCustomizationDict.ContainsKey(ActiveStationPath))
            {
                BlurAllButtonsInPanel(pnlIndividualTrackChooser);
                ((UITabstrip)btnSidePlatformStationTrack.parent).selectedIndex = (int)ActiveStationCustomizations.TrackType;
                SetSliders(ActiveStationCustomizations);
                m_T.Run();
                AssertAddRemoveAvailability();
                BlurAllButtonsInPanel(pnlIndividualTrackManipulator);
            }

        }
        private void AssertAddRemoveAvailability()
        {
            if (PathIndexIsSpecified())
            {
                Debug.Log("Index is " + m_PathIndex + ". Enabling plus.");
                btnCloneSelectedTrack.state = UIButton.ButtonState.Normal;
                btnCloneSelectedTrack.isInteractive = true;
                if (StationPaths.Count() > 1)
                {
                    btnRemoveSelectedTrack.state = UIButton.ButtonState.Normal;
                    btnRemoveSelectedTrack.isInteractive = true;
                }
                else
                {
                    btnRemoveSelectedTrack.state = UIButton.ButtonState.Disabled;
                    btnRemoveSelectedTrack.isInteractive = false;
                }
            }
            else
            {
                Debug.Log("Index is " + m_PathIndex + ". Disabling everything.");
                btnCloneSelectedTrack.state = UIButton.ButtonState.Disabled;
                btnCloneSelectedTrack.isInteractive = false;
                btnRemoveSelectedTrack.state = UIButton.ButtonState.Disabled;
                btnRemoveSelectedTrack.isInteractive = false;
            }
        }

        private void StandardAddRemoveTrackStuff()
        {
            ResetStationPaths();
            UpdateIndividualTrackSelectors();
            ResetPositionSliderProperties();
            btnSelectAllTracks.text = StationPaths.Count().ToString();
            BlurAllButtonsInPanel(pnlIndividualTrackManipulator);
            m_Toggle = MetroStationTrackAlterType.All;
            EnsureActiveStationCustomizations();
            m_PathIndex = StationPaths.Count() - 1;
            ButtonArray[m_PathIndex].SimulateClick();
            AssertAddRemoveAvailability();
            m_T.Run();
        }
        private void btnCloneSelectedTrackOnClick()
        {
            if (PathIndexIsSpecified() && m_currentBuilding.AddStationPath(m_PathIndex) != null)
                StandardAddRemoveTrackStuff();
        }
        private void btnRemoveSelectedTrackOnClick()
        {
            if (PathIndexIsSpecified() && m_currentBuilding.RemoveStationPath(m_PathIndex) != null)
                StandardAddRemoveTrackStuff();
        }
        private void btnRevertAllTracksOnClick()
        {
            m_PathIndex = -1;
            m_currentBuilding.RestoreBuildingDefault();
            ResetPositionSliderProperties();

            SetStationCustomizations.ResetPathCustomizationDict();
            ResetStationPaths();
            btnSelectAllTracks.text = StationPaths.Count().ToString();
            SetSlidersDefault();
            PopulateStationCustomizations();
            ActiveStationCustomizations.AlterType = (MetroStationTrackAlterType.All & ~MetroStationTrackAlterType.Rotation);
            SetStationCustomizations.m_PrevAngle = SetStationCustomizations.DEF_ROTATION;
            DeactivateIndividualTrackSelectors();
            ActivateIndividualTrackSelectors(false);
            btnSidePlatformStationTrack.SimulateClick();
            BlurAllButtonsInPanel(pnlIndividualTrackManipulator);
            BlurAllButtonsInPanel(pnlIndividualTrackChooser);
            AssertAddRemoveAvailability();
            ((UITabstrip)ButtonArray[0].parent).selectedIndex = 0;
        }
        private void btnSelectAllTracksOnClick()
        {
            m_PathIndex = -1;
            if (!SetStationCustomizations.PathCustomizationDict.ContainsKey(DefaultPath))
            {
                SetStationCustomizations.PathCustomizationDict.Add(DefaultPath, new StationTrackCustomizations());
            }
            var pathCustomizer = SetStationCustomizations.PathCustomizationDict[DefaultPath];
            var allPathNodes = StationPaths.SelectMany(p => p.m_nodes).ToArray();
            var center = Vector3.zero;
            for (int i = 0; i < allPathNodes.Count(); i++)
            {
                center += allPathNodes[i];
            }
            center /= allPathNodes.Count();
            pathCustomizer.Horizontal = (float)Math.Round(center.x / HORIZONTAL_STEP) * HORIZONTAL_STEP;
            pathCustomizer.Vertical = (float)Math.Round(center.z / VERTICAL_STEP) * VERTICAL_STEP;
            SetSliders(pathCustomizer);
            BlurAllButtonsInPanel(pnlIndividualTrackManipulator);
            BlurAllButtonsInPanel(pnlIndividualTrackChooser);
            AssertAddRemoveAvailability();
        }

        private void UpdateSelectedIndex()
        {
            var parent = ((UITabstrip)ButtonArray[m_PathIndex].parent);
            parent.selectedIndex = (m_PathIndex + 1) % GetMaxButtonFit(parent, ButtonArray[m_PathIndex].width);
        }

        private void SetSlidersDefault()
        {
            SliderDict[MetroStationTrackAlterType.Horizontal].value = SetStationCustomizations.defHorizontal;
            SliderDict[MetroStationTrackAlterType.Vertical].value = SetStationCustomizations.defVertical;
            SliderDict[MetroStationTrackAlterType.Rotation].value = SetStationCustomizations.DEF_ROTATION;
            SliderDict[MetroStationTrackAlterType.Curve].value = SetStationCustomizations.DEF_CURVE;
            SliderDict[MetroStationTrackAlterType.Depth].value = SetStationCustomizations.DEF_DEPTH;
            SliderDict[MetroStationTrackAlterType.Length].value = SetStationCustomizations.DEF_LENGTH;
        }
        private void SetSliders(StationTrackCustomizations pathCustomizer)
        {
            SliderDict[MetroStationTrackAlterType.Horizontal].value = pathCustomizer.Horizontal;
            SliderDict[MetroStationTrackAlterType.Vertical].value = pathCustomizer.Vertical;
            SliderDict[MetroStationTrackAlterType.Length].value = pathCustomizer.Length;
            SliderDict[MetroStationTrackAlterType.Depth].value = pathCustomizer.Depth;
            SliderDict[MetroStationTrackAlterType.Rotation].value = pathCustomizer.Rotation;
            SliderDict[MetroStationTrackAlterType.Curve].value = pathCustomizer.Curve * 90;

        }
        private void UpdateIndividualTrackSelectors()
        {
            if (ButtonArray.Count() != StationPaths.Count())
            {
                if (ButtonArray.Count() != StationPaths.Count())
                {
                    if (ButtonArray.Count() < StationPaths.Count())
                    {
                        CreateIndividualTrackSelector(StationPaths.Count() - 1);
                        btnSelectAllTracks.text = StationPaths.Count().ToString();
                    }
                    else
                    {
                        DeactivateIndividualTrackSelectors();
                        ActivateIndividualTrackSelectors();
                    }
                }
            }
        }
        private void ActivateIndividualTrackSelectors(bool selectOnCreate = true)
        {
            for (var i = 0; i < StationPaths.Count(); i++)
            {
                CreateIndividualTrackSelector(i, selectOnCreate);
            }
            btnSelectAllTracks.text = StationPaths.Count().ToString();
        }

        private void DeactivateIndividualTrackSelectors()
        {
            RemoveChildren(pnlIndividualTrackChooser, false);
            pnlIndividualTrackChooser.height = 0;
            ClearButtonArray();
        }

        private void ResetStationPaths()
        {
            if (m_StationPaths != null)
            {
                m_StationPaths.Clear();
                m_StationPaths = null;
            }
        }
        private void DeactivatePlanarPositionalSliders()
        {
            RemoveChildren(stationXPanel);
            stationXPanel.height = 0;
            RemoveChildren(stationYPanel);
            stationYPanel.height = 0;
        }
        private void ActivatePlanarPositionalSliders()
        {
            SliderDict[MetroStationTrackAlterType.Horizontal] = CreateSlider(new UISliderParamProps()
            {
                Name = "xSlider",
                ParentComponent = stationXPanel,
                TrackAlterType = MetroStationTrackAlterType.Horizontal,
                //TooltipComponent = ttpStationLengthSelectorTooltip,
                ColumnCount = 1
            });
            SliderDict[MetroStationTrackAlterType.Vertical] = CreateSlider(new UISliderParamProps()
            {
                Name = "ySlider",
                ParentComponent = stationYPanel,
                TrackAlterType = MetroStationTrackAlterType.Vertical,
                //TooltipComponent = ttpStationLengthSelectorTooltip,
                ColumnCount = 1
            });
        }

        private void ResetPositionSliderProperties()
        {
            var maxWidth = Math.Abs(StationPaths.SelectMany(p => p.m_nodes).OrderBy(n => Math.Abs(n.x)).LastOrDefault().x);
            var maxLength = Math.Abs(StationPaths.SelectMany(p => p.m_nodes).OrderBy(n => Math.Abs(n.z)).LastOrDefault().z);
            maxWidth = (float)Math.Round((double)(maxWidth + 100) / HORIZONTAL_STEP) * HORIZONTAL_STEP;
            maxLength = (float)Math.Round((double)(maxLength + 100) / VERTICAL_STEP) * VERTICAL_STEP;
            var center = m_currentBuilding.FindAverageNode(true);
            SetStationCustomizations.maxHorizontal = maxWidth;
            SetStationCustomizations.minHorizontal = -maxWidth;
            SetStationCustomizations.defHorizontal = (float)Math.Round(center.x / HORIZONTAL_STEP) * HORIZONTAL_STEP;
            SliderDataDict[MetroStationTrackAlterType.Horizontal] = new SliderData()
            {
                Max = SetStationCustomizations.maxHorizontal,
                Min = SetStationCustomizations.minHorizontal,
                Def = SetStationCustomizations.defHorizontal,
                Step = HORIZONTAL_STEP
            };

            SetStationCustomizations.maxVertical = maxLength;
            SetStationCustomizations.minVertical = -maxLength;
            SetStationCustomizations.defVertical = (float)Math.Round(center.z / VERTICAL_STEP) * VERTICAL_STEP;
            SliderDataDict[MetroStationTrackAlterType.Vertical] = new SliderData()
            {
                Max = SetStationCustomizations.maxVertical,
                Min = SetStationCustomizations.minVertical,
                Def = SetStationCustomizations.defVertical,
                Step = VERTICAL_STEP
            };
        }
        private void RevertSlidersAndRun()
        {
            var defaultPath = SetStationCustomizations.PathCustomizationDict.Keys.FirstOrDefault(k => k.m_nodes == null);
            if (defaultPath == null)
            {
                defaultPath = new BuildingInfo.PathInfo();
            }
            if (!SetStationCustomizations.PathCustomizationDict.ContainsKey(defaultPath))
            {
                SetStationCustomizations.PathCustomizationDict.Add(defaultPath, new StationTrackCustomizations());
            }
            var pathCustomizer = SetStationCustomizations.PathCustomizationDict[defaultPath];
            pathCustomizer.Horizontal = SetStationCustomizations.defHorizontal;
            pathCustomizer.Vertical = SetStationCustomizations.defVertical;
            pathCustomizer.Length = SetStationCustomizations.DEF_LENGTH;
            pathCustomizer.Depth = SetStationCustomizations.DEF_DEPTH;
            pathCustomizer.Rotation = SetStationCustomizations.DEF_ROTATION;
            pathCustomizer.Curve = SetStationCustomizations.DEF_CURVE;

            SetSliders(pathCustomizer);
            m_T.Run();
        }
        protected override void Activate(PrefabInfo info)
        {
            Deactivate();
            base.Activate(info);
            if (SetDict != null && SetDict.Count > 0)
            {
                DeactivatePlanarPositionalSliders();
                ResetPositionSliderProperties();
                ActivatePlanarPositionalSliders();
                DeactivateIndividualTrackSelectors();
                ResetStationPaths();
                SetStationCustomizations.ResetPathCustomizationDict();
                //DoStationMechanicsResetAngles();
                m_PathIndex = -1;
                m_TrackType = StationTrackType.SidePlatform;
                m_Toggle = MetroStationTrackAlterType.All;
                ActivateIndividualTrackSelectors(false);
                PopulateStationCustomizations();
                SetStationCustomizations.m_PrevAngle = 0;
                Debug.Log("TEMP Activating " + m_currentBuilding.name + ". Had prev angle of " + SetStationCustomizations.m_PrevAngle + " and being set to " + ActiveStationCustomizations.Rotation);
                TunnelStationTrackToggleStyles();
                //m_T.Run();

                if (btnSelectAllTracks != null)
                {
                    btnSelectAllTracks.SimulateClick();
                }
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
            if (m_currentBuilding != null && !InPropagation)
            {
                InPropagation = true;
                Debug.Log("In propagation for " + m_currentBuilding.name);
                try
                {
                    EnsureActiveStationCustomizations();
                    SetStationCustomizations.ModifyStation(m_currentBuilding, ActiveStationCustomizations, null);
                }
                finally
                {
                    InPropagation = false;
                    Debug.Log("Out of propagation for " + m_currentBuilding.name);
                }
            }

        }
        private void DoStationMechanicsResetAngles()
        {
            if (m_currentBuilding != null && !InPropagation)
            {
                InPropagation = true;
                Debug.Log("In propagation for " + m_currentBuilding.name);
                try
                {
                    Debug.Log("TEMP Deactivating " + m_currentBuilding.name + ". Had prev angle of " + SetStationCustomizations.m_PrevAngle + " and being set to 0");
                    ActiveStationCustomizations.AlterType = MetroStationTrackAlterType.All;
                    ActiveStationCustomizations.Rotation = 0;
                    SetStationCustomizations.ModifyStation(m_currentBuilding, ActiveStationCustomizations, null);
                }
                finally
                {
                    InPropagation = false;
                    Debug.Log("Out of propagation for " + m_currentBuilding.name);
                }

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

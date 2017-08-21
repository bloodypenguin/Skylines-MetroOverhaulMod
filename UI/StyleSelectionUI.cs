using ICities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MetroOverhaul.UI
{
    public class StyleSelectionUI : MonoBehaviour
    {
        public Rect window;
        public bool showWindow;
        public bool move;
        public int style;
        public int stationType;
        public int trackType;
        public bool fence;
        public bool station;
        public bool island;

        private NetInfo concretePrefab;
        private NetInfo concretePrefabNoBar;
        private NetInfo concreteStationPrefab;
        private NetInfo concreteStationIslandPrefab;
        private NetInfo concreteSmallPrefab;
        private NetInfo concreteSmallPrefabNoBar;
        private NetInfo concreteStationSmallPrefab;
        private NetInfo steelPrefab;
        private NetInfo steelPrefabNoBar;
        private NetInfo steelStationPrefab;
        private NetInfo steelStationIslandPrefab;
        private NetInfo steelSmallPrefab;
        private NetInfo steelSmallPrefabNoBar;
        private NetInfo steelStationSmallPrefab;

        public StyleSelectionUI()
        {
            this.window = new Rect((float)(Screen.width - 305), (float)(Screen.height - 300), 300f, 134f);
            this.fence = true;
        }
        public void Awake()
        {
            concretePrefab = PrefabCollection<NetInfo>.FindLoaded("Metro Track Ground");
            concretePrefabNoBar = PrefabCollection<NetInfo>.FindLoaded("Metro Track Ground NoBar");
            concreteStationPrefab = PrefabCollection<NetInfo>.FindLoaded("Metro Station Track Ground");
            concreteStationIslandPrefab = PrefabCollection<NetInfo>.FindLoaded("Metro Station Track Ground Island");
            concreteSmallPrefab = PrefabCollection<NetInfo>.FindLoaded("Metro Track Ground Small");
            concreteSmallPrefabNoBar = PrefabCollection<NetInfo>.FindLoaded("Metro Track Ground Small NoBar");
            concreteStationSmallPrefab = PrefabCollection<NetInfo>.FindLoaded("Metro Station Track Ground Small");
            steelPrefab = PrefabCollection<NetInfo>.FindLoaded("Steel Metro Track Ground");
            steelPrefabNoBar = PrefabCollection<NetInfo>.FindLoaded("Steel Metro Track Ground NoBar");
            steelStationPrefab = PrefabCollection<NetInfo>.FindLoaded("Steel Metro Station Track Ground");
            steelStationIslandPrefab = PrefabCollection<NetInfo>.FindLoaded("Steel Metro Station Track Ground Island");
            steelSmallPrefab = PrefabCollection<NetInfo>.FindLoaded("Steel Metro Track Ground Small");
            steelSmallPrefabNoBar = PrefabCollection<NetInfo>.FindLoaded("Steel Metro Track Ground Small NoBar");
            steelStationSmallPrefab = PrefabCollection<NetInfo>.FindLoaded("Steel Metro Station Track Ground Small");
        }

        public void Update()
        {
            try
            {
                if (ToolsModifierControl.GetCurrentTool<ToolBase>() is NetTool)
                {
                    if (this.IsMetroTrack(ToolsModifierControl.GetCurrentTool<NetTool>().m_prefab))
                    {
                        this.showWindow = true;
                        this.SetNetToolPrefab();
                    }
                    else
                        this.showWindow = false;
                }
                else if (ToolsModifierControl.GetCurrentTool<ToolBase>().GetType().Name == "NetToolFine")
                {
                    if (this.IsMetroTrack((ToolsModifierControl.GetCurrentTool<ToolBase>() as NetTool).m_prefab))
                    {
                        this.showWindow = true;
                        this.SetNetToolPrefab();
                    }
                    else
                        this.showWindow = false;
                }
                else
                    this.showWindow = false;
            }
            catch
            {
            }
        }

        private void OnGUI()
        {
            if (!this.showWindow)
            {
                return;
            }
            this.window = GUI.Window(29292929, this.window, Window, "Metro Track Style");
        }

        private void Window(int id)
        {
            if (this.move)
            {
                GUI.DragWindow(new Rect(0.0f, 0.0f, 300f, 100f));
                GUI.Label(new Rect(100f, 45f, 200f, 30f), "<size=20>Move window!</size>");
            }
            else
            {
                var styleList = new List<string>();
                if (concretePrefab != null)
                {
                    styleList.Add("Concrete");
                }
                if (steelPrefab != null)
                {
                    styleList.Add("Steel");
                }
                this.style = GUI.Toolbar(new Rect(5f, 28f, 290f, 32f), this.style, styleList.ToArray());
                var showFenceOption = false;

                if (style == 0)
                {
                    if (concretePrefab != null && concretePrefabNoBar != null)
                    {
                        showFenceOption = true;
                    }
                }
                else if (style == 1)
                {
                    if (steelPrefab != null && concretePrefabNoBar != null)
                    {
                        showFenceOption = true;
                    }
                }
                var trackList = new List<string>();
                trackList.Add("2L Track");
                trackList.Add("1L Track");

                var stationList = new List<string>();
                stationList.Add("Side Platform");
                stationList.Add("Island Platform");

                trackType = GUI.Toolbar(new Rect(5f, 65f, 290f, 32f), trackType, trackList.ToArray());
                this.fence = showFenceOption && GUI.Toggle(new Rect(5f, 100f, 140f, 30f), this.fence, "Fenced track");
                station = GUI.Toggle(new Rect(5f, 150f, 140f, 30f), station, "Station track");
                stationType = GUI.Toolbar(new Rect(5f, 190f, 290f, 32f), this.stationType, stationList.ToArray());

                if (GUI.changed)
                    this.SetNetToolPrefab();
            }
            this.move = GUI.Toggle(new Rect(198f, 100f, 100f, 28f), this.move, "Move window");
        }

        private static bool IsSubversion(NetInfo info, NetInfo metroGroundTrack)
        {
            if (info == null)
            {
                return false;
            }
            var ai = metroGroundTrack?.m_netAI as TrainTrackAI;
            if (ai == null)
            {
                return false;
            }
            if (metroGroundTrack == info)
            {
                return true;
            }
            if (ai.m_bridgeInfo != null && ai.m_bridgeInfo == info)
            {
                return true;
            }
            if (ai.m_elevatedInfo != null && ai.m_elevatedInfo == info)
            {
                return true;
            }
            if (ai.m_slopeInfo != null && ai.m_elevatedInfo == info)
            {
                return true;
            }
            if (ai.m_tunnelInfo != null && ai.m_elevatedInfo == info)
            {
                return true;
            }
            return false;
        }

        private bool IsMetroTrack(NetInfo info)
        {
            return IsSubversion(info, concretePrefab) || IsSubversion(info, concretePrefabNoBar) || IsSubversion(info, concreteSmallPrefab) || IsSubversion(info, concreteSmallPrefabNoBar) ||
                   IsSubversion(info, steelPrefab) || IsSubversion(info, steelPrefabNoBar) || IsSubversion(info,steelSmallPrefab) || IsSubversion(info,steelSmallPrefabNoBar);
        }

        private void SetNetToolPrefab()
        {
            var netTool = ToolsModifierControl.SetTool<NetTool>();
            NetInfo prefab = null;
            switch (style)
            {
                case 0:
                    if (trackType == 0)
                    {
                        if (station)
                        {
                            if (stationType == 0)
                            {
                                prefab = concreteStationPrefab;
                            }
                            else if (stationType == 1)
                            {
                                prefab = concreteStationIslandPrefab;
                            }
                        }
                        else
                        {
                            prefab = fence ? concretePrefab : concretePrefabNoBar;
                        }
                    }
                    else
                    {
                        if (station)
                        {
                            prefab = concreteStationSmallPrefab;
                        }
                        else
                        {
                            prefab = fence ? concreteSmallPrefab : concreteSmallPrefabNoBar;
                        }
                    }

                    break;
                case 1:
                    if (trackType == 0)
                    {
                        if (station)
                        {
                            if (stationType == 0)
                            {
                                prefab = steelStationPrefab;
                            }
                            else if (stationType == 1)
                            {
                                prefab = steelStationIslandPrefab;
                            }
                        }
                        else
                        {
                            prefab = fence ? steelPrefab : steelPrefabNoBar;
                        }
                    }
                    else
                    {
                        if (station)
                        {
                            prefab = steelStationSmallPrefab;
                        }
                        else
                        {
                            prefab = fence ? steelSmallPrefab : steelSmallPrefabNoBar;
                        }
                    }

                    break;
                default:
                    throw new Exception($"Style handling wasn't implemened! style={style}");
            }
            if (prefab != null)
            {
                netTool.m_prefab = prefab;
            }
        }
    }
}
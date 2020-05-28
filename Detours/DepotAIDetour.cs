using ColossalFramework;
using ColossalFramework.Math;
using MetroOverhaul.Extensions;
using MetroOverhaul.Redirection.Attributes;
using MetroOverhaul.UI;
using UnityEngine;

namespace MetroOverhaul.Detours
{
    [TargetType(typeof(DepotAI))]
    public class DepotAIDetour : DepotAI
    {
        [RedirectMethod]
        protected new void RenderPathOverlay(RenderManager.CameraInfo cameraInfo, Color color, Vector3 position, float angle)
        {
            if (this.m_info.m_paths != null)
            {
                var theColor = color;
                var stationCustomizationHasPath = SetStationCustomizations.PathCustomization?.Path != null;
                for (int i = 0; i < this.m_info.m_paths.Length; i++)
                {
                    BuildingInfo.PathInfo pathInfo = this.m_info.m_paths[i];
                    if (pathInfo.m_netInfo.IsUndergroundMetroStationTrack())
                    {
                        if (!stationCustomizationHasPath || SetStationCustomizations.PathCustomization.Path == pathInfo)
                        {
                            theColor.r = 0;
                            theColor.b = 60;
                            theColor.g = 85;
                        }
                        else
                        {
                            theColor = color;
                        }
                    }

                    if (pathInfo.m_finalNetInfo != null && pathInfo.m_nodes != null && pathInfo.m_nodes.Length > 1 && pathInfo.m_finalNetInfo.m_overlayVisible)
                    {
                        for (int j = 1; j < pathInfo.m_nodes.Length; j++)
                        {
                            Bezier3 bezier = default(Bezier3);
                            bezier.a = Building.CalculatePosition(position, angle, pathInfo.m_nodes[j - 1]);
                            bezier.d = Building.CalculatePosition(position, angle, pathInfo.m_nodes[j]);
                            Vector3 startDir;
                            Vector3 endDir;
                            if (pathInfo.m_curveTargets != null && pathInfo.m_curveTargets.Length >= j)
                            {
                                Vector3 a = Building.CalculatePosition(position, angle, pathInfo.m_curveTargets[j - 1]);
                                startDir = VectorUtils.NormalizeXZ(a - bezier.a);
                                endDir = VectorUtils.NormalizeXZ(a - bezier.d);
                            }
                            else
                            {
                                startDir = VectorUtils.NormalizeXZ(bezier.d - bezier.a);
                                endDir = VectorUtils.NormalizeXZ(bezier.a - bezier.d);
                            }
                            NetSegment.CalculateMiddlePoints(bezier.a, startDir, bezier.d, endDir, true, true, out bezier.b, out bezier.c);
                            float cutStart = -100000f;
                            float cutEnd = -100000f;
                            ToolManager expr_182_cp_0 = Singleton<ToolManager>.instance;
                            expr_182_cp_0.m_drawCallData.m_overlayCalls = expr_182_cp_0.m_drawCallData.m_overlayCalls + 1;
                            Singleton<RenderManager>.instance.OverlayEffect.DrawBezier(cameraInfo, theColor, bezier, pathInfo.m_finalNetInfo.m_halfWidth * 2f, cutStart, cutEnd, position.y - 100f, position.y + 100f, false, false);
                        }
                    }
                }
            }
        }
    }
}
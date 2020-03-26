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
        [RedirectMethod]
        public override string GetLocalizedStats(ushort buildingID, ref Building data)
        {
            string text = string.Empty;
            if ((UnityEngine.Object)this.m_transportInfo != (UnityEngine.Object)null && this.m_maxVehicleCount != 0)
            {
                switch (this.m_transportInfo.m_transportType)
                {
                    case TransportInfo.TransportType.Bus:
                    case TransportInfo.TransportType.TouristBus:
                        {
                            int vehicleCount = this.GetVehicleCount(buildingID, ref data);
                            int budget = Singleton<EconomyManager>.instance.GetBudget(this.m_info.m_class);
                            int productionRate = PlayerBuildingAI.GetProductionRate(100, budget);
                            int num = (productionRate * this.m_maxVehicleCount + 99) / 100;
                            text += LocaleFormatter.FormatGeneric("AIINFO_BUSDEPOT_BUSCOUNT", new object[]
                            {
                                vehicleCount,
                                num
                            });
                            break;
                        }
                    case TransportInfo.TransportType.Ship:
                        if (this.m_transportInfo.m_vehicleType == VehicleInfo.VehicleType.Ferry)
                        {
                            int vehicleCount2 = this.GetVehicleCount(buildingID, ref data);
                            int budget2 = Singleton<EconomyManager>.instance.GetBudget(this.m_info.m_class);
                            int productionRate2 = PlayerBuildingAI.GetProductionRate(100, budget2);
                            int num2 = (productionRate2 * this.m_maxVehicleCount + 99) / 100;
                            text += LocaleFormatter.FormatGeneric("AIINFO_FERRYDEPOT_FERRYCOUNT", new object[]
                            {
                                vehicleCount2,
                                num2
                            });
                        }
                        break;
                    case TransportInfo.TransportType.Airplane:
                        if (this.m_transportInfo.m_vehicleType == VehicleInfo.VehicleType.Blimp)
                        {
                            int vehicleCount3 = this.GetVehicleCount(buildingID, ref data);
                            int budget3 = Singleton<EconomyManager>.instance.GetBudget(this.m_info.m_class);
                            int productionRate3 = PlayerBuildingAI.GetProductionRate(100, budget3);
                            int num3 = (productionRate3 * this.m_maxVehicleCount + 99) / 100;
                            text += LocaleFormatter.FormatGeneric("AIINFO_BLIMPDEPOT_BLIMPCOUNT", new object[]
                            {
                                vehicleCount3,
                                num3
                            });
                        }
                        break;
                    case TransportInfo.TransportType.Taxi:
                        {
                            int vehicleCount4 = this.GetVehicleCount(buildingID, ref data);
                            int budget4 = Singleton<EconomyManager>.instance.GetBudget(this.m_info.m_class);
                            int productionRate4 = PlayerBuildingAI.GetProductionRate(100, budget4);
                            int num4 = (productionRate4 * this.m_maxVehicleCount + 99) / 100;
                            text += LocaleFormatter.FormatGeneric("AIINFO_TAXIDEPOT_VEHICLES", new object[]
                            {
                                vehicleCount4,
                                num4
                            });
                            break;
                        }
                    case TransportInfo.TransportType.Tram:
                        {
                            int vehicleCount5 = this.GetVehicleCount(buildingID, ref data);
                            int budget5 = Singleton<EconomyManager>.instance.GetBudget(this.m_info.m_class);
                            int productionRate5 = PlayerBuildingAI.GetProductionRate(100, budget5);
                            int num5 = (productionRate5 * this.m_maxVehicleCount + 99) / 100;
                            text += LocaleFormatter.FormatGeneric("AIINFO_TRAMDEPOT_TRAMCOUNT", new object[]
                            {
                                vehicleCount5,
                                num5
                            });
                            break;
                        }
                    //begin mod
                    case TransportInfo.TransportType.Metro:
                        {
                            int vehicleCount6 = this.GetVehicleCount(buildingID, ref data);
                            int budget6 = Singleton<EconomyManager>.instance.GetBudget(this.m_info.m_class);
                            int productionRate6 = PlayerBuildingAI.GetProductionRate(100, budget6);
                            int num6 = (productionRate6 * this.m_maxVehicleCount + 99) / 100;
                            text += LocaleFormatter.FormatGeneric("AIINFO_TRAMDEPOT_TRAMCOUNT", new object[]
                            {
                                vehicleCount6,
                                num6
                            }).Replace("Tram", "Metro");
                            break;
                        }

                    case TransportInfo.TransportType.Train:
                        {
                            int vehicleCount7 = this.GetVehicleCount(buildingID, ref data);
                            int budget7 = Singleton<EconomyManager>.instance.GetBudget(this.m_info.m_class);
                            int productionRate7 = PlayerBuildingAI.GetProductionRate(100, budget7);
                            int num7 = (productionRate7 * this.m_maxVehicleCount + 99) / 100;
                            text += LocaleFormatter.FormatGeneric("AIINFO_TRAMDEPOT_TRAMCOUNT", new object[]
                            {
                                vehicleCount7,
                                num7
                            }).Replace("Tram", "Train");
                            break;
                            //end mod
                        }
                }
            }
            return text;
        }
    }
}
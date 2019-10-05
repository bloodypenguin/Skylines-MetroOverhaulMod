using ColossalFramework;
using MetroOverhaul.Redirection.Attributes;

namespace MetroOverhaul.Detours
{
    [TargetType(typeof(DepotAI))]
    public class DepotAIDetour : DepotAI
    {
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
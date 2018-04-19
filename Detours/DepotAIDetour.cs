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
            string empty = string.Empty;
            if ((UnityEngine.Object)this.m_transportInfo != (UnityEngine.Object)null && this.m_maxVehicleCount != 0)
            {
                switch (this.m_transportInfo.m_transportType)
                {
                    case TransportInfo.TransportType.Bus:
                        int vehicleCount1 = this.GetVehicleCount(buildingID, ref data);
                        int num1 = (PlayerBuildingAI.GetProductionRate(100, Singleton<EconomyManager>.instance.GetBudget(this.m_info.m_class)) * this.m_maxVehicleCount + 99) / 100;
                        empty += LocaleFormatter.FormatGeneric("AIINFO_BUSDEPOT_BUSCOUNT", (object)vehicleCount1, (object)num1);
                        break;
                    case TransportInfo.TransportType.Ship:
                        if (this.m_transportInfo.m_vehicleType == VehicleInfo.VehicleType.Ferry)
                        {
                            int vehicleCount2 = this.GetVehicleCount(buildingID, ref data);
                            int num2 = (PlayerBuildingAI.GetProductionRate(100, Singleton<EconomyManager>.instance.GetBudget(this.m_info.m_class)) * this.m_maxVehicleCount + 99) / 100;
                            empty += LocaleFormatter.FormatGeneric("AIINFO_FERRYDEPOT_FERRYCOUNT", (object)vehicleCount2, (object)num2);
                            break;
                        }
                        break;
                    case TransportInfo.TransportType.Airplane:
                        if (this.m_transportInfo.m_vehicleType == VehicleInfo.VehicleType.Blimp)
                        {
                            int vehicleCount2 = this.GetVehicleCount(buildingID, ref data);
                            int num2 = (PlayerBuildingAI.GetProductionRate(100, Singleton<EconomyManager>.instance.GetBudget(this.m_info.m_class)) * this.m_maxVehicleCount + 99) / 100;
                            empty += LocaleFormatter.FormatGeneric("AIINFO_BLIMPDEPOT_BLIMPCOUNT", (object)vehicleCount2, (object)num2);
                            break;
                        }
                        break;
                    case TransportInfo.TransportType.Taxi:
                        int vehicleCount3 = this.GetVehicleCount(buildingID, ref data);
                        int num3 = (PlayerBuildingAI.GetProductionRate(100, Singleton<EconomyManager>.instance.GetBudget(this.m_info.m_class)) * this.m_maxVehicleCount + 99) / 100;
                        empty += LocaleFormatter.FormatGeneric("AIINFO_TAXIDEPOT_VEHICLES", (object)vehicleCount3, (object)num3);
                        break;
                    case TransportInfo.TransportType.Tram:
                        int vehicleCount4 = this.GetVehicleCount(buildingID, ref data);
                        int num4 = (PlayerBuildingAI.GetProductionRate(100, Singleton<EconomyManager>.instance.GetBudget(this.m_info.m_class)) * this.m_maxVehicleCount + 99) / 100;
                        empty += LocaleFormatter.FormatGeneric("AIINFO_TRAMDEPOT_TRAMCOUNT", (object)vehicleCount4, (object)num4);
                        break;
                    //begin mod
                    case TransportInfo.TransportType.Metro:
                        int vehicleCount5 = this.GetVehicleCount(buildingID, ref data);
                        int num5 = (PlayerBuildingAI.GetProductionRate(100, Singleton<EconomyManager>.instance.GetBudget(this.m_info.m_class)) * this.m_maxVehicleCount + 99) / 100;
                        empty += $"Metro trains in use: {vehicleCount5}";
                        break;
                    case TransportInfo.TransportType.Train:
                        int vehicleCount6 = this.GetVehicleCount(buildingID, ref data);
                        int num6 = (PlayerBuildingAI.GetProductionRate(100, Singleton<EconomyManager>.instance.GetBudget(this.m_info.m_class)) * this.m_maxVehicleCount + 99) / 100;
                        empty += $"Trains in use: {vehicleCount6}";
                        break;
                        //end mod
                }
            }
            return empty;
        }
    }
}
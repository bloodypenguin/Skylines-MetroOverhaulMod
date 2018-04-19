using MetroOverhaul.Redirection.Attributes;
using System;
using Next;

namespace MetroOverhaul.Detours
{
    [TargetType(typeof(RoadsGroupPanel))]
    class RoadsGroupPanelDetour:RoadsGroupPanel
    {
        [RedirectMethod]
        protected override bool IsServiceValid(PrefabInfo info)
        {
            if (this.isMapEditor)
            {
                if (info.GetService() != this.service)
                    return info.GetService() == ItemClass.Service.PublicTransport;
                return true;
            }
            if (this.isAssetEditor)
            {
                BuildingInfo buildingInfo = info as BuildingInfo;
                if (info.GetService() == this.service && ((Object)buildingInfo == (Object)null || buildingInfo.m_buildingAI.WorksAsNet()) || info.GetService() == ItemClass.Service.PublicTransport && (info.GetSubService() == ItemClass.SubService.PublicTransportTrain || info.GetSubService() == ItemClass.SubService.PublicTransportMetro || info.GetSubService() == ItemClass.SubService.PublicTransportMonorail || info.GetSubService() == ItemClass.SubService.PublicTransportCableCar))
                    return true;
                if (info.GetService() == ItemClass.Service.Beautification && info is NetInfo)
                    return ToolsModifierControl.toolController.m_editPrefabInfo is NetInfo;
                return false;
            }
            NetInfo netInfo = info as NetInfo;
            if (info.GetService() != this.service)
                return false;
            if (!((Object)netInfo == (Object)null))
                return (netInfo.m_vehicleTypes & VehicleInfo.VehicleType.Car) != VehicleInfo.VehicleType.None;
            return true;
        }
    }
}

using ColossalFramework;
using ColossalFramework.Math;
using MetroOverhaul.Redirection;
using UnityEngine;

namespace MetroOverhaul.Detours
{
    [TargetType(typeof(PassengerTrainAI))]
    public class PassengerTrainAIDetour : PassengerTrainAI
    {

        public static void ChangeDeployState(bool state)
        {
            if (state)
            {
                Redirector<PassengerTrainAIDetour>.Deploy();
            }
            else
            {
                Redirector<PassengerTrainAIDetour>.Revert();
            }
        }

        [RedirectMethod]
        protected override bool StartPathFind(ushort vehicleID, ref Vehicle vehicleData)
        {
            if ((int)vehicleData.m_leadingVehicle == 0)
            {
                Vector3 startPos = (vehicleData.m_flags & Vehicle.Flags.Reversed) == ~Vehicle.Flags.All ? (Vector3)vehicleData.m_targetPos0 : (Vector3)Singleton<VehicleManager>.instance.m_vehicles.m_buffer[(int)vehicleData.GetLastVehicle(vehicleID)].m_targetPos0;
                if ((vehicleData.m_flags & Vehicle.Flags.GoingBack) != ~Vehicle.Flags.All)
                {
                    if ((int)vehicleData.m_sourceBuilding != 0)
                    {
                        //begin mod
                        BuildingManager instance = Singleton<BuildingManager>.instance;
                        BuildingInfo info = instance.m_buildings.m_buffer[(int)vehicleData.m_sourceBuilding].Info;
                        Randomizer randomizer = new Randomizer((int)vehicleID);
                        Vector3 position;
                        Vector3 target;
                        info.m_buildingAI.CalculateUnspawnPosition(vehicleData.m_sourceBuilding, ref instance.m_buildings.m_buffer[(int)vehicleData.m_sourceBuilding], ref randomizer, this.m_info, out position, out target);
                        return this.StartPathFind(vehicleID, ref vehicleData, startPos, position);
                        //end mod
                    }
                }
                else if ((vehicleData.m_flags & Vehicle.Flags.DummyTraffic) != ~Vehicle.Flags.All)
                {
                    if ((int)vehicleData.m_targetBuilding != 0)
                    {
                        Vector3 endPos = Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int)vehicleData.m_targetBuilding].m_position;
                        return this.StartPathFind(vehicleID, ref vehicleData, (Vector3)vehicleData.m_targetPos0, endPos);
                    }
                }
                else if ((int)vehicleData.m_targetBuilding != 0)
                {
                    Vector3 endPos = Singleton<NetManager>.instance.m_nodes.m_buffer[(int)vehicleData.m_targetBuilding].m_position;
                    return this.StartPathFind(vehicleID, ref vehicleData, startPos, endPos);
                }
            }
            return false;
        }
    }
}
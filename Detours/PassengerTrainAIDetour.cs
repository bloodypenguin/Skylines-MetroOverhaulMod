using ColossalFramework;
using ColossalFramework.Math;
using MetroOverhaul.Extensions;
using MetroOverhaul.Redirection;
using MetroOverhaul.Redirection.Attributes;
using UnityEngine;

namespace MetroOverhaul.Detours
{
    [TargetType(typeof(PassengerTrainAI))]
    public class PassengerTrainAIDetour : PassengerTrainAI
    {

        public static void ChangeDeployState(bool state)
        {
            if (!Util.IsGameMode())
            {
                return;
            }

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
                if (!vehicleData.TryGetStartPosition(vehicleID, out Vector3 startPos))
                    return false;

                if ((vehicleData.m_flags & Vehicle.Flags.GoingBack) != (Vehicle.Flags)0)
                {
                    if ((int)vehicleData.m_sourceBuilding != 0)
                    {
                        //begin mod
                        var instance = Singleton<BuildingManager>.instance;
                        if (!instance.m_buildings.TryGetFromBuffer(vehicleData.m_sourceBuilding, out Building sourceBuilding))
                            return false;

                        var randomizer = new Randomizer((int)vehicleID);
                        sourceBuilding.Info.m_buildingAI.CalculateUnspawnPosition(vehicleData.m_sourceBuilding, ref sourceBuilding, ref randomizer, this.m_info, out Vector3 position, out Vector3 target);
                        return this.StartPathFind(vehicleID, ref vehicleData, startPos, position);
                        //end mod
                    }
                }
                else if ((vehicleData.m_flags & Vehicle.Flags.DummyTraffic) != (Vehicle.Flags)0)
                {
                    if (vehicleData.m_targetBuilding != 0)
                    {
                        if (!Singleton<BuildingManager>.instance.m_buildings.TryGetFromBuffer(vehicleData.m_targetBuilding, out Building endBuilding))
                            return false;

                        var endPos = endBuilding.m_position;
                        return this.StartPathFind(vehicleID, ref vehicleData, vehicleData.m_targetPos0, endPos);
                    }
                }
                else if (vehicleData.m_targetBuilding != 0)
                {
                    if (!Singleton<NetManager>.instance.m_nodes.TryGetFromBuffer(vehicleData.m_targetBuilding, out NetNode endNode))
                        return false;

                    var endPos = endNode.m_position;
                    return this.StartPathFind(vehicleID, ref vehicleData, startPos, endPos);
                }
            }
            return false;
        }
    }
}
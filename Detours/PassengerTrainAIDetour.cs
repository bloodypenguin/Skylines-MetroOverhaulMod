using ColossalFramework;
using ColossalFramework.Math;
using MetroOverhaul.Redirection;
using MetroOverhaul.Redirection.Attributes;
using UnityEngine;
namespace MetroOverhaul.Detours
{
    [TargetType(typeof(PassengerTrainAI))]
    public class PassengerTrainAIDetour : TrainAI
    {
        public EffectInfo m_arriveEffect;
        public override void InitializeAI()
        {
            base.InitializeAI();
            if (!((UnityEngine.Object)this.m_arriveEffect != (UnityEngine.Object)null))
                return;
            this.m_arriveEffect.InitializeEffect();
        }
        public override void ReleaseAI()
        {
            if ((UnityEngine.Object)this.m_arriveEffect != (UnityEngine.Object)null)
                this.m_arriveEffect.ReleaseEffect();
            base.ReleaseAI();
        }
        public static void ChangeDeployState(bool state)
        {
            if (!Util.IsGameMode())
            {
                return;
            }
            if (state)
            {
                //Redirector<PassengerTrainAIDetour>.Deploy();
            }
            else
            {
                Redirector<PassengerTrainAIDetour>.Revert();
            }
        }

        [RedirectMethod]
        protected override bool StartPathFind(ushort vehicleID, ref Vehicle vehicleData)
        {
            BuildingManager instance = Singleton<BuildingManager>.instance;
            if (vehicleData.m_leadingVehicle == 0)
            {
                Vector3 startPos;
                if ((vehicleData.m_flags & Vehicle.Flags.Reversed) != (Vehicle.Flags)0)
                {
                    ushort lastVehicle = vehicleData.GetLastVehicle(vehicleID);
                    startPos = Singleton<VehicleManager>.instance.m_vehicles.m_buffer[(int)lastVehicle].m_targetPos0;
                }
                else
                {
                    startPos = vehicleData.m_targetPos0;
                }
                if ((vehicleData.m_flags & Vehicle.Flags.GoingBack) != (Vehicle.Flags)0)
                {
                    if (vehicleData.m_sourceBuilding != 0)
                    {
                        //begin mod//
                        BuildingInfo info = instance.m_buildings.m_buffer[(int)vehicleData.m_sourceBuilding].Info;
                        Randomizer randomizer = new Randomizer((int)vehicleID);
                        Vector3 position;
                        Vector3 target;
                        info.m_buildingAI.CalculateUnspawnPosition(vehicleData.m_sourceBuilding, ref instance.m_buildings.m_buffer[(int)vehicleData.m_sourceBuilding], ref randomizer, this.m_info, out position, out target);
                        return this.StartPathFind(vehicleID, ref vehicleData, startPos, position);
                        //end mod//
                    }
                }
                else if ((vehicleData.m_flags & Vehicle.Flags.DummyTraffic) != (Vehicle.Flags)0)
                {
                    if (vehicleData.m_targetBuilding != 0)
                    {
                        Vector3 position2 = Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int)vehicleData.m_targetBuilding].m_position;
                        return this.StartPathFind(vehicleID, ref vehicleData, vehicleData.m_targetPos0, position2);
                    }
                }
                else if (vehicleData.m_targetBuilding != 0)
                {
                    Vector3 position3 = Singleton<NetManager>.instance.m_nodes.m_buffer[(int)vehicleData.m_targetBuilding].m_position;
                    return this.StartPathFind(vehicleID, ref vehicleData, startPos, position3);
                }
            }
            return false;
        }
    }
}

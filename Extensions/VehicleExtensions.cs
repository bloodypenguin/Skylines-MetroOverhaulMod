using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColossalFramework;
using UnityEngine;

namespace MetroOverhaul.Extensions
{
    public static class VehicleExtensions
    {
        public static bool TryGetStartPosition(this Vehicle vehicleData, ushort vehicleId, out Vector3 startPosition)
        {
            if ((vehicleData.m_flags & Vehicle.Flags.Reversed) == (Vehicle.Flags)0)
            {
                startPosition = (Vector3)vehicleData.m_targetPos0;
                return true;
            }
            else
            {
                var instance = Singleton<VehicleManager>.instance;
                var lastVehicleIndex = (int)vehicleData.GetLastVehicle(vehicleId);

                if (!instance.m_vehicles.TryGetFromBuffer((uint)lastVehicleIndex, out Vehicle lastVehicle))
                {
                    startPosition = Vector3.zero;
                    return false;
                }

                startPosition = lastVehicle.m_targetPos0;
                return true;
            }
        }
    }
}

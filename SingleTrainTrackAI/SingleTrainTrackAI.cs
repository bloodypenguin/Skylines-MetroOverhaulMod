
using ColossalFramework;
using ColossalFramework.Math;
using UnityEngine;
using System;

using System.Collections.Generic;

namespace SingleTrainTrack
{

    struct ReservedSegment
    {
        public uint segment_id;
        public ushort vehicle_id;
    }

	public class SingleTrainTrackAI : VehicleAI
    {

        public static String MOD_NAME = "SingleTrainTrackAI";

        static List<ReservedSegment> reservedSegments = new List<ReservedSegment>();

        static List<ushort> vehiclesWithReservation = new List<ushort>();

        public static void Initialize()
        {
            reservedSegments.Clear();
            vehiclesWithReservation.Clear();
            CODebug.Log(LogChannel.Modding, MOD_NAME+" - reservation lists initialized");
        }

        static bool CheckAndAddReservedSegment(uint segment_id, ushort vehicle_id)
        {
            bool include = true;
            bool reserved = false;
            for (int i = 0; i < reservedSegments.Count; i++)
            {
                if (reservedSegments[i].segment_id == segment_id)
                {
                    if (reservedSegments[i].vehicle_id == 0)
                    {
                        ReservedSegment rs = reservedSegments[i];
                        rs.vehicle_id = vehicle_id;
                        reservedSegments[i] = rs;
                        //CODebug.Log(LogChannel.Modding, "changed segment " + segment_id + " for vehicle " + vehicle_id);
                        reserved = true;
                    }
                    include = false;
                    break;
                }
            }

            if(include)
            {
                ReservedSegment rs = new ReservedSegment();
                rs.segment_id = segment_id;
                rs.vehicle_id = vehicle_id;
                reservedSegments.Add(rs);
                //CODebug.Log(LogChannel.Modding, "added segment "+ segment_id+ " for vehicle "+ vehicle_id);
                reserved = true;
            }

            return reserved;
                
        }

        const int CANCEL_RESERVATION_AFTER = 50;

        static uint CheckReservation(uint segment_id)
        {
            for (int i = 0; i < reservedSegments.Count; i++)
            {
                if (reservedSegments[i].segment_id == segment_id)
                {
                    Vehicle v = Singleton<VehicleManager>.instance.m_vehicles.m_buffer[reservedSegments[i].vehicle_id];
                    if (v.GetLastFrameData().m_velocity.magnitude < 0.001f) //remove a vehicle invisble (or blocked for some reason) still having the reservation for the segments active
                    {
                        v.m_blockCounter++;
                        if (v.m_blockCounter > CANCEL_RESERVATION_AFTER) //wait for some time before cancelling reservation, as train could also be starting...
                        {
                            //CODebug.Log(LogChannel.Modding, "unspawn vehicle " + reservedSegments[i].vehicle_id);
                            RemoveReservationForVehicle(reservedSegments[i].vehicle_id);
                            Singleton<VehicleManager>.instance.ReleaseVehicle(reservedSegments[i].vehicle_id);
                            //v.Unspawn(reservedSegments[i].vehicle_id);
                            return 0;
                        }
                        Singleton<VehicleManager>.instance.m_vehicles.m_buffer[reservedSegments[i].vehicle_id] = v;
                    }


                    //CODebug.Log(LogChannel.Modding, "reserved for vehicle "+ reservedSegments[i].vehicle_id + " flags " + v.m_flags+" "+v.m_flags2+" velocity "+v.GetFrameData(Singleton<SimulationManager>.instance.m_currentFrameIndex).m_velocity+" "+v.m_blockCounter+" "+v.m_waitCounter);
                    //CODebug.Log(LogChannel.Modding, "vehicle cat " + v.Info.category);
                    return reservedSegments[i].vehicle_id;
                }
            }
            return 0;
        }

        static void RemoveReservationForVehicle(ushort vehicle_id)
        {
            //if (!vehiclesWithReservation.Contains(vehicle_id))
            //return;

            //vehiclesWithReservation.Remove(vehicle_id);
            int index = vehiclesWithReservation.IndexOf(vehicle_id);
            if(index != -1)
                vehiclesWithReservation[index] = 0;

            for (int i = reservedSegments.Count-1; i >= 0; i--)
            {
                if (reservedSegments[i].vehicle_id == vehicle_id)
                {
                    ReservedSegment rs = reservedSegments[i];
                    rs.vehicle_id = 0;
                    reservedSegments[i] = rs;
                    //reservedSegments.RemoveAt(i); //crash the game??
                }
            }
        }

        const string TARGET_RAIL_NAME = "Rail1L2W";

        static void ReserveSuccessiveSegments(uint segment_id, ushort vehicle_id)
        {
            int index = vehiclesWithReservation.IndexOf(0);
            if (index != -1)
                vehiclesWithReservation[index] = vehicle_id;
            else
                vehiclesWithReservation.Add(vehicle_id);

            NetManager instance = Singleton<NetManager>.instance;

            int nieghbours_count = 0;
            bool include_segments = false;
            List<uint> single_lanes = new List<uint>();
            
            NetNode node;
            NetSegment seg = instance.m_segments.m_buffer[segment_id];

            List<uint> nodes_included = new List<uint>();
            int n = 0;
            nodes_included.Add(seg.m_endNode);
            nodes_included.Add(seg.m_startNode);
            
            while (nodes_included.Count > n)
            {
                nieghbours_count = 0;
                single_lanes.Clear();
                include_segments = false;

                node = instance.m_nodes.m_buffer[(int)((UIntPtr)nodes_included[n])];

                //find every segments attached to node
                for (int i = 0; i < 8; i++)
                {
                    if (node.GetSegment(i) != 0)
                    {
                        nieghbours_count++;
                        if (instance.m_segments.m_buffer[(int)node.GetSegment(i)].Info.name.Contains(TARGET_RAIL_NAME)) //detect 1 lane 2 ways segments
                            single_lanes.Add(node.GetSegment(i));
                    }
                }

                if (nieghbours_count <= 2 && single_lanes.Count > 0) //include single track segments without branching
                    include_segments = true;
                else if (nieghbours_count > 2) //include single track segments with branching. All single track 2 ways segments connected together will get booked
                    include_segments = true;

                if (include_segments)
                {
                    for (int i = 0; i < single_lanes.Count; i++)
                    {
                        if (CheckAndAddReservedSegment(single_lanes[i], vehicle_id)) //if segment gets reserved by this vehicle, look for further segments on the track
                        {
                            seg = instance.m_segments.m_buffer[(int)single_lanes[i]];
                            if (!nodes_included.Contains(seg.m_endNode))
                                nodes_included.Add(seg.m_endNode);
                            if (!nodes_included.Contains(seg.m_startNode))
                                nodes_included.Add(seg.m_startNode);
                        }
                    }
                }

                //CODebug.Log(LogChannel.Modding, "nodes to inspect " + nodes_included.Count + " reserved list " + reservedSegments.Count);
                n++;
            }
        }

        private bool CheckSingleTrack2Ways(ushort vehicleID, ref float maxSpeed, uint laneID, uint prevLaneID)
        {
            NetManager instance = Singleton<NetManager>.instance;
            NetInfo info = instance.m_segments.m_buffer[(int)instance.m_lanes.m_buffer[(int)((UIntPtr)laneID)].m_segment].Info;
            NetInfo info_crt = instance.m_segments.m_buffer[(int)instance.m_lanes.m_buffer[(int)((UIntPtr)prevLaneID)].m_segment].Info;

            //CODebug.Log(LogChannel.Modding, MOD_NAME + " - check track with name " + info.name+" "+ info_crt.name);

            if (!info_crt.name.Contains(TARGET_RAIL_NAME) && vehiclesWithReservation.Contains(vehicleID)) //remove any reservation once the train has left the one lane track section
            {
                RemoveReservationForVehicle(vehicleID);
                //CODebug.Log(LogChannel.Modding, MOD_NAME + " - cleaned list of " + reservedSegments.Count + " reservations for vehicle " + vehicleID);
            }
            if (info.name.Contains(TARGET_RAIL_NAME)) //train has entered a one lane section
            {
                uint reserved_for_vehicle = CheckReservation(instance.m_lanes.m_buffer[(int)((UIntPtr)laneID)].m_segment);
                if (reserved_for_vehicle == 0) //reserve track if it is not reserved by another train
                {
                    //CODebug.Log(LogChannel.Modding, MOD_NAME+" - started reserving for vehicle " + vehicleID);

                    ReserveSuccessiveSegments(instance.m_lanes.m_buffer[(int)((UIntPtr)laneID)].m_segment, vehicleID);
                }
                else if (reserved_for_vehicle != vehicleID) //not allowed on this track, stop
                {
                    //CODebug.Log(LogChannel.Modding, MOD_NAME + " - vehicle " + vehicleID + " cannot proceed because track reserved by "+ reserved_for_vehicle);

                    maxSpeed = 0f;
                    return true;
                }
            }

            return false;
        }

        private void CheckNextLane(ushort vehicleID, ref Vehicle vehicleData, ref float maxSpeed, PathUnit.Position position, uint laneID, byte offset, PathUnit.Position prevPos, uint prevLaneID, byte prevOffset, Bezier3 bezier)
		{
            //code added for single track 2 ways
            if (CheckSingleTrack2Ways(vehicleID, ref maxSpeed, laneID, prevLaneID))
                return;

            NetManager instance = Singleton<NetManager>.instance;
            Vehicle.Frame lastFrameData = vehicleData.GetLastFrameData();
			Vector3 a = lastFrameData.m_position;
			Vector3 a2 = lastFrameData.m_position;
			Vector3 b = lastFrameData.m_rotation * new Vector3(0f, 0f, this.m_info.m_generatedInfo.m_wheelBase * 0.5f);
			a += b;
			a2 -= b;
			float num = 0.5f * lastFrameData.m_velocity.sqrMagnitude / this.m_info.m_braking;
			float a3 = Vector3.Distance(a, bezier.a);
			float b2 = Vector3.Distance(a2, bezier.a);
			if (Mathf.Min(a3, b2) >= num - 5f)
			{
				if (!instance.m_lanes.m_buffer[(int)((UIntPtr)laneID)].CheckSpace(1000f, vehicleID))
				{
					maxSpeed = 0f;
					return;
				}
				Vector3 vector = bezier.Position(0.5f);
				Segment3 segment;
				if (Vector3.SqrMagnitude(vehicleData.m_segment.a - vector) < Vector3.SqrMagnitude(bezier.a - vector))
				{
					segment = new Segment3(vehicleData.m_segment.a, vector);
				}
				else
				{
					segment = new Segment3(bezier.a, vector);
				}
				if (segment.LengthSqr() >= 3f)
				{
					segment.a += (segment.b - segment.a).normalized * 2.5f;
                    if (SingleTrainTrackAI.CheckOverlap(vehicleID, ref vehicleData, segment, vehicleID))
					{
						maxSpeed = 0f;
						return;
					}
				}
				segment = new Segment3(vector, bezier.d);
                if (segment.LengthSqr() >= 1f && SingleTrainTrackAI.CheckOverlap(vehicleID, ref vehicleData, segment, vehicleID))
				{
					maxSpeed = 0f;
					return;
				}
				ushort num2;
				if (offset < position.m_offset)
				{
					num2 = instance.m_segments.m_buffer[(int)position.m_segment].m_startNode;
				}
				else
				{
					num2 = instance.m_segments.m_buffer[(int)position.m_segment].m_endNode;
				}
				ushort num3;
				if (prevOffset == 0)
				{
					num3 = instance.m_segments.m_buffer[(int)prevPos.m_segment].m_startNode;
				}
				else
				{
					num3 = instance.m_segments.m_buffer[(int)prevPos.m_segment].m_endNode;
				}
				if (num2 == num3)
				{
					NetNode.Flags flags = instance.m_nodes.m_buffer[(int)num2].m_flags;
					if ((flags & NetNode.Flags.TrafficLights) != NetNode.Flags.None)
					{
						uint currentFrameIndex = Singleton<SimulationManager>.instance.m_currentFrameIndex;
						uint num4 = (uint)(((int)num3 << 8) / 32768);
						uint num5 = currentFrameIndex - num4 & 255u;
						RoadBaseAI.TrafficLightState vehicleLightState;
						RoadBaseAI.TrafficLightState pedestrianLightState;
						bool flag;
						bool pedestrians;
						RoadBaseAI.GetTrafficLightState(num3, ref instance.m_segments.m_buffer[(int)prevPos.m_segment], currentFrameIndex - num4, out vehicleLightState, out pedestrianLightState, out flag, out pedestrians);
						if (!flag && num5 >= 196u)
						{
							flag = true;
							RoadBaseAI.SetTrafficLightState(num3, ref instance.m_segments.m_buffer[(int)prevPos.m_segment], currentFrameIndex - num4, vehicleLightState, pedestrianLightState, flag, pedestrians);
						}
						switch (vehicleLightState)
						{
						case RoadBaseAI.TrafficLightState.RedToGreen:
							if (num5 < 60u)
							{
								maxSpeed = 0f;
								return;
							}
							break;
						case RoadBaseAI.TrafficLightState.Red:
							maxSpeed = 0f;
							return;
						case RoadBaseAI.TrafficLightState.GreenToRed:
							if (num5 >= 30u)
							{
								maxSpeed = 0f;
								return;
							}
							break;
						}
					}
				}
			}
		}

		private static bool CheckOverlap(ushort vehicleID, ref Vehicle vehicleData, Segment3 segment, ushort ignoreVehicle)
		{
			VehicleManager instance = Singleton<VehicleManager>.instance;
			Vector3 min = segment.Min();
			Vector3 max = segment.Max();
			int num = Mathf.Max((int)((min.x - 30f) / 32f + 270f), 0);
			int num2 = Mathf.Max((int)((min.z - 30f) / 32f + 270f), 0);
			int num3 = Mathf.Min((int)((max.x + 30f) / 32f + 270f), 539);
			int num4 = Mathf.Min((int)((max.z + 30f) / 32f + 270f), 539);
			bool result = false;
			for (int i = num2; i <= num4; i++)
			{
				for (int j = num; j <= num3; j++)
				{
					ushort num5 = instance.m_vehicleGrid[i * 540 + j];
					int num6 = 0;
					while (num5 != 0)
					{
                        num5 = SingleTrainTrackAI.CheckOverlap(vehicleID, ref vehicleData, segment, ignoreVehicle, num5, ref instance.m_vehicles.m_buffer[(int)num5], ref result, min, max);
						if (++num6 > 16384)
						{
							CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
							break;
						}
					}
				}
			}
			return result;
		}

		private static ushort CheckOverlap(ushort vehicleID, ref Vehicle vehicleData, Segment3 segment, ushort ignoreVehicle, ushort otherID, ref Vehicle otherData, ref bool overlap, Vector3 min, Vector3 max)
		{
			if (ignoreVehicle == 0 || (otherID != ignoreVehicle && otherData.m_leadingVehicle != ignoreVehicle && otherData.m_trailingVehicle != ignoreVehicle))
			{
				VehicleInfo info = otherData.Info;
				if (info.m_vehicleType == VehicleInfo.VehicleType.Bicycle)
				{
					return otherData.m_nextGridVehicle;
				}
				if (((vehicleData.m_flags | otherData.m_flags) & Vehicle.Flags.Transition) == (Vehicle.Flags)0 && (vehicleData.m_flags & Vehicle.Flags.Underground) != (otherData.m_flags & Vehicle.Flags.Underground))
				{
					return otherData.m_nextGridVehicle;
				}
				Vector3 vector = Vector3.Min(otherData.m_segment.Min(), otherData.m_targetPos3);
				Vector3 vector2 = Vector3.Max(otherData.m_segment.Max(), otherData.m_targetPos3);
				if (min.x < vector2.x + 2f && min.y < vector2.y + 2f && min.z < vector2.z + 2f && vector.x < max.x + 2f && vector.y < max.y + 2f && vector.z < max.z + 2f)
				{
					Vector3 rhs = Vector3.Normalize(segment.b - segment.a);
					Vector3 lhs = otherData.m_segment.a - vehicleData.m_segment.b;
					Vector3 lhs2 = otherData.m_segment.b - vehicleData.m_segment.b;
					if (Vector3.Dot(lhs, rhs) >= 1f || Vector3.Dot(lhs2, rhs) >= 1f)
					{
						float num2;
						float num3;
						float num = segment.DistanceSqr(otherData.m_segment, out num2, out num3);
						if (num < 4f)
						{
							overlap = true;
						}
						Vector3 a = otherData.m_segment.b;
						segment.a.y = segment.a.y * 0.5f;
						segment.b.y = segment.b.y * 0.5f;
						for (int i = 0; i < 4; i++)
						{
							Vector3 vector3 = otherData.GetTargetPos(i);
							Segment3 segment2 = new Segment3(a, vector3);
							segment2.a.y = segment2.a.y * 0.5f;
							segment2.b.y = segment2.b.y * 0.5f;
							if (segment2.LengthSqr() > 0.01f)
							{
								num = segment.DistanceSqr(segment2, out num2, out num3);
								if (num < 4f)
								{
									overlap = true;
									break;
								}
							}
							a = vector3;
						}
					}
				}
			}
			return otherData.m_nextGridVehicle;
		}



    }

}
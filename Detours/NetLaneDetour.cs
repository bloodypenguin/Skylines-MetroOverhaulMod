using System;
using System.Linq;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.Math;
using NetworkSkins.Data;
using NetworkSkins.Detour;
using UnityEngine;
using MetroOverhaul.Redirection;

namespace NetworkSkins.Props
{
    public struct NetLaneDetour
    {
        private static bool deployed = false;

        private static RedirectCallsState _NetLane_RefreshInstance_state;
        private static MethodInfo _NetLane_RefreshInstance_original;
        private static MethodInfo _NetLane_RefreshInstance_detour;

        private static RedirectCallsState _NetLane_RenderInstance_state;
        private static MethodInfo _NetLane_RenderInstance_original;
        private static MethodInfo _NetLane_RenderInstance_detour;

        private static RedirectCallsState _NetLane_CalculateGroupData_state;
        private static MethodInfo _NetLane_CalculateGroupData_original;
        private static MethodInfo _NetLane_CalculateGroupData_detour;

        private static RedirectCallsState _NetLane_PopulateGroupData_state;
        private static MethodInfo _NetLane_PopulateGroupData_original;
        private static MethodInfo _NetLane_PopulateGroupData_detour;

        public static void Deploy()
        {
            if (!deployed)
            {
                // Called when placing/removing roads
                // one of these also affects the combined LOD mesh
                // NetLane.CalculateGroupData
                // NetLane.PopulateGroupData
                // NetLane.RereshInstance - init/render

                // Called when rendering detailed net
                // NetLane.RenderInstance - render

                _NetLane_RefreshInstance_original = typeof(NetLane).GetMethod("RefreshInstance", BindingFlags.Instance | BindingFlags.Public);
                _NetLane_RefreshInstance_detour = typeof(NetLaneDetour).GetMethod("RefreshInstance", BindingFlags.Instance | BindingFlags.Public);
                _NetLane_RefreshInstance_state = RedirectionHelper.RedirectCalls(_NetLane_RefreshInstance_original, _NetLane_RefreshInstance_detour);

                _NetLane_RenderInstance_original = typeof(NetLane).GetMethod("RenderInstance", BindingFlags.Instance | BindingFlags.Public);
                _NetLane_RenderInstance_detour = typeof(NetLaneDetour).GetMethod("RenderInstance", BindingFlags.Instance | BindingFlags.Public);
                _NetLane_RenderInstance_state = RedirectionHelper.RedirectCalls(_NetLane_RenderInstance_original, _NetLane_RenderInstance_detour);

                _NetLane_CalculateGroupData_original = typeof(NetLane).GetMethod("CalculateGroupData", BindingFlags.Instance | BindingFlags.Public);
                _NetLane_CalculateGroupData_detour = typeof(NetLaneDetour).GetMethod("CalculateGroupData", BindingFlags.Instance | BindingFlags.Public);
                _NetLane_CalculateGroupData_state = RedirectionHelper.RedirectCalls(_NetLane_CalculateGroupData_original, _NetLane_CalculateGroupData_detour);

                _NetLane_PopulateGroupData_original = typeof(NetLane).GetMethod("PopulateGroupData", BindingFlags.Instance | BindingFlags.Public);
                _NetLane_PopulateGroupData_detour = typeof(NetLaneDetour).GetMethod("PopulateGroupData", BindingFlags.Instance | BindingFlags.Public);
                _NetLane_PopulateGroupData_state = RedirectionHelper.RedirectCalls(_NetLane_PopulateGroupData_original, _NetLane_PopulateGroupData_detour);

                deployed = true;
            }
        }

        public static void Revert()
        {
            if (deployed)
            {
                RedirectionHelper.RevertRedirect(_NetLane_RefreshInstance_original, _NetLane_RefreshInstance_state);
                _NetLane_RefreshInstance_original = null;
                _NetLane_RefreshInstance_detour = null;

                RedirectionHelper.RevertRedirect(_NetLane_RenderInstance_original, _NetLane_RenderInstance_state);
                _NetLane_RenderInstance_original = null;
                _NetLane_RenderInstance_detour = null;

                RedirectionHelper.RevertRedirect(_NetLane_CalculateGroupData_original, _NetLane_CalculateGroupData_state);
                _NetLane_CalculateGroupData_original = null;
                _NetLane_CalculateGroupData_detour = null;

                RedirectionHelper.RevertRedirect(_NetLane_PopulateGroupData_original, _NetLane_PopulateGroupData_state);
                _NetLane_PopulateGroupData_original = null;
                _NetLane_PopulateGroupData_detour = null;

                deployed = false;
            }
        }

        /// <summary>
        /// Not called ingame.
        /// </summary>
        /// <param name="laneID"></param>
        /// <param name="laneInfo"></param>
        /// <param name="startAngle"></param>
        /// <param name="endAngle"></param>
        /// <param name="invert"></param>
        /// <param name="data"></param>
        /// <param name="propIndex"></param>
        public void RefreshInstance(uint laneID, NetInfo.Lane laneInfo, float startAngle, float endAngle, bool invert, ref RenderManager.Instance data, ref int propIndex)
        {
            var laneProps = laneInfo.m_laneProps;
            if (laneProps != null && laneProps.m_props != null)
            {
                var flag = (byte)(laneInfo.m_finalDirection & NetInfo.Direction.Both) == 2 || (byte)(laneInfo.m_finalDirection & NetInfo.Direction.Avoid) == 11;
                var flag2 = flag != invert;
                var num = laneProps.m_props.Length;

                // mod begin
                var _this = NetManager.instance.m_lanes.m_buffer[laneID];
               // var PillarPrefabDataIndices = PropCustomizer.Instance.PillarPrefabDataIndices;
                var segmentData = SegmentDataManager.Instance.SegmentToSegmentDataMap?[_this.m_segment];
                // mod end

                for (var i = 0; i < num; i++)
                {
                    var prop = laneProps.m_props[i];
                    if (_this.m_length >= prop.m_minLength)
                    {
                        // mod begin
                        var finalProp = prop.m_finalProp;
                        var finalTree = prop.m_finalTree;
                        var repeatDistance = prop.m_repeatDistance;
                        if (segmentData != null)
                        {
                            // custom street lights
                            if (finalProp != null)
                            {
                                var customPillar = (segmentData.Features & SegmentData.FeatureFlags.StreetLight) != 0;

                                // Contains seems to be faster than array lookup
                                if ((customPillar || segmentData.RepeatDistances.magnitude > 0f))// && PillarPrefabDataIndices.Contains(finalProp.m_prefabDataIndex))
                                {
                                    if (customPillar)
                                    {
                                        finalProp = segmentData.PillarPrefab;
                                    }
                                    if (segmentData.RepeatDistances.w > 0f)
                                    {
                                        repeatDistance = segmentData.RepeatDistances.w;
                                    }
                                }
                            }

                            // custom road trees
                            else if (finalTree != null)
                            {
                                if (laneInfo.m_position < 0) // Left Trees
                                {
                                    if ((segmentData.Features & SegmentData.FeatureFlags.TreeLeft) != 0)
                                    {
                                        finalTree = segmentData.TreeLeftPrefab;
                                    }
                                    if (segmentData.RepeatDistances.x > 0f)
                                    {
                                        repeatDistance = segmentData.RepeatDistances.x;
                                    }
                                }
                                else if (laneInfo.m_position == 0) // Middle Trees
                                {
                                    if ((segmentData.Features & SegmentData.FeatureFlags.TreeMiddle) != 0)
                                    {
                                        finalTree = segmentData.TreeMiddlePrefab;
                                    }
                                    if (segmentData.RepeatDistances.y > 0f)
                                    {
                                        repeatDistance = segmentData.RepeatDistances.y;
                                    }
                                }
                                else // Right Trees
                                {
                                    if ((segmentData.Features & SegmentData.FeatureFlags.TreeRight) != 0)
                                    {
                                        finalTree = segmentData.TreeRightPrefab;
                                    }
                                    if (segmentData.RepeatDistances.z > 0f)
                                    {
                                        repeatDistance = segmentData.RepeatDistances.z;
                                    }
                                }
                            }
                        }
                        // mod end

                        var num2 = 2;
                        if (repeatDistance > 1f)
                        {
                            num2 *= Mathf.Max(1, Mathf.RoundToInt(_this.m_length / repeatDistance));
                        }
                        var num3 = propIndex;
                        propIndex = num3 + (num2 + 1 >> 1);
                        var num4 = prop.m_segmentOffset * 0.5f;
                        if (_this.m_length != 0f)
                        {
                            num4 = Mathf.Clamp(num4 + prop.m_position.z / _this.m_length, -0.5f, 0.5f);
                        }
                        if (flag2)
                        {
                            num4 = -num4;
                        }

                        if (finalProp != null)
                        {
                            var randomizer = new Randomizer((int)(laneID + (uint)i));
                            for (var j = 1; j <= num2; j += 2)
                            {
                                if (randomizer.Int32(100u) < prop.m_probability)
                                {
                                    var num5 = num4 + (float)j / (float)num2;
                                    var variation = finalProp.GetVariation(ref randomizer);
                                    randomizer.Int32(10000u);
                                    if (prop.m_colorMode == NetLaneProps.ColorMode.Default)
                                    {
                                        variation.GetColor(ref randomizer);
                                    }
                                    var worldPos = _this.m_bezier.Position(num5);
                                    var vector = _this.m_bezier.Tangent(num5);
                                    if (vector != Vector3.zero)
                                    {
                                        if (flag2)
                                        {
                                            vector = -vector;
                                        }
                                        vector.y = 0f;
                                        if (prop.m_position.x != 0f)
                                        {
                                            vector = Vector3.Normalize(vector);
                                            worldPos.x += vector.z * prop.m_position.x;
                                            worldPos.z -= vector.x * prop.m_position.x;
                                        }
                                        var num6 = Mathf.Atan2(vector.x, -vector.z);
                                        if (prop.m_cornerAngle != 0f || prop.m_position.x != 0f)
                                        {
                                            var num7 = endAngle - startAngle;
                                            if (num7 > 3.14159274f)
                                            {
                                                num7 -= 6.28318548f;
                                            }
                                            if (num7 < -3.14159274f)
                                            {
                                                num7 += 6.28318548f;
                                            }
                                            var num8 = startAngle + num7 * num5;
                                            num7 = num8 - num6;
                                            if (num7 > 3.14159274f)
                                            {
                                                num7 -= 6.28318548f;
                                            }
                                            if (num7 < -3.14159274f)
                                            {
                                                num7 += 6.28318548f;
                                            }
                                            num6 += num7 * prop.m_cornerAngle;
                                            if (num7 != 0f && prop.m_position.x != 0f)
                                            {
                                                var num9 = Mathf.Tan(num7);
                                                worldPos.x += vector.x * num9 * prop.m_position.x;
                                                worldPos.z += vector.z * num9 * prop.m_position.x;
                                            }
                                        }
                                        worldPos.y = Singleton<TerrainManager>.instance.SampleDetailHeight(worldPos);
                                        data.m_extraData.SetUShort(num3++, (ushort)Mathf.Clamp(Mathf.RoundToInt(worldPos.y * 64f), 0, 65535));
                                    }
                                }
                            }
                        }

                        if (finalTree != null)
                        {
                            var randomizer2 = new Randomizer((int)(laneID + (uint)i));
                            for (var k = 1; k <= num2; k += 2)
                            {
                                if (randomizer2.Int32(100u) < prop.m_probability)
                                {
                                    var t = num4 + (float)k / (float)num2;
                                    finalTree.GetVariation(ref randomizer2);
                                    randomizer2.Int32(10000u);
                                    randomizer2.Int32(10000u);
                                    var worldPos2 = _this.m_bezier.Position(t);
                                    worldPos2.y += prop.m_position.y;
                                    if (prop.m_position.x != 0f)
                                    {
                                        var vector2 = _this.m_bezier.Tangent(t);
                                        if (flag2)
                                        {
                                            vector2 = -vector2;
                                        }
                                        vector2.y = 0f;
                                        vector2 = Vector3.Normalize(vector2);
                                        worldPos2.x += vector2.z * prop.m_position.x;
                                        worldPos2.z -= vector2.x * prop.m_position.x;
                                    }
                                    worldPos2.y = Singleton<TerrainManager>.instance.SampleDetailHeight(worldPos2);
                                    data.m_extraData.SetUShort(num3++, (ushort)Mathf.Clamp(Mathf.RoundToInt(worldPos2.y * 64f), 0, 65535));
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Called 1000s of times every frame! Must be high-performant!
        /// Not called when viewing from distance.
        /// </summary>
        /// <param name="cameraInfo"></param>
        /// <param name="segmentID"></param>
        /// <param name="laneID"></param>
        /// <param name="laneInfo"></param>
        /// <param name="startFlags"></param>
        /// <param name="endFlags"></param>
        /// <param name="startColor"></param>
        /// <param name="endColor"></param>
        /// <param name="startAngle"></param>
        /// <param name="endAngle"></param>
        /// <param name="invert"></param>
        /// <param name="layerMask"></param>
        /// <param name="objectIndex1"></param>
        /// <param name="objectIndex2"></param>
        /// <param name="data"></param>
        /// <param name="propIndex"></param>
        public void RenderInstance(RenderManager.CameraInfo cameraInfo, ushort segmentID, uint laneID, NetInfo.Lane laneInfo, NetNode.Flags startFlags, NetNode.Flags endFlags, Color startColor, Color endColor, float startAngle, float endAngle, bool invert, int layerMask, Vector4 objectIndex1, Vector4 objectIndex2, ref RenderManager.Instance data, ref int propIndex)
        { 
            var laneProps = laneInfo.m_laneProps;
            if (laneProps != null && laneProps.m_props != null)
            {
                var flag = (byte)(laneInfo.m_finalDirection & NetInfo.Direction.Both) == 2 || (byte)(laneInfo.m_finalDirection & NetInfo.Direction.Avoid) == 11;
                var flag2 = flag != invert;
                if (flag)
                {
                    var flags = startFlags;
                    startFlags = endFlags;
                    endFlags = flags;
                }
                var num = laneProps.m_props.Length;

                // mod begin
                var _this = NetManager.instance.m_lanes.m_buffer[laneID];
                var PillarPrefabDataIndices = PropCustomizer.Instance.PillarPrefabDataIndices;
                var segmentData = SegmentDataManager.Instance.SegmentToSegmentDataMap?[_this.m_segment];
                // mod end

                for (var i = 0; i < num; i++)
                {
                    var prop = laneProps.m_props[i];
                    if (_this.m_length >= prop.m_minLength)
                    {
                        // mod begin
                        var prop_m_angle = prop.m_angle;
                        var finalProp = prop.m_finalProp;
                        var finalTree = prop.m_finalTree;
                        var repeatDistance = prop.m_repeatDistance;
                        if (segmentData != null)
                        {
                            // custom street lights
                            if (finalProp != null)
                            {
                                var customLight = (segmentData.Features & SegmentData.FeatureFlags.StreetLight) != 0;

                                // Contains seems to be faster than array lookup
                                if ((customLight || segmentData.RepeatDistances.magnitude > 0f) && PillarPrefabDataIndices.Contains(finalProp.m_prefabDataIndex))
                                {
                                    if (customLight)
                                    {
                                        finalProp = segmentData.PillarPrefab;
                                        if (laneInfo.m_position + prop.m_position.x < 0f) prop_m_angle = 180; //rotate street lights on pedestrian paths correctly
                                    }
                                    if (segmentData.RepeatDistances.w > 0f)
                                    {
                                        repeatDistance = segmentData.RepeatDistances.w;
                                    }
                                }
                            }

                            // custom road trees
                            else if (finalTree != null)
                            {
                                if (laneInfo.m_position < 0) // Left Trees
                                {
                                    if ((segmentData.Features & SegmentData.FeatureFlags.TreeLeft) != 0)
                                    {
                                        finalTree = segmentData.TreeLeftPrefab;
                                    }
                                    if (segmentData.RepeatDistances.x > 0f)
                                    {
                                        repeatDistance = segmentData.RepeatDistances.x;
                                    }
                                }
                                else if (laneInfo.m_position == 0) // Middle Trees
                                {
                                    if ((segmentData.Features & SegmentData.FeatureFlags.TreeMiddle) != 0)
                                    {
                                        finalTree = segmentData.TreeMiddlePrefab;
                                    }
                                    if (segmentData.RepeatDistances.y > 0f)
                                    {
                                        repeatDistance = segmentData.RepeatDistances.y;
                                    }
                                }
                                else // Right Trees
                                {
                                    if ((segmentData.Features & SegmentData.FeatureFlags.TreeRight) != 0)
                                    {
                                        finalTree = segmentData.TreeRightPrefab;
                                    }
                                    if (segmentData.RepeatDistances.z > 0f)
                                    {
                                        repeatDistance = segmentData.RepeatDistances.z;
                                    }
                                }
                            }
                        }
                        // mod end

                        var num2 = 2;
                        if (repeatDistance > 1f)
                        {
                            num2 *= Mathf.Max(1, Mathf.RoundToInt(_this.m_length / repeatDistance));
                        }
                        var num3 = propIndex;
                        if (propIndex != -1)
                        {
                            propIndex = num3 + (num2 + 1 >> 1);
                        }
                        if (prop.CheckFlags((NetLane.Flags)_this.m_flags, startFlags, endFlags))
                        {
                            var num4 = prop.m_segmentOffset * 0.5f;
                            if (_this.m_length != 0f)
                            {
                                num4 = Mathf.Clamp(num4 + prop.m_position.z / _this.m_length, -0.5f, 0.5f);
                            }
                            if (flag2)
                            {
                                num4 = -num4;
                            }

                            if (finalProp != null && (layerMask & 1 << finalProp.m_prefabDataLayer) != 0)
                            {
                                var color = (prop.m_colorMode != NetLaneProps.ColorMode.EndState) ? startColor : endColor;
                                var randomizer = new Randomizer((int)(laneID + (uint)i));
                                for (var j = 1; j <= num2; j += 2)
                                {
                                    if (randomizer.Int32(100u) < prop.m_probability)
                                    {
                                        var num5 = num4 + (float)j / (float)num2;
                                        var variation = finalProp.GetVariation(ref randomizer);
                                        var scale = variation.m_minScale + (float)randomizer.Int32(10000u) * (variation.m_maxScale - variation.m_minScale) * 0.0001f;
                                        if (prop.m_colorMode == NetLaneProps.ColorMode.Default)
                                        {
                                            color = variation.GetColor(ref randomizer);
                                        }
                                        var vector = _this.m_bezier.Position(num5);
                                        if (propIndex != -1)
                                        {
                                            vector.y = (float)data.m_extraData.GetUShort(num3++) * 0.015625f;
                                        }
                                        vector.y += prop.m_position.y;
                                        if (cameraInfo.CheckRenderDistance(vector, variation.m_maxRenderDistance))
                                        {
                                            var vector2 = _this.m_bezier.Tangent(num5);
                                            if (vector2 != Vector3.zero)
                                            {
                                                if (flag2)
                                                {
                                                    vector2 = -vector2;
                                                }
                                                vector2.y = 0f;
                                                if (prop.m_position.x != 0f)
                                                {
                                                    vector2 = Vector3.Normalize(vector2);
                                                    vector.x += vector2.z * prop.m_position.x;
                                                    vector.z -= vector2.x * prop.m_position.x;
                                                }
                                                var num6 = Mathf.Atan2(vector2.x, -vector2.z);
                                                if (prop.m_cornerAngle != 0f || prop.m_position.x != 0f)
                                                {
                                                    var num7 = endAngle - startAngle;
                                                    if (num7 > 3.14159274f)
                                                    {
                                                        num7 -= 6.28318548f;
                                                    }
                                                    if (num7 < -3.14159274f)
                                                    {
                                                        num7 += 6.28318548f;
                                                    }
                                                    var num8 = startAngle + num7 * num5;
                                                    num7 = num8 - num6;
                                                    if (num7 > 3.14159274f)
                                                    {
                                                        num7 -= 6.28318548f;
                                                    }
                                                    if (num7 < -3.14159274f)
                                                    {
                                                        num7 += 6.28318548f;
                                                    }
                                                    num6 += num7 * prop.m_cornerAngle;
                                                    if (num7 != 0f && prop.m_position.x != 0f)
                                                    {
                                                        var num9 = Mathf.Tan(num7);
                                                        vector.x += vector2.x * num9 * prop.m_position.x;
                                                        vector.z += vector2.z * num9 * prop.m_position.x;
                                                    }
                                                }
                                                Vector4 objectIndex3 = (num5 <= 0.5f) ? objectIndex1 : objectIndex2;
                                                num6 += prop_m_angle * 0.0174532924f;

                                                PropInstance.RenderInstance(cameraInfo, variation, new InstanceID
                                                {
                                                    NetSegment = segmentID
                                                }, vector, scale, num6, color, objectIndex3, true);
                                                /*
                                                catch // debug code
                                                {
                                                    throw new Exception($"DEBUG: Prop Rendering Error\n" +
                                                              $"Net Name: {NetManager.instance.m_segments.m_buffer[segmentID].Info?.name}\n" +
                                                              $"Prop Name: {finalProp?.name}\n" +
                                                              $"Variation Name: {variation?.name}\n" +
                                                              $"Camera exists? {cameraInfo != null}");
                                                }*/
                                            }
                                        }
                                    }
                                }
                            }

                            if (finalTree != null && (layerMask & 1 << finalTree.m_prefabDataLayer) != 0)
                            {
                                var randomizer2 = new Randomizer((int)(laneID + (uint)i));
                                for (var k = 1; k <= num2; k += 2)
                                {
                                    if (randomizer2.Int32(100u) < prop.m_probability)
                                    {
                                        var t = num4 + (float)k / (float)num2;
                                        var variation2 = finalTree.GetVariation(ref randomizer2);
                                        var scale2 = variation2.m_minScale + (float)randomizer2.Int32(10000u) * (variation2.m_maxScale - variation2.m_minScale) * 0.0001f;
                                        var brightness = variation2.m_minBrightness + (float)randomizer2.Int32(10000u) * (variation2.m_maxBrightness - variation2.m_minBrightness) * 0.0001f;
                                        var position = _this.m_bezier.Position(t);
                                        if (propIndex != -1)
                                        {
                                            position.y = (float)data.m_extraData.GetUShort(num3++) * 0.015625f;
                                        }
                                        position.y += prop.m_position.y;
                                        if (prop.m_position.x != 0f)
                                        {
                                            var vector3 = _this.m_bezier.Tangent(t);
                                            if (flag2)
                                            {
                                                vector3 = -vector3;
                                            }
                                            vector3.y = 0f;
                                            vector3 = Vector3.Normalize(vector3);
                                            position.x += vector3.z * prop.m_position.x;
                                            position.z -= vector3.x * prop.m_position.x;
                                        }
                                        global::TreeInstance.RenderInstance(cameraInfo, variation2, position, scale2, brightness);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Called when road segment is placed/released.
        /// </summary>
        /// <param name="laneID"></param>
        /// <param name="laneInfo"></param>
        /// <param name="startFlags"></param>
        /// <param name="endFlags"></param>
        /// <param name="invert"></param>
        /// <param name="layer"></param>
        /// <param name="vertexCount"></param>
        /// <param name="triangleCount"></param>
        /// <param name="objectCount"></param>
        /// <param name="vertexArrays"></param>
        /// <param name="hasProps"></param>
        /// <returns></returns>
        public bool CalculateGroupData(uint laneID, NetInfo.Lane laneInfo, NetNode.Flags startFlags, NetNode.Flags endFlags, bool invert, int layer, ref int vertexCount, ref int triangleCount, ref int objectCount, ref RenderGroup.VertexArrays vertexArrays, ref bool hasProps)
        {
            var result = false;
            var laneProps = laneInfo.m_laneProps;
            if (laneProps != null && laneProps.m_props != null)
            {
                var flag = (byte)(laneInfo.m_finalDirection & NetInfo.Direction.Both) == 2 || (byte)(laneInfo.m_finalDirection & NetInfo.Direction.Avoid) == 11;
                if (flag)
                {
                    var flags = startFlags;
                    startFlags = endFlags;
                    endFlags = flags;
                }
                var num = laneProps.m_props.Length;

                // mod begin
                var _this = NetManager.instance.m_lanes.m_buffer[laneID];
                var PillarPrefabDataIndices = PropCustomizer.Instance.PillarPrefabDataIndices;
                var segmentData = SegmentDataManager.Instance.SegmentToSegmentDataMap?[_this.m_segment];
                // mod end

                for (var i = 0; i < num; i++)
                {
                    var prop = laneProps.m_props[i];
                    if (prop.CheckFlags((NetLane.Flags)_this.m_flags, startFlags, endFlags))
                    {
                        if (_this.m_length >= prop.m_minLength)
                        {
                            // mod begin
                            var finalProp = prop.m_finalProp;
                            var finalTree = prop.m_finalTree;
                            var repeatDistance = prop.m_repeatDistance;
                            if (segmentData != null)
                            {
                                // custom street lights
                                if (finalProp != null)
                                {
                                    var customLight = (segmentData.Features & SegmentData.FeatureFlags.StreetLight) != 0;

                                    // Contains seems to be faster than array lookup
                                    if ((customLight || segmentData.RepeatDistances.magnitude > 0f) && PillarPrefabDataIndices.Contains(finalProp.m_prefabDataIndex))
                                    {
                                        if (customLight)
                                        {
                                            finalProp = segmentData.PillarPrefab;
                                        }
                                        if (segmentData.RepeatDistances.w > 0f)
                                        {
                                            repeatDistance = segmentData.RepeatDistances.w;
                                        }
                                    }
                                }

                                // custom road trees
                                else if (finalTree != null)
                                {
                                    if (laneInfo.m_position < 0) // Left Trees
                                    {
                                        if ((segmentData.Features & SegmentData.FeatureFlags.TreeLeft) != 0)
                                        {
                                            finalTree = segmentData.TreeLeftPrefab;
                                        }
                                        if (segmentData.RepeatDistances.x > 0f)
                                        {
                                            repeatDistance = segmentData.RepeatDistances.x;
                                        }
                                    }
                                    else if (laneInfo.m_position == 0) // Middle Trees
                                    {
                                        if ((segmentData.Features & SegmentData.FeatureFlags.TreeMiddle) != 0)
                                        {
                                            finalTree = segmentData.TreeMiddlePrefab;
                                        }
                                        if (segmentData.RepeatDistances.y > 0f)
                                        {
                                            repeatDistance = segmentData.RepeatDistances.y;
                                        }
                                    }
                                    else // Right Trees
                                    {
                                        if ((segmentData.Features & SegmentData.FeatureFlags.TreeRight) != 0)
                                        {
                                            finalTree = segmentData.TreeRightPrefab;
                                        }
                                        if (segmentData.RepeatDistances.z > 0f)
                                        {
                                            repeatDistance = segmentData.RepeatDistances.z;
                                        }
                                    }
                                }
                            }
                            // mod end

                            var num2 = 2;
                            if (repeatDistance > 1f)
                            {
                                num2 *= Mathf.Max(1, Mathf.RoundToInt(_this.m_length / repeatDistance));
                            }

                            if (finalProp != null)
                            {
                                hasProps = true;
                                if (finalProp.m_prefabDataLayer == layer || finalProp.m_effectLayer == layer)
                                {
                                    var randomizer = new Randomizer((int)(laneID + (uint)i));
                                    for (var j = 1; j <= num2; j += 2)
                                    {
                                        if (randomizer.Int32(100u) < prop.m_probability)
                                        {
                                            var variation = finalProp.GetVariation(ref randomizer);
                                            randomizer.Int32(10000u);
                                            variation.GetColor(ref randomizer);
                                            if (PropInstance.CalculateGroupData(variation, layer, ref vertexCount, ref triangleCount, ref objectCount, ref vertexArrays))
                                            {
                                                result = true;
                                            }
                                        }
                                    }
                                }
                            }

                            if (finalTree != null)
                            {
                                hasProps = true;
                                if (finalTree.m_prefabDataLayer == layer)
                                {
                                    var randomizer2 = new Randomizer((int)(laneID + (uint)i));
                                    for (var k = 1; k <= num2; k += 2)
                                    {
                                        if (randomizer2.Int32(100u) < prop.m_probability)
                                        {
                                            finalTree.GetVariation(ref randomizer2);
                                            randomizer2.Int32(10000u);
                                            randomizer2.Int32(10000u);
                                            if (global::TreeInstance.CalculateGroupData(ref vertexCount, ref triangleCount, ref objectCount, ref vertexArrays))
                                            {
                                                result = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Called when road segment is placed/released.
        /// </summary>
        /// <param name="segmentID"></param>
        /// <param name="laneID"></param>
        /// <param name="laneInfo"></param>
        /// <param name="startFlags"></param>
        /// <param name="endFlags"></param>
        /// <param name="startAngle"></param>
        /// <param name="endAngle"></param>
        /// <param name="invert"></param>
        /// <param name="terrainHeight"></param>
        /// <param name="layer"></param>
        /// <param name="vertexIndex"></param>
        /// <param name="triangleIndex"></param>
        /// <param name="groupPosition"></param>
        /// <param name="data"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="maxRenderDistance"></param>
        /// <param name="maxInstanceDistance"></param>
        /// <param name="hasProps"></param>
        public void PopulateGroupData(ushort segmentID, uint laneID, NetInfo.Lane laneInfo, NetNode.Flags startFlags, NetNode.Flags endFlags, float startAngle, float endAngle, bool invert, bool terrainHeight, int layer, ref int vertexIndex, ref int triangleIndex, Vector3 groupPosition, RenderGroup.MeshData data, ref Vector3 min, ref Vector3 max, ref float maxRenderDistance, ref float maxInstanceDistance, ref bool hasProps)
        {
            var laneProps = laneInfo.m_laneProps;
            if (laneProps != null && laneProps.m_props != null)
            {
                var flag = (byte)(laneInfo.m_finalDirection & NetInfo.Direction.Both) == 2 || (byte)(laneInfo.m_finalDirection & NetInfo.Direction.Avoid) == 11;
                var flag2 = flag != invert;
                if (flag)
                {
                    var flags = startFlags;
                    startFlags = endFlags;
                    endFlags = flags;
                }
                var num = laneProps.m_props.Length;

                // mod begin
                var _this = NetManager.instance.m_lanes.m_buffer[laneID];
                var PillarPrefabDataIndices = PropCustomizer.Instance.PillarPrefabDataIndices;
                var segmentData = SegmentDataManager.Instance.SegmentToSegmentDataMap?[_this.m_segment];
                // mod end

                for (var i = 0; i < num; i++)
                {
                    var prop = laneProps.m_props[i];
                    if (prop.CheckFlags((NetLane.Flags)_this.m_flags, startFlags, endFlags))
                    {
                        if (_this.m_length >= prop.m_minLength)
                        {
                            // mod begin
                            var prop_m_angle = prop.m_angle;
                            var finalProp = prop.m_finalProp;
                            var finalTree = prop.m_finalTree;
                            var repeatDistance = prop.m_repeatDistance;
                            if (segmentData != null)
                            {
                                // custom street lights
                                if (finalProp != null)
                                {
                                    var customLight = (segmentData.Features & SegmentData.FeatureFlags.StreetLight) != 0;

                                    // Contains seems to be faster than array lookup
                                    if ((customLight || segmentData.RepeatDistances.magnitude > 0f) && PillarPrefabDataIndices.Contains(finalProp.m_prefabDataIndex))
                                    {
                                        if (customLight)
                                        {
                                            finalProp = segmentData.PillarPrefab;
                                            if (laneInfo.m_position + prop.m_position.x < 0f) prop_m_angle = 180; //rotate street lights on pedestrian paths correctly
                                        }
                                        if (segmentData.RepeatDistances.w > 0f)
                                        {
                                            repeatDistance = segmentData.RepeatDistances.w;
                                        }
                                    }
                                }

                                // custom road trees
                                else if (finalTree != null)
                                {
                                    if (laneInfo.m_position < 0) // Left Trees
                                    {
                                        if ((segmentData.Features & SegmentData.FeatureFlags.TreeLeft) != 0)
                                        {
                                            finalTree = segmentData.TreeLeftPrefab;
                                        }
                                        if (segmentData.RepeatDistances.x > 0f)
                                        {
                                            repeatDistance = segmentData.RepeatDistances.x;
                                        }
                                    }
                                    else if (laneInfo.m_position == 0) // Middle Trees
                                    {
                                        if ((segmentData.Features & SegmentData.FeatureFlags.TreeMiddle) != 0)
                                        {
                                            finalTree = segmentData.TreeMiddlePrefab;
                                        }
                                        if (segmentData.RepeatDistances.y > 0f)
                                        {
                                            repeatDistance = segmentData.RepeatDistances.y;
                                        }
                                    }
                                    else // Right Trees
                                    {
                                        if ((segmentData.Features & SegmentData.FeatureFlags.TreeRight) != 0)
                                        {
                                            finalTree = segmentData.TreeRightPrefab;
                                        }
                                        if (segmentData.RepeatDistances.z > 0f)
                                        {
                                            repeatDistance = segmentData.RepeatDistances.z;
                                        }
                                    }
                                }
                            }
                            // mod end

                            var num2 = 2;
                            if (repeatDistance > 1f)
                            {
                                num2 *= Mathf.Max(1, Mathf.RoundToInt(_this.m_length / repeatDistance));
                            }
                            var num3 = prop.m_segmentOffset * 0.5f;
                            if (_this.m_length != 0f)
                            {
                                num3 = Mathf.Clamp(num3 + prop.m_position.z / _this.m_length, -0.5f, 0.5f);
                            }
                            if (flag2)
                            {
                                num3 = -num3;
                            }

                            if (finalProp != null)
                            {
                                hasProps = true;
                                if (finalProp.m_prefabDataLayer == layer || finalProp.m_effectLayer == layer)
                                {
                                    var color = Color.white;
                                    var randomizer = new Randomizer((int)(laneID + (uint)i));
                                    for (var j = 1; j <= num2; j += 2)
                                    {
                                        if (randomizer.Int32(100u) < prop.m_probability)
                                        {
                                            var num4 = num3 + (float)j / (float)num2;
                                            var variation = finalProp.GetVariation(ref randomizer);
                                            var scale = variation.m_minScale + (float)randomizer.Int32(10000u) * (variation.m_maxScale - variation.m_minScale) * 0.0001f;
                                            if (prop.m_colorMode == NetLaneProps.ColorMode.Default)
                                            {
                                                color = variation.GetColor(ref randomizer);
                                            }
                                            var vector = _this.m_bezier.Position(num4);
                                            if (terrainHeight)
                                            {
                                                vector.y = Singleton<TerrainManager>.instance.SampleDetailHeight(vector);
                                            }
                                            vector.y += prop.m_position.y;
                                            var vector2 = _this.m_bezier.Tangent(num4);
                                            if (vector2 != Vector3.zero)
                                            {
                                                if (flag2)
                                                {
                                                    vector2 = -vector2;
                                                }
                                                vector2.y = 0f;
                                                if (prop.m_position.x != 0f)
                                                {
                                                    vector2 = Vector3.Normalize(vector2);
                                                    vector.x += vector2.z * prop.m_position.x;
                                                    vector.z -= vector2.x * prop.m_position.x;
                                                }
                                                var num5 = Mathf.Atan2(vector2.x, -vector2.z);
                                                if (prop.m_cornerAngle != 0f || prop.m_position.x != 0f)
                                                {
                                                    var num6 = endAngle - startAngle;
                                                    if (num6 > 3.14159274f)
                                                    {
                                                        num6 -= 6.28318548f;
                                                    }
                                                    if (num6 < -3.14159274f)
                                                    {
                                                        num6 += 6.28318548f;
                                                    }
                                                    var num7 = startAngle + num6 * num4;
                                                    num6 = num7 - num5;
                                                    if (num6 > 3.14159274f)
                                                    {
                                                        num6 -= 6.28318548f;
                                                    }
                                                    if (num6 < -3.14159274f)
                                                    {
                                                        num6 += 6.28318548f;
                                                    }
                                                    num5 += num6 * prop.m_cornerAngle;
                                                    if (num6 != 0f && prop.m_position.x != 0f)
                                                    {
                                                        var num8 = Mathf.Tan(num6);
                                                        vector.x += vector2.x * num8 * prop.m_position.x;
                                                        vector.z += vector2.z * num8 * prop.m_position.x;
                                                    }
                                                }
                                                var id = default(InstanceID);
                                                id.NetSegment = segmentID;
                                                num5 += prop_m_angle * 0.0174532924f;
                                                PropInstance.PopulateGroupData(variation, layer, id, vector, scale, num5, color, ref vertexIndex, ref triangleIndex, groupPosition, data, ref min, ref max, ref maxRenderDistance, ref maxInstanceDistance);
                                            }
                                        }
                                    }
                                }
                            }

                            if (finalTree != null)
                            {
                                hasProps = true;
                                if (finalTree.m_prefabDataLayer == layer)
                                {
                                    var randomizer2 = new Randomizer((int)(laneID + (uint)i));
                                    for (var k = 1; k <= num2; k += 2)
                                    {
                                        if (randomizer2.Int32(100u) < prop.m_probability)
                                        {
                                            var t = num3 + (float)k / (float)num2;
                                            var variation2 = finalTree.GetVariation(ref randomizer2);
                                            var scale2 = variation2.m_minScale + (float)randomizer2.Int32(10000u) * (variation2.m_maxScale - variation2.m_minScale) * 0.0001f;
                                            var brightness = variation2.m_minBrightness + (float)randomizer2.Int32(10000u) * (variation2.m_maxBrightness - variation2.m_minBrightness) * 0.0001f;
                                            var vector3 = _this.m_bezier.Position(t);
                                            if (terrainHeight)
                                            {
                                                vector3.y = Singleton<TerrainManager>.instance.SampleDetailHeight(vector3);
                                            }
                                            vector3.y += prop.m_position.y;
                                            if (prop.m_position.x != 0f)
                                            {
                                                var vector4 = _this.m_bezier.Tangent(t);
                                                if (flag2)
                                                {
                                                    vector4 = -vector4;
                                                }
                                                vector4.y = 0f;
                                                vector4 = Vector3.Normalize(vector4);
                                                vector3.x += vector4.z * prop.m_position.x;
                                                vector3.z -= vector4.x * prop.m_position.x;
                                            }
                                            global::TreeInstance.PopulateGroupData(variation2, vector3, scale2, brightness, ref vertexIndex, ref triangleIndex, groupPosition, data, ref min, ref max, ref maxRenderDistance, ref maxInstanceDistance);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

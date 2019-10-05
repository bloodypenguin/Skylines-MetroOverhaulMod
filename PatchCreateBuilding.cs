using ColossalFramework;
using Harmony;
using MetroOverhaul.Extensions;
using MetroOverhaul.OptionsFramework;
using MetroOverhaul.UI;
using System;
using System.Linq;
using System.Reflection;

namespace MetroOverhaul
{
    class PatchCreateBuilding
    {
        private static MetroStationCustomizerUI m_Container2;
        public static void Apply(HarmonyInstance harmony, ref MetroStationCustomizerUI ui)
        {
            m_Container2 = ui;
            var postfix = typeof(PatchCreateBuilding).GetMethod("Postfix");
            harmony.Patch(OriginalMethod, null, new HarmonyMethod(postfix), null);
        }
        public static void Revert(HarmonyInstance harmony)
        {
            //harmony.Unpatch(OriginalMethod, HarmonyPatchType.Prefix);
            harmony.Unpatch(OriginalMethod, HarmonyPatchType.Postfix);
        }
        public static MethodInfo OriginalMethod => typeof(BuildingManager)
            .GetMethods().First(m => m.Name == "UpdateBuilding" && !m.IsGenericMethod);
        public static void Postfix(ushort building)
        {
            try
            {
                var buildingData = Singleton<BuildingManager>.instance.m_buildings.m_buffer[building];
                if (buildingData.Info != null)
                {
                    if (buildingData.Info.HasUndergroundMetroStationTracks())
                    {
                        if (m_Container2 != null)
                        {
                            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_children = StoreCustomization(MetroStationTrackAlterType.Length);
                            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_childHealth = StoreCustomization(MetroStationTrackAlterType.Depth);
                            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_seniors = StoreCustomization(MetroStationTrackAlterType.Rotation);
                            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_seniorHealth = StoreCustomization(MetroStationTrackAlterType.Bend);
                            Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_teens = (byte)m_Container2.m_TrackType;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
            }
        }
        private static byte StoreCustomization(MetroStationTrackAlterType customizationType)
        {
            return (byte)((m_Container2.SetDict[customizationType] - m_Container2.SliderDataDict[customizationType].Min) / m_Container2.SliderDataDict[customizationType].Step);
        }
    }
}

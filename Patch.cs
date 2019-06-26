using Harmony;
using MetroOverhaul.Extensions;
using MetroOverhaul.OptionsFramework;
using System;
using System.Linq;
using System.Reflection;

namespace MetroOverhaul
{
    class Patch
    {
        private static Initializer m_Container;
        public static void Apply(HarmonyInstance harmony, ref Initializer initializer)
        {
            m_Container = initializer;
            var postfix = typeof(Patch).GetMethod("Postfix");
            harmony.Patch(OriginalMethod, null, new HarmonyMethod(postfix), null);
        }
        public static void Revert(HarmonyInstance harmony)
        {
            //harmony.Unpatch(OriginalMethod, HarmonyPatchType.Prefix);
            harmony.Unpatch(OriginalMethod, HarmonyPatchType.Postfix);
        }
        public static MethodInfo OriginalMethod => typeof(BuildingInfo)
            .GetMethods().First(m => m.Name == "InitializePrefab" && !m.IsGenericMethod);
        public static void Postfix(ref BuildingInfo __instance)
        {
            if (OptionsWrapper<Options>.Options.ingameTrainMetroConverter)
            {
                try
                {
                    //if (Container.RegisterWid(info, false))
                    //{
                    var ai = __instance.GetComponent<PlayerBuildingAI>();
                    if (ai != null)
                    {
                        if (ai is TransportStationAI)
                        {
                            if (__instance.m_class.m_subService == ItemClass.SubService.PublicTransportTrain || __instance.m_class.m_subService == ItemClass.SubService.PublicTransportMetro)
                            {
                                if (__instance.name.IndexOf(ModTrackNames.ANALOG_PREFIX) == -1)
                                {
                                    if (__instance.HasAbovegroundTrainStationTracks() || __instance.HasAbovegroundMetroStationTracks())
                                    {
                                        m_Container.InitializeBuildingInfoImpl(__instance);
                                    }
                                }
                            }
                        }
                    }
                    //}
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError(e);
                }
            }
        }
    }
}

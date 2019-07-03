using MetroOverhaul.NEXT.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MetroOverhaul.Extensions
{
    public static class PropInfoExtensions
    {
        public static void SetAsTunnelLightProp(this PropInfo thePropInfo, float intensity, float range)
        {
            //var effectInfo = EffectCollection.Effects.FirstOrDefault(e=> e.name == "Runway Light").ShallowClone();
            var lightEffect = Resources.FindObjectsOfTypeAll<LightEffect>().FirstOrDefault(l => l.name == "Runway Light").ShallowClone();
            lightEffect.SetLightField("m_lightIntensity", intensity);
            lightEffect.SetLightField("m_lightRange", range);

            var effectList = new List<PropInfo.Effect>();
            for (int i = 0; i < thePropInfo.m_effects.Count(); i++)
            {
                var effect = thePropInfo.m_effects[i].ShallowClone();
                effect.m_effect = lightEffect;
                effect.m_position = Vector3.zero;
                effectList.Add(effect);
            }
            thePropInfo.m_effects = effectList.ToArray();
        }
    }
}

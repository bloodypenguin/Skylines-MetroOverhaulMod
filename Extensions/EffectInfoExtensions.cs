using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroOverhaul.Extensions
{
    public static class EffectInfoExtensions
    {
        public static void SetLightField(this EffectInfo lightEffect, string fieldName, float fieldValue)
        {
            if (lightEffect == null)
            {
                UnityEngine.Debug.Log("Effect is null");
            }
            else
            {
            var intensityProp = lightEffect.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (intensityProp == null)
            {
                UnityEngine.Debug.Log("property " + fieldName + " does not exist.");
            }
                else
                {
intensityProp.SetValue(lightEffect, fieldValue);
                }
            
            }

        }
    }
}

using ColossalFramework;
using ColossalFramework.Globalization;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MetroOverhaul.NEXT.Extensions {
    public static partial class NetInfoExtensions
    {
        public static void ReplaceProps(NetInfo info, PropInfo newPropInfo, PropInfo oldPropInfo)
        {
            if (newPropInfo == null || oldPropInfo == null)
            {
                return;
            }

            for (int i = 0; i < info.m_lanes.Count(); i++)
            {
                var lane = info.m_lanes[i].ShallowClone();
                if (lane.m_laneProps == null ||
                    lane.m_laneProps.m_props == null ||
                    lane.m_laneProps.m_props.Length == 0)
                {
                    continue;
                }

                var oldProps = lane
                    .m_laneProps
                    .m_props
                    .Where(prop => prop != null && prop.m_prop == oldPropInfo)
                    .ToList();

                if (oldProps.Count() > 0)
                {
                    var newPropsContent = new List<NetLaneProps.Prop>();
                    newPropsContent.AddRange(lane.m_laneProps.m_props.Where(prop => prop.m_prop != oldPropInfo));
                    for (int j = 0; j < oldProps.Count(); j++)
                    {
                        var newProp = oldProps[j].ShallowClone();
                        newProp.m_prop = newPropInfo;
                        newProp.m_finalProp = newPropInfo;
                        newPropsContent.Add(newProp);
                    }
                    var newProps = ScriptableObject.CreateInstance<NetLaneProps>();
                    newProps.name = lane.m_laneProps.name;
                    newProps.m_props = newPropsContent.ToArray();
                    lane.m_laneProps = newProps;
                }
            }
        }
        public static void ModifyTitle(this NetInfo info, string newTitle)
        {
            var localizedStringsField = typeof(Locale).GetFieldByName("m_LocalizedStrings");
            var locale = SingletonLite<LocaleManager>.instance.GetLocale();
            var localizedStrings = (Dictionary<Locale.Key, string>)localizedStringsField.GetValue(locale);

            var kvp =
                localizedStrings
                .FirstOrDefault(kvpInternal =>
                    kvpInternal.Key.m_Identifier == "NET_TITLE" &&
                    kvpInternal.Key.m_Key == info.name);

            if (!Equals(kvp, default(KeyValuePair<Locale.Key, string>)))
            {
                localizedStrings[kvp.Key] = newTitle;
            }
        }
    }
}
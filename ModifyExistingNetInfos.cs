using SingleTrainTrack.NEXT;
using SingleTrainTrack.NEXT.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SingleTrainTrack
{
    class ModifyExistingNetInfos
    {
        private static bool modified;

        public string Name { get { return "Vanilla Menu Icon Modifier"; } }
        public static void ModifyExistingIcons()
        {
            if (modified)
            {
                return;
            }
            var rail2L = Prefabs.Find<NetInfo>(SharedHelpers.TRAIN_TRACK, false);
            if (rail2L != null)
            {
                rail2L.m_UIPriority = 12;
                var thumbnails = AssetManager.instance.GetThumbnails(SharedHelpers.TRAIN_TRACK, @"Textures\Rail2L\thumbnails.png");
                rail2L.m_Atlas = thumbnails;
                rail2L.m_Thumbnail = thumbnails.name;
                rail2L.ModifyTitle("Two Lane Two-Way Rail");
                modified = true;
            }
        }

        public static void Reset()
        {
            modified = false;
        }
    }
}

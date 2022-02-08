using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroOverhaul
{
    interface IMOMMetroTrackAI
    {
        void UpdateNodeFlags(ushort nodeID, ref NetNode data);
    }
}

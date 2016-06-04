using ICities;
using UnityEngine;

namespace OneWayTrainTrack
{
    public class Mod : IUserMod
    {

        public string Name
        {
            get { return "One-Way Train Track"; }
        }

        public string Description
        {
            get { return "Provides a one-way train track"; }
        }
    }
}
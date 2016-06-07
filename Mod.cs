using ICities;
using UnityEngine;

namespace SingleTrainTrack
{
    public class Mod : IUserMod
    {
        public const string TRAIN_STATION_TRACK = "Train Station Track";
        public const string TRAIN_TRACK = "Train Track";

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
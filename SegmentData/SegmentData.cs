using System;
using ColossalFramework;
using ColossalFramework.IO;
using UnityEngine;

// ReSharper disable NonReadonlyMemberInGetHashCode

namespace NetworkSkins.Data
{
    public class SegmentData : IDataContainer
    {
        [Flags]
        public enum FeatureFlags
        {
            None = 0,
            TreeLeft = 1,
            TreeMiddle = 2,
            TreeRight = 4,
            StreetLight = 8,
            NoDecals = 16,
            RepeatDistances = 32,
        }

        // After setting these fields once, they should only be read!
        public FeatureFlags Features = FeatureFlags.None;
        public string TreeLeft;
        public string TreeMiddle;
        public string TreeRight;
        public string StreetLight;
        public Vector4 RepeatDistances; // x -> left tree, y -> middle tree, z -> right tree, w -> street light

        [NonSerialized]
        public TreeInfo TreeLeftPrefab;
        [NonSerialized]
        public TreeInfo TreeMiddlePrefab;
        [NonSerialized]
        public TreeInfo TreeRightPrefab;
        [NonSerialized]
        public PropInfo PillarPrefab;
        [NonSerialized]
        public int UsedCount = 0;

        public SegmentData() {}

        public SegmentData(SegmentData segmentData)
        {
            if (segmentData == null) return;

            Features = segmentData.Features;
            TreeLeft = segmentData.TreeLeft;
            TreeMiddle = segmentData.TreeMiddle;
            TreeRight = segmentData.TreeRight;
            StreetLight = segmentData.StreetLight;
            RepeatDistances = segmentData.RepeatDistances;

            TreeLeftPrefab = segmentData.TreeLeftPrefab;
            TreeMiddlePrefab = segmentData.TreeMiddlePrefab;
            TreeRightPrefab = segmentData.TreeRightPrefab;
            PillarPrefab = segmentData.PillarPrefab;
        }

        public void SetPrefabFeature<P>(FeatureFlags feature, P prefab = null) where P : PrefabInfo
        {
            Features = Features.SetFlags(feature);

            var flagName = feature.ToString();

            var nameField = GetType().GetField(flagName);
            var prefabField = GetType().GetField(flagName + "Prefab");

            nameField?.SetValue(this, prefab?.name);
            prefabField?.SetValue(this, prefab);
        }

        public void SetStructFeature<V>(FeatureFlags feature, V value) where V : struct
        {
            Features = Features.SetFlags(feature);

            var flagName = feature.ToString();

            var nameField = GetType().GetField(flagName);

            nameField?.SetValue(this, value); //TODO
        }

        public void UnsetFeature(FeatureFlags feature)
        {
            Features = Features.ClearFlags(feature);

            var flagName = feature.ToString();

            var nameField = GetType().GetField(flagName);
            var prefabField = GetType().GetField(flagName + "Prefab");

            nameField?.SetValue(this, null);
            prefabField?.SetValue(this, null);
        }

        public void Serialize(DataSerializer s)
        {
            s.WriteInt32((int)Features);

            if (Features.IsFlagSet(FeatureFlags.TreeLeft))
                s.WriteSharedString(TreeLeft);
            if (Features.IsFlagSet(FeatureFlags.TreeMiddle))
                s.WriteSharedString(TreeMiddle);
            if (Features.IsFlagSet(FeatureFlags.TreeRight))
                s.WriteSharedString(TreeRight);
            if (Features.IsFlagSet(FeatureFlags.StreetLight))
                s.WriteSharedString(StreetLight);
            if (Features.IsFlagSet(FeatureFlags.RepeatDistances))
                s.WriteVector4(RepeatDistances);
        }

        public void Deserialize(DataSerializer s)
        {
            Features = (FeatureFlags)s.ReadInt32();

            if (Features.IsFlagSet(FeatureFlags.TreeLeft))
                TreeLeft = s.ReadSharedString();
            if (Features.IsFlagSet(FeatureFlags.TreeMiddle))
                TreeMiddle = s.ReadSharedString();
            if (Features.IsFlagSet(FeatureFlags.TreeRight))
                TreeRight = s.ReadSharedString();
            if (Features.IsFlagSet(FeatureFlags.StreetLight))
                StreetLight = s.ReadSharedString();
            if (Features.IsFlagSet(FeatureFlags.RepeatDistances))
                RepeatDistances = s.ReadVector4();
        }

        public void AfterDeserialize(DataSerializer s) {}

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == this.GetType() && Equals((SegmentData) obj);
        }

        protected bool Equals(SegmentData other)
        {
            return Features == other.Features
                && string.Equals(TreeLeft, other.TreeLeft)
                && string.Equals(TreeMiddle, other.TreeMiddle)
                && string.Equals(TreeRight, other.TreeRight)
                && string.Equals(StreetLight, other.StreetLight)
                && (Features.IsFlagSet(FeatureFlags.RepeatDistances) == Vector4.Equals(RepeatDistances, other.RepeatDistances));
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) Features;
                hashCode = (hashCode*397) ^ (TreeLeft?.GetHashCode() ?? 0);
                hashCode = (hashCode*397) ^ (TreeMiddle?.GetHashCode() ?? 0);
                hashCode = (hashCode*397) ^ (TreeRight?.GetHashCode() ?? 0);
                hashCode = (hashCode*397) ^ (StreetLight?.GetHashCode() ?? 0);
                hashCode = (hashCode*397) ^ (Features.IsFlagSet(FeatureFlags.RepeatDistances) ? RepeatDistances.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString() 
        {
            return "[SegmentData] features: " + Features
                + ", treeLeftPrefab: " + (TreeLeftPrefab == null ? "null" : TreeLeftPrefab.name)
                + ", treeMiddlePrefab: " + (TreeMiddlePrefab == null ? "null" : TreeMiddlePrefab.name)
                + ", treeRightPrefab: " + (TreeRightPrefab == null ? "null" : TreeRightPrefab.name)
                + ", PillarPrefab: " + (PillarPrefab == null ? "null" : PillarPrefab.name)
                + ", usedCount: " + UsedCount;
        }

        public void FindPrefabs()
        {
            FindPrefab(TreeLeft, out TreeLeftPrefab);
            FindPrefab(TreeMiddle, out TreeMiddlePrefab);
            FindPrefab(TreeRight, out TreeRightPrefab);
            FindPrefab(StreetLight, out PillarPrefab);
        }

        private static void FindPrefab<T>(string prefabName, out T prefab) where T : PrefabInfo
        {
            prefab = prefabName != null ? PrefabCollection<T>.FindLoaded(prefabName) : null;
        }
    }
}

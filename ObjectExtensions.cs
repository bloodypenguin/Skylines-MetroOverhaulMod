namespace SingleTrainTrack
{
    public static class ObjectExtensions
    {
        public static T GetPropery<T>(this object o, string name)
        {
            return (T)o.GetType().GetProperty(name).GetValue(o, null);
        }
    }
}
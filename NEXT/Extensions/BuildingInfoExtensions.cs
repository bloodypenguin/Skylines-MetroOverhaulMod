using MetroOverhaul.NEXT;

namespace MetroOverhaul.NEXT.Extensions
{
    public static class  BuildingInfoExtensions
    {
        public static BuildingInfo SetMeshes(this BuildingInfo info, string newMeshPath, string newLODMeshPath = null)
        {
            info.m_mesh = AssetManager.instance.GetMesh(newMeshPath);

            if (newLODMeshPath != null)
            {
                info.m_lodMesh = AssetManager.instance.GetMesh(newLODMeshPath);
            }

            return info;
        }
    }
}

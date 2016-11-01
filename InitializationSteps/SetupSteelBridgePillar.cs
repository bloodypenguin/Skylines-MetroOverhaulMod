using MetroOverhaul.NEXT.Extensions;
using MetroOverhaul.NEXT.Texturing;

namespace MetroOverhaul.InitializationSteps
{
    public static class SetupSteelBridgePillar
    {
        public static void SetMeshAndTexture(BuildingInfo pillar)
        {
            pillar.SetMeshes(@"Meshes\Elevated_Pillar.obj", @"Meshes\Elevated_Pillar.obj").SetConsistentUVs();
            pillar.SetTextures(new TextureSet(@"Textures\Elevated_Pillar__MainTex.png"));
        }
    }
}

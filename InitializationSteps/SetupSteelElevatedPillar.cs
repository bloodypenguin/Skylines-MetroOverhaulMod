using MetroOverhaul.NEXT.Extensions;
using MetroOverhaul.NEXT.Texturing;

namespace MetroOverhaul.InitializationSteps
{
    public static class SetupSteelElevatedPillar
    {
        public static void SetMeshAndTexture(BuildingInfo pillar)
        {
            pillar.SetMeshes(
                @"Meshes\Elevated_Pillar_Steel.obj",
                @"Meshes\Elevated_Pillar_Steel.obj"
            ).SetConsistentUVs();

            pillar.SetTextures(
                new TextureSet(
                    @"Textures\Elevated_Pillar_Steel__MainTex.png",
                    @"Textures\Elevated_Pillar_Steel__AlphaMap.png",
                    @"Textures\Elevated_Pillar_Steel__XYSMap.png"
                ));
        }
    }
}
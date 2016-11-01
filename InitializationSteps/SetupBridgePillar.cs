using System;
using MetroOverhaul.NEXT.Extensions;
using MetroOverhaul.NEXT.Texturing;

namespace MetroOverhaul.InitializationSteps
{
    public static class SetupBridgePillar
    {
        public static void SetMeshAndTexture(BuildingInfo pillar)
        {
            pillar.SetMeshes(
                @"Meshes\Bridge_Pillar.obj",
                @"Meshes\Bridge_Pillar.obj"
                ).SetConsistentUVs();

            pillar.SetTextures(
                new TextureSet(
                    @"Textures\Bridge_Pillar__MainTex.png",
                    @"Textures\Bridge_Pillar__AlphaMap.png",
                    @"Textures\Bridge_Pillar__XYSMap.png"
                    ));
        }
    }
}

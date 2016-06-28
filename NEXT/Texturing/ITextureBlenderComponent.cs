using UnityEngine;

namespace SubwayOverhaul.NEXT.Texturing
{
    public interface ITextureBlenderComponent
    {
        void Apply(ref Point offset, Texture2D canvas);
    }
}

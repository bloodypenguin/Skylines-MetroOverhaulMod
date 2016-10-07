using UnityEngine;

namespace MetroOverhaul.NEXT.Texturing
{
    public interface ITextureBlenderComponent
    {
        void Apply(ref Point offset, Texture2D canvas);
    }
}

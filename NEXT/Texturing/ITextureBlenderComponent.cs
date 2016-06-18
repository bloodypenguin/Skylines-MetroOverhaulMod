using UnityEngine;

namespace SingleTrainTrack.NEXT.Texturing
{
    public interface ITextureBlenderComponent
    {
        void Apply(ref Point offset, Texture2D canvas);
    }
}

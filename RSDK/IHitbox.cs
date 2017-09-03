using System.IO;

namespace RSDK
{
    public interface IHitbox
    {
        int Left { get; set; }

        int Top { get; set; }

        int Right { get; set; }

        int Bottom { get; set; }

        void SaveChanges(BinaryWriter writer);
    }
}

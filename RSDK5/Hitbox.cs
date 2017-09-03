using RSDK;
using System.IO;

namespace RSDK5
{
    public class Hitbox : IHitbox
    {
        public int Left { get; set; }

        public int Top { get; set; }

        public int Right { get; set; }

        public int Bottom { get; set; }

        public Hitbox() { }

        public Hitbox(BinaryReader reader)
        {
            Left = reader.ReadInt16();
            Top = reader.ReadInt16();
            Right = reader.ReadInt16();
            Bottom = reader.ReadInt16();
        }

        public void SaveChanges(BinaryWriter writer)
        {
            writer.Write((short)Left);
            writer.Write((short)Top);
            writer.Write((short)Right);
            writer.Write((short)Bottom);
        }
    }
}

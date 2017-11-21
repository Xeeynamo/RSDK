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
            Read(reader);
        }

        public void SaveChanges(BinaryWriter writer)
        {
            Write(writer);
        }

        public void Read(BinaryReader reader)
        {
            Left = reader.ReadInt16();
            Top = reader.ReadInt16();
            Right = reader.ReadInt16();
            Bottom = reader.ReadInt16();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write((short)Left);
            writer.Write((short)Top);
            writer.Write((short)Right);
            writer.Write((short)Bottom);
        }

        public object Clone()
        {
            return new Hitbox()
            {
                Left = Left,
                Top = Top,
                Right = Right,
                Bottom = Bottom
            };
        }
    }
}

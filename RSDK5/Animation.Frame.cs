// MIT License
// 
// Copyright(c) 2017 Luciano (Xeeynamo) Ciccariello
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using RSDK;
using System;
using System.IO;
using System.Linq;

namespace RSDK5
{
    public class Frame : IFrame
    {
        private int _collisionBoxesCount;

        public int SpriteSheet { get; set; }

        public int CollisionBox { get; set; }

        public int Duration { get; set; }

        public int Id { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int CenterX { get; set; }

        public int CenterY { get; set; }

        public Hitbox[] Hitboxes { get; private set; }

        public Frame(int collisionBoxesCount = 0)
        {
            _collisionBoxesCount = collisionBoxesCount;

            Hitboxes = new Hitbox[collisionBoxesCount];
            for (int i = 0; i < collisionBoxesCount; i++)
                Hitboxes[i] = new Hitbox();
        }

        public Frame(BinaryReader reader, int collisionBoxesCount = 0)
        {
            _collisionBoxesCount = collisionBoxesCount;
            Hitboxes = new Hitbox[collisionBoxesCount];
            SubRead(reader, collisionBoxesCount, collisionBoxesCount);
        }

        public IHitbox GetHitbox(int index)
        {
            if (Hitboxes.Length >= 0 && index < Hitboxes.Length)
                return Hitboxes[index];
            else
                return null;
        }

        public void SaveChanges(BinaryWriter writer)
        {
            SubWrite(writer);
        }

        public void Read(BinaryReader reader)
        {
            int count = reader.ReadInt32();
            SubRead(reader, _collisionBoxesCount, count);
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(_collisionBoxesCount);
            SubWrite(writer);
        }

        public object Clone()
        {
            return new Frame()
            {
                SpriteSheet = SpriteSheet,
                CollisionBox = CollisionBox,
                Duration = Duration,
                Id = Id,
                X = X,
                Y = Y,
                Width = Width,
                Height = Height,
                CenterX = CenterX,
                CenterY = CenterY,
                Hitboxes = Hitboxes.Select(x => x.Clone() as Hitbox).ToArray()
            };
        }

        private void SubRead(BinaryReader reader, int cbDst, int cbSrc)
        {
            SpriteSheet = reader.ReadByte();
            CollisionBox = 0;
            Duration = reader.ReadInt16();
            Id = reader.ReadUInt16();
            X = reader.ReadInt16();
            Y = reader.ReadInt16();
            Width = reader.ReadInt16();
            Height = reader.ReadInt16();
            CenterX = reader.ReadInt16();
            CenterY = reader.ReadInt16();

            var collisionBoxesCount = Math.Min(cbDst, cbSrc);
            for (int j = 0; j < collisionBoxesCount; j++)
                Hitboxes[j] = new Hitbox(reader);
            for (int j = collisionBoxesCount; j < cbDst; j++)
                Hitboxes[j] = new Hitbox();
        }

        private void SubWrite(BinaryWriter writer)
        {
            writer.Write((byte)SpriteSheet);
            writer.Write((short)Duration);
            writer.Write((ushort)Id);
            writer.Write((short)X);
            writer.Write((short)Y);
            writer.Write((short)Width);
            writer.Write((short)Height);
            writer.Write((short)CenterX);
            writer.Write((short)CenterY);
            foreach (var hb in Hitboxes)
                hb.Write(writer);
        }
    }
}

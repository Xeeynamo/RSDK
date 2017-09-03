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
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RSDK5
{
    public class AnimationEntry : IAnimationEntry
    {
        public string Name { get; set; }

        public List<Frame> Frames { get; } = new List<Frame>();

        public int Speed { get; set; }

        public int Loop { get; set; }

        public int Flags { get; set; }

        public AnimationEntry()
        {
        }

        public AnimationEntry(BinaryReader reader, int collisionBoxesCount)
        {
            Name = StringEncoding.GetString(reader);

            var framesCount = reader.ReadInt16();
            Speed = reader.ReadInt16();
            Loop = reader.ReadByte();
            Flags = reader.ReadByte();
            for (int i = 0; i < framesCount; i++)
            {
                var frame = new Frame(collisionBoxesCount)
                {
                    SpriteSheet = reader.ReadByte(),
                    CollisionBox = 0,
                    Unk00 = reader.ReadInt16(),
                    Unk01 = reader.ReadInt16(),
                    X = reader.ReadInt16(),
                    Y = reader.ReadInt16(),
                    Width = reader.ReadInt16(),
                    Height = reader.ReadInt16(),
                    CenterX = reader.ReadInt16(),
                    CenterY = reader.ReadInt16(),
                };
                for (int j = 0; j < collisionBoxesCount; j++)
                    frame.Hitboxes[j] = new Hitbox(reader);
                Frames.Add(frame);
            }
        }

        public IEnumerable<IFrame> GetFrames()
        {
            return Frames.Select(x => (IFrame)x);
        }

        public void SetFrames(IEnumerable<IFrame> frames)
        {
            Frames.Clear();
            Frames.AddRange(frames
                .Select(x => x as Frame)
                .Where(x => x != null));
        }

        public void SaveChanges(BinaryWriter writer)
        {
            writer.Write(StringEncoding.GetBytes(Name));
            foreach (var entry in Frames)
            {
                writer.Write((byte)entry.SpriteSheet);
                writer.Write((byte)entry.CollisionBox);
                writer.Write((byte)entry.X);
                writer.Write((byte)entry.Y);
                writer.Write((byte)entry.Width);
                writer.Write((byte)entry.Height);
                writer.Write((byte)entry.CenterX);
                writer.Write((byte)entry.CenterY);
            }
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(Name) ?
                "<new animation>" : Name;
        }
    }
}

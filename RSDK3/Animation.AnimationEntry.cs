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

namespace RSDK3
{
    public class AnimationEntry : IAnimationEntry
    {
        public string Name { get; set; }

        public List<Frame> Frames { get; private set; } = new List<Frame>();

        public int Speed { get; set; }

        public int Loop { get; set; }

        public int Flags { get; set; }

        public AnimationEntry()
        {
        }

        public AnimationEntry(BinaryReader reader)
        {
            Name = StringEncoding.GetString(reader);

            var framesCount = reader.ReadByte();
            Speed = reader.ReadByte();
            Loop = reader.ReadByte();
            Flags = reader.ReadByte();
            for (int i = 0; i < framesCount; i++)
            {
                Frames.Add(new Frame()
                {
                    SpriteSheet = reader.ReadByte(),
                    CollisionBox = reader.ReadByte(),
                    X = reader.ReadByte(),
                    Y = reader.ReadByte(),
                    Width = reader.ReadByte(),
                    Height = reader.ReadByte(),
                    CenterX = reader.ReadSByte(),
                    CenterY = reader.ReadSByte()
                });
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
            writer.Write((byte)Frames.Count);
            writer.Write((byte)Speed);
            writer.Write((byte)Loop);
            writer.Write((byte)Flags);
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

        public object Clone()
        {
            return new AnimationEntry()
            {
                Name = Name,
                Frames = Frames.Select(x => x.Clone() as Frame).ToList(),
                Speed = Speed,
                Loop = Loop,
                Flags = Flags
            };
        }
    }
}

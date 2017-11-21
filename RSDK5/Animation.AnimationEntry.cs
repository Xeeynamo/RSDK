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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RSDK5
{
    public class AnimationEntry : IAnimationEntry
    {
        private int _collisionBoxesCount;

        public List<Frame> Frames { get; private set; } = new List<Frame>();

        public string Name { get; set; }
        
        public int Speed { get; set; }

        public int Loop { get; set; }

        public int Flags { get; set; }

        public AnimationEntry(int collisionBoxesCount)
        {
            _collisionBoxesCount = collisionBoxesCount;
        }

        public AnimationEntry(BinaryReader reader, int collisionBoxesCount)
        {
            _collisionBoxesCount = collisionBoxesCount;
            SubRead(reader, collisionBoxesCount, collisionBoxesCount);
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

        public override string ToString()
        {
            return string.IsNullOrEmpty(Name) ?
                "<new animation>" : Name;
        }

        public object Clone()
        {
            return new AnimationEntry(_collisionBoxesCount)
            {
                Frames = Frames.Select(x => x.Clone() as Frame).ToList(),
                Name = Name,
                Speed = Speed,
                Loop = Loop,
                Flags = Flags
            };
        }

        // This part is tricky...
        // Sonic Mania has a different hitboxes count. If you import an
        // animation that has a different hitboxes count than the acutal
        // animation, we need to handle this special case.
        private void SubRead(BinaryReader reader, int cbDst, int cbSrc)
        {
            Name = StringEncoding.GetString(reader);

            var framesCount = reader.ReadInt16();
            Speed = reader.ReadInt16();
            Loop = reader.ReadByte();
            Flags = reader.ReadByte();
            for (int i = 0; i < framesCount; i++)
                Frames.Add(new Frame(reader, cbDst));
        }

        private void SubWrite(BinaryWriter writer)
        {
            writer.Write(StringEncoding.GetBytes(Name));
            writer.Write((short)Frames.Count);
            writer.Write((short)Speed);
            writer.Write((byte)Loop);
            writer.Write((byte)Flags);
            foreach (var entry in Frames)
                entry.SaveChanges(writer);
        }
    }
}

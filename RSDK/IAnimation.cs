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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSDK
{
    public class Hitbox
    {
        public int Left { get; set; }

        public int Top { get; set; }

        public int Right { get; set; }

        public int Bottom { get; set; }

        public Hitbox() { }

        public Hitbox(BinaryReader reader)
        {
            Left = reader.ReadSByte();
            Top = reader.ReadSByte();
            Right = reader.ReadSByte();
            Bottom = reader.ReadSByte();
        }

        public void SaveChanges(BinaryWriter writer)
        {
            writer.Write((sbyte)Left);
            writer.Write((sbyte)Top);
            writer.Write((sbyte)Right);
            writer.Write((sbyte)Bottom);
        }
    }

    public interface IFrame
    {
        int SpriteSheet { get; set; }

        int CollisionBox { get; set; }

        int X { get; set; }

        int Y { get; set; }

        int Width { get; set; }

        int Height { get; set; }

        int CenterX { get; set; }

        int CenterY { get; set; }
    }

    public interface IAnimationEntry
    {
        string Name { get; set; }

        int Speed { get; set; }

        int Loop { get; set; }

        int Flags { get; set; }

        IEnumerable<IFrame> GetFrames();

        void SetFrames(IEnumerable<IFrame> frames);
    }

    public interface IHitboxEntry
    {
        Hitbox Floor { get; set; }
        Hitbox Ceiling { get; set; }
        Hitbox WallLeft { get; set; }
        Hitbox WallRight { get; set; }
        Hitbox Unk04 { get; set; }
        Hitbox Unk05 { get; set; }
        Hitbox Unk06 { get; set; }
        Hitbox Unk07 { get; set; }
    }

    public interface IAnimation
    {
        List<string> SpriteSheets { get; }

        // T Factory<T>();
        void Factory(out IAnimationEntry o);
        void Factory(out IFrame o);
        void Factory(out IHitboxEntry o);

        IEnumerable<IAnimationEntry> GetAnimations();

        void SetAnimations(IEnumerable<IAnimationEntry> animations);

        IEnumerable<IHitboxEntry> GetHitboxes();

        void SetHitboxes(IEnumerable<IHitboxEntry> hitboxes);

        void SaveChanges(BinaryWriter writer);
    }
}

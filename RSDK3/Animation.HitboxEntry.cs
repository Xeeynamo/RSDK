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
using System.IO;

namespace RSDK3
{
    public class HitboxEntry : IHitboxEntry
    {
        public IHitbox Floor { get; set; }
        public IHitbox Ceiling { get; set; }
        public IHitbox WallLeft { get; set; }
        public IHitbox WallRight { get; set; }
        public IHitbox Unk04 { get; set; }
        public IHitbox Unk05 { get; set; }
        public IHitbox Unk06 { get; set; }
        public IHitbox Unk07 { get; set; }

        public HitboxEntry()
        {
            Floor = new Hitbox();
            Ceiling = new Hitbox();
            WallLeft = new Hitbox();
            WallRight = new Hitbox();
            Unk04 = new Hitbox();
            Unk05 = new Hitbox();
            Unk06 = new Hitbox();
            Unk07 = new Hitbox();
        }

		public HitboxEntry(BinaryReader reader)
        {
            Floor = new Hitbox(reader);
            Ceiling = new Hitbox(reader);
            WallLeft = new Hitbox(reader);
            WallRight = new Hitbox(reader);
            Unk04 = new Hitbox(reader);
            Unk05 = new Hitbox(reader);
            Unk06 = new Hitbox(reader);
            Unk07 = new Hitbox(reader);
        }

		public void SaveChanges(BinaryWriter writer)
        {
            Floor.SaveChanges(writer);
            Ceiling.SaveChanges(writer);
            WallLeft.SaveChanges(writer);
            WallRight.SaveChanges(writer);
            Unk04.SaveChanges(writer);
            Unk05.SaveChanges(writer);
            Unk06.SaveChanges(writer);
            Unk07.SaveChanges(writer);
        }
    }
}

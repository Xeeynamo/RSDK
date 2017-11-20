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

namespace RSDK5
{
    public class Frame : IFrame
    {
        public int SpriteSheet { get; set; }

        public int CollisionBox { get; set; }

        public int Duration { get; set; } // Make sure this is correct

        public ushort Id { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int CenterX { get; set; }

        public int CenterY { get; set; }

        public Hitbox[] Hitboxes { get; set; }
        
        public Frame(int hitboxesCount = 0)
        {
            Hitboxes = new Hitbox[hitboxesCount];
            for (int i = 0; i < hitboxesCount; i++)
            {
                Hitboxes[i] = new Hitbox();
            }
        }

        // TODO
        public IHitbox GetHitbox(int index)
        {
            if (Hitboxes.Length >= 0 && index < Hitboxes.Length)
                return Hitboxes[index];
            else
                return null;
        }
    }
}

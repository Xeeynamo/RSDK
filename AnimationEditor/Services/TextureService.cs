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

// Part of this software belongs to XeEngine toolset and United Lines Studio
// and it is currently used to create commercial games by Luciano Ciccariello.
// Please do not redistribuite this code under your own name, stole it or use
// it artfully, but instead support it and its author. Thank you.

// Part of this software belongs to XeEngine toolset and United Lines Studio
// and it is currently used to create commercial games by Luciano Ciccariello.
// Please do not redistribuite this code under your own name, stole it or use
// it artfully, but instead support it and its author. Thank you.

using RSDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AnimationEditor.Services
{
    public class SpriteService
    {
        private IAnimation _animationData;
        public string BasePath { get; set; }

        private Dictionary<string, BitmapSource> _textures =
            new Dictionary<string, BitmapSource>(24);
        private Dictionary<Tuple<string, int>, BitmapSource> _frames =
            new Dictionary<Tuple<string, int>, BitmapSource>(1024);
        
        public BitmapSource this[int texture, IFrame frame]
        {
            get
            {
                if (texture < 0 || texture >= _animationData.SpriteSheets.Count || frame == null)
                    return null;
                var name = _animationData.SpriteSheets[texture];
                var tuple = new Tuple<string, int>(name, frame.GetHashCode());
                if (_frames.TryGetValue(tuple, out BitmapSource bitmap))
                    return bitmap;

                //if (!frame.IsEmpty)
                if (frame.Width > 0 && frame.Height > 0)
                {
                    var textureBitmap = GetTexture(texture);
                    try
                    {
                        bitmap = new CroppedBitmap(textureBitmap,
                        new System.Windows.Int32Rect()
                        {
                            X = frame.X,
                            Y = frame.Y,
                            Width = frame.Width,
                            Height = frame.Height
                        });
                    }
                    catch (ArgumentException)
                    {
                    }
                }
                else
                {
                    bitmap = BitmapSource.Create(1, 1, 96, 96, PixelFormats.Bgr24, null, new byte[3] { 0, 0, 0 }, 3);
                }
                return _frames[tuple] = bitmap;
            }
        }

        public SpriteService(IAnimation animation, string basePath)
        {
            _animationData = animation;
            BasePath = basePath;
        }

        private BitmapSource GetTexture(int index)
        {
            if (index < 0 || index >= _animationData.SpriteSheets.Count)
                return null;
            var name = _animationData.SpriteSheets[index];
            if (_textures.TryGetValue(name, out var bitmap))
                return bitmap;
            var fileName = Path.Combine(BasePath, name);
            bitmap = ImageService.Open(fileName);
            _textures.Add(name, bitmap);
            return bitmap;
        }

        public void Invalidate(int texture, IFrame frame)
        {
            if (texture < 0 || texture >= _animationData.SpriteSheets.Count)
                return;
            var name = _animationData.SpriteSheets[texture];
            _frames.Remove(new Tuple<string, int>(name, frame.GetHashCode()));
        }

        public void InvalidateAll()
        {
            _textures.Clear();
            _frames.Clear();
        }
    }
}

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

using RSDK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;

namespace AnimationEditor.Services
{
    public class AnimationService
    {
        private string _animationName;
        private int _frameIndex;
        private IAnimationEntry _currentAnimation;
        private IFrame _currentFrame;
        private Dictionary<string, IAnimationEntry> _dicAnimations;
        private long _previousTime;
        private long _discardedTime;
        private object _lockTime = new object();

        public event Action<AnimationService> OnFrameChanged;

        public IAnimation AnimationData { get; private set; }

        public Timer Timer { get; private set; }
        public Stopwatch Stopwatch { get; private set; }

        /// <summary>
        /// Get or set if the animation system is running
        /// </summary>
        public bool IsRunning
        {
            get => Timer.Enabled;
            set
            {
                if (Timer.Enabled != value)
                {
                    if (value)
                    {
                        Stopwatch.Start();
                    }
                    else
                    {
                        Stopwatch.Stop();
                    }
                    Timer.Enabled = value;
                }
            }
        }

        /// <summary>
        /// Get the number of frames per second of selected animation
        /// </summary>
        public int FramesPerSecond
        {
            get
            {
                var speed = CurrentAnimation?.Speed ?? 0;
                return speed > 0 ? speed / 4 : 0;
            }
        }

        /// <summary>
        /// Get or set the name of current animation
        /// </summary>
        public string Animation
        {
            get => _animationName;
            set
            {
                _animationName = value;
                // Validate the name
                if (!string.IsNullOrWhiteSpace(value))
                {
                    // Get the animation object from its name
                    if (_dicAnimations.TryGetValue(value, out _currentAnimation))
                    {
                        // Reset the timer
                        if (IsRunning)
                        {
                            Stopwatch.Restart();
                        }
                        else
                        {
                            Stopwatch.Reset();
                        }
                        // Reset the frame index
                        _frameIndex = -1;
                        FrameIndex = 0;
                    }
                }
                else
                {
                    Stopwatch.Reset();
                }
            }
        }

        /// <summary>
        /// Get or set the current frame index
        /// </summary>
        public int FrameIndex
        {
            get => _frameIndex;
            set
            {
                if (_frameIndex != value)
                {
                    _frameIndex = value;
                    _currentFrame = CurrentAnimation?.GetFrames()
                        .Skip(_frameIndex).FirstOrDefault();
                    OnFrameChanged?.Invoke(this);
                }
            }
        }

        /// <summary>
        /// Get the current animation object loaded
        /// </summary>
        public IAnimationEntry CurrentAnimation => _currentAnimation;

        public IFrame CurrentFrame => _currentFrame;
        
        /// <summary>
        /// Initialize a new instance of an animation service
        /// </summary>
        /// <param name="animData">Animation data where information are loaded</param>
        public AnimationService(IAnimation animData)
        {
            AnimationData = animData;
            Timer = new Timer(1);
            Timer.Elapsed += Timer_Elapsed;
            Stopwatch = new Stopwatch();
            
            _dicAnimations = AnimationData.GetAnimations()
                .ToDictionary(x => x.Name, x => x);

            Timer.Enabled = true;
        }

        /// <summary>
        /// Change the frame's reference from the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="frame"></param>
        public void SetFrame(int index, string frameName)
        {
            if (FrameIndex != index && index >= 0)
                FrameIndex = index;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (IsRunning)
            {
                var currentTime = Stopwatch.ElapsedMilliseconds;
                lock (_lockTime)
                {
                    var elapsed = _discardedTime + (currentTime - _previousTime);
                    _previousTime = currentTime;
                    _discardedTime = Update(elapsed);
                }
            }
        }

        /// <summary>
        /// Update animation state by the specified number of milliseconds.
        /// </summary>
        /// <param name="ms">Number of elapsed milliseconds.</param>
        /// <returns>Discarded milliseconds.</returns>
        private long Update(long ms)
        {
            var curAnim = CurrentAnimation;
            if (ms < 0 || curAnim == null ||
                (CurrentAnimation?.Speed ?? 0) <= 0)
                return 0;

            int framesCount = curAnim.GetFrames().Count();
            int loop = curAnim.Loop;
            if (framesCount <= 0)
                return 0;

            const int Divisor = 1024;
            var baseSpeed = CurrentAnimation.Speed;
            long prevMs;
            do
            {
                prevMs = ms;
                var frameSpeed = CurrentFrame?.Duration ?? 256;
                if (frameSpeed > 0)
                {
                    var realSpeed = (baseSpeed * 64 / frameSpeed);
                    if (realSpeed > 0)
                    {
                        var acutalSpeed = Divisor / realSpeed;
                        if (ms > acutalSpeed && acutalSpeed > 0)
                        {
                            ms -= acutalSpeed;
                            FrameIndex = BoundFrameIndex(FrameIndex + 1, curAnim.Loop, framesCount);
                        }
                    }
                }
            } while (prevMs != ms);
            return ms;
        }

        /// <summary>
        /// Move between frames index without to exit from frames boundaries.
        /// </summary>
        /// <param name="index">Frame index to adjust.</param>
        /// <param name="loop">Loop index</param>
        /// <param name="framesCount">Total number of frames.</param>
        /// <returns>New frame index</returns>
        private int BoundFrameIndex(int index, int loop, int framesCount)
        {
            if (index >= framesCount)
            {
                index = loop >= index ? framesCount - 1 : loop;
            }
            else if (index < 0)
            {
                index = 0;
            }
            return index;
        }
    }
}

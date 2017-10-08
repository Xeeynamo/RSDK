using AnimationEditor.Services;
using RSDK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace AnimationEditor.ViewModels
{
    public class MainViewModel : Xe.Tools.Wpf.BaseNotifyPropertyChanged
    {
        private string _pathMod;
        private string _fileName;
        private IAnimation _animationData;
        private IAnimationEntry _selectedAnimation;
        private SpriteService _spriteService;
        private AnimationService _animService;

        #region Animation data

        public string FileName
        {
            get => _fileName;
            set
            {
                _fileName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Title));
            }
        }

        public string Title => string.IsNullOrEmpty(FileName) ? "RSDK Animation Editor" : $"RSDK Animation Editor - {Path.GetFileName(FileName)}";
        
        public ObservableCollection<string> Textures { get; private set; }

        public ObservableCollection<IAnimationEntry> Animations { get; private set; }

        public List<HitboxViewModel> Hitboxes { get; private set; }

        public IAnimation AnimationData
        {
            get => _animationData;
            set
            {
                _animationData = value;
                var basePath = Path.GetDirectoryName(_fileName);
                basePath = Path.Combine(basePath, _pathMod);

                Textures = new ObservableCollection<string>(_animationData.SpriteSheets);
                Animations = new ObservableCollection<IAnimationEntry>(_animationData.GetAnimations());
                Hitboxes = _animationData.GetHitboxes()?.Select(x => new HitboxViewModel() { Hitbox = x.Floor }).ToList() ?? new List<HitboxViewModel>();
                _animService = new AnimationService(_animationData);
                _animService.OnFrameChanged += OnFrameChanged;
                _spriteService = new SpriteService(_animationData, basePath);

                OnPropertyChanged(nameof(IsAnimationDataLoaded));
                OnPropertyChanged(nameof(Textures));
                OnPropertyChanged(nameof(Animations));
                OnPropertyChanged(nameof(Hitboxes));
            }
        }

        public bool IsAnimationDataLoaded => AnimationData != null;

        #endregion

        #region Animation view view

        private double _viewWidth, _viewHeight, _zoom = 1.0;

        public double ViewWidth
        {
            get => _viewWidth;
            set
            {
                _viewWidth = value;
                OnPropertyChanged(nameof(SpriteLeft));
                OnPropertyChanged(nameof(SpriteTop));
                OnPropertyChanged(nameof(SpriteRight));
                OnPropertyChanged(nameof(SpriteBottom));
                OnPropertyChanged(nameof(SpriteCenter));
            }
        }

        public double ViewHeight
        {
            get => _viewHeight;
            set
            {
                _viewHeight = value;
                OnPropertyChanged(nameof(SpriteLeft));
                OnPropertyChanged(nameof(SpriteTop));
                OnPropertyChanged(nameof(SpriteRight));
                OnPropertyChanged(nameof(SpriteBottom));
                OnPropertyChanged(nameof(SpriteCenter));
            }
        }

        public double Zoom
        {
            get => _zoom;
            set
            {
                _zoom = Math.Max(Math.Min(value, 16), 0.25);
                OnPropertyChanged();
                InvalidateCanvas();
            }
        }

        public BitmapSource Sprite => _spriteService?[SelectedFrameTexture, _animService.CurrentFrame];

        public double SpriteLeft => ViewWidth / 2.0 + _animService?.CurrentFrame?.CenterX ?? 0;
        public double SpriteTop => ViewHeight / 2.0 + _animService?.CurrentFrame?.CenterY ?? 0;
        public double SpriteRight => SpriteLeft + _animService?.CurrentFrame?.Width ?? 0;
        public double SpriteBottom => SpriteTop + _animService?.CurrentFrame?.Height ?? 0;
        public Point SpriteCenter
        {
            get
            {
                var frame = _animService?.CurrentFrame;
                if (frame != null)
                {
                    return new Point((double)-frame.CenterX / frame.Width, (double)-frame.CenterY / frame.Height);
                }
                return new Point(0.5, 0.5);
            }
        }
        public double SpriteScaleX => Zoom;
        public double SpriteScaleY => Zoom;
        
        public bool IsRunning
        {
            get => _animService?.IsRunning ?? false;
            set
            {
                _animService.IsRunning = value;
                OnPropertyChanged(nameof(IsNotRunning));
            }
        }
        public bool IsNotRunning => !IsRunning;

        #endregion

        #region Current animation properties

        public ObservableCollection<FrameViewModel> AnimationFrames { get; private set; }

        public IAnimationEntry SelectedAnimation
        {
            get => _selectedAnimation;
            set
            {
                if (_animService == null)
                    return;

                _selectedAnimation = value;
                _animService.Animation = value?.Name;

                if (_selectedAnimation != null)
                {
                    ChangeAllFrames();
                }
                else
                {
                    AnimationFrames = null;
                }
                OnPropertyChanged(nameof(AnimationFrames));

                OnPropertyChanged(nameof(IsAnimationSelected));
                OnPropertyChanged(nameof(FramesCount));
                OnPropertyChanged(nameof(Speed));
                OnPropertyChanged(nameof(Loop));
                OnPropertyChanged(nameof(Flags));
            }
        }

        public int SelectedAnimationIndex { get; set; }

        public bool IsFrameSelected => SelectedFrame != null;

        public int FramesCount => SelectedAnimation?.GetFrames().Count() ?? 0;

        public int Speed
        {
            get => SelectedAnimation != null ? SelectedAnimation.Speed : 0;
            set => SelectedAnimation.Speed = value;
        }

        public int Loop
        {
            get => SelectedAnimation?.Loop ?? 0;
            set => SelectedAnimation.Loop = value;
        }

        public int Flags
        {
            get => SelectedAnimation?.Flags ?? 0;
            set => SelectedAnimation.Flags = value;
        }

        #endregion

        #region selected frame
        
        public bool IsAnimationSelected => SelectedAnimation != null;

        public int SelectedFrameIndex
        {
            get => _animService?.FrameIndex ?? 0;
            set
            {
                if (value >= 0)
                {
                    _animService.FrameIndex = value;
                }
            }
        }

        public IFrame SelectedFrame => _animService?.CurrentFrame;


        /// <summary>
        /// Get or set the texture for the selected animation
        /// </summary>
        public int SelectedFrameTexture
        {
            get => SelectedFrame?.SpriteSheet ?? 0;
            set
            {
                if (SelectedFrame != null)
                {
                    SelectedFrame.SpriteSheet = value;
                    _spriteService.InvalidateAll();
                    ChangeAllFrames();
                }
            }
        }

        public int SelectedFrameHitbox
        {
            get => SelectedFrame?.CollisionBox ?? 0;
            set
            {
                SelectedFrame.CollisionBox = value;
            }
        }

        public int SelectedFrameLeft
        {
            get => SelectedFrame?.X ?? 0;
            set
            {
                SelectedFrame.X = value;
                CurrentFrameChanged();
                InvalidateCanvas();
            }
        }

        public int SelectedFrameTop
        {
            get => SelectedFrame?.Y ?? 0;
            set
            {
                SelectedFrame.Y = value;
                CurrentFrameChanged();
                InvalidateCanvas();
            }
        }

        public int SelectedFrameWidth
        {
            get => SelectedFrame?.Width ?? 0;
            set
            {
                SelectedFrame.Width = value;
                CurrentFrameChanged();
                InvalidateCanvas();
            }
        }

        public int SelectedFrameHeight
        {
            get => SelectedFrame?.Height ?? 0;
            set
            {
                SelectedFrame.Height = value;
                CurrentFrameChanged();
                InvalidateCanvas();
            }
        }

        public int SelectedFramePivotX
        {
            get => SelectedFrame?.CenterX ?? 0;
            set
            {
                SelectedFrame.CenterX = value;
                InvalidateCanvas();
            }
        }

        public int SelectedFramePivotY
        {
            get => SelectedFrame?.CenterY ?? 0;
            set
            {
                SelectedFrame.CenterY = value;
                InvalidateCanvas();
            }
        }

        #endregion

        #region methods

        private void InvalidateCanvas()
        {
            OnPropertyChanged(nameof(Sprite));
            OnPropertyChanged(nameof(SpriteLeft));
            OnPropertyChanged(nameof(SpriteTop));
            OnPropertyChanged(nameof(SpriteRight));
            OnPropertyChanged(nameof(SpriteBottom));
            OnPropertyChanged(nameof(SpriteCenter));
            OnPropertyChanged(nameof(SpriteScaleX));
            OnPropertyChanged(nameof(SpriteScaleY));
        }

        public void InvalidateFrameProperties()
        {
            OnPropertyChanged(nameof(IsFrameSelected));
            OnPropertyChanged(nameof(SelectedFrameIndex));
            OnPropertyChanged(nameof(SelectedFrameTexture));
            OnPropertyChanged(nameof(SelectedFrameHitbox));
            OnPropertyChanged(nameof(SelectedFrameLeft));
            OnPropertyChanged(nameof(SelectedFrameTop));
            OnPropertyChanged(nameof(SelectedFrameWidth));
            OnPropertyChanged(nameof(SelectedFrameHeight));
            OnPropertyChanged(nameof(SelectedFramePivotX));
            OnPropertyChanged(nameof(SelectedFramePivotY));
        }

        private void OnFrameChanged(AnimationService service)
        {
            InvalidateCanvas();
            InvalidateFrameProperties();
        }

        public bool FileOpen(string fileName)
        {
            if (File.Exists(fileName))
            {
                var ext = Path.GetExtension(fileName);
                using (var fStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                {
                    using (var reader = new BinaryReader(fStream))
                    {
                        FileName = fileName;
                        switch (ext)
                        {
                            case ".ani":
                                _pathMod = "..\\sprites";
                                AnimationData = new RSDK3.Animation(reader);
                                break;
                            case ".bin":
                                _pathMod = "..";
                                AnimationData = new RSDK5.Animation(reader);
                                return false;
                            default:
                                return false;
                        }
                    }
                }
                return true;
            }
            return false;
        }

        public void FileSave(string fileName = null)
        {
            SaveChanges();

            if (string.IsNullOrWhiteSpace(fileName))
                fileName = FileName;

            using (var fStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                using (var writer = new BinaryWriter(fStream))
                {
                    _animationData.SaveChanges(writer);
                }
            }
            FileName = fileName;
        }

        public void AnimationAdd()
        {
            _animationData.Factory(out IAnimationEntry o);
            Animations.Add(o);
        }
        public void AnimationRemove()
        {
            Animations.Remove(SelectedAnimation);
        }
        public void FrameAdd()
        {
            _animationData.Factory(out IFrame o);
            AnimationFrames.Add(new FrameViewModel(_spriteService, o));
            var frames = SelectedAnimation.GetFrames().ToList();
            frames.Add(o);
            SelectedAnimation.SetFrames(frames);
        }
        public void FrameRemove()
        {
            if (SelectedFrameIndex >= 0)
            {
                AnimationFrames.RemoveAt(SelectedFrameIndex);
                var frames = SelectedAnimation.GetFrames().ToList();
                frames.RemoveAt(SelectedFrameIndex);
                SelectedAnimation.SetFrames(frames);
            }
        }

        public void CurrentFrameChanged()
        {
            var curAnim = SelectedAnimation;
            var curFrameIndex = SelectedFrameIndex;
            if (curAnim != null && curFrameIndex >= 0 &&
                curFrameIndex < curAnim.GetFrames().Count())
            {
                var animationFrames = AnimationFrames;
                var item = animationFrames[curFrameIndex];
                animationFrames.RemoveAt(curFrameIndex);
                animationFrames.Insert(curFrameIndex, item);
                SelectedFrameIndex = curFrameIndex;
                _spriteService.Invalidate(SelectedFrameTexture, item.Frame);
                OnPropertyChanged(nameof(SelectedFrameIndex));
            }
        }

        private void ChangeAllFrames()
        {
            AnimationFrames = new ObservableCollection<FrameViewModel>(
                _selectedAnimation.GetFrames()
                    .Select(x => new FrameViewModel(_spriteService, x)));
            OnPropertyChanged(nameof(AnimationFrames));
        }

        public bool ChangeCurrentAnimationName(string name)
        {
            if (Animations.Any(x => x.Name == name))
                return false;

            SelectedAnimation.Name = name;
            var index = SelectedAnimationIndex;
            var item = Animations[index];
            Animations.RemoveAt(index);
            Animations.Insert(index, item);
            SelectedAnimationIndex = index;
            OnPropertyChanged(nameof(SelectedAnimationIndex));
            return true;
        }

        public void SaveChanges()
        {
            _animationData.SetAnimations(Animations);
        }
        #endregion
    }
}

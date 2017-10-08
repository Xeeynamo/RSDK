using RSDK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimationEditor.ViewModels
{
    public class HitboxV3EditorViewModel : Xe.Tools.Wpf.BaseNotifyPropertyChanged
    {
        private IAnimation _animationData;
        private int _selectedHitboxEntryIndex;
        private int _selectedHitboxIndex;

        public ObservableCollection<IHitboxEntry> HitboxEntries { get; }

        public ObservableCollection<string> HitboxItems { get; }

        public IEnumerable<string> AnimationsUsed { get; set; }

        public bool CanHitboxBeingRemoved => (AnimationsUsed?.Count() ?? 0) == 0 && IsHitboxEntrySelected;

        #region Hitbox entries

        public int SelectedHitboxEntryIndex
        {
            get => _selectedHitboxEntryIndex;
            set
            {
                _selectedHitboxEntryIndex = value;
                AnimationsUsed = _animationData.GetAnimations()
                    .SelectMany(x => x.GetFrames(), (a, f) => new
                    {
                        Name = a.Name,
                        Hitbox = f.CollisionBox
                    })
                    .Where(x => x.Hitbox == value)
                    .Select(x => x.Name)
                    .Distinct();

                OnPropertyChanged();
                OnPropertyChanged(nameof(CanHitboxBeingRemoved));
                OnPropertyChanged(nameof(IsHitboxEntrySelected));
                OnPropertyChanged(nameof(SelectedHitboxEntryValue));
                OnPropertyChanged(nameof(AnimationsUsed));
                var entry = SelectedHitboxEntryValue;
            }
        }

        public bool IsHitboxEntrySelected => SelectedHitboxEntryIndex >= 0;

        public IHitboxEntry SelectedHitboxEntryValue
        {
            get
            {
                IHitboxEntry entry;
                var index = SelectedHitboxEntryIndex;
                if (index < 0 || HitboxEntries == null || (entry = HitboxEntries[index]) == null)
                    _animationData.Factory(out entry);
                return entry;
            }
        }

        #endregion

        #region Hitbox

        public int SelectedHitboxIndex
        {
            get => _selectedHitboxIndex;
            set
            {
                _selectedHitboxIndex = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsHitboxEntrySelected));
                OnPropertyChanged(nameof(SelectedHitboxValue));
                var entry = SelectedHitboxEntryValue;
            }
        }

        public bool IsHitboxSelected => SelectedHitboxIndex >= 0;

        public IHitbox SelectedHitboxValue => SelectedHitboxEntryValue.GetHitbox(SelectedHitboxIndex);

        #endregion
        
        public HitboxV3EditorViewModel(MainViewModel vm)
        {
            _animationData = vm.AnimationData;
            HitboxEntries = vm.HitboxEntries;
            HitboxItems = vm.HitboxItems;
        }

        public void AddHitboxEntry()
        {
            _animationData.Factory(out IHitboxEntry entry);
            HitboxEntries.Add(entry);
            HitboxItems.Add(GetHitboxEntryString(entry));
            SelectedHitboxEntryIndex = HitboxEntries.Count - 1;
        }

        public void RemoveHitboxEntry()
        {
            HitboxEntries.RemoveAt(SelectedHitboxEntryIndex);
            HitboxItems.RemoveAt(SelectedHitboxEntryIndex);
        }

        private static string GetHitboxEntryString(IHitboxEntry entry)
        {
            return MainViewModel.GetHitboxEntryString(entry);
        }
        private static string GetHitboxString(IHitbox hb)
        {
            return MainViewModel.GetHitboxString(hb);
        }
    }
}

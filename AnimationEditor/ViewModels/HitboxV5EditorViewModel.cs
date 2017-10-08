using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimationEditor.ViewModels
{
    public class HitboxV5EditorViewModel : Xe.Tools.Wpf.BaseNotifyPropertyChanged
    {
        private string _selectedValue;
        private int _selectedIndex;

        public ObservableCollection<string> HitboxTypeItems { get; set; }

        public string SelectedValue
        {
            get => _selectedValue;
            set
            {
                _selectedValue = value;
                OnPropertyChanged(nameof(SelectedValue));
            }
        }

        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                _selectedIndex = value;
                OnPropertyChanged(nameof(IsValueSelected));
            }
        }

        public bool IsValueSelected => SelectedIndex >= 0;

        public HitboxV5EditorViewModel(MainViewModel vm)
        {
            HitboxTypeItems = vm.HitboxTypes;
        }

        public void UpdateList(string value)
        {
            var index = SelectedIndex;
            if (index >= 0)
            {
                HitboxTypeItems.RemoveAt(index);
                HitboxTypeItems.Insert(index, value);
            }
        }
    }
}

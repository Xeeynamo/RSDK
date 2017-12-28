using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimationEditor.ViewModels
{
    public class TextureWindowViewModel : Xe.Tools.Wpf.BaseNotifyPropertyChanged
    {
        private string BasePath { get; set; }
        private string _selectedValue;

        public TextureWindowViewModel(MainViewModel mainViewModel, string basePath)
        {
            BasePath = basePath;
            Textures = mainViewModel.Textures;
        }

        public ObservableCollection<string> Textures { get; set; }

        public TextureViewModel CurrentTexture { get; set; }

        public int Count => Textures.Count;

        public bool IsValueSelected => SelectedValue != null;

        public string SelectedValue
        {
            get => _selectedValue;
            set
            {
                _selectedValue = value;
                if (_selectedValue != null)
                    CurrentTexture = new TextureViewModel(value, BasePath);
                else
                    CurrentTexture = null;
                OnPropertyChanged(nameof(SelectedValue));
                OnPropertyChanged(nameof(IsValueSelected));
                OnPropertyChanged(nameof(CurrentTexture));
            }
        }

        public int SelectedIndex { get; set; }

        #region methods
        
        public int AddTexture(string texture)
        {
            Textures.Add(texture);
            return Textures.Count - 1;
        }

        public void ReplaceTexture(int index, string texture)
        {
            if (index >= 0 && index < Count)
            {
                Textures[index] = texture;
            }
        }

        public void RemoveTexture(int index, bool physicalDelete)
        {
            if (index >= 0 && index < Count)
            {
                if (physicalDelete)
                {
                    var texture = Textures[index];
                    var filePath = Path.Combine(BasePath, texture);
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                        //Log.Message($"File {fileName} was deleted");
                    }
                    else
                    {
                        //Log.Warning($"File {fileName} was not deleted because it does not exists.");
                    }
                }
                Textures.RemoveAt(index);
            }
            else
            {
                //Log.Error($"Unable to remove the item because index {index} is invalid");
            }
        }

        #endregion
    }
}

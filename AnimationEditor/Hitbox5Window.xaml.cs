using AnimationEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AnimationEditor
{
    /// <summary>
    /// Interaction logic for Hitbox5Window.xaml
    /// </summary>
    public partial class Hitbox5Window : Window
    {
        public HitboxV5EditorViewModel ViewModel => DataContext as HitboxV5EditorViewModel;

        public Hitbox5Window(MainViewModel vm)
        {
            InitializeComponent();
            DataContext = new HitboxV5EditorViewModel(vm);
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.HitboxTypeItems.Add("EMPTY");
        }

        private void ButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.IsValueSelected)
                ViewModel.HitboxTypeItems.RemoveAt(ViewModel.SelectedIndex);
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                ViewModel.UpdateList(textBox.Text);
            }
        }
    }
}

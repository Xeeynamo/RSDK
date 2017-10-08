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
    /// Interaction logic for Hitbox3Window.xaml
    /// </summary>
    public partial class Hitbox3Window : Window
    {
        public HitboxV3EditorViewModel ViewModel => DataContext as HitboxV3EditorViewModel;

        public Hitbox3Window(MainViewModel vm)
        {
            InitializeComponent();
            DataContext = new HitboxV3EditorViewModel(vm);
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AddHitboxEntry();
        }

        private void ButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.RemoveHitboxEntry();
        }
    }
}

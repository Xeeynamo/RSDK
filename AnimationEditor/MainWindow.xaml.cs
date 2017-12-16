using AnimationEditor.ViewModels;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Xe.Tools.Wpf.Dialogs;

namespace AnimationEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel ViewModel => (MainViewModel)DataContext;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void MenuFileOpen_Click(object sender, RoutedEventArgs e)
        {
            var fd = Xe.Tools.Wpf.Dialogs.FileDialog.Factory(this,
                Xe.Tools.Wpf.Dialogs.FileDialog.Behavior.Open,
                Xe.Tools.Wpf.Dialogs.FileDialog.Type.Any, false);
            if (fd.ShowDialog() == true)
            {
                ViewModel.FileOpen(fd.FileName);
            }
        }

        private void MenuFileSave_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.FileSave();
        }

        private void MenuFileSaveAs_Click(object sender, RoutedEventArgs e)
        {
            var fd = Xe.Tools.Wpf.Dialogs.FileDialog.Factory(this,
                Xe.Tools.Wpf.Dialogs.FileDialog.Behavior.Save,
                Xe.Tools.Wpf.Dialogs.FileDialog.Type.Any, false);
            if (fd.ShowDialog() == true)
            {
                ViewModel.FileSave(fd.FileName);
            }
        }

        private void MenuFileExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonAnimationAdd_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AnimationAdd();
        }

        private void ButtonAnimationDuplicate_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AnimationDuplicate();
        }

        private void ButtonAnimationRemove_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AnimationRemove();
        }

        private void ButtonAnimationImport_Click(object sender, RoutedEventArgs e)
        {
            var version = ViewModel.AnimationData.Version;
            Xe.Tools.Wpf.Dialogs.FileDialog.Type fileType;
            switch (version)
            {
                case 3:
                    fileType = Xe.Tools.Wpf.Dialogs.FileDialog.Type.Rsdk3Animation;
                    break;
                case 5:
                    fileType = Xe.Tools.Wpf.Dialogs.FileDialog.Type.Rsdk5Animation;
                    break;
                default:
                    fileType = Xe.Tools.Wpf.Dialogs.FileDialog.Type.Any;
                    break;
            }

            var fd = Xe.Tools.Wpf.Dialogs.FileDialog.Factory(this,
                Xe.Tools.Wpf.Dialogs.FileDialog.Behavior.Open, fileType, false);
            if (fd.ShowDialog() == true)
            {
                ViewModel.AnimationImport(fd.FileName);
            }
        }

        private void ButtonAnimationExport_Click(object sender, RoutedEventArgs e)
        {
            var version = ViewModel.AnimationData.Version;
            Xe.Tools.Wpf.Dialogs.FileDialog.Type fileType;
            switch (version)
            {
                case 3:
                    fileType = Xe.Tools.Wpf.Dialogs.FileDialog.Type.Rsdk3Animation;
                    break;
                case 5:
                    fileType = Xe.Tools.Wpf.Dialogs.FileDialog.Type.Rsdk5Animation;
                    break;
                default:
                    fileType = Xe.Tools.Wpf.Dialogs.FileDialog.Type.Any;
                    break;
            }

            var fd = Xe.Tools.Wpf.Dialogs.FileDialog.Factory(this,
                Xe.Tools.Wpf.Dialogs.FileDialog.Behavior.Save, fileType, false);
            if (fd.ShowDialog() == true)
            {
                ViewModel.AnimationExport(fd.FileName);
            }
        }

        private void ButtonFrameAdd_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.FrameAdd();
        }

        private void ButtonFrameDupe_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.DupeFrame();
        }

        private void ButtonFrameRemove_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.FrameRemove();
        }

        private void ButtonFrameImport_Click(object sender, RoutedEventArgs e)
        {
            var version = ViewModel.AnimationData.Version;
            Xe.Tools.Wpf.Dialogs.FileDialog.Type fileType;
            switch (version)
            {
                case 3:
                    fileType = Xe.Tools.Wpf.Dialogs.FileDialog.Type.Rsdk3Frame;
                    break;
                case 5:
                    fileType = Xe.Tools.Wpf.Dialogs.FileDialog.Type.Rsdk5Frame;
                    break;
                default:
                    fileType = Xe.Tools.Wpf.Dialogs.FileDialog.Type.Any;
                    break;
            }

            var fd = Xe.Tools.Wpf.Dialogs.FileDialog.Factory(this,
                Xe.Tools.Wpf.Dialogs.FileDialog.Behavior.Open, fileType, false);
            if (fd.ShowDialog() == true)
            {
                ViewModel.FrameImport(fd.FileName);
            }
        }

        private void ButtonFrameExport_Click(object sender, RoutedEventArgs e)
        {
            var version = ViewModel.AnimationData.Version;
            Xe.Tools.Wpf.Dialogs.FileDialog.Type fileType;
            switch (version)
            {
                case 3:
                    fileType = Xe.Tools.Wpf.Dialogs.FileDialog.Type.Rsdk3Frame;
                    break;
                case 5:
                    fileType = Xe.Tools.Wpf.Dialogs.FileDialog.Type.Rsdk5Frame;
                    break;
                default:
                    fileType = Xe.Tools.Wpf.Dialogs.FileDialog.Type.Any;
                    break;
            }

            var fd = Xe.Tools.Wpf.Dialogs.FileDialog.Factory(this,
                Xe.Tools.Wpf.Dialogs.FileDialog.Behavior.Save, fileType, false);
            if (fd.ShowDialog() == true)
            {
                ViewModel.FrameExport(fd.FileName);
            }
        }

        private void ButtonZoomIn_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Zoom += 0.25;
        }

        private void ButtonZoomOut_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Zoom -= 0.25;
        }

        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ViewModel.ViewWidth = e.NewSize.Width;
            ViewModel.ViewHeight = e.NewSize.Height;
        }

        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            ViewModel.Zoom += (e.Delta / 120) * 0.25;
        }

        private void List_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var dialog = new SingleInputDialog()
            {
                Text = ViewModel.SelectedAnimation.Name,
                Description = "Please select the name of the animation"
            };

            if (dialog.ShowDialog() == true)
            {
                if (string.IsNullOrWhiteSpace(dialog.Text))
                {
                    MessageBox.Show("You have specified an empty file name.",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (!ViewModel.ChangeCurrentAnimationName(dialog.Text))
                {
                    MessageBox.Show("An animation with the name {dialog.Name} already exists.\nPlease specify another name.",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void MenuViewTexture_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.IsAnimationDataLoaded)
            {
                var basePath = Path.Combine(Path.GetDirectoryName(ViewModel.FileName), ViewModel.PathMod);
                new TextureWindow(ViewModel, basePath).Show();
            }
        }

        private void MenuViewHitbox_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.IsHitboxV3)
                new Hitbox3Window(ViewModel).Show();
            else if (ViewModel.IsHitboxV5)
                new Hitbox5Window(ViewModel).Show();
        }

        private void MenuInfoAbout_Click(object sender, RoutedEventArgs e)
        {
            new AboutWindow().Show();
        }
    }
}

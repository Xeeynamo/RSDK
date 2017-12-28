using AnimationEditor.ViewModels;
using System;
using System.IO;
using System.Windows;
using Xe.Tools.Wpf.Dialogs;

namespace AnimationEditor
{
    /// <summary>
    /// Interaction logic for TextureWindow.xaml
    /// </summary>
    public partial class TextureWindow : Window
    {
        public string BasePath { get; private set; }

        public TextureWindowViewModel ViewModel => DataContext as TextureWindowViewModel;
        public MainViewModel MainViewModel { get; }

        public int SelectedIndex
        {
            get => ViewModel.SelectedIndex;
            set => ViewModel.SelectedIndex = value;
        }

        public TextureWindow(MainViewModel mainViewModel)
        {
            InitializeComponent();
            MainViewModel = mainViewModel;
            DataContext = new TextureWindowViewModel(mainViewModel, BasePath);
        }

        public TextureWindow(MainViewModel mainViewModel, string basePath)
        {
            InitializeComponent();
            BasePath = basePath;
            MainViewModel = mainViewModel;
            DataContext = new TextureWindowViewModel(mainViewModel, BasePath);
        }

        private void ButtonChange_Click(object sender, RoutedEventArgs e)
        {
            var dialog = FileDialog.Factory(this, FileDialog.Behavior.Open, FileDialog.Type.ImageGif);
            if (dialog.ShowDialog() == true)
            {
                var fileName = AddTextureToDirectory(dialog.FileName);
                if (fileName != null)
                {
                    var index = SelectedIndex;
                    ViewModel.ReplaceTexture(SelectedIndex, fileName);
                    SelectedIndex = index;
                }
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = FileDialog.Factory(this, FileDialog.Behavior.Open, FileDialog.Type.ImageGif);
            if (dialog.ShowDialog() == true)
            {
                var fileName = AddTextureToDirectory(dialog.FileName);
                if (fileName != null)
                {
                    ViewModel.AddTexture(fileName);
                    SelectedIndex = ViewModel.Count - 1;
                }
            }
        }

        private void ButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            const string message = "Do you want to delete the file? If you click No, it will be deleted only from this application.";

            var index = SelectedIndex;
            switch (MessageBox.Show(message, "Delete confirmation", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning))
            {
                case MessageBoxResult.Yes:
                    ViewModel.RemoveTexture(SelectedIndex, true);
                    break;
                case MessageBoxResult.No:
                    ViewModel.RemoveTexture(SelectedIndex, false);
                    break;
                default:
                    // Do nothing.
                    index = -1;
                    break;
            }
            if (index >= 0)
            {
                if (index >= ViewModel.Count)
                    index--;
                SelectedIndex = index;
            }
        }

        private void ButtonFramesImport_Click(object sender, RoutedEventArgs e)
        {
            var dialog = FileDialog.Factory(this, FileDialog.Behavior.Open, FileDialog.Type.ImagePng, true);
            if (dialog.ShowDialog() == true)
            {
                /*var spriteService = new SpriteService(ViewModel.SelectedValue.Image, _animationData.Frames);
                spriteService.ImportFrames(dialog.FileNames, 1);
                _animationData.Frames = spriteService.Frames;
                spriteService.Texture.Save(ViewModel.SelectedValue.FileName);
                ViewModel.SelectedValue.Image = spriteService.Texture;*/
            }
        }

        private void ButtonFramesExport_Click(object sender, RoutedEventArgs e)
        {
            var dialog = FileDialog.Factory(this, FileDialog.Behavior.Folder, FileDialog.Type.ImagePng);
            if (dialog.ShowDialog() == true)
            {
                /*var spriteService = new SpriteService(ViewModel.SelectedValue.Image, _animationData.Frames);
                spriteService.ExportFrames(dialog.FileName, (frameName, fileName) =>
                {
                    return true;
                });*/
            }
        }

        /// <summary>
        /// Copy the specified file to animation project's directory
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>File name only</returns>
        private string AddTextureToDirectory(string filePath)
        {
            var basePath = Path.GetFullPath(BasePath);

            var fileName = filePath.Substring(basePath.Length + 1);
            var outputPath = Path.Combine(basePath, fileName);
            if (Path.GetFullPath(filePath) != outputPath)
            {
                if (!File.Exists(outputPath))
                {
                    try
                    {
                        File.Copy(filePath, outputPath);
                    }
                    catch (Exception e)
                    {
                        //Log.Error(e.Message);
                        fileName = null;
                    }
                }
            }
            else
            {
                //Log.Message($"Input and output file {filePath} does match; no need to copy.");
            }
            return fileName.Replace('\\', '/');
        }
    }
}

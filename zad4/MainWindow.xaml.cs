using Azure.Storage.Blobs.Models;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using zad4.ViewModel;

namespace zad4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private readonly ItemsViewModel itemsViewModel;
        public MainWindow()
        {
            InitializeComponent();
            itemsViewModel = new ItemsViewModel();
            Init();
        }

        private void Init()
        {
            cbDirectories.ItemsSource = itemsViewModel.Directories;
            lbItems.ItemsSource = itemsViewModel.Items;
        }

   
        private void LbItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
         => DataContext = lbItems.SelectedItem as BlobItem;

        private void CbDirectories_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                itemsViewModel.Directory = cbDirectories.Text.Trim();
                cbDirectories.Text = itemsViewModel.Directory;
            }
        }

        private void CbDirectories_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (itemsViewModel.Directories.Contains(cbDirectories.Text))
            {
                itemsViewModel.Directory = cbDirectories.Text;
                cbDirectories.SelectedItem = itemsViewModel.Directory;
            }
        }

       
        

        private async void BtnUpload_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.tiff;*.svg;*.....";
            if (openFileDialog.ShowDialog() == true)
            {
                var dir =  Path.GetExtension(openFileDialog.FileName).TrimStart('.');
                await itemsViewModel.UploadAsync(openFileDialog.FileName, dir);
                cbDirectories.Text = dir;
            }

        }

        private async void BtnDownload_Click(object sender, RoutedEventArgs e)
        {
            if (lbItems.SelectedItem is not BlobItem item)
            {
                return;
            }
            var saveFileDialog = new SaveFileDialog()
            {
                FileName = item.Name[(item.Name.LastIndexOf(ItemsViewModel.ForwardSlash) + 1)..]
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                await itemsViewModel.DownloadAsync(item, saveFileDialog.FileName);
            }
            cbDirectories.Text = itemsViewModel.Directory;
            

        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Realy delete?",
                     "Delete Item",
                     MessageBoxButton.YesNo,
                     MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
            if (lbItems.SelectedItem is not BlobItem item)
            {
                return;
            }
            await itemsViewModel.DeleteAsync(item);
            cbDirectories.Text = itemsViewModel.Directory;
            }


        }

    }
}

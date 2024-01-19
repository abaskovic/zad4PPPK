using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zad4.Dal;

namespace zad4.ViewModel
{
    internal class ItemsViewModel
    {

        public const string ForwardSlash = "/";

        public ObservableCollection<BlobItem> Items { get; }
        public ObservableCollection<String> Directories { get; }

        private string? directory;
        public string? Directory
        {
            get => directory;
            set
            {
                directory = value;
                Refresh();
            }
        }
        public ItemsViewModel()
        {
            Items = new ObservableCollection<BlobItem>();
            Directories = new ObservableCollection<string>();
            Refresh();

        }


        private void Refresh()
        {
            Directories.Clear();
            Items.Clear();
            Repository.Container.GetBlobs().ToList().ForEach(item =>
            {
                if (item.Name.Contains(ForwardSlash))
                {
                    var directory = item.Name[..item.Name.LastIndexOf(ForwardSlash)];
                    if (!Directories.Contains(directory))
                    {
                        Directories.Add(directory);
                    }
                }
                if (string.IsNullOrEmpty(Directory) && !item.Name.Contains(ForwardSlash))
                {
                    Items.Add(item);
                }
                else if (!string.IsNullOrEmpty(Directory) && item.Name.StartsWith($"{Directory}{ForwardSlash}"))
                {
                    Items.Add(item);
                }



            });
        }




        public async Task UploadAsync(string path, string directory)
        {
            var filename = path[(path.LastIndexOf(Path.DirectorySeparatorChar) + 1)..];
            if (!string.IsNullOrEmpty(directory))
            {
                filename = $"{directory}{ForwardSlash}{filename}";
            }
            using var fs = File.OpenRead(path);
            await Repository.Container.GetBlobClient(filename).UploadAsync(fs, true);
            Refresh();
        }
        public async Task DownloadAsync(BlobItem blobItem, string path)
        {            
            using var fs = File.OpenWrite(path);
            await Repository.Container.GetBlobClient(blobItem.Name).DownloadToAsync(fs);
        }
         public async Task DeleteAsync(BlobItem blobItem)
        {            
            await Repository.Container.GetBlobClient(blobItem.Name).DeleteAsync();
            Refresh();

        }

    }
}

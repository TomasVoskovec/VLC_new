using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace VLC_new
{
    /// <summary>
    /// Interakční logika pro MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Dictionary<Button, string> filePaths = new Dictionary<Button, string>();
        List<string> playlistPaths = new List<string>();

        int curPlaylistItem = 0;

        public MainWindow()
        {
            InitializeComponent();

            string currentDirectory = Directory.GetCurrentDirectory();

            DirectoryInfo vlcLibDirectory = new DirectoryInfo(System.IO.Path.Combine(currentDirectory, "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));

            MyControl.SourceProvider.CreatePlayer(vlcLibDirectory);

            MyControl.SourceProvider.MediaPlayer.EndReached += new EventHandler<Vlc.DotNet.Core.VlcMediaPlayerEndReachedEventArgs>(VideoEndAction);
        }

        private void VideoEndAction(object sender, EventArgs e)
        {
            curPlaylistItem++;

            if (curPlaylistItem >= playlistPaths.Count())
            {
                curPlaylistItem = 0;
            }

            MyControl.SourceProvider.MediaPlayer;
            MyControl.SourceProvider.MediaPlayer.Play();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "Find video or audio file",
                InitialDirectory = @"C:\Users\Tomas\Documents\Projects\C#\VLC\VLC\VLC\videos",
                Multiselect = true
            };

            ofd.ShowDialog();

            foreach (string path in ofd.FileNames)
            {
                Uri uri = new Uri(path);
                string fileName = System.IO.Path.GetFileName(path);

                ContextMenu contextMenu = new ContextMenu();

                MenuItem delete = new MenuItem();
                delete.Header = "Delete";
                delete.Tag = uri.AbsolutePath;
                delete.Click += DeleteItem_Click;
                contextMenu.Items.Add(delete);

                Button newItem = new Button();
                newItem.ContextMenu = contextMenu;
                newItem.Content = fileName;
                newItem.Click += PlayItem_Click;

                playlistItems.Children.Add(newItem);

                filePaths.Add(newItem, uri.AbsoluteUri);
            }
        }

        private void PlayItem_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            string fileName = button.Content.ToString();
            string filePath = filePaths[button];

            playlistPaths.Clear();
            playlistPaths.Add(filePath);
            curPlaylistItem = 0;
            MyControl.SourceProvider.MediaPlayer.Play(playlistPaths[curPlaylistItem]);
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;

            Button button = new Button();

            foreach (KeyValuePair<Button, string> item in filePaths)
            {
                if (item.Value == "file:///" + menuItem.Tag.ToString())
                {
                    button = item.Key;
                }
            }

            if (button.Content.ToString() != "")
            {
                filePaths.Remove(button);
            }

            addAllToPlaylist(filePaths);
        }

        private void addAllToPlaylist(Dictionary<Button, string> paths)
        {
            playlistItems.Children.Clear();

            if (paths.Any())
            {
                foreach (KeyValuePair<Button, string> item in paths)
                {
                    playlistItems.Children.Add(item.Key);
                }
            }
        }

        private void PlayPlaylist_Click(object sender, RoutedEventArgs e)
        {
            //vlc.playlist.items.clear();
            //vlc.playlist.stop();

            playlistPaths.Clear();

            foreach (KeyValuePair<Button, string> item in filePaths)
            {
                playlistPaths.Add(item.Value);
            }

            curPlaylistItem = 0;
            MyControl.SourceProvider.MediaPlayer.Play(playlistPaths[curPlaylistItem]);
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            //vlc.playlist.next();
        }

        private void Prev_Click(object sender, RoutedEventArgs e)
        {
            //vlc.playlist.prev();
        }
    }
}

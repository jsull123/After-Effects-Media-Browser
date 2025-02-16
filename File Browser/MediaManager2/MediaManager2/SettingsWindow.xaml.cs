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

namespace MediaManager2
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }
        private async void SettingsWindow_Loaded(object sender, RoutedEventArgs e)
        {
         //   Console.WriteLine(MediaPlayer.GetCacheSize());
            long cacheSize = await MediaCache.GetCacheSize();
            CacheSizeTextBlock.Text = $"{ConvertToReadableSize(cacheSize)}";
        }
        private string ConvertToReadableSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            double len = bytes;

            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }

            return $"{len:0.##} {sizes[order]}";
        }
        private async void ClearMediaCache_Click(object sender, RoutedEventArgs e)
        {
            CacheProgressBar.Visibility = Visibility.Visible;
            CacheProgressBar.Value = 0;

            var progress = new Progress<int>(value =>
            {
                CacheProgressBar.Value = value;
            });

            try
            {
                await MediaCache.ClearCacheWithProgress(progress);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
            finally
            {
                CacheProgressBar.Visibility = Visibility.Collapsed;
            }

            CacheSizeTextBlock.Text = $"{ConvertToReadableSize(0)}";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;


namespace MediaManager2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public static class Settings
    {
        public static double scrollSpeedSetting = 25.0d;
    }
  
    public class Folder
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public List<Folder> Subfolders { get; set; } = new List<Folder>();
    }
    public class FileItem
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Type { get; set; } // Type: "video", "image", "gif"
    }
    public partial class MainWindow : Window
    {
        private DateTime lastClickTime = DateTime.MinValue;
        private const int DoubleClickTime = 300;
        private static long lastUIResponseTime;

        public MainWindow()
        {
            InitializeComponent();
        }
        private Folder ActiveFolder { get; set; } = null;
        private Folder ActiveSubFolder { get; set; } = null;
        public static void MonitorUIResponsiveness()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    long startTime = Environment.TickCount64;

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        lastUIResponseTime = Environment.TickCount64 - startTime;
                    });

                    //Console.WriteLine($"UI Response Time: {lastUIResponseTime} ms");

                    await Task.Delay(100);
                }
            });
        }
        public static long GetUIResponseTime()
        {
            return lastUIResponseTime;
        }
        private void Media_Unloaded(object sender, RoutedEventArgs e)
        {
            if (sender is Image image)
            {   
                if (image.Tag is CancellationTokenSource cts)
                {
                    cts.Cancel();
                    cts.Dispose();
                    image.Tag = null;
                }
            }
        }
        private async void Media_Loaded(object sender, RoutedEventArgs e)
        {
            
            if (sender is Image image && image.DataContext is FileItem fileItem)
            {
                var cts = new CancellationTokenSource();
                image.Tag = cts;

                await MediaPlayer.PlayMediaAsync(fileItem, image, cts.Token);
            }
        }
        private void SelectFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {  

                ActiveFolder = new Folder
                {
                    Path = dialog.FileName,
                    Name = System.IO.Path.GetFileName(dialog.FileName)
                };

                PopulateSubfolders(ActiveFolder, 0, 5);

                ActiveFolderButton.Content = ActiveFolder.Name;
                FoldersTreeView.ItemsSource = ActiveFolder.Subfolders;
            }
        }
        private void ActiveFolderButton_Click(object sender, RoutedEventArgs e)
        {
            if (ActiveFolder == null)
                return;
            if (ActiveSubFolder == ActiveFolder)
                return;

            ResetTreeViewSelection(FoldersTreeView);
            ActiveSubFolder = ActiveFolder;
            UpdateFileListItems(true);
            ActiveFolderButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0078d7"));
        }
        private void ResetTreeViewSelection(TreeView treeView)
        {
            if (treeView.SelectedItem != null)
            {
                var selectedItem = treeView.SelectedItem;
              
                TreeViewItem treeViewItem = GetTreeViewItem(treeView, selectedItem);
                if (treeViewItem != null)
                {
                    treeViewItem.IsSelected = false;
                }
            }
        }
        private TreeViewItem GetTreeViewItem(ItemsControl container, object item)
        {
            if (container == null)
                return null;

            foreach (object child in container.Items)
            {
                TreeViewItem childContainer = container.ItemContainerGenerator.ContainerFromItem(child) as TreeViewItem;
                if (childContainer == null)
                    continue;

                if (child == item)
                    return childContainer;

                TreeViewItem descendant = GetTreeViewItem(childContainer, item);
                if (descendant != null)
                    return descendant;
            }

            return null;
        }
        private void PopulateSubfolders(Folder folder, int depth, int maxdepth)
        {
            if (depth >= maxdepth)
            {
                return;
            }
            try
            { 
                var subdirectories = System.IO.Directory.GetDirectories(folder.Path);

                foreach (var subdirectory in subdirectories)
                {
                    var subfolder = new Folder
                    {
                        Path = subdirectory,
                        Name = System.IO.Path.GetFileName(subdirectory)
                    };

                    PopulateSubfolders(subfolder, depth + 1, maxdepth);
                    
                    folder.Subfolders.Add(subfolder);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"Access denied to {folder.Path}: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error accessing {folder.Path}: {ex.Message}");
            }
        }
        private List<FileItem> PopulateFilesRecursive(Folder folder)
        {
            List<FileItem> files = PopulateFiles(folder);
            foreach (var subfolder in folder.Subfolders)
            {
                files.AddRange(PopulateFilesRecursive(subfolder));
            }
            return files;
        }
        private List<FileItem> PopulateFiles(Folder folder)
        {
            List<FileItem> filesList = new List<FileItem>();
            try
            {
                // Get all files in the current folder
                var files = System.IO.Directory.GetFiles(folder.Path);

                foreach (var filePath in files)
                {
                    var type = GetFileType(filePath);

                    if (type == "unknown" || type == "audio")
                    {
                        continue;
                    }
                   
                    var fileItem = new FileItem
                    {
                        Name = System.IO.Path.GetFileName(filePath),
                        Path = filePath,
                        Type = GetFileType(filePath),                    
                    };

                    // Add the file to the folder's file list
                    filesList.Add(fileItem);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"Access denied to {folder.Path}: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error accessing {folder.Path}: {ex.Message}");
            }

            return filesList;
        }
        public static string GetFileType(string filePath)
        {
            var extension = System.IO.Path.GetExtension(filePath).ToLower();
            if (extension == ".mp4" || extension == ".mov" || extension == ".avi" || extension == ".m4v")
                return "video";
            else if (extension == ".jpg" || extension == ".png" || extension == ".jpeg")
                return "image";
            else if (extension == ".gif")
                return "gif";
            else if (extension == ".mp3" || extension == ".wav")
                return "audio";
            else
                return "unknown";
        }
        private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (SearchTextBox.IsKeyboardFocusWithin)
            {
                FocusManager.SetFocusedElement(this, (IInputElement)sender);
            }
        }
        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ClearSearchButton.Visibility = Visibility.Visible;
            SearchTextBoxPlaceholder.Visibility = Visibility.Collapsed;
        }
        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!ClearSearchButton.IsMouseOver)
                ClearSearchButton.Visibility = Visibility.Hidden;
            if (SearchTextBox.Text == string.Empty)
            {
                SearchTextBoxPlaceholder.Visibility = Visibility.Visible;
            }

        }
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {          
            UpdateFileListItems(ActiveSubFolder == ActiveFolder);
        }
        private void FoldersTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (FoldersTreeView.SelectedItem is Folder selectedFolder)
            {
                ActiveSubFolder = selectedFolder;
                ActiveFolderButton.Background = new SolidColorBrush(Colors.Transparent);
                UpdateFileListItems(false);
            }
        }
        // Was async
        private void UpdateFileListItems(bool recursive)
        {
            List<FileItem> fileItems = null;

            if (ActiveSubFolder == null || ActiveFolder == null)
                return;

            if (recursive)
                fileItems = PopulateFilesRecursive(ActiveSubFolder);        
            else
                fileItems = PopulateFiles(ActiveSubFolder);

            _ = Task.Run(async () =>
            {
                await MediaCache.CreateAsync(fileItems); 
            });

            _ = Task.Run(async () =>
            {
                await CacheProgressWatcher();
            });


            var filteredFileItems = fileItems
                    .Where(file => file.Name.IndexOf(SearchTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();

            FileListView.ItemsSource = filteredFileItems;

            if (filteredFileItems.Count == 0 && fileItems.Count != 0)        
                FileListCenterText.Text = $"No results for \"{SearchTextBox.Text}\"";
            else 
                FileListCenterText.Text = string.Empty;                

            ResetFileListViewScroller();
        }
        private void ClearSearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = string.Empty;
            SearchTextBoxPlaceholder.Visibility = Visibility.Visible;
            ClearSearchButton.Visibility = Visibility.Collapsed;

            UpdateFileListItems(ActiveSubFolder == ActiveFolder);
        }

        private async Task CacheProgressWatcher()
        {
            await Task.Delay(1000);

            if (!MediaCache.GetStatus(out _))
            {
                return;
            }

            CacheProgressBar.Dispatcher.Invoke(() =>
            {
                CacheProgressBar.Visibility = Visibility.Visible;
                CacheProgressBar.Value = 0;
            });

            while (true)
            {
                if (!MediaCache.GetStatus(out double progress))
                {
                    break;
                }

                CacheProgressText.Dispatcher.Invoke(() =>
                {
                    CacheProgressText.Text = $"Processing... {progress:F2}%";
                });

                CacheProgressBar.Dispatcher.Invoke(() =>
                {
                    CacheProgressBar.Value = progress;
                });

                await Task.Delay(500);
            }

            CacheProgressText.Dispatcher.Invoke(() =>
            {
                CacheProgressText.Text = string.Empty;
            });

            CacheProgressBar.Dispatcher.Invoke(() =>
            {
                CacheProgressBar.Visibility = Visibility.Collapsed;
            });
        }

        private void ResetFileListViewScroller()
        {
            ScrollViewer scrollViewer = GetScrollViewer(FileListView);
            if (scrollViewer != null)
            {
                scrollViewer.ScrollToVerticalOffset(0);
            }
        }
        private void SizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ResetFileListViewScroller();
        }
        private ScrollViewer GetScrollViewer(DependencyObject depObj)
        {
            if (depObj == null)
                return null;

            if (depObj is ScrollViewer)
            {
                return (ScrollViewer)depObj;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);
                ScrollViewer result = GetScrollViewer(child);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }     
        private bool IsDoubleClick()
        {
            DateTime now = DateTime.Now;
            TimeSpan timeSinceLastClick = now - lastClickTime;
            lastClickTime = now;
            return timeSinceLastClick.TotalMilliseconds <= DoubleClickTime;
        }
        private void OpenFileWithDefaultApplication(string filePath)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to open file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.DataContext is FileItem fileItem)
            {
                OpenFileWithDefaultApplication(fileItem.Path);
            }
        }
        private void FileItem_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {  
            if (IsDoubleClick())
            {
                if (sender is MediaElement mediaElement)
                {
                    var parentStackPanel = FindParent<StackPanel>(mediaElement);
                    if (parentStackPanel != null && parentStackPanel.DataContext is FileItem fileItem)
                    {
                        OpenFileWithDefaultApplication(fileItem.Path);
                    }
                }
                if (sender is Image image)
                {
                    var parentStackPanel = FindParent<StackPanel>(image);
                    if (parentStackPanel != null && parentStackPanel.DataContext is FileItem fileItem)
                    {
                        OpenFileWithDefaultApplication(fileItem.Path);
                    }
                }
            }
        }
        private T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null) return null;
            if (parentObject is T parent) return parent;

            return FindParent<T>(parentObject);
        }    
        private void FileListView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollViewer = GetScrollViewer(FileListView);
            if (scrollViewer != null)
            {
                double scrollAmount = e.Delta > 0 ? -100 : 100;
  
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + scrollAmount);

                e.Handled = true;
            }
        }
        private void ImportItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.DataContext is FileItem fileItem)
            {
                Console.WriteLine("Import!" + ActiveFolder.Name + "!" + fileItem.Path);
            }
        }
        private void OpenFileLocation_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.DataContext is FileItem fileItem)
            {
                if (FileListView.SelectedItem != null){
                    FileListView.SelectedItems.Clear();
                }
               
                Process.Start(new ProcessStartInfo("explorer.exe", "/select, "+fileItem.Path));
            }
        }
        private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            MediaPlayer.IsPlaying = !MediaPlayer.IsPlaying;

            var button = sender as Button;
            if (button != null)
            {
                var playPauseIcon = button.Template.FindName("PlayPauseIcon", button) as System.Windows.Shapes.Path;

                if (playPauseIcon != null)
                {
                    if (MediaPlayer.IsPlaying)
                    {
                        playPauseIcon.Data = Geometry.Parse("M10,10 H16 V25 H10 Z M20,10 H26 V25 H20 Z");
                    }
                    else
                    {
                        playPauseIcon.Data = Geometry.Parse("M10,10 L30,20 L10,30 Z");
                    }
                }
            }
        }
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
        }
        private void FileItem_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is StackPanel stackPanel && stackPanel.DataContext is FileItem fileItem)
            {
                MediaPlayer.MouseEnter(fileItem);
            }
        }
        private void FileItem_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is StackPanel stackPanel && stackPanel.DataContext is FileItem fileItem)
            {
                MediaPlayer.MouseLeave(fileItem);
            }
        }
    }
}

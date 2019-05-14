using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Threading;
using ICSharpCode.SharpZipLib.Zip;
using NUnrar.Archive;

namespace Slider1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static readonly DependencyProperty RandomProperty;
        private System.Random randomizer = new System.Random();
        DispatcherTimer dtt = new DispatcherTimer(DispatcherPriority.ApplicationIdle);
        int[] timespans = {31,62,125,250,500,1000,2000,4000,8000,16000 };

        static MainWindow()
        {
            FrameworkPropertyMetadata metadata = new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.None);
            RandomProperty = DependencyProperty.Register("Random", typeof(bool), typeof(MainWindow), metadata);
        }

        public bool Random
        {
            set { SetValue(RandomProperty, value); System.Diagnostics.Debug.WriteLine(value); }
            get { return (bool)GetValue(RandomProperty); }
        }

        string placeholder = "Placeholder.png";
        SortedSet<string> pictures;
        int i = 0;
        public MainWindow()
        {
            InitializeComponent();

            pictures = EmptyCollection();
            pictures.Add(placeholder);
        }

        private void LoadData(string startingPath)
        {
            Traverse(startingPath,pictures);
        }

        internal void LoadDataFromArchive(string z)
        {
            if (z.ToLower().EndsWith(".zip"))
                TraverseZip(z, pictures);
            if (z.ToLower().EndsWith(".rar"))
                TraverseRar(z, pictures);
        }

        private void Traverse(string path, SortedSet<string> files)
        {
            foreach (string s in Directory.GetFiles(path, "*.jpg"))
                files.Add(s);
            foreach (string s in Directory.GetFiles(path, "*.jpeg"))
                files.Add(s);
            foreach (string s in Directory.GetFiles(path, "*.png"))
                files.Add(s);
            foreach (string z in Directory.GetFiles(path, "*.zip"))
                TraverseZip(z, files);
            foreach (string z in Directory.GetFiles(path, "*.rar"))
                TraverseRar(z, files);
            foreach (string d in Directory.GetDirectories(path))
                Traverse(d, files);
        }

        private void TraverseZip(string z, SortedSet<string> files)
        {
            try
            {
                using (ZipInputStream zis = new ZipInputStream(File.OpenRead(z)))
                {
                    ZipEntry zie;
                    while ((zie = zis.GetNextEntry()) != null)
                    {
                        if (zie.IsFile)
                        {
                            string xt = System.IO.Path.GetExtension(zie.Name).ToLower();
                            if ( xt == ".jpg" || xt ==".jpeg" )
                            {
                                files.Add(z + "?" + zie.Name);
                            }
                        }
                    }
                }
            }
            catch { }
        }

        private void TraverseRar(string z, SortedSet<string> files)
        {
            try
            {
                RarArchive ras = RarArchive.Open(File.OpenRead(z));
                foreach(RarArchiveEntry rae in ras.Entries)
                {
                    if (!rae.IsDirectory)
                    {
                        string xt = System.IO.Path.GetExtension(rae.FilePath).ToLower();
                        if (xt == ".jpg" || xt == ".jpeg")
                        {
                            files.Add(z + "|" + rae.FilePath);
                        }
                    }
                }
            }
            catch { }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ShowPicture(i);
            dtt.Tick += new EventHandler(dtt_Tick);
            dtt.Interval = TimeSpan.FromMilliseconds(Properties.Settings.Default.PictureDelay);
            dtt.Start();
        }

        void dtt_Tick(object sender, EventArgs e)
        {
            try
            {
                if (Random)
                    i = randomizer.Next(pictures.Count);
                else
                    i++;
                if (i < pictures.Count)
                    ShowPicture(i);
                else
                {
                    i = pictures.Count - 1;
                    ((DispatcherTimer)sender).Stop();
                }
            }
            catch (Exception)
            {
                ShowPlaceHolder();
            }
        }

        private void ShowPlaceHolder()
        {
            var bi = GetImage(placeholder);
            myImage.Source = bi;
        }

        private void ShowPicture(int i)
        {
            string status = string.Format("{0} of {1}", i + 1, pictures.Count);
            label1.Content = status;
            myImage.Source = null;
            var bi = GetImage(pictures.ElementAt(i));
            myImage.Source = bi;
        }

        private ImageSource GetImage(string p)
        {
            if (p.Contains('?')) // E' un contenuto di uno zip?
                return ImageSourceFromZip(p);

            if (p.Contains("|"))
                return ImageSourceFromRar(p);

            var bi = new BitmapImage();
            try
            {
                bi.BeginInit();
                bi.UriSource = new Uri(p, UriKind.RelativeOrAbsolute);
                bi.EndInit();
            }
            catch (ArgumentException)
            {
                bi = new BitmapImage(); 
                bi.BeginInit();
                bi.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
                bi.UriSource = new Uri(p, UriKind.RelativeOrAbsolute);
                bi.EndInit();
            }
            return bi;
        }

        private ImageSource ImageSourceFromZip(string p)
        {
            string[] spl = p.Split(new char[] { '?' });
            string zipname = spl[0];
            string innername = spl[1];
            using (ZipInputStream zis = new ZipInputStream(File.OpenRead(zipname)))
            {
                ZipEntry zie;
                while ((zie = zis.GetNextEntry()) != null)
                {
                    if (zie.IsFile)
                    {
                        if (zie.Name == innername)
                        {
                            System.Diagnostics.Debug.WriteLine(zie.CompressionMethod);
                            byte[] buffer = new byte[16384];
                            int bytesread;
                            using (MemoryStream ms = new MemoryStream())
                            {
                                while ((bytesread = zis.Read(buffer, 0, buffer.Length)) != 0)
                                    ms.Write(buffer, 0, bytesread);
                                ms.Position = 0;
                                try
                                {
                                    BitmapImage bi = new BitmapImage();
                                    bi.BeginInit();
                                    bi.CacheOption = BitmapCacheOption.OnLoad;
                                    bi.StreamSource = ms;
                                    bi.EndInit();
                                    return bi;
                                }
                                catch (ArgumentException)
                                {
                                    ms.Position = 0;
                                    BitmapImage bi = new BitmapImage();
                                    bi.BeginInit();
                                    bi.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
                                    bi.CacheOption = BitmapCacheOption.OnLoad;
                                    bi.StreamSource = ms;
                                    bi.EndInit();
                                    return bi;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        private ImageSource ImageSourceFromRar(string p)
        {
            string[] spl = p.Split(new char[] { '|' });
            string rarname = spl[0];
            string innername = spl[1];
            RarArchive zis = RarArchive.Open(rarname);
            if(zis!=null)
            {
                foreach (RarArchiveEntry zie in zis.Entries)
                {
                    if (!zie.IsDirectory)
                    {
                        if (zie.FilePath == innername)
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                zie.WriteTo(ms);
                                ms.Position = 0;
                                try
                                {

                                    return BitmapFrame.Create(ms, BitmapCreateOptions.IgnoreColorProfile, BitmapCacheOption.OnLoad);
                                }
                                catch (ArgumentException)
                                {
                                    ms.Position = 0;
                                    BitmapImage bi = new BitmapImage();
                                    bi.BeginInit();
                                    bi.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
                                    bi.CacheOption = BitmapCacheOption.OnLoad;
                                    bi.StreamSource = ms;
                                    bi.EndInit();
                                    return bi;
                                }

                            }
                        }
                    }
                }
            }
            return null;
        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void maxim_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Maximized)
                this.WindowState = System.Windows.WindowState.Normal;
            else
                this.WindowState = System.Windows.WindowState.Maximized;
        }

        private void Window_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            dtt.Stop();
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        private void ToggleButton_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                int original_i = i;
                e.Handled = true;
                switch (e.Key)
                {
                    case Key.Space:
                        if (dtt.IsEnabled)
                            dtt.Stop();
                        else
                            dtt.Start();
                        break;
                    case Key.Down:
                        i = 0;
                        break;
                    case Key.Left:
                        PreviousImage(false);
                        dtt.Stop();
                        break;
                    case Key.Right:
                        NextImageLooping(false);
                        dtt.Stop();
                        break;
                    case Key.D:
                        dtt.Stop();
                        var dlg = new System.Windows.Forms.FolderBrowserDialog();
                        System.Windows.Forms.DialogResult result = dlg.ShowDialog();
                        if (result == System.Windows.Forms.DialogResult.OK)
                        {
                            this.Cursor = Cursors.Wait;
                            pictures = EmptyCollection();
                            LoadData(dlg.SelectedPath);
                            ShowPicture(i = 0);
                            this.Cursor = Cursors.Arrow;
                        }
                        this.Focus();
                        break;
                    case Key.Z:
                        dtt.Stop();
                        var dlgF = new System.Windows.Forms.OpenFileDialog();
                        dlgF.Filter = "Archivi zip|*.zip";
                        System.Windows.Forms.DialogResult dr = dlgF.ShowDialog();
                        if (dr == System.Windows.Forms.DialogResult.OK)
                        {
                            pictures = EmptyCollection();
                            this.Cursor = Cursors.Wait;
                            LoadDataFromArchive(dlgF.FileName);
                            ShowPicture(i = 0);
                            this.Cursor = Cursors.Arrow;
                        }
                        this.Focus();
                        break;
                    case Key.Add:
                        FastenUpTimer();
                        break;
                    case Key.Subtract:
                        SlowDownTimer();
                        break;
                    case Key.D1:
                    case Key.D2:
                    case Key.D3:
                    case Key.D4:
                    case Key.D5:
                    case Key.D6:
                    case Key.D7:
                    case Key.D8:
                    case Key.D9:
                        SetTimer(e.Key-Key.D0);
                        break;
                    default:
                        e.Handled = false;
                        break;
                }
                if (i != original_i)
                    ShowPicture(i);
            }
            catch (Exception)
            {
                ShowPlaceHolder();
            }
        }

        private void SetTimer(int v)
        {
            if (v > 0 && v <9)
                dtt.Interval = TimeSpan.FromMilliseconds(timespans[v]);
        }

        private SortedSet<string> EmptyCollection()
        {
            return new SortedSet<string>(new NumericFilenameComparer());
        }

        private void SlowDownTimer()
        {
            double millis = dtt.Interval.TotalMilliseconds;
            dtt.Interval = TimeSpan.FromMilliseconds(millis * 2);
        }

        private void FastenUpTimer()
        {
            double maxSpeed = Properties.Settings.Default.MinPictureDelay;
            double millis = dtt.Interval.TotalMilliseconds;
            millis /= 2;
            if (millis < maxSpeed)
                millis = maxSpeed;
            dtt.Interval = TimeSpan.FromMilliseconds(millis);
        }

        private void PreviousImage(bool looping)
        {
            if (i > 0)
                i--;
            else
                if (looping)
                    i = pictures.Count - 1;
        }

        private void NextImageLooping(bool looping)
        {
            i++;
            if (i >= pictures.Count)
                if (looping)
                    i = 0;
                else
                    i = pictures.Count - 1;
        }

        private void IO_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            try
            {
                dtt.Stop();
                if (e.Delta < 0)
                    NextImageLooping(true);
                else
                    PreviousImage(true);
                ShowPicture(i);
            }
            catch (Exception)
            {
                ShowPlaceHolder();
            }
        }

        private void IO_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch { }
        }

        private void IO_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("FileName"))
                return;
            string[] names = e.Data.GetData("FileName") as string[];
            bool auto = dtt.IsEnabled;
            dtt.Stop();
            pictures = EmptyCollection();
            foreach (string filename in names)
            {
                if (Directory.Exists(filename))
                    LoadData(filename);
                else
                {
                    if (Path.GetExtension(filename).ToLower() == ".zip")
                        if (File.Exists(filename))
                            LoadDataFromArchive(filename);
                    if (Path.GetExtension(filename).ToLower() == ".rar")
                        if (File.Exists(filename))
                            LoadDataFromArchive(filename);
                }
            }
            ShowPicture(i = 0);
            if (auto)
                dtt.Start();
            this.Focus();

        }
    }
}

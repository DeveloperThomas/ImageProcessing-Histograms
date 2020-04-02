using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
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
using Image = System.Drawing.Image;

namespace ImageProcessing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public Bitmap bitmapImage { get; set; }
        public int R_Value { get; set; }
        public int G_Value { get; set; }
        public int B_Value { get; set; }

        public MainWindow()
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            R_Value = 0;
            B_Value = 0;
            G_Value = 0;
        }

        private void MenuItemFileOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif|PNG Image|*.png|TIFF Image|*.tiff|TIFF Image|*.tif";
            if (openFileDialog.ShowDialog() == true)
            {
                Uri fileUri = new Uri(openFileDialog.FileName);
                loadedImage.Source = new BitmapImage(fileUri);
                Image img = Image.FromFile(openFileDialog.FileName);
                bitmapImage = new Bitmap(openFileDialog.FileName);
                MemoryStream MS = new MemoryStream();
                bitmapImage.Save(MS, System.Drawing.Imaging.ImageFormat.Jpeg);
                bitmapImage = new Bitmap(MS);


                using (MemoryStream memory = new MemoryStream())
                {
                    bitmapImage.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                    memory.Position = 0;
                    BitmapImage bitmap= new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = memory;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    modifiedImage.Source = bitmap;
                }
            }
        }

        private void MenuItemFileSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "Document";
            dlg.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif|PNG Image|*.png|TIFF Image|*.tiff|TIFF Image|*.tif";
            dlg.Title = "Save and Image File";
            var encoder = new PngBitmapEncoder();
            if (dlg.ShowDialog() == true && modifiedImage != null)
            {
                try
                {
                    encoder.Frames.Add(BitmapFrame.Create((BitmapSource)modifiedImage.Source));
                    using (var stream = dlg.OpenFile())
                    {
                        encoder.Save(stream);
                    }
                }
                catch (System.ArgumentNullException)
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }

        private void MenuItemFileExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        public void CreateEditableImage(Bitmap image)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                image.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                modifiedImage.Source = bitmapimage;
            }
        }

        private void ModifiedImage_MouseMove(object sender, MouseEventArgs e)
        {
            System.Windows.Point p = e.GetPosition(modifiedImage);

            double originalModifiedImageWidth = bitmapImage.Width;
            double originalModifiedImageHeight = bitmapImage.Height;
            double actualModifiedImageWidth = modifiedImage.ActualWidth;
            double actualModifiedImageHeight = modifiedImage.ActualHeight;

            double pixelIndexX = p.X * originalModifiedImageWidth / actualModifiedImageWidth;
            double pixelIndexY = p.Y * originalModifiedImageHeight / actualModifiedImageHeight;

            System.Drawing.Color CurrentColor = bitmapImage.GetPixel(Convert.ToInt32(Math.Floor(pixelIndexX)), Convert.ToInt32(Math.Floor(pixelIndexY)));

            current_R_Value.Text = CurrentColor.R.ToString();
            current_G_Value.Text = CurrentColor.G.ToString();
            current_B_Value.Text = CurrentColor.B.ToString();
        }

        private void ModifiedImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point p = e.GetPosition(modifiedImage);

            double originalModifiedImageWidth = bitmapImage.Width;
            double originalModifiedImageHeight = bitmapImage.Height;
            double actualModifiedImageWidth = modifiedImage.ActualWidth;
            double actualModifiedImageHeight = modifiedImage.ActualHeight;

            double pixelIndexX = originalModifiedImageWidth * p.X / actualModifiedImageWidth;
            double pixelIndexY = originalModifiedImageHeight * p.Y / actualModifiedImageHeight;

            int x = Convert.ToInt32(Math.Floor(pixelIndexX));
            int y = Convert.ToInt32(Math.Floor(pixelIndexY));

            System.Drawing.Color CurrentColor = new System.Drawing.Color();
            CurrentColor = System.Drawing.Color.FromArgb(255, R_Value, G_Value, B_Value);

            bitmapImage.SetPixel(x,y, CurrentColor);
            this.CreateEditableImage(bitmapImage);


        }

        private void ZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double newUserZoomValue = e.NewValue;
            ScaleTransform scaleTransform = new ScaleTransform(newUserZoomValue, newUserZoomValue);
            modifiedImage.LayoutTransform = scaleTransform;
        }

        private void RSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            R_Value = Convert.ToInt32(e.NewValue);
        }

        private void GSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            G_Value = Convert.ToInt32(e.NewValue);
        }

        private void BSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            B_Value = Convert.ToInt32(e.NewValue);
        }

        private void GoToHistogram_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HistogramWindow histogramWindow = new HistogramWindow(this);
                histogramWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                histogramWindow.Show();
            }
            catch (NullReferenceException){}

        }
    }
}

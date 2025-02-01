using System;
using System.Windows;

namespace D2RLAN.Views.Dialogs
{
    /// <summary>
    /// Window Owner Centering and Form Cropping logic for ImagePreviewerView.xaml
    /// </summary>
    public partial class ImagePreviewerView : Window
    {
        public ImagePreviewerView()
        {
            InitializeComponent();
            Loaded += ImagePreviewerView_Loaded;
        }

        private void ImagePreviewerView_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.Dialogs.ImagePreviewerViewModel viewModel)
            {
                if (viewModel.ImagePath != null)
                {
                    try
                    {
                        // Load the preview image and adjust form to match it
                        var bitmapImage = new System.Windows.Media.Imaging.BitmapImage(new Uri(viewModel.ImagePath));
                        Width = bitmapImage.Width + 1; // Adding some margin
                        Height = bitmapImage.Height + 1; // Adding some margin

                        // Verify owner window and then center this window relative to it
                        if (Owner != null)
                        {
                            Left = Owner.Left + (Owner.Width - Width) / 2;
                            Top = Owner.Top + (Owner.Height - Height) / 2;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error loading image: " + ex.Message);
                    }
                }
            }
        }

    }
}

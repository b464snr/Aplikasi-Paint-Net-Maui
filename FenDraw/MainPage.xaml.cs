using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Views;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui;
using Syncfusion.Maui.Core.Internals;
using System.Collections.ObjectModel;

namespace FenDraw;


public partial class MainPage : ContentPage
{
    double panX, panY;
    int state;
    int lineorder = 0;
    double currentScale = 1;
    double startScale = 1;
    bool redoable = false;
    IDrawingLine linebefore;
    FileResult poto;
    public MainPage(FileResult photo)
    {
        InitializeComponent();
        if (photo != null)
        {
            AlasKertas.Source = photo.FullPath;
            poto = photo;
        };
        AlasKertas.WidthRequest = 250;
        AlasKertas.HeightRequest = 300;
        Adjust(250, 300);
    } 
    private void Adjust(double width, double height)
    {
        Kertas.WidthRequest = width;
        Kertas.HeightRequest = height;
        Pann.WidthRequest = width;
        Pann.HeightRequest = height;
    }
    // back button
    private void UndoCommand(object sender, EventArgs e)
    {
        if (lineorder != 0)
        {
            Kertas.Lines.RemoveAt(lineorder - 1);
            lineorder -= 1;
            if (redoable)
            {
                linebefore = Kertas.Lines[lineorder - 1];
            } else redoable = true;
        } else
        {
            Kertas.Clear();
            linebefore = null;
        }
    }
    // redo button
    private void RedoCommand(object sender, EventArgs e)
    {
        if (lineorder != 0 && redoable)
        {
            Kertas.Lines.Add(linebefore);
            redoable = false;
            lineorder += 1;
        }
    }
    // state
    private void StateCommand(object sender, CommunityToolkit.Maui.Core.DrawingLineCompletedEventArgs e)
    {
        linebefore = Kertas.Lines[lineorder];
        redoable = false;
        lineorder += 1;
    }
    // for saving purpose

    private async void SavingCommand(object sender, EventArgs e)
    {
        using var stream = await DrawingView.GetImageStream(Kertas.Lines, new Size(250, 300), Color.FromArgb("#FFFFFF"));
        using var memoryStream = new MemoryStream();
        if(poto != null)
        {
            using var photostream = await poto.OpenReadAsync();
            await photostream.CopyToAsync(memoryStream);
        }
        stream.CopyTo(memoryStream);
        stream.Position = 0;
        memoryStream.Position = 0;

        // windows for testing only
#if WINDOWS 
           await System.IO.File.WriteAllBytesAsync(
                @"C:\Users\Lenovo\Pictures\Sketch.png", memoryStream.ToArray() );
#elif ANDROID
        var context = Platform.CurrentActivity;

        if (OperatingSystem.IsAndroidVersionAtLeast(29))
        {
            Android.Content.ContentResolver resolver = context.ContentResolver;
            Android.Content.ContentValues contentValues = new();
            contentValues.Put(Android.Provider.MediaStore.IMediaColumns.DisplayName, "Sketch.png");
            contentValues.Put(Android.Provider.MediaStore.IMediaColumns.MimeType, "image/png");
            contentValues.Put(Android.Provider.MediaStore.IMediaColumns.RelativePath, "DCIM/" + "FenDraw");
            Android.Net.Uri imageUri = resolver.Insert(Android.Provider.MediaStore.Images.Media.ExternalContentUri, contentValues);
            var os = resolver.OpenOutputStream(imageUri);
            Android.Graphics.BitmapFactory.Options options = new();
            options.InJustDecodeBounds = true;
            var bitmap = Android.Graphics.BitmapFactory.DecodeStream(stream);
            bitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Png, 100, os);
            os.Flush();
            os.Close();
        }
        else
        {
            Java.IO.File storagePath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures);
            string path = System.IO.Path.Combine(storagePath.ToString(), "Sketch.png");
            System.IO.File.WriteAllBytes(path, memoryStream.ToArray());
            var mediaScanIntent = new Android.Content.Intent(Android.Content.Intent.ActionMediaScannerScanFile);
            mediaScanIntent.SetData(Android.Net.Uri.FromFile(new Java.IO.File(path)));
            context.SendBroadcast(mediaScanIntent);
        }
#endif
    }
    // gesture from Pan
    // Kanvas Following Pan
    void OnPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Running:
                // Translate and pan.
                double boundsX = Kertas.Width;
                double boundsY = Kertas.Height;
                Dview.TranslationX = Math.Clamp(panX + e.TotalX, -boundsX, boundsX);
                Dview.TranslationY = Math.Clamp(panY + e.TotalY, -boundsY, boundsY);
                break;

            case GestureStatus.Completed:
                // Store the translation applied during the pan
                panX = Dview.TranslationX;
                panY = Dview.TranslationY;
                break;
        }
    }
    void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
    {
        if (e.Status == GestureStatus.Started)
        {
            startScale = Kertas.Scale;
        }
        if (e.Status == GestureStatus.Running)
        {
            // Calculate
            currentScale += (e.Scale - 1) * startScale;
            currentScale = Math.Max(1, currentScale);
            //Aplying
            Kertas.Scale = currentScale;
            Pann.Scale = Kertas.Scale;
        }
        }

    private void Tools(object sender, EventArgs e)
    {
        if (sender == lining)
        {
            state = 1;
        }
        else if (sender == viewing)
        {
            state = 2;
        } else if (sender == LineColor)
        {
            state = 3;
        }
        switch (state)
        /*
         * case 1 : Lining
         * Case 2 : Viewing
         */
        {
            case 1:
                if (LineCommand.IsVisible == true)
                {
                    LineCommand.IsVisible = false;
                    break;
                };
                LineCommand.IsVisible = true;
                ViewControl.IsVisible = false;
                break;
            case 2:
                if (ViewControl.IsVisible == true)
                {
                    ViewControl.IsVisible = false;
                    break;
                }
                ViewControl.IsVisible = true;
                break;
            case 3:
                if (ColorChanger.IsVisible == true)
                {
                    ColorChanger.IsVisible = false;
                    break;
                }
                ColorChanger.IsVisible = true;
                break;
        };
        // line command and sublinecommand control
        if (LineCommand.IsVisible && state == 1)
        {
            linebox.IsVisible = true;
            ViewControl.IsVisible = false;
        } else if (state == 3)
        {
            SizeCommand.IsVisible = false;
            ViewControl.IsVisible = false;
            LineCommand.IsVisible = false;
        }
        else
        {
            SizeCommand.IsVisible = false;
            ColorChanger.IsVisible = false;
        }
        // activate or deactivate pann
        if (ViewControl.IsVisible)
        {
            LineCommand.IsVisible = false;
            Pann.IsVisible = true;
        }
        else
        {
            Pann.IsVisible = false;
        }
    }
    private void LineSubCommand(object sender, EventArgs e)
    {
        linebox.IsVisible = false;
        SizeCommand.IsVisible = true;
    }

    private void ViewCommand(object sender, EventArgs e)
    {
        if (sender == RotateLeft)
        {
            Dview.Rotation -= 20;
        }
        else
        {
            Dview.Rotation += 20;
        };
    }

}

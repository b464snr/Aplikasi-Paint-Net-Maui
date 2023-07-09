using CommunityToolkit.Maui.Views;
using System.Collections.ObjectModel;

namespace FenDraw;


public partial class MainPage : ContentPage
{
    double panX, panY;
    int state;
    double currentScale = 1;
    double startScale = 1;

    public MainPage()
    {
        InitializeComponent();
    }
    // for saving purpose
    private async void SaveCommand(object sender, CommunityToolkit.Maui.Core.DrawingLineCompletedEventArgs e)
    {
        var stream = await Kertas.GetImageStream(Kertas.Width, Kertas.Height);
        ImageView.Source = ImageSource.FromStream(() => stream);
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
                Kertas.TranslationX = Math.Clamp(panX + e.TotalX, -boundsX, boundsX);
                Kertas.TranslationY = Math.Clamp(panY + e.TotalY, -boundsY, boundsY);
                Pann.TranslationX = Kertas.TranslationX;
                Pann.TranslationY = Kertas.TranslationY;
                break;

            case GestureStatus.Completed:
                // Store the translation applied during the pan
                panX = Kertas.TranslationX;
                panY = Kertas.TranslationY;
                Pann.TranslationX = Kertas.TranslationX;
                Pann.TranslationY = Kertas.TranslationY;
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
        };
        // line command and sublinecommand control
        if (LineCommand.IsVisible && state == 1)
        {
            linebox.IsVisible = true;
            ViewControl.IsVisible = false;
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
        if (sender == LineSize)
        {
            ColorChanger.IsVisible = false;
            SizeCommand.IsVisible = true;
        }
        else
        {
            SizeCommand.IsVisible = false;
            ColorChanger.IsVisible = true;
        };
    }

    private void ViewCommand(object sender, EventArgs e)
    {
        if (sender == RotateLeft)
        {
            Kertas.Rotation -= 20;
        }
        else
        {
            Kertas.Rotation += 20;
        };
    }

}

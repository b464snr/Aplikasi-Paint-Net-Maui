namespace FenDraw;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

        MainPage = new NavigationPage(new  Menu());
    }
    protected override Window CreateWindow(IActivationState activationState)
    {
        var windows = base.CreateWindow(activationState);

        // Add here your sizing code
        windows.Width = 393;
        windows.Height = 851;
        // Add here your positioning code
        windows.X = 600;
        windows.Y = 30;
        return windows;
    }
}

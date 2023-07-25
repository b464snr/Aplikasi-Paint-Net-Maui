using Microsoft.Maui.ApplicationModel.Communication;
using System.Runtime.Remoting;
namespace FenDraw;

public partial class Menu : ContentPage
{
    FileResult photo;
    public Menu()
	{
		InitializeComponent();
	}

	private async void NProject(object sender, EventArgs e)
	{
        await Navigation.PushAsync(new MainPage(photo));
    }
	private async void CProject(object sender, EventArgs e)
	{
        photo = await MediaPicker.Default.PickPhotoAsync();
        if (photo != null)
        {   
        await Navigation.PushModalAsync(new MainPage(photo));
        }
    }
}
using ServusAppNet8.MVVM.ViewModels;

namespace ServusAppNet8.MVVM.Views;

public partial class SignupPageView : ContentPage
{
	public SignupPageView()
	{
		InitializeComponent();
        BindingContext = new UserViewModel();
	}

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new LoginPageView());
    }
}
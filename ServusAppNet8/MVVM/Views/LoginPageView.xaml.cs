namespace ServusAppNet8.MVVM.Views;

public partial class LoginPageView : ContentPage
{
	public LoginPageView()
	{
		InitializeComponent();
	}

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new LandingPageView());
    }

    private async void SignUpButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SignupPageView());
    }
}
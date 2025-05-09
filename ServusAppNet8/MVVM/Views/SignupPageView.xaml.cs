using ServusAppNet8.MVVM.ViewModels;

namespace ServusAppNet8.MVVM.Views;

public partial class SignupPageView : ContentPage
{
	public SignupPageView()
	{
		InitializeComponent();
        BindingContext = new UserViewModel();
	}
}
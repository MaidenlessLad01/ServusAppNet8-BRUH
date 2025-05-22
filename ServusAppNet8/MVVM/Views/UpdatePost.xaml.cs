using ServusAppNet8.MVVM.ViewModels;

namespace ServusAppNet8.MVVM.Views;

public partial class UpdatePost : ContentPage
{
	public UpdatePost()
	{
		InitializeComponent();
		BindingContext = new PostViewModel();
	}
}
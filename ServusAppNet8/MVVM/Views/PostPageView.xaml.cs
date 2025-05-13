using ServusAppNet8.MVVM.ViewModels;

namespace ServusAppNet8.MVVM.Views;

public partial class PostPageView : ContentPage
{
	public PostPageView()
	{
		InitializeComponent();
		BindingContext = new PostViewModel();
	}
}
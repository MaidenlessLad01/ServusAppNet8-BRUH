using ServusAppNet8.MVVM.Models;
using ServusAppNet8.MVVM.ViewModels;

namespace ServusAppNet8.MVVM.Views;

public partial class Profile : ContentPage
{
    //public string UserId { get; set; }
    public Profile(User registeredUser)
    {
        InitializeComponent();
        BindingContext = new UserViewModel(registeredUser);
    }

    public Profile()
    {
        InitializeComponent();

    }
}
using Microsoft.Maui.Dispatching;
using System;
using System.Threading;
using Microsoft.Maui.Controls;
using ServusAppNet8.MVVM.ViewModels;
namespace ServusAppNet8.MVVM.Views;

public partial class LandingPageView : ContentPage
{
    //Var dec
    public LandingPageView()
	{
		InitializeComponent();
        BindingContext = new UserViewModel();
    }
}
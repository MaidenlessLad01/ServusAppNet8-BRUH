﻿using ServusAppNet8.MVVM.Views;

namespace ServusAppNet8
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new SplashPage());
        }
    }
}

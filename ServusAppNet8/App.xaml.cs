using ServusAppNet8.MVVM.Views;

namespace ServusAppNet8
{
    public partial class App : Application
    {
        //public static IServiceProvider Services { get; private set; }

        public static IServiceProvider Services { get;  private set; }
        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            Services = serviceProvider;
            MainPage = new NavigationPage(new LandingPageView());
           
        }
    }
}

using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using ServusAppNet8.MVVM.ViewModels;
using ServusAppNet8.MVVM.Views;

namespace ServusAppNet8
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkitMediaElement()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("fontello.ttf", "icon");
                });

#if DEBUG
    		builder.Logging.AddDebug();
            builder.Services.AddSingleton<UserViewModel>();
            builder.Services.AddSingleton<LoginPageView>();
            builder.Services.AddSingleton<LandingPageView>();
            builder.Services.AddSingleton<SignupPageView>();
            builder.Services.AddSingleton<Profile>();
            builder.Services.AddSingleton<Home>();
            builder.Services.AddSingleton<SplashPage>();
            builder.Services.AddSingleton<CreatePostPageView>();
            builder.Services.AddSingleton<UpdatePost>();
#endif

            return builder.Build();
        }
    }
}

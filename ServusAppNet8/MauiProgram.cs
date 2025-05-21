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
            builder.Services.AddSingleton(new CloudinaryDotNet.Cloudinary(
                new CloudinaryDotNet.Account(
                    "dvpu1zjo1", // Replace with your Cloudinary Cloud Name
                    "129476462218521",    // Replace with your Cloudinary API Key
                    "f6fTvQJh6TlVGyS1u56kfv5vqkA"  // Replace with your Cloudinary API Secret
                )
            ));
#endif

            return builder.Build();
        }
    }
}

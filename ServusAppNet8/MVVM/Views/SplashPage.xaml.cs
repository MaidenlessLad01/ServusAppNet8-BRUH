using System.Reflection;

namespace ServusAppNet8.MVVM.Views;

public partial class SplashPage : ContentPage
{
	public SplashPage()
	{
		InitializeComponent();
        LoadEmbeddedVideo();
	}
    private void LoadEmbeddedVideo()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = "ServusAppNet8.Resources.Raw.background1.mp4";

        using (var stream = assembly.GetManifestResourceStream(resourceName))
        {
            if (stream != null)
            {
                var filepath = Path.Combine(FileSystem.CacheDirectory, "background1.mp4");
                using (var fileStream = File.Create(filepath))
                {
                    stream.CopyTo(fileStream);
                }

                SplashVideo.Source = filepath; // Assuming SplashVideo is your MediaElement control.
            }
            else
            {
                // Handle the case where the resource is not found
                Console.WriteLine("Resource not found");
            }
        }
    }
    private async void SplashVideo_MediaEnded(object sender, EventArgs e)
    {
        await Task.Delay(500);
        await Navigation.PushAsync(new LandingPageView());
        Navigation.RemovePage(this);
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using ServusAppNet8.MVVM.Models;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Text.Json;
using ServusAppNet8.MVVM.Views;
using System.Collections.ObjectModel;

namespace ServusAppNet8.MVVM.ViewModels
{
    internal class PostViewModel : INotifyPropertyChanged
    {
        #region Variable Declaration

        public string baseURL = "https://68107efd27f2fdac241199ad.mockapi.io/Post";
        private HttpClient _httpClient;

        public ObservableCollection<Post> Posts { get; set; } = new ObservableCollection<Post>();

        private string? _caption, _picture;
        private DateTime _postDate;

        #endregion

        #region Getters and Setters

        public string? Caption
        {
            get => _caption;
            set
            {
                _caption = value;
                OnPropertyChanged();
            }
        }

        public string? Picture
        {
            get => _picture;
            set
            {
                _picture = value;
                OnPropertyChanged();
            }
        }

        public DateTime PostDate
        {
            get => _postDate;
            set
            {
                _postDate = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Command Declaration

        public ICommand GetImageCommand => new Command(async () => await PickImage());

        public ICommand PostUploadCommand => new Command(async () => await PostUpload());

        public ICommand PostDeleteCommand { get; set; }

        public ICommand PostUpdateCommand { get; set; }

        public ICommand navToPostPage => new Command(() =>
        {
            Application.Current.MainPage = App.Services.GetRequiredService<CreatePostPageView>();
        });

        public ICommand gotoLanding => new Command(() => Application.Current.MainPage = App.Services.GetRequiredService<Home>());

        public ICommand PickImageCommand => new Command(async() => await PickImage());

        #endregion

        public PostViewModel()
        {
            _httpClient = new HttpClient();

            PostDeleteCommand = new Command<Post>(PostDelete);
            PostUpdateCommand = new Command<Post>(PostUpdate);

            GetPosts();
        }

        private async void GetPosts()
        {
            var res = await _httpClient.GetAsync(baseURL);
            
            if(res.IsSuccessStatusCode)
            {
                var json = await res.Content.ReadAsStringAsync();
                var posts = JsonSerializer.Deserialize<List<Post>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                Posts.Clear();

                foreach (var post in posts) {
                    Posts.Add(post);
                };
            }
        }

        private async Task PostUpload()
        {
            if (string.IsNullOrWhiteSpace(Caption) && string.IsNullOrWhiteSpace(Picture)) 
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Post must not be empty", "OK");
                return;
            }

            var userId = Preferences.Get("UserId", string.Empty);

            var newPost = new Post
            {
                UserId = userId,
                PostDate = DateTime.Now
            };

            if (Picture != null)
                newPost.Picture = Picture;

            if(Caption != null)
                newPost.Caption = Caption;

            //Changes the data into json to be readable for the API
            var json = JsonSerializer.Serialize(newPost);
            var content = new StringContent(json, Encoding.UTF8, "Application/json");

            var res = await _httpClient.PostAsync(baseURL, content);

            if(res.IsSuccessStatusCode)
            {
                //Checks if the post has been uploaded to the API properly
                var responseContent = await res.Content.ReadAsStringAsync();
                var createdPost = JsonSerializer.Deserialize<Post>(responseContent);

                if(createdPost != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Success", "The Post has been uploaded!", "OK");
                    Picture = null;
                    Caption = null;
                    Application.Current.MainPage = new NavigationPage(new Home());
                }
                else
                {
                    Application.Current.MainPage.DisplayAlert("Error", "Failed to upload the post", "OK");
                }
            }
            else
            {
                Application.Current.MainPage.DisplayAlert("Error", "Post was not uploaded", "OK");
            }
        }

        private async void PostDelete(Post post)
        {
            if (post == null) return;

            var confirmed = await Application.Current.MainPage.DisplayAlert("Confirm", "Are you sure you want to delete this post?", "Confirm", "Cancel");

            if (!confirmed) return;

            //Deletes the post on the API
            var res = await _httpClient.DeleteAsync($"{baseURL}/{post.PostId}");

            if (res.IsSuccessStatusCode)
            {
                //Deletes the current post from the list after deleting it on the API
                Posts.Remove(post);
            }
        }

        private async void PostUpdate(Post post)
        {
            await Application.Current.MainPage.DisplayAlert("Error", "lmao", "OK");
            return;
        }

        //Picks an image from the local file system
        private async Task PickImage()
        {
            var fileResult = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Please select an image",
                FileTypes = FilePickerFileType.Images //only images are allowed
            });

            if(fileResult != null)
            {
                var stream = await fileResult.OpenReadAsync();
                var uploadImagePath = await UploadLocalAsync(fileResult.FileName, stream);
                Picture = uploadImagePath;
            }
        }

        //Uploads the image to the app
        private async Task<string> UploadLocalAsync(string fileName, Stream stream)
        {
            var localPath = Path.Combine(FileSystem.AppDataDirectory, fileName);

            using var fs = new FileStream(localPath, FileMode.Create, FileAccess.Write);
            await stream.CopyToAsync(fs);

            return localPath;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) => 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
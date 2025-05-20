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

        public string baseURL = "https://68107efd27f2fdac241199ad.mockapi.io/";
        private HttpClient _httpClient;

        //Get all users and posts
        public ObservableCollection<PostUser> PostWithUser { get; set; } = new ObservableCollection<PostUser>();

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

        public ICommand PostUpdateCommand { get; }

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

            PostDeleteCommand = new Command<PostUser>(PostDelete);
            PostUpdateCommand = new Command<PostUser>(PostUpdate);

            GetPosts();
        }

        private async void GetPosts()
        {
            //Get all posts
            var resPosts = await _httpClient.GetAsync($"{baseURL}/Post");
            var posts = new List<Post>();
            
            if(resPosts.IsSuccessStatusCode)
            {
                var json = await resPosts.Content.ReadAsStringAsync();
                posts = JsonSerializer.Deserialize<List<Post>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            //Get all users
            var resUsers = await _httpClient.GetAsync($"{baseURL}/User");
            var users = new List<User>();

            if (resUsers.IsSuccessStatusCode) 
            {
                var json = await resUsers.Content.ReadAsStringAsync();
                users = JsonSerializer.Deserialize<List<User>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            //Get the current users UserId
            var userId = Preferences.Get("UserId", string.Empty);

            //Merge the needed information into one
            var merged = from post in posts
                         join user in users on post.UserId equals user.UserId
                         select new PostUser
                         {
                             UserName = $"{user.FirstName} {user.LastName}",
                             Caption = post.Caption,
                             Picture = post.Picture,
                             PostDate = post.PostDate,
                             PostId = post.PostId,
                             SameUser = (user.UserId == userId)
                         };

            PostWithUser.Clear();

            foreach(var item in merged)
            {
                PostWithUser.Add(item);
            }
        }

        private async Task PostUpload()
        {
            if (string.IsNullOrWhiteSpace(Caption) && string.IsNullOrWhiteSpace(Picture)) 
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Post must not be empty", "OK");
                return;
            }

            //Gets the current user's Id for the post
            var userId = Preferences.Get("UserId", string.Empty);

            var newPost = new Post
            {
                UserId = userId,
                PostDate = DateTime.Now
            };

            //If picture or caption is not empty, then it will be added to the api
            if (Picture != null)
            { 
                newPost.Picture = Picture; 
            }

            if(Caption != null)
                newPost.Caption = Caption;

            //Changes the data into json to be readable for the API
            var json = JsonSerializer.Serialize(newPost);
            var content = new StringContent(json, Encoding.UTF8, "Application/json");

            var res = await _httpClient.PostAsync($"{baseURL}/Post", content);

            if(res.IsSuccessStatusCode)
            {
                //Checks if the post has been uploaded to the API properly
                var responseContent = await res.Content.ReadAsStringAsync();
                var createdPost = JsonSerializer.Deserialize<Post>(responseContent);

                if(createdPost != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Success", "The Post has been uploaded!", "OK");

                    //Sets the picture and caption to null before going to homepage
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

        //Change the image into base64
        private async Task<string> ConvertImageToBase64(FileResult file)
        {
            if (file == null) return null;

            using var stream = await file.OpenReadAsync();
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            var bytes = ms.ToArray();
            return Convert.ToBase64String(bytes);
        }

        //Deletes post from the api
        private async void PostDelete(PostUser post)
        {
            if (post == null) return;

            var confirmed = await Application.Current.MainPage.DisplayAlert("Confirm", "Are you sure you want to delete this post?", "Confirm", "Cancel");

            if (!confirmed) return;

            //Deletes the post on the API
            var res = await _httpClient.DeleteAsync($"{baseURL}/Post/{post.PostId}");

            if (res.IsSuccessStatusCode)
            {
                //Deletes the current post from the list after deleting it on the API
                PostWithUser.Remove(post);
                await Application.Current.MainPage.DisplayAlert("Success", "Post deleted.", "OK");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to delete post.", "OK");
            }
        }

        //Updates the post
        private async void PostUpdate(PostUser post)
        {
            if (post == null) return;

            PostWithUser.Clear();

            //Deletes the post on the API
            var res = await _httpClient.DeleteAsync($"{baseURL}/Post/{post.PostId}");

            if (res.IsSuccessStatusCode)
            {
                //Deletes the current post from the list after deleting it on the API
                PostWithUser.Remove(post);
                await Application.Current.MainPage.DisplayAlert("Success", "Post deleted.", "OK");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to delete post.", "OK");
            }
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
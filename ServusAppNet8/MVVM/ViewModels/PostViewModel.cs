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
using CloudinaryDotNet; // Add this using statement
using CloudinaryDotNet.Actions; // Add this using statement

namespace ServusAppNet8.MVVM.ViewModels
{
    internal class PostViewModel : INotifyPropertyChanged
    {
        #region Variable Declaration

        public string baseURL = "https://68107efd27f2fdac241199ad.mockapi.io/";
        private HttpClient _httpClient;
        private CloudinaryDotNet.Cloudinary _cloudinary; // Cloudinary instance

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

        public ICommand PickImageCommand => new Command(async () => await PickImage());

        #endregion

        public PostViewModel()
        {
            _httpClient = new HttpClient();
            // Initialize Cloudinary here (if not using DI)
            _cloudinary = new CloudinaryDotNet.Cloudinary(
                new CloudinaryDotNet.Account(
                    "YOUR_CLOUD_NAME", // Replace with your Cloudinary Cloud Name
                    "YOUR_API_KEY",    // Replace with your Cloudinary API Key
                    "YOUR_API_SECRET"  // Replace with your Cloudinary API Secret
                )
            );

            PostDeleteCommand = new Command<PostUser>(PostDelete);
            PostUpdateCommand = new Command<PostUser>(PostUpdate);

            GetPosts();
        }

        // If you are using Dependency Injection, your constructor would look like this:
        // public PostViewModel(CloudinaryDotNet.Cloudinary cloudinary)
        // {
        //     _httpClient = new HttpClient();
        //     _cloudinary = cloudinary;
        //     PostDeleteCommand = new Command<PostUser>(PostDelete);
        //     PostUpdateCommand = new Command<PostUser>(PostUpdate);
        //     GetPosts();
        // }

        private async void GetPosts()
        {
            //Get all posts
            var resPosts = await _httpClient.GetAsync($"{baseURL}/Post");
            var posts = new List<Post>();

            if (resPosts.IsSuccessStatusCode)
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
                             Picture = post.Picture, // This will now be the Cloudinary URL
                             PostDate = post.PostDate,
                             PostId = post.PostId,
                             SameUser = (user.UserId == userId)
                         };

            PostWithUser.Clear();

            foreach (var item in merged)
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

            // If a picture is selected, upload it to Cloudinary
            if (!string.IsNullOrWhiteSpace(Picture))
            {
                try
                {
                    // The Picture property is already set to the Cloudinary URL by PickImage()
                    // So we just assign it directly to newPost.Picture
                    newPost.Picture = Picture;
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", $"Failed to upload image to Cloudinary: {ex.Message}", "OK");
                    return;
                }
            }

            if (Caption != null)
                newPost.Caption = Caption;

            //Changes the data into json to be readable for the API
            var json = JsonSerializer.Serialize(newPost);
            var content = new StringContent(json, Encoding.UTF8, "Application/json");

            var res = await _httpClient.PostAsync($"{baseURL}/Post", content);

            if (res.IsSuccessStatusCode)
            {
                //Checks if the post has been uploaded to the API properly
                var responseContent = await res.Content.ReadAsStringAsync();
                var createdPost = JsonSerializer.Deserialize<Post>(responseContent);

                if (createdPost != null)
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

        // This method will now upload the image to Cloudinary
        private async Task PickImage()
        {
            var fileResult = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Please select an image",
                FileTypes = FilePickerFileType.Images //only images are allowed
            });

            if (fileResult != null)
            {
                using var stream = await fileResult.OpenReadAsync();
                var uploadResult = await UploadImageToCloudinary(stream, fileResult.FileName);

                if (uploadResult != null && !string.IsNullOrEmpty(uploadResult.Url.ToString()))
                {
                    Picture = uploadResult.Url.ToString(); // Set Picture to the Cloudinary URL
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Failed to upload image to Cloudinary.", "OK");
                }
            }
        }

        // New method to upload image to Cloudinary
        private async Task<ImageUploadResult> UploadImageToCloudinary(Stream imageStream, string fileName)
        {
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(fileName, imageStream),
                // Optional: You can specify a folder, public_id, transformations, etc.
                // Folder = "social_media_posts",
                // PublicId = $"post_{Guid.NewGuid().ToString()}"
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult;
        }

        //Deletes post from the api
        private async void PostDelete(PostUser post)
        {
            if (post == null) return;

            var confirmed = await Application.Current.MainPage.DisplayAlert("Confirm", "Are you sure you want to delete this post?", "Confirm", "Cancel");

            if (!confirmed) return;

            // In a real application, you might want to delete the image from Cloudinary as well
            // if (post.Picture != null && post.Picture.Contains("res.cloudinary.com"))
            // {
            //     try
            //     {
            //         var publicId = GetPublicIdFromCloudinaryUrl(post.Picture);
            //         if (!string.IsNullOrEmpty(publicId))
            //         {
            //             var deletionResult = await _cloudinary.DestroyAsync(new DeletionParams(publicId));
            //             if (deletionResult.Result != "ok")
            //             {
            //                 Console.WriteLine($"Warning: Failed to delete image from Cloudinary: {deletionResult.Error?.Message}");
            //             }
            //         }
            //     }
            //     catch (Exception ex)
            //     {
            //         Console.WriteLine($"Error deleting from Cloudinary: {ex.Message}");
            //     }
            // }

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

        // Helper to extract public ID from Cloudinary URL for deletion
        private string GetPublicIdFromCloudinaryUrl(string url)
        {
            // Example: "https://res.cloudinary.com/your_cloud_name/image/upload/v1234567890/folder/public_id.jpg"
            // We need to extract "folder/public_id"
            var uri = new Uri(url);
            var path = uri.Segments.Last(); // Gets "public_id.jpg"
            var parts = path.Split('.');
            if (parts.Length > 0)
            {
                return parts[0]; // Returns "public_id"
            }
            return null;
        }


        //Updates the post
        private async void PostUpdate(PostUser post)
        {
            if (post == null) return;

            // This method currently deletes the post and then re-fetches all posts.
            // For a proper update, you'd typically have a separate update mechanism
            // where the user can modify the caption or replace the image.
            // If replacing the image, you'd upload the new image to Cloudinary and
            // then update the Post record in MockAPI.io with the new Cloudinary URL.

            PostWithUser.Clear(); // Clear the collection to refetch, this seems part of your current update logic.

            // Here's where you'd typically implement the actual update logic.
            // Example:
            // var updatedPost = new Post
            // {
            //     PostId = post.PostId,
            //     UserId = Preferences.Get("UserId", string.Empty), // Assuming userId is stored
            //     Caption = post.Caption, // User might edit this in a UI
            //     Picture = post.Picture, // New picture URL if changed, otherwise old one
            //     PostDate = post.PostDate // Keep original post date or update to DateTime.Now
            // };

            // var json = JsonSerializer.Serialize(updatedPost);
            // var content = new StringContent(json, Encoding.UTF8, "Application/json");
            // var res = await _httpClient.PutAsync($"{baseURL}/Post/{post.PostId}", content);

            // if (res.IsSuccessStatusCode)
            // {
            //     await Application.Current.MainPage.DisplayAlert("Success", "Post updated.", "OK");
            //     GetPosts(); // Refresh posts after update
            // }
            // else
            // {
            //     await Application.Current.MainPage.DisplayAlert("Error", "Failed to update post.", "OK");
            // }

            // Your existing delete logic for update is below, which might not be what you intend for "update"
            var res = await _httpClient.DeleteAsync($"{baseURL}/Post/{post.PostId}");

            if (res.IsSuccessStatusCode)
            {
                PostWithUser.Remove(post);
                await Application.Current.MainPage.DisplayAlert("Success", "Post deleted.", "OK");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to delete post.", "OK");
            }
            GetPosts(); // Re-fetch all posts after the "update" (delete + refresh)
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
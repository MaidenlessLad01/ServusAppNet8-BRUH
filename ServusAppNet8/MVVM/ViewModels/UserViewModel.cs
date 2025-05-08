using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Input;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServusAppNet8.MVVM.Models;
using ServusAppNet8.Database;
using ServusAppNet8.MVVM.Views;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Collections.ObjectModel;

namespace ServusAppNet8.MVVM.ViewModels
{
    internal class UserViewModel : INotifyPropertyChanged
    {
        #region Variables and Commands
        //var declaration
        public User users { get; set; }
        
        private string _selectedGender;
        //public string SelectedGender { get; set; }
        private string emailOrPhone;
        private string _password;
        private string _fName;
        private string _lName;
        private string nameError;
        private DateTime _dob;
        private string passwordError;
        private string signUpError;
        private string emailOrPhoneError;
        private string _confirmpass;
        //Commands for logging in and registering
        public ICommand LoginButton => new Command(Login);
        public ICommand ContinueCommand => new Command(OnContinue);
        public ICommand RegisterButton => new Command(Register);

        public string baseURL = "https://68107efd27f2fdac241199ad.mockapi.io/User";
        private HttpClient _httpClient;

        //ObservableCollection because List won't work
        public ObservableCollection<string> _listGenders;
        #endregion

        public UserViewModel()
        {
            users = new User();
            _httpClient = new HttpClient();
            ListGenders = new ObservableCollection<string> { "Male", "Female", "Shopping Cart", "Godzilla", "Walmart Bag", "Attack Helicopter", "Prefer Not To Say" };

            DoB = DateTime.Now;

        }
        #region Mga Get Sets

        // First Name property with data binding
        public string FName
        {
            get => _fName;
            set
            {   
                _fName = value;
                OnPropertyChanged(nameof(FName)); 
            }
        }

        public string LName
        {
            get => _lName;
            set
            {
                
                _lName = value;
                OnPropertyChanged(nameof(LName));
            }
        }
        public string SelectedGender
        {
            get => _selectedGender;
            set
            {
               _selectedGender = value;
                OnPropertyChanged(nameof(SelectedGender));
            }
        }
        // Date of Birth property with data binding
        public DateTime DoB
        {
            get => _dob;
            set
            {    
                _dob = value;
                OnPropertyChanged(nameof(DoB));
            }
        }
        //set value for username
        public string EmailOrPhone
        {
            get => emailOrPhone;
            set
            {
                emailOrPhone = value;
                OnPropertyChanged(nameof(EmailOrPhone));
                ValidateEmailOrPhone();
            }
        }

        //set value for password
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
                ValidatePassword();
            }
        }

        
        //set value for password confirmation
        public string ConfirmPassword
        {
            get => _confirmpass;
            set
            {
                _confirmpass  = value;
                OnPropertyChanged(nameof(ConfirmPassword));
            }
        }

        public ObservableCollection<string> ListGenders
        {
            get => _listGenders;
            set
            {
                _listGenders = value;
                OnPropertyChanged(nameof(ListGenders));
            }
        }

        //validate whether email or phone num is correct format
        private void ValidateEmailOrPhone()
        {
            if (string.IsNullOrEmpty(EmailOrPhone))
            {
                EmailOrPhoneError = "This field cannot be empty.";
            }
            else if (IsEmail(EmailOrPhone))
            {
                ValidateEmail();
            }
            else if (IsPhoneNumber(EmailOrPhone))
            {
                ValidatePhoneNumber();
            }
            else
            {
                EmailOrPhoneError = "Please enter a valid email or phone number.";
            }
        }
        //among us sussy baka
        private void ValidateEmail()
        {
            if (!IsEmail(EmailOrPhone))
            {
                EmailOrPhoneError = "Invalid email format.";
            }
            else
            {
                EmailOrPhoneError = string.Empty;
            }
        }
        //yeahhhh
        private void ValidatePhoneNumber()
        {
            if (!IsPhoneNumber(EmailOrPhone))
            {
                EmailOrPhoneError = "Invalid phone number format.";
            }
            else
            {
                EmailOrPhoneError = string.Empty;
            }
        }
        #endregion

        #region Login and Register Methods
        private async void OnContinue()
        {
            if (!ValidateNames())
            {
                await Application.Current.MainPage.DisplayAlert("Error", NameError, "OK");
                return; // Exit if validation fails
            }

            var response = await _httpClient.GetAsync(baseURL);

            if (response.IsSuccessStatusCode)
            {
                var contents = await response.Content.ReadAsStringAsync();
                var users = JsonSerializer.Deserialize<List<User>>(contents);

                foreach (var user in users)
                {
                    //gets the userId
                    if (user.Email == emailOrPhone || user.PhoneNum == emailOrPhone)
                    {
                        var additionalDetails = new User
                        {
                            Email = EmailOrPhone,
                            PhoneNum = EmailOrPhone,
                            Password = Password,
                            FirstName = FName,
                            LastName = LName,
                            DoB = DoB,
                            Gender = SelectedGender
                        };

                        var json = JsonSerializer.Serialize(additionalDetails);
                        var content = new StringContent(json, Encoding.UTF8, "Application/json");

                        var res = await _httpClient.PutAsync($"{baseURL}/{user.UserId}", content);

                        if (res.IsSuccessStatusCode)
                        {
                            await Application.Current.MainPage.Navigation.PushAsync(new Home
                            {
                                BindingContext = this
                            });
                        }
                        else
                        {
                            await App.Current.MainPage.DisplayAlert("Error", "Couldn't update details", "OK");
                        }
                    }
                }
            }
        }

        private async void Login()
        {
            // Check if username already exists
            var response = await _httpClient.GetAsync(baseURL);
            if (response.IsSuccessStatusCode)
            {
                var contents = await response.Content.ReadAsStringAsync();
                var users = JsonSerializer.Deserialize<List<User>>(contents);

                //Check if username and password exists
                if(users.Any(u => (u.Email == EmailOrPhone || u.PhoneNum == EmailOrPhone) && u.Password == Password))
                {
                    await App.Current.MainPage.DisplayAlert("Welcome", "Welcome to Servus!", "OK");

                    if (users.Any(u => (string.IsNullOrEmpty(u.FirstName) || string.IsNullOrEmpty(u.LastName) || string.IsNullOrEmpty(u.Gender))))
                    {
                        App.Current.MainPage = new NavigationPage(new Profile { BindingContext = this });
                        return;
                    }

                    App.Current.MainPage = new NavigationPage(new Home { BindingContext = this });
                    
                }
                else
                {
                    App.Current.MainPage.DisplayAlert("Login Unsuccessful", "Invalid Credentials", "OK");
                }
            }
        }

        private async void Register()
        {
            if (string.IsNullOrEmpty(EmailOrPhone) || string.IsNullOrEmpty(Password))
            {
                App.Current.MainPage.DisplayAlert("Register", "Please Enter Fields Properly", "OK");
                return; // Exit the method if fields are empty
            }

            // Validate Email/Phone
            ValidateEmailOrPhone();
            if (!string.IsNullOrEmpty(EmailOrPhoneError))
            {
                App.Current.MainPage.DisplayAlert("Register", EmailOrPhoneError, "OK");
                return; // Exit if Email/Phone validation fails
            }

            // Validate Password
            if (!ValidatePassword())
            {
                App.Current.MainPage.DisplayAlert("Register", PasswordError, "OK");
                return; // Exit if Password validation fails
            }

            // Check if username already exists
            var response = await _httpClient.GetAsync(baseURL);
            if (response.IsSuccessStatusCode)
            {
                var contents = await response.Content.ReadAsStringAsync();
                var users = JsonSerializer.Deserialize<List<User>>(contents);

                if(users.Any(u => u.Email == EmailOrPhone || u.PhoneNum == EmailOrPhone))
                {
                    App.Current.MainPage.DisplayAlert("Register", "Account Already Exists", "OK");
                    return; // Exit if account already exists
                }
            }

            // Check if passwords match
            if (Password != ConfirmPassword)
            {
                App.Current.MainPage.DisplayAlert("Register", "Passwords Do Not Match!", "OK");
                return; // Exit if passwords don't match
            }

            // Add user/register
            var newUser = new User 
            {
                Email = EmailOrPhone,
                PhoneNum = EmailOrPhone,
                Password = Password
            };

            //Changes the data into json to be readable for the API
            var json = JsonSerializer.Serialize(newUser);
            var content = new StringContent(json, Encoding.UTF8, "Application/json");
            
            //Stores into the API
            var res = await _httpClient.PostAsync(baseURL, content);
            
            //Checks if the account was successfully added
            if (res.IsSuccessStatusCode)
            {
                await App.Current.MainPage.DisplayAlert("Register", "Account Registered", "OK");
                App.Current.MainPage = new NavigationPage(new LandingPageView());
            }
            else
            {
                App.Current.MainPage.DisplayAlert("Error", "Account Was Not Registered", "OK");
            }
        }
      
        #endregion

        #region Error Handlers
        public string PasswordError
        {
            get => passwordError;
            set { passwordError = value; OnPropertyChanged(nameof(PasswordError)); }
        }
        
        public string NameError
        {
            get => nameError;
            set { nameError = value; OnPropertyChanged(nameof(NameError)); }
        }
        public string SignUpError
        {
            get => signUpError;

            set { signUpError = value; OnPropertyChanged(nameof(SignUpError)); }
        }

        public string EmailOrPhoneError
        {
            get => emailOrPhoneError;
            set { emailOrPhoneError = value; OnPropertyChanged(nameof(EmailOrPhoneError)); }
        }

        private bool ValidateNames()
        {
            if (string.IsNullOrEmpty(FName) || string.IsNullOrEmpty(LName))
            {
                NameError = "Please enter both First and Last names.";
                return false;
            }

            if (FName.Length < 2 || LName.Length < 2)
            {
                NameError = "Names must be at least 2 characters long.";
                return false;
            }

            if (!Regex.IsMatch(FName, @"[A-Z]") || !Regex.IsMatch(LName, @"[A-Z]"))
            {
                NameError = "Names must contain at least one uppercase letter.";
                return false;
            }

            if (!Regex.IsMatch(FName, @"[a-z]") || !Regex.IsMatch(LName, @"[a-z]"))
            {
                NameError = "Names must contain at least one lowercase letter.";
                return false;
            }

            if (FName.Length > 7 || LName.Length > 7)
            {
                NameError = "Names must be at most 7 characters long.";
                return false;
            }

            if (Regex.IsMatch(FName, @"\d") || Regex.IsMatch(LName, @"\d"))
            {
                NameError = "Names cannot contain numbers.";
                return false;
            }

            NameError = string.Empty; // No errors
            return true;
        }

        private bool ValidatePassword()
        {
            if (string.IsNullOrEmpty(Password))
            {
                PasswordError = "Password cannot be empty.";
                return false;
            }

            if (Password.Length < 8)
            {
                PasswordError = "Password must be at least 8 characters long.";
                return false;
            }

            if (!Regex.IsMatch(Password, @"[A-Z]"))
            {
                PasswordError = "Password must contain at least one uppercase letter.";
                return false;
            }

            if (!Regex.IsMatch(Password, @"[a-z]"))
            {
                PasswordError = "Password must contain at least one lowercase letter.";
                return false;
            }
            if(Password.Length > 13)
            {
                PasswordError = "Password Too Long!\nMaximum of 13 Characters Only!\nKalimtanon raba ka LMAO";
                return false;
            }

            if (!Regex.IsMatch(Password, @"[0-9]"))
            {
                PasswordError = "Password must contain at least one number.";
                return false;
            }

            if (!Regex.IsMatch(Password, @"[\W_]"))
            {
                PasswordError = "Password must contain at least one special character.";
                return false;
            }

            PasswordError = string.Empty;  // No errors
            return true;
        }

        private bool IsEmail(string input)
        {
            // A simple check for email format
            return Regex.IsMatch(input, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        private bool IsPhoneNumber(string input)
        {
            // A simple check for phone numbers (supports international format)
            return Regex.IsMatch(input, @"^\+?[1-9]\d{1,14}$");
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        
    }
}
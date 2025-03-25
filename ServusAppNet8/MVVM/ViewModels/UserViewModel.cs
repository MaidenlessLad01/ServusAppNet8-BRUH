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

namespace ServusAppNet8.MVVM.ViewModels
{
    internal class UserViewModel : INotifyPropertyChanged
    {
        #region Variables and Commands
        //var declaration
        private string _username;
        private string emailOrPhone;
        private string _password;
        private string passwordError;
        private string signUpError;
        private string emailOrPhoneError;
        private string _confirmpass;
        //Commands for logging in and registering
        public ICommand LoginButton => new Command(Login);
        public ICommand RegisterButton => new Command(Register);
        #endregion
        #region Mga Get Sets
        //set value for username
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }
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
        private void Login()
        {
            //Check if username and password exists
            if (FakeDB.Users.Any(u => u.Email == emailOrPhone || u.PhoneNum == emailOrPhone && u.Password == Password))
            {
                App.Current.MainPage.DisplayAlert("Welcome","Wazgud Cuh","Nigga");
                App.Current.MainPage = new NavigationPage(new Home());
            }
            else
            {
                App.Current.MainPage.DisplayAlert("Login", "Login Unsuccessful\nInvalid Email/Phone Number or Password", "OK");
            }
        }
        private void Register()
        {
            if (!string.IsNullOrEmpty(emailOrPhone) && !string.IsNullOrEmpty(Password) /*&& !string.IsNullOrEmpty(ConfirmPassword)*/) 
            {
                // Check if username already exists
                if (FakeDB.Users.Any(u => u.Email == emailOrPhone || u.PhoneNum == emailOrPhone))
                {
                    App.Current.MainPage.DisplayAlert("Register", "Account Already Exists", "OK");
                }
                else
                {
                    //Register account
                    if(Password != ConfirmPassword)
                    {
                        App.Current.MainPage.DisplayAlert("Register", "Passwords Do Not Match, Nigga", "OK");
                    }
                    else
                    {
                        //Add user/register
                        FakeDB.Users.Add(new User { Email = emailOrPhone,PhoneNum = emailOrPhone, Password = Password });
                        App.Current.MainPage.DisplayAlert("Register", "Account Registered", "OK");
                        App.Current.MainPage = new NavigationPage(new LandingPageView());
                        
                    }
                }
            }
            else
            {
                //error hangling charsutgyausxahjd nouhj
                App.Current.MainPage.DisplayAlert("Register", "Please Enter Fields Properly", "OK");
            }
        }
        #endregion

        #region Error Handlers
        public string PasswordError
        {
            get => passwordError;
            set { passwordError = value; OnPropertyChanged(nameof(PasswordError)); }
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

        protected virtual void OnPropertyChanged(string propertyName)
        {
            
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

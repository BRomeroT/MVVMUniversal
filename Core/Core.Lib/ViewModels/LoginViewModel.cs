using Core.BL;
using Sysne.Core.MVVM;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.ViewModels
{
    public class LoginViewModel : ViewModelWithBL<SecurityBL> //: ViewModelBase
    {
        //readonly SecurityBL bl;
        //public LoginViewModel() => bl = new SecurityBL();

        private string user;
        [Required(ErrorMessage = "We need your user")]
        public string User { get => user; set => Set(ref user, value); }

        private string password;
        [Required]
        public string Password { get => password; set => Set(ref password, value); }

        private string message;
        public string Messagge { get => message; set => Set(ref message, value); }


        RelayCommand loginCommand = null;
        public RelayCommand LoginCommand
        {
            get => loginCommand ??= new RelayCommand(async () =>
            {
                var (isValid, name) = await bl.Login(User, Password);
                Messagge = isValid ? name : "Access denied";
            }, () => Validate(this, false)
            , dependencies: (this, new[] { nameof(User), nameof(Password) }));
        }
    }
}
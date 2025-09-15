using Core.BL;
using Core.Lib.OS;
using Codeland.Core.MVVM;
using Codeland.Core.MVVM.Patterns;
using Codeland.Core.OS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.ViewModels
{
    public partial class LoginViewModel : ViewModelWithBL<SecurityBL> //: ViewModelBase
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
                if (isValid)
                {
                    Messagge = name;
                    await Codeland.Core.OS.DependencyService.Get<INavigationService>().NavigateTo(PagesKeys.Crud);
                }
                else
                {
                    Messagge = "Access denied";
                }
            }, () => Validate(this, false)
            , dependencies: (this, new[] { nameof(User), nameof(Password) }));
        }
    }
}
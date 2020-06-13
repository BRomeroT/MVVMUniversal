using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Model;

namespace WebAPI.BL
{
    public class SecurityBL
    {
        readonly Repository repository;
        public SecurityBL(Repository repository) => this.repository = repository;

        public (bool IsValid, string Name) ValidateUser(string user, string password)
        {
            var credential = (from c in repository.Credentials
                             where c.User.ToLower() == user.ToLower()
                             && c.Password == password
                             select c).FirstOrDefault();
            if (credential == null) return (false, string.Empty);
            return (credential.Active, credential.Name);
        }
    }
}

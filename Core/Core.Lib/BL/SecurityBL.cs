using Core.Lib.ApiClient;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Core.BL
{
    public class SecurityBL
    {
        readonly SecurityApi api;
        public SecurityBL() => api = new SecurityApi();

        public async Task<(bool IsValid, string? Name)> Login(string user, string password)
        {
            var (statusCode, name) = await api.Login(new SharedAPIModel.Credential()
            {
                User = user,
                Password = password
            });
            return statusCode switch
            {
                HttpStatusCode.OK => (true, name),
                HttpStatusCode.Unauthorized => (false, string.Empty),
                _ => (false, statusCode.ToString())
            };
        }
    }
}

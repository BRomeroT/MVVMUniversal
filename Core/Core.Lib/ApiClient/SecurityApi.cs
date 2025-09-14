using Core.Helpers;
using SharedAPIModel;
using Codeland.Core.ApiClient;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Core.Lib.ApiClient
{
    public class SecurityApi : WebApiClient
    {
        public SecurityApi() : base(Settings.Current.WebAPIUrl, "Security") { }

        public async Task<(HttpStatusCode StatusCode, string? Name)> Login(Credential credential) =>
            await CallPostAsync<Credential, string>("Login", credential);
    }
}

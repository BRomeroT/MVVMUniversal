using Core.Helpers;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Sysne.Core.ApiClient
{
    public class WebApiClient : HttpClient
    {
        public WebApiClient(string urlBase = null, string urlController = null) : base(CertValidator.CertHandler)
        {
            CertValidator.HandleCertValidation();

            var isUrlBaseNull = string.IsNullOrWhiteSpace(urlBase);
            if (isUrlBaseNull && string.IsNullOrWhiteSpace(urlController))
            {
                if (!string.IsNullOrWhiteSpace(UrlBaseWebApi))
                    BaseAddress = new Uri(UrlBaseWebApi);
            }
            else
            {
                if (!isUrlBaseNull)
                {
                    UrlBaseWebApi = urlBase;
                    BaseAddress = new Uri(UrlBaseWebApi);
                }
                UrlController = urlController;
            }
            InitHeaders();
            //Timeout = TimeSpan.FromMinutes(5);
        }

        public string UrlBaseWebApi { get; set; } = string.Empty;
        string urlController;
        protected string UrlController
        {
            get => urlController;
            set
            {
                urlController = !value.EndsWith("/") ? value + "/" : value;
                UrlBaseWebApi += (!UrlBaseWebApi.EndsWith("/") ? "/" : string.Empty) + urlController;
                BaseAddress = new Uri(UrlBaseWebApi);
            }
        }

        protected virtual void InitHeaders()
        {
            DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //if (Helpers.IdentityService.Instance.Usuario != null && !string.IsNullOrEmpty(Helpers.IdentityService.Instance.Usuario.Token))
            //{
            //    DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Helpers.IdentityService.Instance.Usuario.Token);
            //}
        }

        /// <summary>
        /// Formatea los parámetros para el tipo DateTime.
        /// </summary>
        /// <param name="sb">StringBuilder donde se concatenan los parametros.</param>
        /// <param name="param">Tupla de parametros.</param>
        public void FormatingParameter(StringBuilder sb, (string, object) param)
        {
            if (param.Item2 != null && param.Item2.GetType() == typeof(DateTime))
                sb.Append($"{param.Item1}={(DateTime)param.Item2:yyyy-MM-dd HH:mm:ss}&");
            else
                sb.Append($"{param.Item1}={param.Item2}&");
        }

        private async Task<HttpStatusCode> ProcessResponse(HttpRequestMessage requestMessage)
        {
            try
            {
                using var res = await SendAsync(requestMessage);
                return res.StatusCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return default;
            }
        }

        private async Task<(HttpStatusCode StatusCode, TResponse Content)> ProcessResponse<TResponse>(HttpRequestMessage requestMessage)
        {
            try
            {
                using var res = await SendAsync(requestMessage);
                //if (res.IsSuccessStatusCode)
                //{
                //    var response = await res.Content.ReadFromJsonAsync<TResponse>();
                //    return (res.StatusCode, response);
                //}
                //else
                //{
                //    return (res.StatusCode, default(TResponse));
                //}

                var response = await res.Content.ReadFromJsonAsync<TResponse>();
                response.ChangeInTimeZone(DateTimeZones.ToLocal);
                return (res.StatusCode, response);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return default;
            }
        }

        public async Task<HttpStatusCode> CallAsync(HttpMethod method, string url, HttpContent content = null)
        {
            using var requestMessage = new HttpRequestMessage(method, url);
            if (!requestMessage.RequestUri.IsAbsoluteUri)
                requestMessage.RequestUri = new Uri(UrlBaseWebApi + url);
            if (content != null) requestMessage.Content = content;
            return await ProcessResponse(requestMessage);
        }

        public async Task<(HttpStatusCode StatusCode, TResponse Content)> CallAsync<TResponse>(HttpMethod method, string url, HttpContent content = null)
        {
            using var requestMessage = new HttpRequestMessage(method, url);
            if (!requestMessage.RequestUri.IsAbsoluteUri)
                requestMessage.RequestUri = new Uri(UrlBaseWebApi + url);
            if (content != null) requestMessage.Content = content;
            return await ProcessResponse<TResponse>(requestMessage);
        }

        public async Task<(HttpStatusCode StatusCode, TResponse Content)> CallGetAsync<TResponse>(string url) =>
            await CallAsync<TResponse>(HttpMethod.Get, url);

        public async Task<(HttpStatusCode StatusCode, TResponse Content)> CallGetAsync<TResponse>(string url, params (string, object)[] parameters)
        {
            var sb = new StringBuilder();
            if (parameters.Length > 0)
            {
                sb.Append("?");
                foreach (var param in parameters)
                {
                    var tmp = param.Item2.ChangeOutDateTime(DateTimeZones.ToUTC);
                    FormatingParameter(sb, (param.Item1, tmp));
                }
                sb.Remove(sb.Length - 1, 1);
            }
            return await CallAsync<TResponse>(HttpMethod.Get, $"{url}{sb}");
        }

        public async Task<(HttpStatusCode StatusCode, TResponse Content)> CallPostAsync<TRequest, TResponse>(string url, TRequest req)
        {
            req.ChangeOutDateTime(DateTimeZones.ToUTC);
            return await CallAsync<TResponse>(HttpMethod.Post, url, JsonContent.Create(req));
        }

        public async Task<(HttpStatusCode StatusCode, TResponse Content)> CallPostAsync<TResponse>(string url, params (string, object)[] parameters)
        {
            var sb = new StringBuilder();
            if (parameters.Length > 0)
            {
                sb.Append("?");
                foreach (var param in parameters)
                {
                    var tmp = param.Item2.ChangeOutDateTime(DateTimeZones.ToUTC);
                    FormatingParameter(sb, (param.Item1, tmp));
                }
                sb.Remove(sb.Length - 1, 1);
            }
            return await CallAsync<TResponse>(HttpMethod.Post, $"{url}{sb}");
        }

        public async Task<HttpStatusCode> CallPostAsync<TRequest>(string url, TRequest req)
        {
            req.ChangeOutDateTime(DateTimeZones.ToUTC);
            return await CallAsync(HttpMethod.Post, url, JsonContent.Create(req));
        }

        public async Task<HttpStatusCode> CallPostAsync(string url, params (string, object)[] parameters)
        {
            var sb = new StringBuilder();
            if (parameters.Length > 0)
            {
                sb.Append("?");
                foreach (var param in parameters)
                {
                    var tmp = param.Item2.ChangeOutDateTime(DateTimeZones.ToUTC);
                    FormatingParameter(sb, (param.Item1, tmp));
                }
                sb.Remove(sb.Length - 1, 1);
            }
            return await CallAsync(HttpMethod.Post, $"{url}{sb}");
        }

        public async Task<(HttpStatusCode StatusCode, TResponse Content)> CallPutAsync<TRequest, TResponse>(string url, TRequest req)
        {
            req.ChangeOutDateTime(DateTimeZones.ToUTC);
            return await CallAsync<TResponse>(HttpMethod.Put, url, JsonContent.Create(req));
        }

        public async Task<HttpStatusCode> CallPutAsync<TRequest>(string url, TRequest req)
        {
            req.ChangeOutDateTime(DateTimeZones.ToUTC);
            return await CallAsync(HttpMethod.Put, url, JsonContent.Create(req));
        }

        public async Task<(HttpStatusCode StatusCode, TResponse Content)> CallDeleteAsync<TResponse>(string url) =>
            await CallAsync<TResponse>(HttpMethod.Delete, url);
        public async Task<(HttpStatusCode StatusCode, TResponse Content)> CallDeleteAsync<TResponse>(string url, params (string, object)[] parameters)
        {
            var sb = new StringBuilder();
            if (parameters.Length > 0)
            {
                sb.Append("?");
                foreach (var param in parameters)
                {
                    var tmp = param.Item2.ChangeOutDateTime(DateTimeZones.ToUTC);
                    FormatingParameter(sb, (param.Item1, tmp));
                }
                sb.Remove(sb.Length - 1, 1);
            }
            return await CallAsync<TResponse>(HttpMethod.Delete, $"{url}{sb}");
        }

        public async Task<HttpStatusCode> CallDeleteAsync(string url) =>
           await CallAsync(HttpMethod.Delete, url);
        public async Task<HttpStatusCode> CallDeleteAsync(string url, params (string, object)[] parameters)
        {
            var sb = new StringBuilder();
            if (parameters.Length > 0)
            {
                sb.Append("?");
                foreach (var param in parameters)
                {
                    var tmp = param.Item2.ChangeOutDateTime(DateTimeZones.ToUTC);
                    FormatingParameter(sb, (param.Item1, tmp));
                }
                sb.Remove(sb.Length - 1, 1);
            }
            return await CallAsync(HttpMethod.Delete, $"{url}{sb}");
        }

        public async Task<(HttpStatusCode StatusCode, TResponse Content)> CallPostFileAsync<TResponse>(string url, byte[] file, string contentName, string fileName, string mediaType, HttpContent extraContent = null, string extraName = "")
        {
            //http://stackoverflow.com/questions/16416601/c-sharp-httpclient-4-5-multipart-form-data-upload
            using var requestContent = new MultipartFormDataContent();
            using var imageContent = new ByteArrayContent(file);
            imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse(mediaType);
            requestContent.Add(imageContent, contentName, fileName);
            if (extraContent != null)
                requestContent.Add(extraContent, extraName);

            return await CallAsync<TResponse>(HttpMethod.Post, url, requestContent);
        }
    }

    /// <summary>
    /// Validate SSL Certificate for HttpClient calls
    /// </summary>
    static class CertValidator
    {
        internal static HttpClientHandler CertHandler { get; } = new HttpClientHandler();
        static bool setServerCertificateCallback;

        [DebuggerStepThrough]
        internal static void HandleCertValidation()
        {
            if (setServerCertificateCallback) return;
            setServerCertificateCallback = true;
            try
            {
#if !BROWSER
                if (CertHandler.ServerCertificateCustomValidationCallback == null)
                {
                    CertHandler.ServerCertificateCustomValidationCallback = (m, cer, chain, e) => true;
                }
#endif
            }
#pragma warning disable CA1031 // Only because for Blazor don't implments ServerCertificateCustomValidationCallback
            catch { return; }
#pragma warning restore CA1031 // Do not catch general exception types
        }
    }
}
namespace ToDoApp.WebApplication.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Common.Models;
    using Newtonsoft.Json;

    public class ApplicationFeatureController : Controller
    {
        private const string ApplicationFeatureFlagResourceName = "applicationfeatureflag";
        private const string MimeTypeApplicationJson = "application/json";
        private readonly string _apiUrl = ConfigurationManager.AppSettings["apiUrl"];
        private readonly HttpClient _httpClient;

        public ApplicationFeatureController()
        {
            _httpClient = new HttpClient
                          {
                              BaseAddress = new Uri(_apiUrl)
                          };
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MimeTypeApplicationJson));
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            IList<ApplicationFeatureFlag> applicationFeatureFlags = null;
            HttpResponseMessage response = await _httpClient.GetAsync(ApplicationFeatureFlagResourceName);
            if (response.IsSuccessStatusCode) { applicationFeatureFlags = await response.Content.ReadAsAsync<IList<ApplicationFeatureFlag>>(); }

            return View(applicationFeatureFlags?.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update([Bind(Include = "Id,Name,IsEnabled")] IList<ApplicationFeatureFlag> applicationFeatureFlags)
        {
            if (!ModelState.IsValid) { View(nameof(Index), applicationFeatureFlags); }

            foreach (var applicationFeatureFlag in applicationFeatureFlags)
            {
                var applicationFeatureFlagUpdate = new ApplicationFeatureFlagUpdate
                                                   {
                                                       IsEnabled = applicationFeatureFlag.IsEnabled
                                                   };
                var applicationFeatureFlagUpdateSerialized = JsonConvert.SerializeObject(applicationFeatureFlagUpdate);
                HttpContent content = new StringContent(applicationFeatureFlagUpdateSerialized, Encoding.UTF8, MimeTypeApplicationJson);
                await _httpClient.SendAsync(
                    new HttpRequestMessage(new HttpMethod("PATCH"), $"{ApplicationFeatureFlagResourceName}/{applicationFeatureFlag.Id}")
                    {
                        Content = content
                    });
            }

            return View(nameof(Index), applicationFeatureFlags);
        }
    }
}
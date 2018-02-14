namespace ToDoApp.WebApplication.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Common.Models;
    using Newtonsoft.Json;

    public class TodoController : Controller
    {
        private readonly string _apiUrl = ConfigurationManager.AppSettings["apiUrl"];
        private const string TodoResourceName = "todo";
        private readonly HttpClient _httpClient;
        private const string MimeTypeApplicationJson = "application/json";

        public TodoController()
        {
            _httpClient = new HttpClient
                          {
                              BaseAddress = new Uri(_apiUrl)
                          };
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MimeTypeApplicationJson));
        }

        public ActionResult Create() => View();

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Description")] Todo todo)
        {
            if (!ModelState.IsValid) { return View(todo); }
            TodoInsert todoInsert = new TodoInsert
                                    {
                                        Description = todo.Description
                                    };
            var todoInsertSerialized = JsonConvert.SerializeObject(todoInsert);
            await _httpClient.PostAsync(TodoResourceName, new StringContent(todoInsertSerialized, Encoding.UTF8, MimeTypeApplicationJson));
            await _httpClient.PostAsJsonAsync(TodoResourceName, todoInsert);
            return RedirectToAction("Index");
        }

        //// GET: Todos/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            HttpResponseMessage response = await _httpClient.GetAsync($"{TodoResourceName}/{id}");
            if (response.StatusCode == HttpStatusCode.NotFound) { return HttpNotFound(); }
            Todo todo = await response.Content.ReadAsAsync<Todo>();
            return View(todo);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            await _httpClient.DeleteAsync($"{TodoResourceName}/{id}");
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            HttpResponseMessage response = await _httpClient.GetAsync($"{TodoResourceName}/{id}");
            if (response.StatusCode == HttpStatusCode.NotFound) { return HttpNotFound(); }
            Todo todo = await response.Content.ReadAsAsync<Todo>();
            return View(todo);
        }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0)]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Description,IsCompleted")] Todo todo)
        {
            if (!ModelState.IsValid) { View(todo); }
            TodoUpdate todoUpdate = new TodoUpdate
                                    {
                                        IsCompleted = todo.IsCompleted,
                                        Description = todo.Description
                                    };

            var todoUpdateSerialized = JsonConvert.SerializeObject(todoUpdate);
            HttpContent content = new StringContent(todoUpdateSerialized, Encoding.UTF8, MimeTypeApplicationJson);
            await _httpClient.SendAsync(
                new HttpRequestMessage(new HttpMethod("PATCH"), $"{TodoResourceName}/{todo.Id}")
                {
                    Content = content
                });
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            IEnumerable<Todo> todos = null;
            HttpResponseMessage response = await _httpClient.GetAsync(TodoResourceName);
            if (response.IsSuccessStatusCode) { todos = await response.Content.ReadAsAsync<IEnumerable<Todo>>(); }
            return View(todos);
        }
    }
}
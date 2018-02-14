namespace ToDoApp.Api.Controllers
{
    using System.Data.Entity;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Common.Models;
    using Common.Repository;

#pragma warning disable 1591
    public class TodoController : ApiController
#pragma warning restore 1591
    {
        private readonly TodoDbContext _todoDbContext = new TodoDbContext();

        /// <summary>
        /// Creates Todo.
        /// </summary>
        /// <param name="todoInsert">The Todo to be created.</param>
        /// <response code="201">Returns if Todo was created.</response>
        /// <response code="400">Returns if Todo validation failed.</response>
        /// <response code="500">Returns if unexpected error happened.</response>
        [Route("todo")]
        [HttpPost]
        public async Task<IHttpActionResult> Create([FromBody] TodoInsert todoInsert)
        {
            if (todoInsert == null || ModelState.IsValid == false) { return BadRequest(); }

            Todo todo = new Todo
                        {
                            Description = todoInsert.Description
                        };

            _todoDbContext.Todoes.Add(todo);
            await _todoDbContext.SaveChangesAsync();

            return CreatedAtRoute(
                "GetById",
                new
                {
                    id = todo.Id
                },
                todo);
        }

        /// <summary>
        /// Delete Todo.
        /// </summary>
        /// <param name="id">Id of the Todo to be deleted.</param>
        /// <response code="204">Returns if Todo was deleted.</response>
        /// <response code="404">Returns if Todo doesn't exists.</response>
        /// <response code="500">Returns if unexpected error happened.</response>
        [Route("todo/{id:int}")]
        [HttpDelete]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var foundTodo = await _todoDbContext.Todoes.SingleOrDefaultAsync(todo => todo.Id == id);
            if (foundTodo == null) { return NotFound(); }
            _todoDbContext.Todoes.Remove(foundTodo);
            await _todoDbContext.SaveChangesAsync();
            return ResponseMessage(new HttpResponseMessage(HttpStatusCode.NoContent));
        }

        /// <summary>
        /// Get all Todos.
        /// </summary>
        /// <response code="200"/>
        /// <response code="500">Returns if unexpected error happened.</response>
        [Route("todo")]
        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            var todoes = await _todoDbContext.Todoes.ToListAsync();
            return Ok(todoes);
        }

        /// <summary>
        /// Get Todo with specific id.
        /// </summary>
        /// <param name="id">Id of the Todo to be returned.</param>
        /// <response code="200"/>
        /// <response code="404">Returns if Todo doesn't exists.</response>
        /// <response code="500">Returns if unexpected error happened.</response>
        [Route("todo/{id:int}", Name = "GetById")]
        [HttpGet]
        public async Task<IHttpActionResult> GetById(int id)
        {
            var foundTodo = await _todoDbContext.Todoes.SingleOrDefaultAsync(todo => todo.Id == id);
            return foundTodo != null ? (IHttpActionResult)Ok(foundTodo) : NotFound();
        }

        /// <summary>
        /// Update Todo with specific id.
        /// </summary>
        /// <param name="id">Id of the Todo to be updated.</param>
        /// <param name="todoUpdate">Updated Todo.</param>
        /// <response code="200"/>
        /// <response code="400">Returns if the Todo validation failed.</response>
        /// <response code="404">Returns if Todo doesn't exists.</response>
        /// <response code="500">Returns if unexpected error happened.</response>
        [Route("todo/{id:int}")]
        [HttpPatch]
        public async Task<IHttpActionResult> Update(int id, [FromBody] TodoUpdate todoUpdate)
        {
            if (todoUpdate == null || ModelState.IsValid == false) { return BadRequest(); }
            var foundTodo = await _todoDbContext.Todoes.SingleOrDefaultAsync(todo => todo.Id == id);
            if (foundTodo == null) { return NotFound(); }
            _todoDbContext.Entry(foundTodo).CurrentValues.SetValues(todoUpdate);
            await _todoDbContext.SaveChangesAsync();
            return ResponseMessage(new HttpResponseMessage(HttpStatusCode.NoContent));
        }
    }
}
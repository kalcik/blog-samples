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
    public class ApplicationFeatureFlagController : ApiController
#pragma warning restore 1591
    {
        private readonly TodoDbContext _todoDbContext = new TodoDbContext();
        private const string ApplicationFeatureFlagResourceName = "applicationfeatureflag";

        /// <summary>
        /// Gets all application feature flags.
        /// </summary>
        /// <response code="200"/>
        /// <response code="500">Returns if unexpected error happened.</response>
        [Route(ApplicationFeatureFlagResourceName)]
        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            var applicationFeatures = await _todoDbContext.ApplicationFeaturesFlags.ToListAsync();
            return Ok(applicationFeatures);
        }

        /// <summary>
        /// Update Application Feature Flag with specific id.
        /// </summary>
        /// <param name="id">Id of the Application Feature Flag to be updated.</param>
        /// <param name="applicationFeatureFlagUpdate">Updated Application Feature Flag.</param>
        /// <response code="200"/>
        /// <response code="400">Returns if the Application Feature Flag validation failed.</response>
        /// <response code="404">Returns if Updated Application Feature Flag doesn't exists.</response>
        /// <response code="500">Returns if unexpected error happened.</response>
        [Route("applicationfeatureflag/{id:int}")]
        [HttpPatch]
        public async Task<IHttpActionResult> Update(int id, [FromBody] ApplicationFeatureFlagUpdate applicationFeatureFlagUpdate)
        {
            if (applicationFeatureFlagUpdate == null || ModelState.IsValid == false) { return BadRequest(); }

            var foundApplicationFeatureFlag = await _todoDbContext.ApplicationFeaturesFlags.SingleOrDefaultAsync(applicationFeatureFlag => applicationFeatureFlag.Id == id);
            if (foundApplicationFeatureFlag == null) { return NotFound(); }
            _todoDbContext.Entry(foundApplicationFeatureFlag).CurrentValues.SetValues(applicationFeatureFlagUpdate);
            await _todoDbContext.SaveChangesAsync();
            return ResponseMessage(new HttpResponseMessage(HttpStatusCode.NoContent));
        }

    }
}
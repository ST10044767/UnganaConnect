using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UnganaConnect.Service;

namespace UnganaConnect.Controllers.Resources
{

    [ApiController]
    [Route("api/[controller]")]



    public class ResourceController : Controller
    {

        private readonly FileServices fileServices;

        // GET: api/Resource/5
        [HttpGet("{id}")]
        public ActionResult Details(int id)
        {
            return Ok(new { id });
        }

        // POST: api/Resource
        [HttpPost]
        public ActionResult Create([FromBody] object data)
        {
            return CreatedAtAction(nameof(Details), new { id = 1 }, data);
        }

        // PUT: api/Resource/5
        [HttpPut("{id}")]
        public ActionResult Edit(int id, [FromBody] object data)
        {
            return NoContent();
        }

        // DELETE: api/Resource/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            return NoContent();
        }
    }
}

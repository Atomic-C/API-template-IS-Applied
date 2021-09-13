using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApiAuthenticate.Filters;

namespace PHCWS.Controllers
{
    [BasicAuthentication]
    [EnableCorsAttribute("*", "*", "*")]
    public class ValuesController : ApiController
	{


        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return id.ToString();
        }

        // POST api/values
        public HttpResponseMessage Post()
        {
            var response = Request.CreateResponse<string>(System.Net.HttpStatusCode.Created, "Your first POST request!");
            return response;
        }
        public HttpResponseMessage Post(int id)
        {

            var response = Request.CreateResponse<string>(System.Net.HttpStatusCode.Created, id.ToString());
            return response;
        }

        // PUT api/values/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}

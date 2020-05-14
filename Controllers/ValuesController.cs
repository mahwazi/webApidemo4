using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace webApidemo4.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        [Authorize]
        public String Get()
        {
            return "value1 ,value2" ;
            
        }

        // GET api/values/5
        [Authorize]
        public string Get(int id)
        {
            return "value" + id;
        }

        // POST api/values
        [Authorize]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [Authorize]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [Authorize]
        public void Delete(int id)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Muestra.api.Models; 

namespace Muestra.api.Controllers
{
    public class BaseController : ApiController
    {
        public string error = "";
        public bool verify(string token)
        {   
            using (muestra_apiEntities db = new muestra_apiEntities())
            {
                if (db.user.Where(p => p.token == token && p.idEstatus == 1).Count() > 0)
                
                    return true;
                
            }
            return false;

        }

    }
}

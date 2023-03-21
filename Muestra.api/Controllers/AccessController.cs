using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Muestra.api.Models.WS;
using Muestra.api.Models;

namespace Muestra.api.Controllers
{
    public class AccessController : ApiController
    {   [HttpGet]
        public Reply HelloWord()
        {
            Reply oR = new Reply();
            oR.result = 1;
            oR.menssage = "Hi Word";

            return oR;
        }
        [HttpPost]
        public Reply Login([FromBody]AccessViewModel model)
        {

            Reply oR = new Reply();
            oR.result = 0;
            try
            {
                using (muestra_apiEntities db = new muestra_apiEntities())
                {
                    var ltar = db.user.Where(p => p.email == model.email && p.password == model.password && p.idEstatus == 1);

                    if (ltar.Count()>0)
                    {
                        oR.result = 1;
                        oR.data = Guid.NewGuid().ToString();

                        user oUser = ltar.First();
                        oUser.token = (string)oR.data;
                        db.Entry(oUser).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        oR.menssage = "Datos Incorrectos";
                    }
                }
            }
            catch(Exception ex)
            {
                oR.menssage = "Ocurrio un Error, estamos corrigiendo";
            }
            return oR;
        }
    }
}

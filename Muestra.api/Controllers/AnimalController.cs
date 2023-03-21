using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Muestra.api.Models;
using Muestra.api.Models.WS;
using System.Threading.Tasks;
using System.IO;
using System.Text;

namespace Muestra.api.Controllers
{
    public class AnimalController : BaseController
    {
        [HttpPost]
        public Reply Get(SecurityViewModel model)
        {
            Reply oR = new Reply();
            oR.result = 0;

            if (!verify(model.token))
            {
                oR.menssage = "No Autorizado";
                return oR;
            }

            try
            {
                using (muestra_apiEntities db = new muestra_apiEntities())
                {
                    List<ListAnimalViewModel> Lst = list(db);

                    oR.data = Lst;
                    oR.result = 1;

                }

            }
            catch(Exception ex)
            {
                oR.menssage = "Ocurrio un error en el Servidor Intenta mas Tarde";
            }

            return oR;
        }

        [HttpPost]
        public Reply Add([FromBody]AnimalViewModel model)
        {
            Reply oR = new Reply();
            oR.result = 0;

            if (!verify(model.token))
            {
                oR.menssage = "No Autorizado";
                return oR;
            }
            //Validaciones
            if (!Validate(model))
            {
                oR.menssage = error;
                return oR;
            }


            try
            {
                using (muestra_apiEntities db = new muestra_apiEntities())
                {
                    animal oAnimal = new animal();
                    oAnimal.idState = 1;
                    oAnimal.name = model.Nombre;
                    oAnimal.patas = model.Patas;

                    db.animal.Add(oAnimal);
                    db.SaveChanges();

                    List<ListAnimalViewModel> Lst = list(db);

                    oR.data = Lst;
                    oR.result = 1;
                }
            }
            catch (Exception ex)
            {
                oR.menssage = "Ocurrio un error en el Servidor Intenta mas Tarde";
            }
            return oR;
        }

        [HttpPut]
        public Reply Edit([FromBody]AnimalViewModel model)
        {
            Reply oR = new Reply();
            oR.result = 0;

            if (!verify(model.token))
            {
                oR.menssage = "No Autorizado";
                return oR;
            }
            //Validaciones
            if (!Validate(model))
            {
                oR.menssage = error;
                return oR;
            }


            try
            {
                using (muestra_apiEntities db = new muestra_apiEntities())
                {
                    animal oAnimal = db.animal.Find(model.Id);
                    oAnimal.name = model.Nombre;
                    oAnimal.patas = model.Patas;

                    db.Entry(oAnimal).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    List<ListAnimalViewModel> Lst = list(db);

                    oR.data = Lst;
                    oR.result = 1;
                }
            }
            catch (Exception ex)
            {
                oR.menssage = "Ocurrio un error en el Servidor Intenta mas Tarde";
            }
            return oR;
        }


        [HttpPost]
        public Reply Delete([FromBody]AnimalViewModel model)
        {
            Reply oR = new Reply();
            oR.result = 0;

            if (!verify(model.token))
            {
                oR.menssage = "No Autorizado";
                return oR;
            }
     


            try
            {
                using (muestra_apiEntities db = new muestra_apiEntities())
                {
                    animal oAnimal = db.animal.Find(model.Id);

                    oAnimal.idState = 2;
                    
                    db.Entry(oAnimal).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    List<ListAnimalViewModel> Lst = list(db);

                    oR.data = Lst;
                    oR.result = 1;
                }
            }
            catch (Exception ex)
            {
                oR.menssage = "Ocurrio un error en el Servidor Intenta mas Tarde";
            }
            return oR;
        }
        
        [HttpPost]
        public async Task<Reply> Photo([FromUri]AnimalPictureViewModel model)
        {
            Reply oR = new Reply();
            oR.result = 0;

            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);

            if (!verify(model.token))
            {
                oR.menssage = "No Autorizado";
                return oR;
            }
            //recibir el multipart
            if(!Request.Content.IsMimeMultipartContent())
            {
                oR.menssage = "No estamos recibiendo una imagen";
                return oR;
            }

            await Request.Content.ReadAsMultipartAsync(provider);

            FileInfo fileInfoPicture = null;

            foreach(MultipartFileData fileData in provider.FileData)
            {
                if (fileData.Headers.ContentDisposition.Name.Replace("\\", "").Replace("\"","").Equals("picture"))
                    fileInfoPicture = new FileInfo(fileData.LocalFileName);
            }

            if(fileInfoPicture != null)
            {
                using(FileStream fs = fileInfoPicture.Open(FileMode.Open, FileAccess.Read))
                {
                    byte[] b = new byte[fileInfoPicture.Length];
                    UTF8Encoding temp = new UTF8Encoding(true);
                    while (fs.Read(b, 0, b.Length) > 0);

                    try
                    {
                        using(muestra_apiEntities db = new muestra_apiEntities())
                        {
                            var oAnimal = db.animal.Find(model.Id);
                            oAnimal.picture = b;
                            db.Entry(oAnimal).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            oR.result = 1;
                        }
                    }
                    catch (Exception ex)
                    {
                        oR.menssage = "intenta mas tarde";
                    }
                }
            }


            return oR;

        } 


        #region HELPERS

        private bool Validate(AnimalViewModel model)
        {
            if (model.Nombre == " ")
            {
                error = "El nombre es obligatorio";
                return false;
            }
            return true;
        }

        private List<ListAnimalViewModel> list(muestra_apiEntities db)
        {
            List<ListAnimalViewModel> Lst = (from p in db.animal
             where p.idState == 1
             select new ListAnimalViewModel
             {
                 Name = p.name,
                 Patas = p.patas
             }).ToList();

            return Lst;
        }

        #endregion

    }
}

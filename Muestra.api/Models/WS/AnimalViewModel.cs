using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Muestra.api.Models.WS
{
    public class AnimalViewModel : SecurityViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Patas { get; set; }
    }
}
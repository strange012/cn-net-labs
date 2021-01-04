using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNnet_Lab1.Models
{
   public class IdValidationsAttribute:System.Attribute
    {
        public int Id { get; private set; }
        public IdValidationsAttribute()
        { }
        public IdValidationsAttribute (int id)
        {
            Id = id;
        }

       

    }

}

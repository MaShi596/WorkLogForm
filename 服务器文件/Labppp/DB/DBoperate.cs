using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KjqbService.DB
{
    public class DBoperate
    {
        protected KjqbServiceEntities3 context;
        public DBoperate()
        {
            context = new KjqbServiceEntities3();
        }
        

    }
}
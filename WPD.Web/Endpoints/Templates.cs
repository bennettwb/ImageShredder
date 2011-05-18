using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WPD.Web.Endpoints
{
    public class Templates : Nina.Application
    {
        public Templates()
        {
            Post("", (m, c) =>
                         {
                             var results = new {Meid = 12345, Name = "Boo bear"};
                             return Json(results);
                         });

        }
    }
}
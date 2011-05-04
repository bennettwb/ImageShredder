using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shredder
{
    public class Broker
    {
        public void MakeKnown()
        {
            var db = Mongo.GetDatabase();

        }


    }

    public class Shredder
    {
        public string MachineName = Environment.MachineName;

        //public string s
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;

namespace WPD.Shared
{
    public static class Mongo
    {
        private const string _db = "shredder";

         static Mongo()
        {
            //if(CreateServer().GetDatabaseNames().Contains(_db))
            //{
            //    var mdb = CreateServer().CreateDatabaseSettings(_db);
            //    CreateServer().
            //}
        }

        public static MongoServer CreateServer()
        {
           return  MongoServer.Create("mongodb://fresewsmedia01,fresewsmedia02");
          
        }

        public static MongoDatabase GetDatabase()
        {
            return CreateServer().GetDatabase("shredder");
        }
    }
}

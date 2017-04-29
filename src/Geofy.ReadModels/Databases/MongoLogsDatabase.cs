﻿using System;
using MongoDB.Driver;

namespace Geofy.ReadModels.Databases
{
    public class MongoLogsDatabase
    {
        private readonly MongoInstance _mongo;

        public MongoLogsDatabase(String connectionString, bool authenticateToAdmin)
        {
            _mongo = new MongoInstance(connectionString, authenticateToAdmin);
        }

        /// <summary>
        /// Get database
        /// </summary>
        public IMongoDatabase Database => _mongo.Client.GetDatabase(_mongo.DefaultDatabase);

        //public MongoCollection Logs => Database.GetCollection("logs");

        //public MongoCollection AppLogs => Database.GetCollection("app_logs");
    }
}
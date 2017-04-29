﻿namespace Geofy.ReadModels.Databases
{
    public class MongoEventsDatabase
    {
        public MongoEventsDatabase(string connectionString, bool authenticateToAdmin)
        {
            MongoInstance = new MongoInstance(connectionString, authenticateToAdmin);
        }

        public MongoInstance MongoInstance { get; }
    }
}

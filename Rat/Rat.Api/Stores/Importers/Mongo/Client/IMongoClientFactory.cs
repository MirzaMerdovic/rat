using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rat.Api.Stores.Importers.Mongo.Client
{
    public interface IMongoClientFactory
    {
        IMongoClient Create();
    }
}

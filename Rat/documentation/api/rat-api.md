# Description

REST API built using AspNet.Core 5.0 and C#

# Responsibilites

1. Import configuration data from different data sources into it's own database
2. Expose endpoint that will enable consumers to manipulate configuration entries
3. Offer Pub-Sub functionality

## Configuration sources
Configuration sources can be:
* KV stores:
  - Consul (v2.0)
  - Etcd (v2.0)
* API
  - Rat Api (v1.0)
  - Other Api that can integrate with Rat Api (v1.0)

Note:  
Importing from other databases should be possible, but we need to think of an elegant and re-usable approach for it.  
For SQL databases (_SqlServer_, _Postgre_ and _MySQL_) we should be able to require a procedure to be supplied that will return a set of configuration entries. This way
only thing required from the use would be to write a stored procedure that returns a set of configuration entries. Rat Api would enforce expectation so that returned data could be consumed.
For no-Sql databases (_MongoDb_ and _CosmosDb_) this can be achieved via functions ([MongoDb](https://docs.mongodb.com/realm/functions/define-a-function/), [CosmosDb](https://docs.microsoft.com/en-us/azure/cosmos-db/sql-query-udfs)  

### Rat Api database
Supported databases:
* SqlServer (v1.0)
* MongoDb (v1.0)
* CosmosDb (v1.0)
* In memory (v1.0)
* MySql
* Postgre

# PubSub
Rat Api should support well known message brokers like:
* [RabbitMQ](https://www.rabbitmq.com/tutorials/tutorial-one-dotnet.html)
* [Azure ServiceBus](https://azure.microsoft.com/sv-se/blog/an-introduction-to-service-bus-topics/)
but should be able to provide PubSub capabilities using simple web sockets as well which should be considered the default PubSub provider.  

_How it should work?_  
Rat Api should expose subscribe endpoint which clients could use to subscribe to one or more configuration entries (keys). Rat Api should also provide abstraction layer so that messaging system can just be configured, which translates to the fact that client must not need to know or care which messeging system is used for PubSub. 

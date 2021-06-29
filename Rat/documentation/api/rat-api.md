# Description

REST API built using AspNet.Core 5.0 and C#

# Responsibilites

1. Import configuration data from different data sources into it's own database
2. Expose endpoint that will enable consumers to manipulate configuration entries
3. Offer Pub-Sub functionality

## Configuration sources
Configuration sources can be:
* databases:
  - SQL (v1.0)
  - MongoDb (v1.0)
  - Cosmos (v1.0)
* KV stores:
  - Consul (v2.0)
  - Etcd (v2.0)
* API
  - Rat Api (v1.0)
  - Other Api that can integrate with Rat Api (v1.0)

# PubSub
Rat Api should support well known message brokers like:
* RabbitMQ
* Azure ServiceBus
* Kafka
* SignalR (maybe?)

How it should work?
Rat Api should expose subscribe endpoint which clients could use to subscribe to one or more configuration entries (keys). Rat Api should also provide abstraction layer
so that messaging system can just be configured, which translates to the fact that client must not need to know or care which messeging system is used for PubSub. 

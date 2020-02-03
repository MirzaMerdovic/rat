using System;
using System.Runtime.Serialization;

namespace Rat.Exceptions
{
    public class ConfigurationEntryRetrievalFailed : Exception
    {
        public ConfigurationEntryRetrievalFailed()
        {
        }

        public ConfigurationEntryRetrievalFailed(string message)
            : base(message)
        {
        }

        public ConfigurationEntryRetrievalFailed(string key, string errorMessage)
            : base($"Retrieving of the item: {key} failed with error: {errorMessage}.")
        {
        }

        public ConfigurationEntryRetrievalFailed(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ConfigurationEntryRetrievalFailed(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
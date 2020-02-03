using System;
using System.Runtime.Serialization;

namespace Rat.Exceptions
{
    public class ConfigurationEntryNotFoundException : Exception
    {
        public ConfigurationEntryNotFoundException()
        {
        }

        public ConfigurationEntryNotFoundException(string key)
            : base($"The item with: {key} does not exist")
        {
        }

        public ConfigurationEntryNotFoundException(string key, Exception innerException)
            : base($"The item with: {key} does not exist", innerException)
        {
        }

        protected ConfigurationEntryNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
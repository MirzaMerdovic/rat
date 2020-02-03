﻿using System;

namespace Rat.Data
{
    public class ConfigurationEntry
    {
        public string Key { get; set; }

        public object Value { get; set; }

        public TimeSpan? Expiration { get; set; }
    }
}
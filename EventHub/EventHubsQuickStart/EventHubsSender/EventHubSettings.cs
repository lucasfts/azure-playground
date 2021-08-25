using System;
using System.Collections.Generic;
using System.Text;

namespace EventHubsSender
{
    public class EventHubSettings
    {
        public string ConnectionString { get; set; }
        public string Namespace { get; set; }
        public string EventHubName { get; set; }
        public int NumOfEvents { get; set; }
    }
}

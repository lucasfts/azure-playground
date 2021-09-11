using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TableSample.Models
{
    [Serializable]
    public class MessageModel
    {
        public MessageLevel Level { get; set; }
        public string Message { get; set; }
    }
}

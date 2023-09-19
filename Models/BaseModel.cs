using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoApi.Models
{
    public class BaseModel
    { 
        private string _EventLogMessage = "";

        public void SetEventLogMessage(string EventLogMessage)
        {
            _EventLogMessage = EventLogMessage;
        }

        public string GetEventLogMessage()
        {
            
            return _EventLogMessage;
        }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApi.Models
{
     [Table("tbl_eventlog")]
    public class EventLog: BaseModel
    {
        [Key]
        public long ID { get; set; }
        public EventLogType LogType { get; set; }
        public DateTime LogDateTime { get; set; }
        public string Source { get; set; } = string.Empty; // api or web or mobile
        public string FormName {get;set;}  = string.Empty; // controller.function name
        public string LogMessage { get; set; } = string.Empty;
        public string ErrMessage {get;set;}  = string.Empty; //ErrMessage for developer troubleshooting, do not show to user in Event Log report
        public int UserID { get; set; }
        public string UserType { get; set; } = string.Empty;
        public string ipAddress { get; set; } = string.Empty;
    }

    public enum EventLogType
    {
        Info = 1,
        Error = 2,
        Warning = 3,
        Insert = 4,
        Update = 5,
        Delete = 6
    }
}
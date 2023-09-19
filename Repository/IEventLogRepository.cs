using System.Collections.Generic;
using TodoApi.Models;

namespace TodoApi.Repositories
{
    public interface IEventLogRepository
    {
        Task AddEventLog(EventLogType LogTypeEnum, string LogMessage, string ErrMessage, string FormName="");
        Task Insert(dynamic obj);
        Task Update(dynamic obj);
        Task Delete(dynamic obj);
        Task Info(string LogMessage);
        Task Error(string LogMessage, string ErrMessage);
        Task Warning(string LogMessage);
    }
}

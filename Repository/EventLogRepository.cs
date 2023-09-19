using System.Collections.Generic;
using System.Linq;
using System;
using System.Security.Claims;
using System.Dynamic;
using TodoApi.Models;
using TodoApi.Util;
using Serilog;

namespace TodoApi.Repositories
{
    public class EventLogRepository : RepositoryBase<EventLog>, IEventLogRepository
    {
        public EventLogRepository(TodoContext repositoryContext)
            : base(repositoryContext)
        {
            
        }
       
        public async Task AddEventLog(EventLogType LogTypeEnum, string LogMessage, string ErrMessage, string FormName="")
        {
            EventLogType LogType = LogTypeEnum;

            string LoginRemoteIpAddress = "";
            ClaimsIdentity? objclaim = null;
                        
            string LoginUserID = "0";
            string LoginTypeParam = "0";
            string Source = "";

            if (RepositoryContext._httpContextAccessor.HttpContext != null) {
                LoginRemoteIpAddress = Globalfunction.GetClientIP(RepositoryContext._httpContextAccessor.HttpContext);
                objclaim = RepositoryContext._httpContextAccessor.HttpContext.User.Identities.Last(); //need to get Last, in login, normally single identity but during login tokenprovidermiddleware add custom identity because default identity has delay to get data.
                Source = RepositoryContext._httpContextAccessor.HttpContext.Request.Path.ToString().Split(new char[] { '/' })[1].ToString();
            }
            
            if(objclaim != null)
            {
                if(objclaim.FindFirst("UserID") != null) LoginUserID = objclaim.FindFirst("UserID")!.Value;
                if(objclaim.FindFirst("LoginType") != null) LoginTypeParam = objclaim.FindFirst("LoginType")!.Value;
            }           

            
            if(RepositoryContext._actionContextAccessor.ActionContext != null && FormName == "")
                FormName = RepositoryContext._actionContextAccessor.ActionContext.ActionDescriptor.DisplayName!.ToString().Replace("TodoApi.Controllers.", "").Replace("(TodoApi)", "");    
        
            if (LogMessage != "" || ErrMessage != "")
            {
                string LoginTypestr = "public";
                
                if (LoginTypeParam == "1")
                    LoginTypestr = "web";
                // else if(LoginTypeParam == "2")
                //     LoginTypestr = "Mobile";
                try
                {
                    var newobj = new EventLog
                    {
                        LogType = LogType,
                        LogDateTime = DateTime.Now,
                        Source = Source,
                        FormName = FormName,
                        LogMessage = LogMessage,
                        ErrMessage = ErrMessage,
                        UserID = int.Parse(LoginUserID),
                        UserType = LoginTypestr,
                        ipAddress = LoginRemoteIpAddress
                    };

                    await CreateAsync(newobj, true);
                }
                catch (Exception ex)
                {
                    Log.Error("Add Event Log SQL Exception :" + ex.Message);
                }
            }
        }
        
        public async Task Insert(dynamic obj)
        {
            string LogMessage = "";
            LogMessage += "Created :\r\n";
            LogMessage += this.SetOldObjectToString(obj);  // to include generate auto ID, do not use obj.GetEventLogMessage()
            await AddEventLog(EventLogType.Insert, LogMessage, "");
        }

        public async Task Update(dynamic obj)
        {
            string LogMessage = "";
            LogMessage += "Updated :\r\n";
            LogMessage += obj.GetEventLogMessage();
            await AddEventLog(EventLogType.Update, LogMessage, "");
        }

        public async Task Delete(dynamic obj)
        {
            string LogMessage = "";
            LogMessage += "Deleted :\r\n";
            LogMessage += obj.GetEventLogMessage();
            await AddEventLog(EventLogType.Delete, LogMessage, "");
        }

        public async Task Error(string LogMessage, string ErrMessage)
        {
            await AddEventLog(EventLogType.Error, LogMessage, ErrMessage);
        }

        public async Task Info(string LogMessage)
        {
            await AddEventLog(EventLogType.Info, LogMessage, "");
        }

        public async Task Warning(string LogMessage)
        {
            await AddEventLog(EventLogType.Warning, LogMessage, "");
        }
    }
}

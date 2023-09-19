using TodoApi.Models;
namespace TodoApi.Repositories
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private readonly TodoContext _repoContext;

        public RepositoryWrapper(TodoContext repositoryContext)
        {
            _repoContext = repositoryContext;
        }

        private IHeroRepository? oHero;
        public IHeroRepository Hero
        {
            get
            {
                if (oHero == null)
                {
                    oHero = new HeroRepository(_repoContext);
                }

                return oHero;
            }
        }

        // public object Employee => throw new NotImplementedException();
        private IEmployeeRepository? oEmployee;
        public IEmployeeRepository Employees
        {
            get
            {
                if (oEmployee == null)
                {
                    oEmployee = new EmployeeRepository(_repoContext);
                }

                return oEmployee;
            }
        }

         private ICustomerRepository? oCustomer;
        public ICustomerRepository Customer
        {
            get
            {
                if (oCustomer == null)
                {
                    oCustomer = new CustomerRepository(_repoContext);
                }

                return oCustomer;
            }
        }

         private IAdminRepository? oAdmin;
        public IAdminRepository Admin
        {
            get
            {
                if (oAdmin == null)
                {
                    oAdmin = new AdminRepository(_repoContext);
                }

                return oAdmin;
            }
        }
        private IAdminRepository? oAdminLevel;
        public IAdminRepository AdminLevel
        {
            get
            {
                if (oAdminLevel == null)
                {
                    oAdminLevel = new AdminRepository(_repoContext);
                }

                return oAdminLevel;
            }
        }

        private IOTPRepository? oOTP;
        public IOTPRepository OTP
        {
            get
            {
                if (oOTP == null)
                {
                    oOTP = new OTPRepository(_repoContext);
                }

                return oOTP;
            }
        }
        
        private IEventLogRepository? oEventLog;
        public IEventLogRepository EventLog
        {
            get
            {
                if (oEventLog == null)
                {
                    oEventLog = new EventLogRepository(_repoContext);
                }

                return oEventLog;
            }
        }
        
    }
    
}
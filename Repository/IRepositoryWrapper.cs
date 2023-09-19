namespace TodoApi.Repositories
{
    public interface IRepositoryWrapper
    {
        IHeroRepository Hero { get; }

        IEmployeeRepository Employees { get; }

        ICustomerRepository Customer { get; }

        IAdminRepository Admin {get; }

        IAdminRepository AdminLevel { get;}

        IOTPRepository OTP { get;}

        IEventLogRepository EventLog { get;}

    }
}

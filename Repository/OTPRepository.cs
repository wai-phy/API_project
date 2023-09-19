using System;
using System.Collections.Generic;
using System.Linq;
using TodoApi.Models;

namespace TodoApi.Repositories
{
    public class OTPRepository : RepositoryBase<OTP>, IOTPRepository
    {
        public OTPRepository(TodoContext repositoryContext) : base(repositoryContext)
        {
        }

    }
}

using LotTraceApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotTraceApp.Services
{
    public class BottleResultService
    {
        private readonly BottleResultRepositories _repo;
        public BottleResultService(BottleResultRepositories bottleResultRepositories) 
        {
            _repo = bottleResultRepositories ?? throw new ArgumentNullException(nameof(bottleResultRepositories));
        }
    }
}

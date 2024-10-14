using Fun_Funding.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun_Funding.Infrastructure.BackgroundWorkerService
{
    public class WorkerService : BackgroundService
    {

        private readonly ILogger<WorkerService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public WorkerService(IServiceScopeFactory serviceScopeFactory, ILogger<WorkerService> logger)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            TestHello();
            //await Task.Delay(2 * 3600 * 1000, stoppingToken);
        }

        public async Task TestHello()
        {
            _logger.LogInformation("Hello World at: {time}", DateTimeOffset.Now);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace Shared.Extensions
{
    public static class SeriLogExtension
    {
        public static void RegisterSeriLog(this IServiceCollection services, string logName)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
                .WriteTo.File($"logs/{logName}.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
            services.AddSerilog();
            
        }
    }
}
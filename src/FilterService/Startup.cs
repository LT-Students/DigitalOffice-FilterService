using HealthChecks.UI.Client;
using LT.DigitalOffice.FilterService.Models.Dto.Configurations;
using LT.DigitalOffice.Kernel.BrokerSupport.Configurations;
using LT.DigitalOffice.Kernel.BrokerSupport.Extensions;
using LT.DigitalOffice.Kernel.BrokerSupport.Helpers;
using LT.DigitalOffice.Kernel.BrokerSupport.Middlewares.Token;
using LT.DigitalOffice.Kernel.Configurations;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Helpers;
using LT.DigitalOffice.Kernel.Middlewares.ApiInformation;
using LT.DigitalOffice.Kernel.RedisSupport.Configurations;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Serilog;
using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.FilterService
{
  public class Startup : BaseApiInfo
  {
	public const string CorsPolicyName = "LtDoCorsPolicy";

	private readonly BaseServiceInfoConfig _serviceInfoConfig;
	private readonly RabbitMqConfig _rabbitMqConfig;

	public IConfiguration Configuration { get; }

	#region private methods

	private void ConfigureMassTransit(IServiceCollection services)
	{
      (string username, string password) = RabbitMqCredentialsHelper
        .Get(_rabbitMqConfig, _serviceInfoConfig);

      services.AddMassTransit(busConfigurator =>
	  {
		busConfigurator.UsingRabbitMq((context, cfg) =>
		{
		  cfg.Host(_rabbitMqConfig.Host, "/", host =>
		  {
			host.Username(username);
			host.Password(password);
		  });
		});

      busConfigurator.AddRequestClients(_rabbitMqConfig);
	  });

	  services.AddMassTransitHostedService();
	}

	#endregion

	public Startup(IConfiguration configuration)
	{
	  Configuration = configuration;

	  _serviceInfoConfig = Configuration
		.GetSection(BaseServiceInfoConfig.SectionName)
		.Get<BaseServiceInfoConfig>();

	  _rabbitMqConfig = Configuration
	    .GetSection(BaseRabbitMqConfig.SectionName)
	    .Get<RabbitMqConfig>();

	  Version = "1.0.0.0";
	  Description = "FilterService is an API that intended to find userss update user's their parameters.";
	  StartTime = DateTime.UtcNow;
	  ApiName = $"LT Digital Office - {_serviceInfoConfig.Name}";
	}

	public void ConfigureServices(IServiceCollection services)
	{
	  services.AddCors(options =>
	  {
	    options.AddPolicy(
		CorsPolicyName,
		builder =>
		{
		  builder
			.AllowAnyOrigin()
			.AllowAnyHeader()
			.AllowAnyMethod();
	    });
	  });

	  services.AddHttpContextAccessor();

	  services.AddHealthChecks()
          .AddRabbitMqCheck();

      services.Configure<TokenConfiguration>(Configuration.GetSection("CheckTokenMiddleware"));
	  services.Configure<BaseServiceInfoConfig>(Configuration.GetSection(BaseServiceInfoConfig.SectionName));
	  services.Configure<BaseRabbitMqConfig>(Configuration.GetSection(BaseRabbitMqConfig.SectionName));
	  services.Configure<ForwardedHeadersOptions>(options =>
	  {
		options.ForwardedHeaders =
			ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
      });

	  services.AddBusinessObjects();

	  services.AddControllers();
      services.AddTransient<IRedisHelper, RedisHelper>();

      ConfigureMassTransit(services);

      string redisConnStr = Environment.GetEnvironmentVariable("RedisConnectionString");
      if (string.IsNullOrEmpty(redisConnStr))
      {
        redisConnStr = Configuration.GetConnectionString("Redis");

        Log.Information($"Redis connection string from appsettings.json was used. Value '{PasswordHider.Hide(redisConnStr)}'");
      }
      else
      {
        Log.Information($"Redis connection string from environment was used. Value '{PasswordHider.Hide(redisConnStr)}'");
      }

      services.AddSingleton<IConnectionMultiplexer>(
        x => ConnectionMultiplexer.Connect(redisConnStr));
    }

	public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
	{
	  app.UseForwardedHeaders();

	  app.UseExceptionsHandler(loggerFactory);

	  app.UseApiInformation();

	  app.UseRouting();

	  app.UseMiddleware<TokenMiddleware>();

	  app.UseCors(CorsPolicyName);

	  app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers().RequireCors(CorsPolicyName);
        endpoints.MapHealthChecks($"/{_serviceInfoConfig.Id}/hc", new HealthCheckOptions
        {
          ResultStatusCodes = new Dictionary<HealthStatus, int>
          {
            { HealthStatus.Unhealthy, 200 },
            { HealthStatus.Healthy, 200 },
            { HealthStatus.Degraded, 200 },
          },
          Predicate = check => check.Name != "masstransit-bus",
          ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
      });
	}
  }
}

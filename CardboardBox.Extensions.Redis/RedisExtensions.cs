using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Net;

namespace CardboardBox.Extensions
{
	public static class RedisExtensions
	{
		public static IServiceCollection AddRedis(this IServiceCollection services, Func<ConnectionMultiplexer> getMultiplexer)
		{
			var multiplexer = getMultiplexer();

			return services
				.AddTransient<IRedisService, RedisService>()
				.AddSingleton<IConnectionMultiplexer>(multiplexer);
		}

		public static IServiceCollection AddRedis(this IServiceCollection services, Action<ConfigurationOptions> connectionOptions)
		{
			return services.AddRedis(() =>
			{
				var opts = new ConfigurationOptions();
				connectionOptions?.Invoke(opts);
				return ConnectionMultiplexer.Connect(opts);
			});
		}

		public static IServiceCollection AddRedis(this IServiceCollection services, bool allowAdmin, string password, params string[] hosts)
		{
			return services.AddRedis((opts) =>
			{
				opts.AllowAdmin = allowAdmin;
				opts.Password = password;

				if (hosts == null || hosts.Length <= 0)
				{
					opts.EndPoints.Add("localhost");
					return;
				}

				foreach (var host in hosts)
					opts.EndPoints.Add(host);
			});
		}

		public static IServiceCollection AddRedis(this IServiceCollection services, bool allowAdmin, string password, params (IPAddress, int)[] hosts)
		{
			return services.AddRedis((opts) =>
			{
				opts.AllowAdmin = allowAdmin;
				opts.Password = password;

				if (hosts == null || hosts.Length <= 0)
				{
					opts.EndPoints.Add("localhost");
					return;
				}

				foreach (var host in hosts)
					opts.EndPoints.Add(host.Item1, host.Item2);
			});
		}

		public static IServiceCollection AddRedis(this IServiceCollection services, bool allowAdmin, string password, params (string, int)[] hosts)
		{
			return services.AddRedis((opts) =>
			{
				opts.AllowAdmin = allowAdmin;
				opts.Password = password;

				if (hosts == null || hosts.Length <= 0)
				{
					opts.EndPoints.Add("localhost");
					return;
				}

				foreach (var host in hosts)
					opts.EndPoints.Add(host.Item1, host.Item2);
			});
		}
	}
}

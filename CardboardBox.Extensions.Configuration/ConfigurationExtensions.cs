using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;

namespace CardboardBox.Extensions
{
	/// <summary>
	/// A collection of extensions for handling Configuration functions
	/// </summary>
	public static class ConfigurationExtensions
	{
		/// <summary>
		/// A default collection of a loaders, supporting XML, JSON, and INI files
		/// </summary>
		private static Dictionary<string, Action<IConfigurationBuilder, string, bool, bool>> _loaders
			= new Dictionary<string, Action<IConfigurationBuilder, string, bool, bool>>
			{
				["xml"] = (b, f, o, r) => b.AddXmlFile(f, o, r),
				["json"] = (b, f, o, r) => b.AddJsonFile(f, o, r),
				["ini"] = (b, f, o, r) => b.AddIniFile(f, o, r)
			};

		/// <summary>
		/// Adds a configuration file to the specified builder
		/// </summary>
		/// <param name="builder">The configuration builder to add the settings file to</param>
		/// <param name="filename">The file path for the configuration settings file</param>
		/// <param name="optional">Whether or not the settings file is optional (defaults to true)</param>
		/// <param name="reloadOnChange">Whether or not to reload the configuration option when the file changes (defaults to true)</param>
		/// <returns>The configuration builder that was passed in</returns>
		public static IConfigurationBuilder AddFile(this IConfigurationBuilder builder, string filename, bool optional = true, bool reloadOnChange = true)
		{
			var ext = Path.GetExtension(filename).ToLower().Trim('.');

			if (_loaders.ContainsKey(ext))
			{
				_loaders[ext](builder, filename, optional, reloadOnChange);
				return builder;
			}

			throw new NotSupportedException($"Files with \"{ext}\" extensions are not yet supported by automatic resolution. Please resolve manully, or add an automatic resolver");
		}

		/// <summary>
		/// Adds all of the given configuration files to the specified builder
		/// </summary>
		/// <param name="builder">The builder to add the files to</param>
		/// <param name="filenames">The file names to add to the builder</param>
		/// <returns>The builder that was passed in</returns>
		public static IConfigurationBuilder AddFiles(this IConfigurationBuilder builder, params string[] filenames)
		{
			foreach (var file in filenames)
				builder.AddFile(file);

			return builder;
		}

		/// <summary>
		/// Adds a file loader to support a different type of settings file
		/// </summary>
		/// <param name="builder">The settings builder</param>
		/// <param name="extension">The extension of the settings file</param>
		/// <param name="loader">The loader to use</param>
		/// <returns>The configuration builder that was passed in</returns>
		public static IConfigurationBuilder AddLoader(this IConfigurationBuilder builder, string extension, Action<IConfigurationBuilder, string, bool, bool> loader)
		{
			extension = extension.ToLower().Trim('.');
			if (_loaders.ContainsKey(extension))
				_loaders[extension] = loader;
			else
				_loaders.Add(extension, loader);

			return builder;
		}

		/// <summary>
		/// Gets an object from the given configuration object
		/// </summary>
		/// <typeparam name="T">The type of object to build</typeparam>
		/// <param name="config">The configuration object</param>
		/// <param name="section">The section of the config file to use (or none if to bind the entire config object)</param>
		/// <returns>The configuration object</returns>
		public static T Get<T>(this IConfiguration config, string section = null) where T : new()
		{
			var settings = new T();

			if (string.IsNullOrEmpty(section))
			{
				var part = config.GetSection(section);
				part.Bind(settings);
				return settings;
			}

			config.Bind(settings);
			return settings;
		}

		/// <summary>
		/// Gets an object from the given configuration builder object
		/// </summary>
		/// <typeparam name="T">The type of object to build</typeparam>
		/// <param name="builder">The configuration builder object</param>
		/// <param name="section">The section of the config file to use (or none if to bind the entire config object)</param>
		/// <returns>The configuration object</returns>
		public static T Get<T>(this IConfigurationBuilder builder, string section = null) where T : new()
		{
			return builder.Build().Get<T>(section);
		}

		/// <summary>
		/// Fetches an object from the given configuration object
		/// </summary>
		/// <typeparam name="T">The configuration object to get</typeparam>
		/// <param name="config">The configuration object of fetch from</param>
		/// <param name="value">The value that was fetched from the configuration object</param>
		/// <param name="section">The section of the config file to use (or none if to bind the entire config object)</param>
		/// <returns>The configuration object that was passed in</returns>
		public static IConfiguration Get<T>(this IConfiguration config, out T value, string section = null) where T : new()
		{
			value = config.Get<T>(section);
			return config;
		}

		/// <summary>
		/// Fetches an object from the given configuration object
		/// </summary>
		/// <typeparam name="T">The configuration object to get</typeparam>
		/// <param name="builder">The configuration builder object of fetch from</param>
		/// <param name="value">The value that was fetched from the configuration object</param>
		/// <param name="section">The section of the config file to use (or none if to bind the entire config object)</param>
		/// <returns>The configuration object that was passed in</returns>
		public static IConfiguration Get<T>(this IConfigurationBuilder builder, out T value, string section = null) where T : new()
		{
			var config = builder.Build();
			value = config.Get<T>(section);
			return config;
		}
	}
}

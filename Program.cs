﻿using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using Spectre.Console.Cli.Extensions.DependencyInjection;
using TinyCity.BookmarkEngines;
using TinyCity.Commands;

namespace TinyCity
{
    public class Program
    {
        async static Task<int> Main(string[] args)
        {
            var services = SetupIoC();
            using var registrar = new DependencyInjectionRegistrar(services);
            var app = new CommandApp(registrar);
            app.Configure(config =>
            {
                config.SetApplicationVersion(GetVersion());
                config.SetApplicationName("tinycity");

                config.AddCommand<SearchCommand>("search")
                      .WithDescription("Search the bookmarks.");

                config.AddCommand<ListCommand>("list")
                      .WithAlias("ls")
                      .WithDescription("List all bookmarks.");

                config.AddCommand<ConfigCommand>("config")
                      .WithDescription("Configure bookmark sources.");
            });

            return await app.RunAsync(args);
        }

        static string GetVersion()
        {
            // Generated by GitVersion in its msbuild task
            return Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.1";
        }

        static ServiceCollection SetupIoC()
        {
            var settings = TinyCitySettings.Load();

            var services = new ServiceCollection();
            services.AddSingleton<ChromeBookmarks>();
            services.AddSingleton<MarkdownBookmarks>();
            services.AddSingleton<TinyCitySettings>(settings);

            return services;
        }
    }
}
﻿using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Shiny.Infrastructure;
using Shiny.Infrastructure.DependencyInjection;
using Shiny.Settings;
using Shiny.Testing.Settings;
using Xunit;


namespace Shiny.Tests.Infrastructure
{
    public class DependencyInjectionTests
    {
        static IServiceProvider Create(Action<TestSettings> addSettings = null, Action<IServiceCollection> addServices = null)
        {
            var serializer = new ShinySerializer();
            var settings = new TestSettings(serializer);
            addSettings?.Invoke(settings);

            var services = new ShinyServiceCollection();
            services.AddSingleton<IFullService, FullService>();
            services.AddSingleton<ISerializer>(serializer);
            services.AddSingleton<ISettings>(settings);
            addServices?.Invoke(services);

            var sp = services.BuildShinyServiceProvider(true);
            return sp;
        }


        static void SetCountKey(ISettings settings, int value)
        {
            var key = $"{typeof(FullService).FullName}.{nameof(FullService.Count)}";
            settings.Set(key, value);
        }


        [Fact]
        public void PostBuildRunsOnlyOnItsContainer()
        {
            var reg1 = 0;
            var reg2 = 0;
            var post1 = 0;
            var post2 = 0;

            var module1 = new TestModule(
                () => reg1++,
                () => post1++
            );
            var s1 = Create(null, s => s.RegisterModule(module1));

            var module2 = new TestModule(
                () => reg2++,
                () => post2++
            );
            var s2 = Create(null, s => s.RegisterModule(module2));
            reg1.Should().Be(1);
            reg2.Should().Be(1);

            post1.Should().Be(1);
            post2.Should().Be(1);
        }


        [Fact]
        public void ServiceRestoresStateAndStartsUp()
        {
            var setValue = new Random().Next(1, 9999);
            var postValue = setValue + 1;

            var services = Create(s => SetCountKey(s, setValue));
            services
                .GetService<IFullService>()
                .Count
                .Should()
                .Be(postValue);
        }
    }
}

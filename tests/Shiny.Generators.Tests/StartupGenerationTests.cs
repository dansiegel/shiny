﻿using System;

using Microsoft.CodeAnalysis;

using Xunit;
using Xunit.Abstractions;


namespace Shiny.Generators.Tests
{
    public class StartupGenerationTests : IDisposable
    {
        readonly ITestOutputHelper output;
        readonly AssemblyGenerator generator;
        Compilation compilation;


        public StartupGenerationTests(ITestOutputHelper output)
        {
            this.output = output;
            this.generator = new AssemblyGenerator();
            this.generator.AddReference("Mono.Android");
            this.generator.AddReference("Shiny");
            this.generator.AddReference("Shiny.Core");
        }


        public void Dispose()
        {
            if (this.compilation != null)
                this.output.WriteSyntaxTrees(this.compilation);
        }


        [Fact]
        public void Test()
        {
            this.generator.AddSource("[assembly: Shiny.ShinyApplicationAttribute]");
            this.compilation = this.generator.DoGenerate(
                nameof(Test),
                new AndroidApplicationSourceGenerator()
            );
            //compile.AssertTypesExist(""); // android app, shiny startup
        }


        [Fact]
        public void TestJobDetection()
        {
            this.generator.AddSource(@"
[assembly: Shiny.ShinyApplicationAttribute]
using System;
using System.Threading;
using System.Threading.Tasks;


namespace MyTest 
{
    public class DetectionJob : Shiny.Jobs.IJob 
    {
        public async Task Run(JobInfo jobInfo, CancellationToken cancelToken) {}
    }
}");
            this.compilation = this.generator.DoGenerate(
                nameof(TestJobDetection),
                new AndroidActivitySourceGenerator()
            );
        }


        public void TestModuleDetection()
        {

        }


        public void TestStartupTaskDetection()
        {

        }
    }
}

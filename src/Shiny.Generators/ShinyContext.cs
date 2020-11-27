﻿using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;


namespace Shiny.Generators
{
    public interface IShinyContext
    {
        GeneratorExecutionContext Context { get; }
        INamedTypeSymbol? GetShinyType(string fullyQualifiedMetadataName);
        //string? GetShinyStartupClassFullName();
        //string? GetXamFormsAppClassFullName();
    }


    public class ShinyContext : IShinyContext
    {
        readonly Lazy<IAssemblySymbol[]> shinyAssemblies;


        public ShinyContext(GeneratorExecutionContext context)
        {
            this.Context = context;
            this.shinyAssemblies = new Lazy<IAssemblySymbol[]>(() => context
                .Compilation
                .References
                .Where(x =>
                    x.Display != null &&
                    x.Properties.Kind == MetadataImageKind.Assembly &&
                    Regex.IsMatch(x.Display, "Shiny.(.*).dll")
                )
                .Select(context.Compilation.GetAssemblyOrModuleSymbol)
                .OfType<IAssemblySymbol>()
                .ToArray()
            );
        }


        public GeneratorExecutionContext Context { get; private set; }
        public INamedTypeSymbol? GetShinyType(string fullyQualifiedMetadataName) => this.shinyAssemblies
            .Value
            .Select(x => x.GetTypeByMetadataName(fullyQualifiedMetadataName))
            .FirstOrDefault();



        //public string? GetXamFormsAppClassFullName()
        //{
        //    var classes = this
        //        .GetAllDerivedClassesForType("Xamarin.Forms.Application")
        //        .Where(x => !x.ContainingNamespace.Name.StartsWith("Prism."))
        //        .WhereNotSystem()
        //        .ToList();

        //    INamedTypeSymbol? appClass = null;
        //    switch (classes.Count)
        //    {
        //        case 0:
        //            //this.Log.Warn("No Xamarin Forms App implementations found");
        //            break;

        //        case 1:
        //            appClass = classes.First();
        //            break;

        //        default:
        //            //this.Log.Warn(classes.Count + " Xamarin Forms App implementations found");
        //            //foreach (var cls in classes)
        //            //    this.Log.Warn(" - " + cls.ToDisplayString());

        //            //appClass = this.FindClosestType(classes);
        //            //if (appClass != null)
        //            //    this.Log.Warn($"Found closest type - {appClass.ToDisplayString()}.  IF this is wrong, please override the type where this is being used");

        //            break;
        //    }
        //    return appClass?.ToDisplayString();
        //}


        //public string? GetShinyStartupClassFullName()
        //{
        //    if (this.IsStartupGenerated)
        //    {
        //        var ns = this.GetRootNamespace();
        //        return $"{ns}.AppShinyStartup";
        //    }
        //    var startupClasses = this
        //        .GetAllImplementationsOfType("Shiny.IShinyStartup")
        //        .WhereNotSystem()
        //        .ToList();

        //    INamedTypeSymbol? startupClass = null;
        //    switch (startupClasses.Count)
        //    {
        //        case 0:
        //            //this.Log.Warn("No Shiny Startup implementation found");
        //            break;

        //        case 1:
        //            startupClass = startupClasses.First();
        //            break;

        //        default:
        //            this.Context.ReportDiagnostic(
        //                Diagnostic.Create(
        //                    new DiagnosticDescriptor(
        //                        "SHINY",
        //                        "",
        //                        "",
        //                        "SHINY",
        //                        DiagnosticSeverity.Warning,
        //                        true
        //                    ),
        //                    Location.None
        //                )
        //            );
        //            //this.Log.Warn(startupClasses.Count + " Shiny Startup implementations found");
        //            //foreach (var sc in startupClasses)
        //            //    this.Log.Warn(" - " + sc.ToDisplayString());

        //            startupClass = this.FindClosestType(startupClasses);
        //            //if (startupClass != null)
        //            //    this.Log.Warn($"Found closest type - {startupClass.ToDisplayString()}.  IF this is wrong, please override the type where this is being used");

        //            break;
        //    }
        //    return startupClass?.ToDisplayString();
        //}


        //public INamedTypeSymbol FindClosestType(List<INamedTypeSymbol> symbols)
        //{
        //    var ns = this.GetRootNamespace();
        //    var index = ns.IndexOf(".");
        //    if (index > -1)
        //        ns = ns.Substring(0, index);

        //    var found = symbols.FirstOrDefault(x => x.ContainingNamespace.Name.StartsWith(ns));
        //    return found;
        //}


        //public bool IsStartupGenerated { get; set; }
    }
}

using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;

namespace CreatorChannelsXrmToolbox
{
    // Do not forget to update version number and author (company attribute) in AssemblyInfo.cs class
    // To generate Base64 string for Images below, you can use https://www.base64-image.de/
    [Export(typeof(IXrmToolBoxPlugin)),
        ExportMetadata("Name", "Generate custom channel solution"),
        ExportMetadata("Description", "This component allows you to generate a Dynamics 365 solution to establish a custom channel in Customer Insights - Journeys."),
        // Please specify the base64 content of a 32x32 pixels image
        ExportMetadata("SmallImageBase64", "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"),
        // Please specify the base64 content of a 80x80 pixels image
        ExportMetadata("BigImageBase64", "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"),
        ExportMetadata("BackgroundColor", "Lavender"),
        ExportMetadata("PrimaryFontColor", "Black"),
        ExportMetadata("SecondaryFontColor", "Gray")]
public class MyPlugin : PluginBase
{
    public override IXrmToolBoxPluginControl GetControl()
    {
        return new MyCustomChannelControl();
    }

    /// <summary>
    /// Constructor 
    /// </summary>
    public MyPlugin() {
        // If you have external assemblies that you need to load, uncomment the following to 
        // hook into the event that will fire when an Assembly fails to resolve
        // AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyResolveEventHandler);
    }

    /// <summary>
    /// Event fired by CLR when an assembly reference fails to load
    /// Assumes that related assemblies will be loaded from a subfolder named the same as the Plugin
    /// For example, a folder named Sample.XrmToolBox.MyPlugin 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    private Assembly AssemblyResolveEventHandler(object sender, ResolveEventArgs args)
    {
        Assembly loadAssembly = null;
        Assembly currAssembly = Assembly.GetExecutingAssembly();

        // base name of the assembly that failed to resolve
        var argName = args.Name.Substring(0, args.Name.IndexOf(","));

        // check to see if the failing assembly is one that we reference.
        List<AssemblyName> refAssemblies = currAssembly.GetReferencedAssemblies().ToList();
        var refAssembly = refAssemblies.Where(a => a.Name == argName).FirstOrDefault();

        // if the current unresolved assembly is referenced by our plugin, attempt to load
        if (refAssembly != null)
        {
            // load from the path to this plugin assembly, not host executable
            string dir = Path.GetDirectoryName(currAssembly.Location).ToLower();
            string folder = Path.GetFileNameWithoutExtension(currAssembly.Location);
            dir = Path.Combine(dir, folder);

            var assmbPath = Path.Combine(dir, $"{argName}.dll");

            if (File.Exists(assmbPath))
            {
                loadAssembly = Assembly.LoadFrom(assmbPath);
            }
            else
            {
                throw new FileNotFoundException($"Unable to locate dependency: {assmbPath}");
            }
        }

        return loadAssembly;
    }
}
}
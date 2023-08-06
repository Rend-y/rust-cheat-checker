using System.Collections.Generic;

namespace RCC.Modules.DangerousApp;

public interface IDangerousApp<TDangerousApplication>
{
    /// <summary>
    ///     List file name dangerous application
    /// </summary>
    public List<string> ListDangerousApplications { get; }

    /// <summary>
    ///     List all dangerous application
    /// </summary>
    public List<TDangerousApplication> ListFindDangerousApplications { get; }

    public List<string> ListRegistryKey { get; }

    /// <summary>
    ///     Get all dangerous application in registry and return list dangerous application
    /// </summary>
    /// <returns></returns>
    public List<TDangerousApplication> FindAllApplicationInRegistry(in string registryKey);
}
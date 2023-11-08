namespace RCC.Modules.DangerousApp;

public struct SDangerousApplication
{
    /// <summary>
    ///     Fine name of the application
    /// </summary>
    public string DisplayName { get; private set; }

    /// <summary>
    ///     Full path to the application
    /// </summary>
    public string InstallPath { get; private set; }

    public SDangerousApplication(string displayName, string installPath)
    {
        DisplayName = displayName;
        InstallPath = installPath;
    }
}
namespace Coda.RoundRobin.WebApi.Constants;

using System.Diagnostics;

internal static class ServiceConstants
{
    private static readonly FileVersionInfo FILE_VERSION_INFO = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);

    public static string ServiceName => FILE_VERSION_INFO.ProductName!;
    public static string ServiceVersion => FILE_VERSION_INFO.ProductVersion!;
}

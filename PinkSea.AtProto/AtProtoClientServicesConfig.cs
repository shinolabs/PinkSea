namespace PinkSea.AtProto;

/// <summary>
/// Configuration for the ATProto client services DI injector.
/// </summary>
public class AtProtoClientServicesConfig
{
    /// <summary>
    /// The PLC directory.
    /// </summary>
    public Uri PlcDirectory { get; set; } = new("https://plc.directory");
}
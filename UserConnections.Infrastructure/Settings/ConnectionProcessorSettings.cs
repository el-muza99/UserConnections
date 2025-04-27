namespace UserConnections.Infrastructure.Settings;

public class ConnectionProcessorSettings
{
    public const string SectionName = "ConnectionProcessorSettings";
    
    public int BatchSize { get; set; } = 100;
} 
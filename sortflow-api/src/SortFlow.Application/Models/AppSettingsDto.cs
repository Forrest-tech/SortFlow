namespace SortFlow.Application.Models;

public sealed class AppSettingsDto
{
    public Guid Id { get; set; }
    public double GeneratorRatePerSecond { get; set; }
    public double AddressMismatchProbability { get; set; }
    public double InvalidPostalProbability { get; set; }
    public double DamagedLabelProbability { get; set; }
    public int DashboardWindowMinutes { get; set; }
    public string EnableModules { get; set; } = string.Empty;
    public DateTimeOffset UpdatedAt { get; set; }
}

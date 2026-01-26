namespace SortFlow.Domain.Entities;

public class AppSettings
{
    public Guid Id { get; set; }
    public double GeneratorRatePerSecond { get; set; } = 1.0;
    public double AddressMismatchProbability { get; set; } = 0.03;
    public double InvalidPostalProbability { get; set; } = 0.02;
    public double DamagedLabelProbability { get; set; } = 0.03;
    public int DashboardWindowMinutes { get; set; } = 60;
    /// <summary>JSON: {"Dashboard":true,"Events":true,"Exceptions":true,"Zones":true,"Stations":true,"History":true,"Settings":true}</summary>
    public string EnableModules { get; set; } = "{\"Dashboard\":true,\"Events\":true,\"Exceptions\":true,\"Zones\":true,\"Stations\":true,\"History\":true,\"Settings\":true}";
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

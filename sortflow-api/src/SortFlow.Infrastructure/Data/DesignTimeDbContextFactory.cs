using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SortFlow.Infrastructure.Data;

public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<SortFlowDbContext>
{
    public SortFlowDbContext CreateDbContext(string[] args)
    {
        var conn = Environment.GetEnvironmentVariable("SortFlowDb")
            ?? "Host=localhost;Port=5432;Database=sortflow;Username=sortflow;Password=sortflow_pw";

        var options = new DbContextOptionsBuilder<SortFlowDbContext>()
            .UseNpgsql(conn);

        return new SortFlowDbContext(options.Options);
    }
}

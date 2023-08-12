using Models.Common;

namespace Models;

public record StatementsReport
{
    public string Name { get; set; }

    public Period Period { get; set; }
    
    public IReadOnlyCollection<ReportCategory> Categories { get; set; }
}
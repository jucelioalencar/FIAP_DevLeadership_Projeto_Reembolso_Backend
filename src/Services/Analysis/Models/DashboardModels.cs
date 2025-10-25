namespace CorisSeguros.Analysis.Models;

public class DashboardMetrics
{
    public int TotalDocuments { get; set; }
    public int ApprovedDocuments { get; set; }
    public int RejectedDocuments { get; set; }
    public double AverageProcessingTime { get; set; }
    public double AutomationRate { get; set; }
    public double ErrorRate { get; set; }
    public double TmaReduction { get; set; }
    public double ErrorReduction { get; set; }
}

public class TmaEvolutionPoint
{
    public string Month { get; set; } = string.Empty;
    public double Tma { get; set; }
}

public class StatusDistribution
{
    public int Total { get; set; }
    public int Approved { get; set; }
    public int Rejected { get; set; }
    public int InAnalysis { get; set; }
    public int Pending { get; set; }
}

public class DailyProcessing
{
    public string Day { get; set; } = string.Empty;
    public int Automatic { get; set; }
    public int Manual { get; set; }
    public int Total { get; set; }
}

public class ServiceStatus
{
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int Latency { get; set; }
    public DateTime LastCheck { get; set; }
}




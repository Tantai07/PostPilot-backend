namespace PostPilot.Api.Shared;

public class BaseQuery
{
    public string? Keyword { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
    public bool? NoPaging { get; set; }
}

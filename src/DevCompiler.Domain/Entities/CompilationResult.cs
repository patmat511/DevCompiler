namespace DevCompiler.Domain.Entities;  

public class CompilationResult
{
    public bool Success { get; set; }
    public string Output { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warrnings { get; set; } = new();
    public TimeSpan ExecutionTime { get; set; }

}
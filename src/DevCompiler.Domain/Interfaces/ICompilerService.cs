using DevCompiler.Domain.Entities;

namespace DevCompiler.Domain.Interfaces;

public interface ICompilerService
{
    Task<CompilationResult> CompileAndExecuteAsync(string code, CancellationToken cancellationToken = default); 
}   
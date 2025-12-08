using DevCompiler.Domain.Entities;
using DevCompiler.Domain.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace DevCompiler.Infrastructure.Compiler;

public class RoslynCompilerService : ICompilerService
{
    // Namespaces that cannot be executed for security reasons

    private static readonly string[] BlockedNamespaces = new[]
    {
        "System.IO",
        "System.Net",
        "System.Net.Sockets",
        "System.Reflection",
        "System.Threading",
        "System.Threading.Tasks",
        "System.Runtime",
        "System.Diagnostics",
        "System.Security",
        "System.Security.Cryptography",
        "Microsoft.Win32",
        "System.Environment",
        "System.AppDomain"
    };


    public async Task<CompilationResult> CompileAndExecuteAsync(string code, CancellationToken cancellationToken = default)
    {
        var result = new CompilationResult();
        var stopwatch = Stopwatch.StartNew();

        try
        {
            // Block dangerous namespaces
            foreach (var blockedNs in BlockedNamespaces)
            {
                if (code.Contains(blockedNs))
                {
                    result.Success = false;
                    result.Errors.Add($"Security: Usage of namespace '{blockedNs}' is not allowed");
                    return result;
                }
            }

            if (code.Length > 10000)
            {
                result.Success = false;
                result.Errors.Add("Code exceeds maximum length of 10,000 characters");
                return result;
            }

            // Wrap code in Main method if needed
            var wrappedCode = WrapCodeIfNeeded(code);

            var syntaxTree = CSharpSyntaxTree.ParseText(wrappedCode);

            var references = new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
                MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location),
                MetadataReference.CreateFromFile(Assembly.Load("System.Console").Location)
            };

            var compilation = CSharpCompilation.Create(
                "DynamicAssembly",
                new[] { syntaxTree },
                references,
                new CSharpCompilationOptions(OutputKind.ConsoleApplication)
            );

            using var ms = new MemoryStream();
            EmitResult emitResult = compilation.Emit(ms);

            if (!emitResult.Success)
            {
                result.Success = false;
                foreach (var diagnostic in emitResult.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error))
                {
                    result.Errors.Add(diagnostic.GetMessage());
                }

                return result;
            }

            // Execute with timeout
            ms.Seek(0, SeekOrigin.Begin);
            var assembly = Assembly.Load(ms.ToArray());
            var entryPoint = assembly.EntryPoint;

            if (entryPoint == null)
            {
                result.Success = false;
                result.Errors.Add("No entry point found in compiled code");
                return result;
            }

            // Capture console output
            var originalOut = Console.Out;
            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            {
                Console.SetOut(writer);

                try
                {
                    // Execute with timeout
                    var task = Task.Run(() =>
                    {
                        try
                        {
                            entryPoint.Invoke(null, entryPoint.GetParameters().Length == 0 ? null : new object[] { Array.Empty<string>() });
                        }
                        catch (Exception ex)
                        {
                            throw new InvalidOperationException($"Runtime error: {ex.InnerException?.Message ?? ex.Message}");
                        }
                    }, cancellationToken);

                    if (!task.Wait(TimeSpan.FromSeconds(5)))
                    {
                        result.Success = false;
                        result.Errors.Add("Execution timeout (5 seconds exceeded)");
                        return result;
                    }

                    result.Success = true;
                    result.Output = sb.ToString();
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Errors.Add(ex.Message);
                }
                finally
                {
                    Console.SetOut(originalOut);
                }
            }

            stopwatch.Stop();
            result.ExecutionTime = stopwatch.Elapsed;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Errors.Add($"Compilation error: {ex.Message}");
        }

        return result;
    }

    private static string WrapCodeIfNeeded(string code)
    {
        // If code already has a Main method or class, return as is
        if (code.Contains("static void Main") || code.Contains("class Program"))
            return code;

        // Otherwise, wrap in a Main method
        return $@"
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

public class Program
{{
    public static void Main(string[] args)
    {{
        {code}
    }}
}}";
    }
}
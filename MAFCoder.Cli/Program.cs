using System.ComponentModel;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using Spectre.Console;

#pragma warning disable OPENAI001

Console.OutputEncoding = System.Text.Encoding.UTF8;

AnsiConsole.Background = Color.Black;
AnsiConsole.Clear();
AnsiConsole.Write(new FigletText("MAFCoder.CLI").Color(Color.Blue).Centered());
AnsiConsole.Write(new Markup("[blue]a CLI coding agent you can actually understand[/] - [green]built on .NET[/]\n\n\n").Centered());

const string model = "gpt-4o";
var key = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

if (key == null)
{
    AnsiConsole.WriteLine("Environment variable OPENAI_API_KEY is not set. Exiting...");
    return;
}

var workingDirectory = Directory.GetCurrentDirectory();

if (args.Length == 0)
{
    AnsiConsole.WriteLine($"No arguments provided. Defaulting to current directory. '{workingDirectory}'");
}
else
{
    workingDirectory = args[0];
    if (!Directory.Exists(workingDirectory))
    {
        AnsiConsole.WriteLine($"Provided directory '{workingDirectory}' does not exist. Exiting...");
        return;
    }
    AnsiConsole.WriteLine($"Using provided working directory: '{workingDirectory}'");
}

var calculator = new Calculator();
var fileOperators = new FileOperators(workingDirectory);

AIAgent agent = new OpenAIClient(key).GetOpenAIResponseClient(model).CreateAIAgent(
    "You are an annoyingly friendly AI Assistant. Explain what you are doing.",
    tools:
    [
        new HostedWebSearchTool(),
        AIFunctionFactory.Create(calculator.Add),
        AIFunctionFactory.Create(calculator.Random),
        AIFunctionFactory.Create(calculator.Subtract),
        AIFunctionFactory.Create(fileOperators.ReadFile),
        AIFunctionFactory.Create(fileOperators.WriteFile),
        AIFunctionFactory.Create(fileOperators.ListFilesAndDirectories),
        AIFunctionFactory.Create(fileOperators.CreateFolder),
        AIFunctionFactory.Create(fileOperators.InvokeCommandLine),
    ])
.AsBuilder()
.Use(CustomFunctionCallingMiddleware)
.Build();

var thread = agent.GetNewThread();

while (true)
{
    AnsiConsole.WriteLine();
    AnsiConsole.WriteLine();
    var userInput = await AnsiConsole.AskAsync<string>("[red]> [/]", "Enter for samples");
    
    if (userInput == "Enter for samples")
    {
        var samples = new[]
        {
            "Add 2 random numbers",
            "What files are in the current directory? Just give a summary.",
            "Use command line to get the date",
            "Create a new Hello World .net app here, using the InvokeCommandLine tool.",
        };
        
        userInput = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select a sample prompt:")
                .AddChoices(samples));
        
        AnsiConsole.Markup($"[yellow]{userInput}\n\n[/]");
    }

    AnsiConsole.Foreground = Color.White;
    
    await foreach (var update in agent.RunStreamingAsync(userInput, thread))
    {
        Console.Write(update.Text);   // using Console to avoid AnsiConsole interpreting special characters and throwing errors
    }
}

async ValueTask<object?> CustomFunctionCallingMiddleware(
    AIAgent aiAgent,
    FunctionInvocationContext context,
    Func<FunctionInvocationContext, CancellationToken, ValueTask<object?>> next,
    CancellationToken cancellationToken)
{
    AnsiConsole.Foreground = Color.Yellow;
    AnsiConsole.WriteLine($"Function Call: {context.Function.Name}({string.Join(',', context.Arguments.Select(a => $"{a.Key}: {a.Value}"))})");
    var result = await next(context, cancellationToken);
    AnsiConsole.WriteLine($"Function Call Result: {result}");
    return result;
}

public class Calculator
{
    public int Add(int a, int b) => a + b;
    public int Subtract(int a, int b) => a - b;
    public int Random(int min, int max) => System.Random.Shared.Next(min, max);
}

public class FileOperators(string rootPath)
{
    public string ReadFile(string path)
    {
        return File.ReadAllText(Path.Combine(rootPath, path));
    }
    
    public string WriteFile(string path, string content)
    {
        File.WriteAllText(Path.Combine(rootPath, path), content);
        return "File written successfully.";
    }
    
    public string CreateFolder(string path)
    {
        Directory.CreateDirectory(Path.Combine(rootPath, path));
        return "Directory created successfully.";
    }
    
    public string ListFilesAndDirectories(string directory)
    {
        var path = Path.Combine(rootPath, directory);
        var entries = new DirectoryInfo(path).EnumerateFileSystemInfos();
        return string.Join(Environment.NewLine, 
            entries.Select(entry => 
            {
                var size = entry is FileInfo file ? $"{file.Length:N0} bytes" : "<DIR>";
                return $"{entry.Name} [[{(entry is FileInfo ? "File" : "Directory")}]] {size}";
            }));
    }
    
    [Description("Use this command to execute command line commands. Returns the output of the command. You won't be able to use commands that require user interaction.")]
    public string InvokeCommandLine(string command)
    {
        using var process = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("cmd.exe", $"/c {command}")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = rootPath
        });

        if (process == null)
        {
            return "Error: Unable to start process.";
        }

        process.StandardInput.Close();  // Close input to signal no more input will be sent.
        
        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();
        
        process.WaitForExit();

        return !string.IsNullOrWhiteSpace(error) ? $"Error: {error.Trim()}" : output;
    }
}
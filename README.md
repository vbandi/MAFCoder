# MAFCoder.CLI

**Build Your Own CLI Coding Agent with Microsoft's Agent Framework**

> *"Coding agents are everywhere - but they are not entirely magic."*

This is the sample code from the session ["The Agent Awakens: Build Your Own CLI Coding Agent"](https://sessionize.com/s/velvart-andras/the-agent-awakens-build-your-own-cli-coding-agent-/151656) by Velvart András.

A fast-paced, live-coding exploration showing how to build a functioning coding agent from scratch in under an hour - going from blank console app to an agent that understands tasks, calls tools, and writes its own code.

## 🎯 Overview

Coding agents like Claude Code and Codex are everywhere - but they're **not entirely magic**. 

This project shows that you can build your own CLI coding agent **from scratch, in under an hour**, using Microsoft's Agent Framework and .NET. Starting with a blank console app, this fast-paced session demonstrates how modern CLI coding agents actually work: understanding tasks, calling tools, and even writing and testing their own code.

**The result?** Part engineering experiment, part live exploration of how the next generation of developer tools will be built. You'll see that with just .NET, the Agent Framework, an API key, and curiosity - you can demystify coding agents and understand exactly how they work.

## ✨ What You'll See

- 🎯 **Behind the Scenes**: How modern CLI coding agents actually work
- 🤖 **Microsoft Agent Framework**: Building with Microsoft's official Agents AI framework
- 🛠️ **Tool Calling in Action**: Watch the agent understand tasks and invoke tools (file operations, command execution, web search)
- 💻 **Self-Writing Code**: An agent that can write and test its own code
- 🔍 **Transparent Operations**: Middleware that shows exactly what the agent is doing at each step
- 🎨 **Beautiful CLI**: Powered by Spectre.Console for rich terminal output
- 🎓 **Clear Mental Model**: Simple, understandable code - no magic, just engineering

## 📋 Prerequisites

- .NET 9.0 SDK or later
- OpenAI API Key
- Windows OS (uses `cmd.exe` for command execution)

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone <repository-url>
cd MAFCoder
```

### 2. Set Up Your OpenAI API Key

Set the `OPENAI_API_KEY` environment variable:

**Windows (PowerShell):**
```powershell
$env:OPENAI_API_KEY = "your-api-key-here"
```

**Windows (Command Prompt):**
```cmd
set OPENAI_API_KEY=your-api-key-here
```

For permanent setup, add it to your system environment variables.

### 3. Build the Project

```bash
dotnet build
```

### 4. Run the Application

```bash
dotnet run --project MAFCoder.Cli
```

Or with a specific working directory:

```bash
dotnet run --project MAFCoder.Cli -- "C:\path\to\your\directory"
```

## 💬 Usage

> **Session Rule**: No AI-written code until our agent learns to code for itself! 🤖

Once running, you can interact with the agent using natural language. The CLI provides sample prompts to get you started:

- **"Add 2 random numbers"** - Demonstrates the calculator functionality
- **"What files are in the current directory? Just give a summary."** - Lists directory contents
- **"Use command line to get the date"** - Executes a shell command
- **"Create a new Hello World .net app here"** - Creates a new .NET application

Or ask your own questions and give custom instructions!

## 🛠️ Available Tools

The agent has access to the following tools:

### Calculator
- `Add(int a, int b)` - Add two numbers
- `Subtract(int a, int b)` - Subtract two numbers
- `Random(int min, int max)` - Generate a random number

### File Operations
- `ReadFile(string path)` - Read file contents
- `WriteFile(string path, string content)` - Write content to a file
- `CreateFolder(string path)` - Create a new directory
- `ListFilesAndDirectories(string directory)` - List directory contents

### System
- `InvokeCommandLine(string command)` - Execute shell commands
- `HostedWebSearchTool()` - Search the web

## 🏗️ Architecture - How Coding Agents Actually Work

The application demystifies coding agents by demonstrating the key concepts behind tools like Claude Code and Codex:

1. **Microsoft Agents Framework**: Using `Microsoft.Agents.AI.OpenAI` for AI agent capabilities
2. **Tool/Function Calling**: Exposing C# methods as tools the AI can invoke - this is how agents "do" things
3. **Middleware Pattern**: Custom function calling middleware for complete transparency - see exactly what the agent is thinking and doing
4. **Thread Management**: Maintaining conversation context across multiple interactions
5. **Streaming Responses**: Real-time output as the AI generates responses
6. **Scoped Operations**: Safe file system access limited to a working directory

**The Mental Model**: An agent is essentially an LLM + Tools + a conversation loop. That's it. No magic.

## 📦 Dependencies

- **Microsoft.Agents.AI.OpenAI** (v1.0.0-preview.251028.1) - AI agent framework
- **Spectre.Console** (v0.53.1-preview.0.2) - Terminal UI library
- **OpenAI** - OpenAI API client

## 🔍 Code Highlights

### Creating the AI Agent

```csharp
AIAgent agent = new OpenAIClient(key)
    .GetOpenAIResponseClient("gpt-4o-mini")
    .CreateAIAgent(
        "You are an annoyingly friendly AI Assistant...",
        tools: [
            new HostedWebSearchTool(),
            AIFunctionFactory.Create(calculator.Add),
            // ... more tools
        ])
    .AsBuilder()
    .Use(CustomFunctionCallingMiddleware)
    .Build();
```

### Custom Middleware

The application includes transparent function calling middleware that displays:
- Which function is being called
- What arguments are passed
- The result of the function call

## 🎓 What You'll Learn

This project gives you a **clear mental model of how coding agents work** and shows you how far you can go with just:
- ✅ .NET
- ✅ The Microsoft Agent Framework
- ✅ An API key
- ✅ Curiosity

**Key Takeaways:**
- Coding agents aren't magic - they're understandable and buildable
- How the Microsoft Agent Framework makes AI agent development straightforward
- The architecture behind tools like Claude Code and Codex
- How agents understand tasks, call tools, and write their own code
- Building AI agents with .NET - from blank console app to functioning agent in under an hour

## ⚠️ Security Considerations

This is a **demonstration project** and should not be used in production without additional security measures:

- File operations are scoped to a working directory
- Command execution has no input validation
- No rate limiting or error recovery
- API key is read from environment variables

## 📝 License

[Add your license information here]

## 👤 Author

**Velvart András**

- Sessionize: [The Agent Awakens: Build Your Own CLI Coding Agent](https://sessionize.com/s/velvart-andras/the-agent-awakens-build-your-own-cli-coding-agent-/151656)

## 🤝 Contributing

This is sample code from a conference talk. Feel free to fork and experiment!

## 📧 Contact

For questions about the talk or this code, please reach out through the Sessionize link above.

---

*Part engineering experiment, part live exploration of the next generation of developer tools.*

*Built from scratch with .NET 9, Microsoft's Agent Framework, an API key, and curiosity.*


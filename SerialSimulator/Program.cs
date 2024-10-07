using System.Diagnostics;

const string scriptPath = "python/run_sim.py";
const string venvPath = ".venv";

var python = Path.Combine(venvPath, "Scripts", "python.exe");

// Ensure the virtual environment exists
if (!Directory.Exists(venvPath))
{
    // Create the virtual environment
    var createVenv = new ProcessStartInfo
    {
        FileName = "python",
        Arguments = "-m venv .venv",
        UseShellExecute = false,
        RedirectStandardOutput = true,
        RedirectStandardError = true
    };

    using var process = Process.Start(createVenv);

    process!.WaitForExit();

    if (process.ExitCode != 0)
    {
        Console.WriteLine("Failed to create virtual environment.");
        return;
    }
}

// Install the pyserial package
var installSerial = new ProcessStartInfo
{
    FileName = python,
    Arguments = "-m pip install pyserial",
    UseShellExecute = false,
    RedirectStandardOutput = true,
    RedirectStandardError = true
};

using (var process = Process.Start(installSerial))
{
    process!.WaitForExit();

    if (process.ExitCode != 0)
    {
        Console.WriteLine("Failed to install pyserial.");
        return;
    }
}

// Configure the process start info
var start = new ProcessStartInfo
{
    FileName = python,
    // Run for 99999 minutes
    Arguments = $"{scriptPath} 99999",
    UseShellExecute = false,
    RedirectStandardOutput = true,
    RedirectStandardError = true
};

// Start the process (looped so if it fails it can be retried until stopped via IDE)
while (true)
{
    using var process = Process.Start(start);
    
    if (process == null)
    {
        Console.WriteLine("Failed to start the process.");
        
        // Sleep for 5 seconds before retrying
        Thread.Sleep(5000);
        return;
    }

    // Pipe the process output to the console
    process.OutputDataReceived += (sender, args) => Console.WriteLine(args.Data);
    process.ErrorDataReceived += (sender, args) => Console.WriteLine(args.Data);

    process.BeginErrorReadLine();
    process.BeginOutputReadLine();

    // Wait for the process to exit
    process.WaitForExit();
    
    // Sleep for a second before retrying
    Thread.Sleep(1000);
}
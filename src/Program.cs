using System.IO.MemoryMappedFiles;
using System.Reflection;
using System.Text;

if (args.Length != 0)
{
    Console.WriteLine(@$"Version: {Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion}
Project website: https://github.com/PolarGoose/WindowsDebugOutputViewer

This application prints the Win32 debug output to console.
To use this application you need to run it without parameters");
    return 0;
}

using var cancellationTokenSource = new CancellationTokenSource();
Console.CancelKeyPress += (_, args) =>
{
    cancellationTokenSource.Cancel();
    args.Cancel = true;
};

using var bufferReadyEvent = CreateNamedAutoResetEvent("DBWIN_BUFFER_READY", initialState: true);
using var dataReadyEvent = CreateNamedAutoResetEvent("DBWIN_DATA_READY", initialState: false);
using var memoryMappedBuffer = MemoryMappedFile.CreateNew("DBWIN_BUFFER", 4096);
using var memoryMappedBufferAccessor = memoryMappedBuffer.CreateViewAccessor();
var debugMessageBuffer = new byte[4096 - sizeof(Int32)];

while (WaitHandle.WaitAny([dataReadyEvent, cancellationTokenSource.Token.WaitHandle]) == 0)
{
    memoryMappedBufferAccessor.Read(0, out UInt32 procId);
    memoryMappedBufferAccessor.ReadArray(sizeof(UInt32), debugMessageBuffer, 0, debugMessageBuffer.Length);

    Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.ffffff} [{procId}] {ReadNullTerminatedAnsiString(debugMessageBuffer)}");

    bufferReadyEvent.Set();
}

return 0;

static EventWaitHandle CreateNamedAutoResetEvent(string name, bool initialState)
{
    var bufferReadyEvent = new EventWaitHandle(initialState, EventResetMode.AutoReset, name, out var createdNew);
    if (!createdNew)
    {
        throw new InvalidOperationException($"Another application is already listening to the debug output. Can't create {name} event.");
    }
    return bufferReadyEvent;
}

static string ReadNullTerminatedAnsiString(byte[] bytes)
{
    int length = Array.IndexOf(bytes, (byte)0);
    return Encoding.ASCII.GetString(bytes, 0, length).TrimEnd(['\n', '\r']);
}
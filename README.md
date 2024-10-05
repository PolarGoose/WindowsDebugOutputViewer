# WindowsDebugOutputViewer
A utility to print the Win32 debug output of Windows applications to the console.<br>
For example it is the output that it printed using [OutputDebugString](https://learn.microsoft.com/en-us/windows/win32/api/debugapi/nf-debugapi-outputdebugstringw) or [Debug.WriteLine](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.debug.writeline?view=net-8.0).<br>
The printed messages have the following format:<br>
`local_timestamp [processId] debug_message`

# Usage
* Download the [latest release](https://github.com/PolarGoose/WindowsDebugOutputViewer/releases)
* Run the application without parameters.

# Example
```
>WindowsDebugOutputViewer.exe
2024-10-05 19:57:48.145844 [31480] Test Debug message 1
2024-10-05 19:57:48.150865 [31480] Test Debug multiline message
another line
2024-10-05 19:57:50.153056 [31480] Test Debug message 3
```

# System requirements
* Windows 7 or later

# Reference
* [Mechanism of OutputDebugString](https://www.codeproject.com/Articles/23776/Mechanism-of-OutputDebugString)
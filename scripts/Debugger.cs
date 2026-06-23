using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Godot;

public sealed partial class Debugger : Logger {

    // Vars

    public delegate void MessageLogged(string message, LogType logType);
    public event MessageLogged messageLogged = (string message, LogType logType) => {};
    
    /// <summary>
    /// Logs may come from any thread so we use a lock to only allow one thread to write logs at a time.
    /// </summary>
    private static readonly object logLock = new object();

    /// <summary>
    /// <para> infinite loop guard: prevents a crash loop if logging throws an exception in OnAnyException </para>
    /// <para>
    ///  Why? 'AppDomain.CurrentDomain.FirstChanceException += OnAnyException;'
    ///  this means OnAnyException will be called if ANY exception (even if handled) is throw ANYWHERE in the app which inculdes the OnAnyException function.
    /// </para>
    /// <para>
    ///  Why catch handled exceptions? Becuase godot... GodotSharp catches all exceptions thrown so every exception is handled by godot.
    ///  If godot did not if C# crashes so would godot
    /// </para>
    /// </summary>
    [ThreadStatic] private static bool isProcessingException;

    public Debugger() {
        AppDomain.CurrentDomain.FirstChanceException += OnAnyException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
    }

    // Functions

    // Catches exceptions from C#
    private void OnAnyException(object sender, FirstChanceExceptionEventArgs eventArgs) {
        if (isProcessingException) return;

        try {
            isProcessingException = true;
            lock (logLock) {
                Exception ex = eventArgs.Exception.GetBaseException();
                string formattedException = $"[.NET EXCEPTION] {ex.GetType().Name}: {ex.Message}\nStack Trace:\n{ex.StackTrace}";
                
                // Use CallDeferred here to safely pass too the main thread
                Callable.From(() => messageLogged.Invoke(formattedException, LogType.ERROR)).CallDeferred();
            }
        }
        catch {
            // Fail-safe
        }
        finally {
            isProcessingException = false;
        }
    }

    // Catches exceptions from C#
    private void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs eventArgs) {
        lock (logLock) {
            Exception ex = eventArgs.Exception.GetBaseException();

            string formattedException = $"[.NET ASYNC EXCEPTION] {ex.GetType().Name}: {ex.Message}\nStack Trace:\n{ex.StackTrace}";
            
            // Use CallDeferred here to safely pass too the main thread
            Callable.From(() => messageLogged.Invoke(formattedException, LogType.ERROR)).CallDeferred();
            
            // Must be set otherwise we crash
            eventArgs.SetObserved();
        }
    }

    // Catches errors from Godot
    public override void _LogMessage(string message, bool error) {
        lock (logLock) {
            string formattedMessage = "";
            LogType logType = LogType.NORMAL;

            if (error) {
                // Errors can come from STDOUT instead of STDEND
                formattedMessage = $"[ERROR from STDOUT] {message}";
                logType = LogType.ERROR;
            }
            else {
                formattedMessage = $"{message}";
                logType = LogType.NORMAL;
            }

            // Use CallDeferred here to safely pass too the main thread
            Callable.From(() => messageLogged.Invoke(formattedMessage, logType)).CallDeferred();
        }
    }

    // Catches errors from Godot
    public override void _LogError(
        string function,
        string file,
        int line,
        string code,
        string rationale,
        bool editorNotify,
        int errorType,
        Godot.Collections.Array<ScriptBacktrace> scriptBacktraces
    ) {
        lock (logLock) {

            string typeStr;
            LogType logType;

            switch ((ErrorType) errorType) {
                case ErrorType.Error:
                    typeStr = "ERROR";
                    logType = LogType.ERROR;
                    break;

                case ErrorType.Warning:
                    typeStr = "WARNING";
                    logType = LogType.WARNING;
                    break;

                case ErrorType.Script:
                    typeStr = "SCRIPT ERROR";
                    logType = LogType.ERROR;
                    break;

                case ErrorType.Shader:
                    typeStr = "SHADER ERROR";
                    logType = LogType.ERROR;
                    break;
                
                default:
                    typeStr = "UNKNOWN";
                    logType = LogType.ERROR;
                    break;
            }

            string message = string.IsNullOrEmpty(rationale) ? code : rationale;
            string formattedError = $"[{typeStr}] {message} (At: {file}:{line} in {function})";

            // Use CallDeferred here to safely pass too the main thread
            Callable.From(() => messageLogged.Invoke(formattedError, logType)).CallDeferred();
        }
    }

    public enum LogType {
        NORMAL,
        WARNING,
        ERROR
    }
}
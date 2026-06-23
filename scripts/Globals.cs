using System;
using System.Globalization;
using Godot;

/// <summary>
/// This class is autoloaded.
/// Which means godot will create an instance of this class automatically when the project starts before any normal nodes are created and attach it to a node called 'Globals'.
/// The 'instance' var is safe to use and will never be null unless somthing is very wrong
/// <summary>
public sealed partial class Globals : Node {

    // vars

    public const string SAVE_PATH = "";

    public static Debugger debugger;

    // nodes

    public static Globals instance {get; private set;}

    // godot functions

    public override void _EnterTree() {
        instance = GetTree().Root.GetNodeOrNull<Globals>("Globals");

        if (instance == null) {
            GD.PrintErr($"Fatal: Unable to init autoload '{nameof(Globals)}'. Exiting...");
            GetTree().Quit(1);
        }

        debugger = new Debugger();

        OS.AddLogger(debugger);

        Serializer.initDataFiles();
    }
}
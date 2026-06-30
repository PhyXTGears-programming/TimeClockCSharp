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

    public static Debugger debugger;

    
    private static PackedScene notificationScene;

    private static VBoxContainer notificationsContainer;

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

        notificationScene = ResourceLoader.Load<PackedScene>("res://scenes/notificationPopup.tscn");
        notificationsContainer = GetNode<VBoxContainer>("notificationsContainer");

        debugger.messageLogged += (string message, Debugger.LogType type) => {
            // Normal print statements are not shown
            if (type == Debugger.LogType.NORMAL) return;

            showNotification(message, type);
            
        };

        Serializer.initDataFiles();
    }

    // Functions

    public void showNotification(string message, Debugger.LogType type) {
        Control notification = NotificationPopup.instantiateWithArgs(notificationScene, message, type);

        notificationsContainer.AddChild(notification);
    }

    public void showNotification(string message) {
        Control notification = NotificationPopup.instantiateWithArgs(notificationScene, message, Debugger.LogType.NORMAL);

        notificationsContainer.AddChild(notification);
    }
}
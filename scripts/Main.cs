using Godot;
using System;

public partial class Main : Control {

    // Vars

    private PackedScene notificationScene;

    private VBoxContainer errorPopupContainer;

    // Godot functions

    public override void _Ready() {
        notificationScene = ResourceLoader.Load<PackedScene>("res://scenes/notificationPopup.tscn");
        errorPopupContainer = GetNode<VBoxContainer>("errorPopupContainer");

        Globals.debugger.messageLogged += showNotification;

        // Testing debugger
        // GD.PushError("A");
        // GD.PrintErr("B");
        // GD.PushWarning("C");
        // GD.Print("D");
    }

    // Functions

    public void showNotification(string message, Debugger.LogType logType) {
        Control notification = NotificationPopup.instantiateWithArgs(notificationScene, message, logType);

        errorPopupContainer.AddChild(notification);
    }
}
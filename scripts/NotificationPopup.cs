using Godot;
using System;

public partial class NotificationPopup : BoxContainer {

    // Vars

    public string message {get; private set;}
    public Debugger.LogType type {get; private set;}

    // Nodes

    private Label messageLabel;
    private AnimationPlayer animationPlayer;

    // Godot functions

    public override void _Ready() {
        messageLabel = GetNode<Label>("%text");
        animationPlayer = GetNode<AnimationPlayer>("%animations");

        // Set text
        messageLabel.Text = message;

        // Set text color
        switch (type) {
            case Debugger.LogType.NORMAL:
                messageLabel.AddThemeColorOverride("font_color", Colors.White);
                break;
            
            case Debugger.LogType.WARNING:
                messageLabel.AddThemeColorOverride("font_color", Colors.Yellow);
                break;

            case Debugger.LogType.ERROR:
                messageLabel.AddThemeColorOverride("font_color", Colors.Red);
                break;

            default:
                messageLabel.AddThemeColorOverride("font_color", Colors.White);
                break;
        }

        const string ANIMATION_NAME = "displayMessage";

        animationPlayer.Play(ANIMATION_NAME);

        animationPlayer.AnimationFinished += (StringName name) => {
            // Make sure
            if (name == ANIMATION_NAME) {
                // Delete self
                QueueFree();
            }
        };
    }

    /// <summary>
    /// Creates instance of NotificationPopup with initial data like a constructor
    /// <summary>
    public static NotificationPopup instantiateWithArgs(
        PackedScene scene,
        string message,
        Debugger.LogType logType
    ) {
       NotificationPopup notificationPopup = scene.Instantiate<NotificationPopup>();

       notificationPopup.message = message;
       notificationPopup.type = logType;

       return notificationPopup;
    }
}

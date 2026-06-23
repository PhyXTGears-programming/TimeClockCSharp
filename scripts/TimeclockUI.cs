using Godot;
using System;

public partial class TimeclockUI : Control {

    // Vars

    // Nodes

    private Button inButton;
    private Button outButton;

    private VBoxContainer usersVBox;
    
    private ButtonGroup userSelectButtons;
    private PackedScene userUIScene;

    // Godot functions

    public override void _Ready() {
        inButton = GetNode<Button>("%inButton");
        outButton = GetNode<Button>("%outButton");
        
        usersVBox = GetNode<VBoxContainer>("%userSelectPanel/scrollContainer/vbox");

        userSelectButtons = ResourceLoader.Load<ButtonGroup>("res://art/userSelectPanelButtonGroup.tres");
        userUIScene = ResourceLoader.Load<PackedScene>("res://scenes/user.tscn");

        string[] allUsers = Serializer.allUsers();

        if (allUsers.Length == 0) {
            GD.PushWarning($"Warnning no users found check '{Serializer.USERS_FILE_PATH}'");
        }

        for (int i = 0; i < allUsers.Length; i++) {
            string user = allUsers[i];

            UserStatus status = Serializer.readStatus(user);
            TimeSpan time = Serializer.readTime(user);

            UserUI userUIInstance = UserUI.instantiateWithArgs(userUIScene, user);

            // Add the node to the tree BEFORE asking to update text
            usersVBox.AddChild(userUIInstance);

            userUIInstance.updateUI(user, status, time);
        }

        inButton.Pressed += inButtonPressed;
        outButton.Pressed += outButtonPressed;
    }

    // Functions

    private void inButtonPressed() {

        BaseButton button = userSelectButtons.GetPressedButton();
        
        if (button == null) {
            Globals.instance.showNotification("No user selected.");
            return;
        }
        
        // Incorrect type handling
        if (userSelectButtons.GetPressedButton() is not UserUI user) {
            GD.PushError($"'{nameof(userSelectButtons)}' button group is expected to have only buttons that are of type '{nameof(UserUI)}'");
            return;
        }

        UserStatus status = Serializer.readStatus(user.userName);

        if (!status.isClockedIn()) {
            // Was auto clocked out
            if (status == UserStatus.AUTO_OUT) {
                Globals.instance.showNotification("You were clock out automatically.\n Please clock out correctly next time, this has been noted...");
            }
            
            status = UserStatus.IN;
        }
        else {
            // Already was clocked in
            status = UserStatus.DOUBLE_IN;

            Globals.instance.showNotification("Already clocked IN!");
        }

        user.updateStatus(status);

        Serializer.appendTime(user.userName, status, DateTime.Now);
    }

    private void outButtonPressed() {

        BaseButton button = userSelectButtons.GetPressedButton();

        if (button == null) {
            Globals.instance.showNotification("No user selected.");
            return;
        }
        
        // Incorrect type handling
        if (userSelectButtons.GetPressedButton() is not UserUI user) {
            GD.PushError($"'{nameof(userSelectButtons)}' button group is expected to have only buttons that are of type '{nameof(UserUI)}'");
            return;
        }

        UserStatus status = Serializer.readStatus(user.userName);

        if (status.isClockedIn()) {
            status = UserStatus.OUT;
        }
        else {
            // Already was clocked out
            status = UserStatus.DOUBLE_OUT;

            Globals.instance.showNotification("Already clocked OUT!");
        }
        
        user.updateStatus(status);

        Serializer.appendTime(user.userName, status, DateTime.Now);
    }
}

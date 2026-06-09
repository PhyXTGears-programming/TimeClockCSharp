using Godot;
using System;

public partial class TimeclockUI : Control {

    // vars

    // nodes

    private Button inButton;
    private Button outButton;
    
    private ButtonGroup userSelectButtons;

    // godot functions

    public override void _Ready() {
        inButton = GetNode<Button>("%inButton");
        outButton = GetNode<Button>("%outButton");

        userSelectButtons = ResourceLoader.Load<ButtonGroup>("res://art/userSelectPanelButtonGroup.tres");

        inButton.Pressed += inButtonPressed;
        outButton.Pressed += outButtonPressed;
    }

    // functions

    private void inButtonPressed() {

        UserUI user = userSelectButtons.GetPressedButton() as UserUI;
        
        if (user == null) return;

        if (!user.status.isClockedIn()) {
            user.status = UserStatus.IN;
        }
        else {
            // already was clocked in
            user.status = UserStatus.DOUBLE_IN;
        }

        user.updateText();
    }

    private void outButtonPressed() {
       UserUI user = userSelectButtons.GetPressedButton() as UserUI;

        if (user == null) return;

        if (user.status.isClockedIn()) {
            user.status = UserStatus.OUT;
        }
        else {
            // already was clocked out
            user.status = UserStatus.DOUBLE_OUT;
        }
        
        user.updateText();
    }
}

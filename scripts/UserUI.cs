using Godot;
using System;

/// <summary>
/// Instance of a node that controls UI for the user plate
/// <summary>
public partial class UserUI : Button {
	// vars

	[Export] public string userName = "NULL";

	[Export] public UserStatus status = UserStatus.OUT;

    // nodes

	private Label nameLabel;
	private Label statusLabel;

	// godot functions

	public override void _Ready() {
		nameLabel = GetNode<Label>("margin/hbox/name");
		statusLabel = GetNode<Label>("margin/hbox/status");

		updateText();
	}

	// functions

	public void updateText() {
		if (nameLabel == null || statusLabel == null) {
			GD.PrintErr($"Unable to update text '{nameof(UserUI)}' node is not in tree yet.");
			return;
		}

		nameLabel.Text = userName;
		statusLabel.Text = status.ToStringFancy();
	}
}

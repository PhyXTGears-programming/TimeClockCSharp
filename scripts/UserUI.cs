using Godot;
using System;

/// <summary>
/// Instance of a node that controls UI for the user plate
/// <summary>
public partial class UserUI : Button {
    // Vars

    public string userName {get; private set;}

    // Nodes

	private Label displayNameLabel;
    private Label timeDisplayLabel;
	private Label statusLabel;

	// Godot functions

	public override void _Ready() {
		displayNameLabel = GetNode<Label>("%name");
        timeDisplayLabel = GetNode<Label>("%time");
		statusLabel = GetNode<Label>("%status");
	}

    /// <summary>
    /// Creates instance of UserUI with initial data like a constructor
    /// <summary>
    public static UserUI instantiateWithArgs(PackedScene scene, string userName) {
        UserUI userUIInstance = scene.Instantiate<UserUI>();

        userUIInstance.userName = userName;

        return userUIInstance;
    }

	// Functions

	public void updateUI(string displayName, UserStatus status, TimeSpan time) {
		updateDisplayName(displayName);
        updateStatus(status);
        updateTimeDisplay(time);
	}

    public void updateStatus(UserStatus status) {
		if (statusLabel == null) {
			throw new Exception($"Unable to update status text '{nameof(UserUI)}' node is not in tree yet.");
		}

		statusLabel.Text = status.ToStringFancy();
	}

    public void updateDisplayName(string displayName) {
		if (statusLabel == null) {
			throw new Exception($"Unable to update display name text '{nameof(UserUI)}' node is not in tree yet.");
		}

		displayNameLabel.Text = displayName;
	}

    public void updateTimeDisplay(TimeSpan time) {
		if (statusLabel == null) {
			throw new Exception($"Unable to update time display text '{nameof(UserUI)}' node is not in tree yet.");
		}

        int hours = (int) time.TotalHours;
        int minutes = time.Minutes;

        if (hours >= 0) {
            timeDisplayLabel.Text += $"{(int)time.TotalHours}h ";
        }

        timeDisplayLabel.Text += $"{time.Minutes}m";
	}
}

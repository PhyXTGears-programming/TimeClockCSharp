using Godot;
using System;

/// <summary>
/// A modified scroll bar that has up and down buttons
/// <summary>
public partial class FancyVScrollBar : VBoxContainer {

    // vars

    /// <summary>
    /// How fast the up / down buttons should traverse the effective area (The area the grabber can traverse) of the scroll bar
    /// <summary>
    [Export] public double buttonStepRatio = 0.8f;

    public VScrollBar scrollBar {get; protected set;}
    public Button upButton {get; protected set;}
    public Button downButton {get; protected set;}

    // godot functions

    public override void _Ready() {
        scrollBar = GetNode<VScrollBar>("scrollBar");
        upButton = GetNode<Button>("up");
        downButton = GetNode<Button>("down");
    }

    public override void _Process(double delta) {

        double effectiveArea = scrollBar.MaxValue - scrollBar.Page;

        if (upButton.ButtonPressed) {
            scrollBar.Value -= effectiveArea * buttonStepRatio * delta;
        }
        if (downButton.ButtonPressed) {
            scrollBar.Value += effectiveArea * buttonStepRatio * delta;
        }
    }

}

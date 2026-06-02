using Godot;
using System;

public partial class FancyScrollContainer : ScrollContainer {
    // vars

    [Export] public float vScrollBarWidth = 20.0f;
    [Export] public float hScrollBarWidth = 20.0f;

    [Export] public bool vScrollButtons = true;
    [Export] public bool hScrollButtons = true;

    private VScrollBar vScrollBar;
    private HScrollBar hScrollBar;

    // godot functions

    public override void _Ready() {
        vScrollBar = GetVScrollBar();
        hScrollBar = GetHScrollBar();

        
    }
}

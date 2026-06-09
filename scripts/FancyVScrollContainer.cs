using Godot;
using System;


/// <summary>
/// A modified scroll container with only a vertical scroll bar that has up and down buttons
/// <summary>
public partial class FancyVScrollContainer : HBoxContainer {
    // vars

    public ScrollContainer scrollContainer {get; protected set;}
    public VScrollBar scrollBar {get; protected set;}
    
    public FancyVScrollBar fancyScrollBar {get; protected set;}

    // godot functions

    public override void _Ready() {
        scrollContainer = GetNode<ScrollContainer>("scrollContainer");
        scrollBar = scrollContainer.GetVScrollBar();

        fancyScrollBar = GetNode<FancyVScrollBar>("fancyVScrollBar");
        
        // This is called when the value of the scroll bar is changed
        fancyScrollBar.scrollBar.ValueChanged += updateInternalScrollBar;

        // This gets called when the scroll container has resized as well as all of its children
        scrollContainer.SortChildren += updateSettings;
    }

    private void updateInternalScrollBar(double newValue) {
        scrollBar.Value = newValue;
    }

    private void updateSettings() {
        // Copy auto generated settings
        fancyScrollBar.scrollBar.MinValue = scrollBar.MinValue;
        fancyScrollBar.scrollBar.MaxValue = scrollBar.MaxValue;
        fancyScrollBar.scrollBar.Page = scrollBar.Page;
        fancyScrollBar.scrollBar.Step = scrollBar.Step;
        fancyScrollBar.scrollBar.Value = scrollBar.Value;
    }
}

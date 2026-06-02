using Godot;

/// <summary>
/// This class is autoloaded.
/// Which means godot will create an instance of this class automatically when the project starts and attach it to a node called 'globals'.
/// The 'instance' var is safe to use and will never be null
/// <summary>
public partial class Globals : Node {

    public static Globals instance {
        get {
            // Fetch the autoload node directly from the root if not cached
            if (field == null) {
                field = (Engine.GetMainLoop() as SceneTree).Root.GetNode<Globals>("globals");
            }

            return field;
        }
    }

}
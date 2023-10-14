#if TOOLS
using Godot;

[Tool]
public partial class PreviewCameraPlugin : EditorPlugin {

  // Private vars  
  private Window _window;
  private Camera3D _activeCamera;
  /////private Camera2D _activeCameraUI;
  private Camera3D _previewCamera;
  /////private Camera2D _previewCameraUI;
  private Vector2I _windowSize16x9;
  private Vector2I _windowSize4x3;
  private Vector2I _windowSize21x9;
  private Vector2 _currentWindowSize;
  private Vector2I _currentWindowPosition;
  private bool _isLandscapeMode;
  private float _windowSizeMultiplier;

  /// <summary>
  /// 
  /// </summary>
  public override void _EnterTree() {
    GD.Print("_EnterTree");    
    // Create window
    CreateWindow();

    // Set some other variables used for configuration of the window
    _windowSize16x9 = new Vector2I(512, 288);
    _windowSize4x3 = new Vector2I(512, 384);
    _windowSize21x9 = new Vector2I(511, 219);
    _currentWindowSize = _windowSize16x9;
    _currentWindowPosition = Vector2I.Zero;
    _isLandscapeMode = true;
    _windowSizeMultiplier = 1.0f;    
    
    // Need to know when the scene changes to re-create the window
    SceneChanged += OnSceneChanged;
  }

  /// <summary>
  /// 
  /// </summary>
  public void CreateWindow() {
    // Create window to display the output for the preview camera
    _window = new Window() {
      Name = "PreviewCamera",
      Title = "Preview Camera",
      InitialPosition = Window.WindowInitialPosition.CenterMainWindowScreen,
      Size = new Vector2I(512, 288),
      AlwaysOnTop = true,
      Unresizable = true,
      Visible = true
    };

    // Attempt to see if there is an active camera
    _activeCamera = GetEditorInterface()?.GetEditedSceneRoot()?.GetViewport()?.GetCamera3D();
    /////_activeCameraUI = GetEditorInterface()?.GetEditedSceneRoot()?.GetViewport()?.GetCamera2D() ;
    _previewCamera = null;
    /////_previewCameraUI = null;
    if (_activeCamera == null /*&& _activeCameraUI == null*/) {
      // Alert user that there is currently no active cameras
      string message = "No active cameras could be found";
      GD.PrintErr(message);
      Label label = new Label() { Text = message };
      _window.AddChild(label);
      label.Owner = _window;
    }
    else {
      // Active camera found, create preview camera for this window
      _previewCamera = new Camera3D();
      _window.AddChild(_previewCamera);
      _previewCamera.Owner = _window;

      /////_previewCameraUI = new Camera2D();
      /////_window.AddChild(_previewCameraUI);
      /////_previewCameraUI.Owner = _window;

      _window.WindowInput += OnInput;
    }

    // Finally attach the window as a child
    GetEditorInterface().GetEditorMainScreen().AddChild(_window);
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="node"></param>
  public void OnSceneChanged(Node node) {
    // Clean up and recreate the window
    if (_window != null) {
      _currentWindowPosition = _window.Position;
    }
    _ExitTree();
    if (node != null) {
      GD.Print("OnSceneChanged");
      CreateWindow();
      _window.Position = _currentWindowPosition;
      ResizeWindow();
    }
    else {
      GD.Print("This is a new scene. After saving this new scene, close and reopen this scene to initialize Camera Preview plugin.");
    }
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="delta"></param>
  public override void _Process(double delta) {
    if (_activeCamera != null && _previewCamera != null) {
      // Sync the preview camera's pos/rot with the active camera
      _previewCamera.GlobalPosition = _activeCamera.GlobalPosition;
      _previewCamera.GlobalRotation = _activeCamera.GlobalRotation;
    }

    /////if (_activeCameraUI != null && _previewCameraUI != null) {
    // Sync the preview camera's pos/rot with the active camera
    /////_previewCameraUI.GlobalPosition = _activeCameraUI.GlobalPosition;
    /////_previewCameraUI.GlobalRotation = _activeCameraUI.GlobalRotation;      
    /////}
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="inputEvent"></param>
  public void OnInput(InputEvent inputEvent) {
    if (inputEvent is InputEventKey inputEventKey) {
      if (inputEventKey.IsPressed()) {
        bool willProcessResize = false;
        switch (inputEventKey.Keycode) {
          case Key.F1:
            _currentWindowSize = _windowSize16x9;
            GD.Print("Setting to default 16/9 aspect");
            willProcessResize = true;
            break;

          case Key.F2:
            _currentWindowSize = _windowSize4x3;
            GD.Print("Setting to 4/3 aspect");
            willProcessResize = true;
            break;

          case Key.F3:
            _currentWindowSize = _windowSize21x9;
            GD.Print("Setting to 21/9 aspect");
            willProcessResize = true;
            break;

          case Key.F5:
            _isLandscapeMode = !_isLandscapeMode;
            GD.Print(string.Format("Setting to {0} mode", _isLandscapeMode ? "Landscape" : "Portrait"));
            willProcessResize = true;            
            break;

          case Key.KpAdd:
            _windowSizeMultiplier = Mathf.Min(3.0f, _windowSizeMultiplier + 0.1f);
            GD.Print("Increasing window size");            
            willProcessResize = true;
            break;

          case Key.KpSubtract:
            _windowSizeMultiplier = Mathf.Max(0.5f, _windowSizeMultiplier - 0.1f);
            GD.Print("Decreasing window size");            
            willProcessResize = true;
            break;
        }

        if (willProcessResize) {
          ResizeWindow();
        }
      }
    }
  }

  /// <summary>
  /// 
  /// </summary>
  private void ResizeWindow() {
    // Only want to calculate a resize on specific inputs
    Vector2I newScreenSize = new Vector2I() {
      X = Mathf.RoundToInt((_isLandscapeMode ? _currentWindowSize.X : _currentWindowSize.Y) * _windowSizeMultiplier),
      Y = Mathf.RoundToInt((_isLandscapeMode ? _currentWindowSize.Y : _currentWindowSize.X) * _windowSizeMultiplier)
    };
    _window.Size = newScreenSize;
  }

  /// <summary>
  /// 
  /// </summary>
  public override void _ExitTree() {    
    if (_window != null) {
      // General cleanup
      //GD.Print("_ExitTree");
      _window.QueueFree();
      _window = null;
      _activeCamera = null;
      /////_activeCameraUI = null;
    }
  }
}
#endif

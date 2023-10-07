# godot4-preview-camera
Had a difficult time finding a plugin to display a separate preview of the active camera in my 3D scene.
Didn't really like splitting the viewport as that seemd to be overkill for just needing a small preview.
Saw a few other scripts out there but they seemed to be mainly for Godot 3.x and 4.0.x so there were some issues with getting them to work with Godot 4.1.1 which is what I'm currently using.

Not sure if this will work on other versions of Godot and have only tested this in a Windows environment.

Once you enable the plugin, a small window will pop up with a display from the active camera. The aspect ratio of the preview window is not tied to the active camera, but it can be changed on the fly.
- F1 - Set to 16x9 aspect
- F2 - Set to 4x3 aspect
- F3 - Set to 21x9 aspect
- F5 - Toggle between Landscape and Portrait
- Plus on Keypad - Increase size of window
- Minus on Keypad - Decrease size of window

Unzip/clone this to your addons folder, there are just 2 files in this plugin:
- plugin.cfg
- PreviewCameraPlugin.cs

# Slider1
Slider1 is an image viewer application designed to display images in a sequential or slideshow format. It supports loading images from directories, ZIP archives, and RAR archives, along with features like random display and customizable slideshow intervals.

# Features
Image Viewing: Displays images one at a time in the main window.
Slideshow Mode: Automatically cycles through images with customizable interval settings.

Randomized Display: Allows images to be displayed in random order.
Drag-and-Drop Support: Drag folders or archives into the application to load images.

Error Handling: Displays messages for unhandled exceptions to assist the user.
Interactive Controls: Navigate through images using keyboard shortcuts and mouse inputs.

Archive Support: Load images from ZIP and RAR files.
Requirements

Before running Slider1, ensure the following prerequisites are installed:

Microsoft .NET Framework 4.8.1 (x86 and x64)
Windows Installer 4.5
# Installation
Clone this repository:

```
git clone https://github.com/AndreaOmodeo/Slider1.git
cd Slider1
Build the project in your favorite IDE (e.g., Visual Studio) or use the provided deployment tools.
```

## Install required prerequisites:

Use the included setup.exe for ClickOnce deployment.
Alternatively, install the prerequisites manually from the official Microsoft website.
## How to Use Slider1
Launch Methods
### Command-Line:

Run the application from the terminal or command prompt with an optional file path as an argument:
```
Slider1.exe path-to-file.zip
```

### Direct Launch:

Double-click the Slider1.application file (available via ClickOnce deployment) or the executable in the build directory.
### Drag-and-Drop:

Drag a folder or supported archive (ZIP or RAR) into the application window to load and view images.
ClickOnce Deployment:

Open the provided index.htm file in the publish folder.
Follow the instructions to install prerequisites and launch the application.
Controls and On-Screen Behavior
### Navigation:

Use Left/Right arrow keys to move between images.
Use the Mouse Wheel to scroll through images.
Press Spacebar to start/stop the slideshow.
### Slideshow Settings:

Adjust the slideshow speed:
Add Key (+): Increase speed.
Subtract Key (-): Decrease speed.
Enable/disable random mode by toggling the random property in the settings.
### Window Management:

Minimize: Right-click on the window.
Maximize/Restore: Use the maximize button.
Drag-and-Drop:

Drag folders or ZIP/RAR archives into the application to load images.
Error Handling:

If an error occurs, a dialog box will display the error message with an option to continue or cancel.

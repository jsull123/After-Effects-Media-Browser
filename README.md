# After-Effects-Media-Browser

![demo](https://github.com/user-attachments/assets/6d3dee1d-25ce-4c22-b5a1-089370cb7f41)


## Overview

.NET Framework 6.0

This project is a Windows Presentation Foundation (WPF) application designed as a media browser with integration into Adobe After Effects. It consists of two main components:

**After Effects Plugin:**

A lightweight plugin that allows you to launch the Windows application from within After Effects.
Executes a script based on the console output of the media browser (via piping).

**Windows Form Media Browser:**

Allows browsing through folders containing media files (videos and images).
Displays all videos in a looping preview, eliminating the need to open each video individually.
Utilizes FFmpeg to downscale and cache media files for optimized performance.
Enables quick import into After Effects, preserving the folder hierarchy.

## Key Features

Simultaneous Video Preview:
  - All videos are displayed playing in a loop, allowing users to quickly preview multiple files without clicking into each one.

Optimized Performance:
  - Media files are downscaled and cached on disk using FFmpeg, improving playback speed and memory efficiency.

After Effects Integration:
  - Right-click on any file → Import into After Effects.
The plugin automatically creates folders inside After Effects to match the existing directory structure of your files.

Simple UI:
  - Users can navigate folders, search for media files, and view images and videos effortlessly.

## Installation & Usage

1. Setup the After Effects Plugin
    - Copy the release version of the project into C:\Program Files\Adobe\Common\Plug-ins\7.0\MediaCore
2. In After Effects, go to File > smm. This will open the Media Browser
3. In the Media Browser, click select folder to open your main folder where you store your assets. From there you can click into any subfolder using the file tree on the left, or click the main folder name at the top to recursively display every file.
4. Displaying any video or image file for the first time will trigger it to be downscaled and cached which can take some time but it only needs to be done once. The cache is located at "%USERPROFILE%\AppData\Local\smm" and can be cleared in settings.
5. Right click files to import

## Current Project Status

The project is in a usable state but is not complete. There are performance issues and other features I had planned but for now I have moved on from the project.

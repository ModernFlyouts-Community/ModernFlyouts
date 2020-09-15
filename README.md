# ModernFlyouts
### A modern replacement for existing flyouts in Windows

![ModernFlyouts](ModernFlyouts/Assets/Images/ModernFlyouts_128.png)

[Overview](#overview) | [Features](#features) | [Installation](#installation) | [Screenshots](#screenshots)

## Overview

**Default one**

![Overview](docs/images/Overview.png)

This application will replace the default audio/airplane/brightness flyouts found in Windows shown when the volume or brightness changes or when airplane mode key is pressed with a new modern UI.

This project is based on [ADeltaX/AudioFlyout](https://github.com/ADeltaX/AudioFlyout). With additional implementation for airplane mode and brightness flyouts. This project also includes a flyout for lock keys (caps lock, scroll lock & num lock).

> Note : The native flyout is not permanently closed but will be hidden when this flyout is shown
> Thus, users have the freedom to choose between the windows default one. this modern one or none.

## Features
- Fluent UI (similar to the one in Windows 10X)
- Follows system Light/Dark theme (the theme that taskbar has and not **App Theme**).
- Media session control has new buttons such as **Shuffle**, **Repeat**, **Stop** and **Timeline Info**
- AirplaneMode-flyout Module.
- LockKeys-flyout Module.
- Brightness-flyout Module.
- Each Module can be disabled separately.
- Can Choose either Windows Default Flyout, ModernFlyouts or None.
- Flyout's TopBar can be hidden.
- Flyout is **Draggable**
- Flyout can **aligned to default position**
- **Slide Animation** when hiding & showing.
- Transitions.
- This application works on **Windows 10 1809 and above** (Version 0.3.0 supported from Windows 8+, however to simplify distribution and due to .MSIX limitations 0.4.0 and above does not) 
- Targets .NET Core 3.1

## Installation
Modern Flyouts is Available on GitHub and the [Microsoft Store](https://www.microsoft.com/store/apps/9MT60QV066RP).

Both distribution methods will be supported, however the store version is recommended as it is easier to install and will automatically remain up to date.

**Microsoft Store:**

<a href='https://www.microsoft.com/store/apps/9MT60QV066RP?ocid=badge'><img src='https://assets.windowsphone.com/85864462-9c82-451e-9355-a3d5f874397a/English_get-it-from-MS_InvariantCulture_Default.png' alt='Microsoft Store' width='160'/></a>


**Alternatively download from Github:**

1. Go to the [latest releases' page](https://github.com/ShankarBUS/ModernFlyouts/releases/latest).
2. Download the latest zip file.
3. Make sure you have .NET Core 3.1 installed on your machine.
4. Install the **ModernFlyouts.MSIX**.



## IMPORTANT NOTE
This project is made possible due to the work of **[ADeltaX](https://github.com/ADeltaX/)**.
I could have improved the original project but the project is called **AUDIO**Flyout and adding airplane-mode/brightness flyouts wouldn't be a good idea, also the project has no recent activity. He's developing a **AudioFlyout**v2 with a refreshed UI. So, I had to make my own with the said additional features.

### Credits
- **[@ADeltaX](https://github.com/ADeltaX/)** laid the foundation for this project and has been a huge support along the way.
- **[@Samuel12321](https://github.com/Samuel12321/)** for publishing this app to **Microsoft Store**.

### Dependencies and References 
- [NAudio](https://github.com/naudio/NAudio)
- [ModernWpf](https://github.com/Kinnara/ModernWpf) (That's how I got the name **Modern**Flyouts)
- [Hardcodet.NotifyIcon.Wpf](https://github.com/hardcodet/wpf-notifyicon)

## Screenshots

### Audio Flyout

**Windows Default One**

![Audio_Old](docs/images/Audio_Old.png)

**Dark Theme :** 

![Audio_Dark](docs/images/Audio_Dark.png)

_With TopBar_

![Audio_Dark_NoTop](docs/images/Audio_Dark_NoTop.png)

_Without TopBar_

**Light Theme :** 

![Audio_Light](docs/images/Audio_Light.png)

_With TopBar_

![Audio_Light_NoTop](docs/images/Audio_Light_NoTop.png)

_Without TopBar_

**SMTC audio playback session in old one**

![Audio_Old_Session](docs/images/Audio_Old_Session.png)

**SMTC audio playback session**

![Audio_Session_Music_NoTop](docs/images/Audio_Session_Music_NoTop.png)

**SMTC audio playback session with additional info**

![Audio_Session_Music_NoTop_More](docs/images/Audio_Session_Music_NoTop_More.png)

**Fallback thumbnail for music playback with no album art**

![Audio_Session_Music_NoTop_NoAlbumArt](docs/images/Audio_Session_Music_NoTop_NoAlbumArt.png)

**SMTC video playback session with additional info**

![Audio_Session_Video](docs/images/Audio_Session_Video.png)

### Brightness Flyout

**Windows Default One**

![Brightness_Old](docs/images/Brightness_Old.png)

**Dark Theme :** 

![Brightness_Dark](docs/images/Brightness_Dark.png)

_With TopBar_

![Brightness_Dark_NoTop](docs/images/Brightness_Dark_NoTop.png)

_Without TopBar_

**Light Theme :** 

![Brightness_Light](docs/images/Brightness_Light.png)

_With TopBar_

![Brightness_Light_NoTop](docs/images/Brightness_Light_NoTop.png)

_Without TopBar_

### Airplane-Mode Flyout

**Light Theme : (On state)**

![Airplane_On_Light](docs/images/Airplane_On_Light.png)

_With TopBar_

![Airplane_On_Light_NoTop](docs/images/Airplane_On_Light_NoTop.png)

_Without TopBar_

### Lock-keys flyout

**Light Theme : (Caps-lock On)**

![LockKey_Caps_Light](docs/images/LockKey_Caps_Light.png)

_With TopBar_

![LockKey_Caps_Light_NoTop](docs/images/LockKey_Caps_Light_NoTop.png)

_Without TopBar_

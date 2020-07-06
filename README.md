# ModernFlyouts
### A modern replacement for existing flyouts in Windows

**Dark Theme :** 

![Audio_Dark](docs/images/Audio_Dark.png)

_With TopBar_

![Audio_Dark_NoTop](docs/images/Audio_Dark_NoTop.png)

_Without TopBar_

**Light Theme :** 

![Audio_Light](docs/images/Audio_Light.png)

_With TopBar_

![Audio_Light_NoTop](docs/images/Audio_Dark_NoTop.png)

_Without TopBar_

**Default one**

![Audio_Old](docs/images/Audio_Old.png)

This application will replace the default audio/airplane/brightness flyouts found in Windows shown when the volume or brightness changes or when airplane mode key is pressed.

This project is based on [ADeltaX/AudioFlyout](https://github.com/ADeltaX/AudioFlyout). I have implemented airplane mode and brightness (incomplete) flyout as an addition. I have also added a flyout for lock keys (caps lock, scroll lock & num lock).

> Note : The native flyout is not permanently closed but will be hidden when this flyout is shown
> Thus, only audio/airplane-mode flyouts will replace the old one. **Brightness flyout** is incomplete and may not work on all devices.

## Features (new ones compared to [ADeltaX/AudioFlyout](https://github.com/ADeltaX/AudioFlyout).)
- Follows system Light/Dark theme (the theme that taskbar has and not **App Theme**).
- SMTC has new controls such as **Shuffle**, **Repeat**, **Stop** and **Timeline Info**
- AirplaneMode-flyout Module.
- LockKeys-flyout Module.
- Note : **Brightness flyout** is not yet fully implemented and may not work on all devices.
- Each Module can be disabled separately.
- Can Choose either Windows Default Flyout, ModernFlyouts or None.
- Flyout's TopBar can be hidden.
- Flyout is **Draggable**
- Flyout can **aligned to default position**
- **Slide Animation** when hide & showing
- And finally this application works in **all versions of Windows 10** 🎉🎉🎉 (and also tested on Windows 8) (fallback method is added for **SMTC** and not restricted to Windows 10 17763+)
- Supports both .NET 4.6.2 & .NET Core 3.1

## Disadvantages compared to [ADeltaX/AudioFlyout](https://github.com/ADeltaX/AudioFlyout).
- Flyout can't be shown in **LockScreen** and above **TaskManager**, since UIAccess is not taken into account from the beginning.
- No **Acrylic** backdrop (will cost animation so ...)

## IMPORTANT NOTE :
This project is made possible because of the legend **[ADeltaX](https://github.com/ADeltaX/)**.
Hey @ADeltaX, I respect you man 😙. I could have made changes to the original project but the project is called **AUDIO**Flyout and adding airplane-mode to it seemed silly 😅, also the project has no recent activity. He's a legendary dev, I thought he know what he's doing. He's doing a **AudioFlyout**v2 with a new UI. So, I had to make my own with the said additional features.

This project depends on : 
- [NAudio](https://github.com/naudio/NAudio)
- [ModernWpf](https://github.com/Kinnara/ModernWpf) (That's how I got the name **Modern**Flyout)
- [Hardcodet.NotifyIcon.Wpf](https://github.com/hardcodet/wpf-notifyicon)

## Screenshots

**SMTC audio playback session**

![Audio_Session_Music_NoTop](docs/images/Audio_Session_Music_NoTop.png)

**SMTC audio playback session with additional info**

![Audio_Session_Music_NoTop_More](docs/images/Audio_Session_Music_NoTop_More.png)

**Fallback thumbnail for music playback with no album art**

![Audio_Session_Music_NoTop_NoAlbumArt](docs/images/Audio_Session_Music_NoTop_NoAlbumArt.png)

**SMTC video playback session with additional info**

![Audio_Session_Video](docs/images/Audio_Session_Video.png)

**Airplane-Mode Flyout**

![Airplane_On_Light](docs/images/Airplane_On_Light.png)

_With TopBar_

![Airplane_On_Light_NoTop](docs/images/Airplane_On_Light_NoTop.png)

_Without TopBar_

**Lock key flyout**

![LockKey_Caps_Light](docs/images/LockKey_Caps_Light.png)

_With TopBar_

![LockKey_Caps_Light_NoTop](docs/images/LockKey_Caps_Light_NoTop.png)

_Without TopBar_
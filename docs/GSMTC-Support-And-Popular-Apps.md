# Global System Media Transport Controls (GSMTC) support and popular media apps

ModernFlyouts' Media Session Controls will support every application that makes use of the [System Media Transport Controls **aka SMTC** API](https://docs.microsoft.com/en-us/uwp/api/windows.media.systemmediatransportcontrols) to manage their media playback.

The built-in Windows Media Flyout will also only support those apps.

Users tend to know which functionalities are supported by ModernFlyouts' Media Sessions Controls for their apps of interest ([#230](https://github.com/ModernFlyouts-Community/ModernFlyouts/issues/230) for instance).


### Legend

#### App support type

- 🟢 - Supported by the app out-of-the-box i.e. built-in support for SMTC
- 🟡 - No built-in support for SMTC but could be overcame around by **[additional software (plugins, add-ons, etc.](#plugins)**
- 🔴 - **Not supported by any means** (see [Help us include support for more applications that are currently unsupported](#help-us-include-support-for-more-applications-that-are-currently-unsupported))

#### Feature support type

- 🟩 - Feature supported
- 🟪 - Feature support is incomplete i.e. in-progress or being improved or expected to arrive soon or temporarily unavailable
- 🟥 - Feature not supported

| Application | Support | Play/Pause | Previous | Next | Thumbnail | Media Title | Media Artist | App Info | Shuffle | Repeat | Stop | Timeline information |
| --- | --- | ---| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
||<H3> **BROWSERS** </H3> |
| Chromium based | 🟢 | 🟩 | 🟩 | 🟩 | 🟩 | 🟩 | 🟩 | 🟪 | 🟥 | 🟥 | 🟥 | 🟥 |
| Firefox        | 🟢 | 🟩 | 🟩 | 🟩 | 🟩 | 🟩 | 🟩 | 🟪 | 🟥 | 🟥 | 🟥 | 🟥 |
||<H3> **MEDIA**</H3> | <H3>**PLAYERS**</H3>|
|**MUSIC**|**ONLY**|
| Spotify        | 🟢 | 🟩 | 🟩 | 🟩 | 🟩 | 🟩 | 🟩 | 🟪 | 🟩 | 🟩 | 🟩 | 🟩 |
| Groove Music   | 🟢 | 🟩 | 🟩 | 🟩 | 🟩 | 🟩 | 🟩 | 🟩 | 🟩 | 🟩 | 🟥 | 🟥 |
| [Dopamine-Windows](https://github.com/digimezzo/dopamine-windows) | 🟢 | 🟩 | 🟩 | 🟩 | 🟩 | 🟩 | 🟩 | 🟪 | 🟩 | 🟩 | 🟩 | 🟩 |
| [foobar2000](https://www.foobar2000.org/) | 🟢 (v1.5.1+) | 🟩 | 🟩 | 🟩 | 🟩 | 🟩 | 🟩 | 🟪 | 🟥 | 🟥 | 🟩 | 🟥 |
| [MusicBee](https://getmusicbee.com/downloads/) | 🟡 | 🟩 | 🟩 | 🟩 | 🟩 | 🟩 | 🟩 | 🟪 | 🟩 | 🟩 | 🟩 | 🟪 |
| [AIMP](https://www.aimp.ru/?do=download&os=windows) | 🟡 | 🟩 | 🟩 | 🟩 | 🟩 | 🟩 | 🟩 | 🟪 | 🟥 | 🟥 | 🟥 | 🟥 |
| [Winamp](https://www.winamp.com/) | 🟡 | 🟩 | 🟩 | 🟩 | 🟩 | 🟩 | 🟩 | 🟩 | 🟥 | 🟥 | 🟥 | 🟥 |
|**VIDEO**|**ONLY**|
| Movies & TV    | 🟢 | 🟩 | 🟩 | 🟩 | 🟪 | 🟩 | 🟪 | 🟩 | 🟥 | 🟥 | 🟩 | 🟩 |
| [myTube Beta](https://www.microsoft.com/store/productId/9WZDNCRDT29J) | 🟢 | 🟩 | 🟩 | 🟩 | 🟥 | 🟩 | 🟩 | 🟩 | 🟥 | 🟥 | 🟩 | 🟥 |
| [Amazon Prime Video](https://www.microsoft.com/store/productId/9P6RC76MSMMJ) | 🔴 | 🟥 | 🟥 | 🟥 | 🟥 | 🟥 | 🟥 | 🟥 | 🟥 | 🟥 | 🟥 | 🟥 |
|**AUDIO**|**AND**|**VIDEO**|
| Media Player PREVIEW (Formerly Groove Music)  | 🟢 | 🟩 | 🟥 | 🟥 | 🟪 | 🟩 | 🟥 | 🟩 | 🟩 | 🟩 | 🟥 | 🟩 |
| [MPC-BE](https://sourceforge.net/projects/mpcbe/) | 🟢 | 🟩 | 🟩 | 🟩 | 🟪 | 🟩 | 🟩 | 🟩 | 🟥 | 🟥 | 🟥 | 🟥 |
| [VLC (UWP)](https://www.microsoft.com/store/productId/9NBLGGH4VVNH) | 🟢 | 🟩 | 🟩 | 🟩 | 🟩 | 🟩 | 🟩 | 🟩 | 🟥 | 🟥 | 🟩 | 🟥 |
| [VLC (Desktop)](https://www.videolan.org/vlc/) | 🟡 (VLC 3.0.x)| 🟩 | 🟩 | 🟩 | 🟪 | 🟩 | 🟩 | 🟩 | 🟥 | 🟥 | 🟥 | 🟥 |
| [PotPlayer](https://daumpotplayer.com/download/) | 🔴 | 🟥 | 🟥 | 🟥 | 🟥 | 🟥 | 🟥 | 🟥 | 🟥 | 🟥 | 🟥 | 🟥 |
| [MPC-HC (clsid2)](https://github.com/clsid2/mpc-hc) | 🔴 | 🟥 | 🟥 | 🟥 | 🟥 | 🟥 | 🟥 | 🟥 | 🟥 | 🟥 | 🟥 | 🟥 |
| Application | Support | Play\Pause | Previous | Next | Thumbnail | Media Title | Media Artist | App Info | Shuffle | Repeat | Stop | Timeline information |

NOTE- Chromium based includes Chromium browsers and its derivatives (Chrome, New MS Edge, Opera, Brave, Vivaldi, etc.). 


### Plugins

- **MusicBee** - https://github.com/ameer1234567890/mb_MediaControl
- **AIMP** - https://www.aimp.ru/?do=catalog&rec_id=1097
- **Winamp** - https://github.com/NanMetal/gen_smtc
- **VLC (Desktop)** - https://github.com/spmn/vlc-win10smtc

## Help us include support for more applications that are currently unsupported

- **VLC** : We have filed a [feature request on VLC's bug tracker site](https://trac.videolan.org/vlc/ticket/25258#ticket) asking built-in support for the SMTC APIs for the VLC Desktop (Win32) app.
Hopefully, if enough people request it they might add it. Please check out the ticket.

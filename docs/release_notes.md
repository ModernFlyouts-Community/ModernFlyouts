# Release Notes

## v0.9

### Bug Fixes

- Fixed [#29](https://github.com/ModernFlyouts-Community/ModernFlyouts/issues/29) - **Bug: Media Sessions are not updating in Windows 10 2004**. This bug is caused by an issue in a Windows 10 WinRT API called `GlobalSystemMediaTransportControlsSessionManager`. Its `SessionsChanged` event didn't get raised since Windows 10 2004 which they fixed in a later insider build. A huge thanks to [@ADeltaX](https://github.com/ADeltaX)! Also see [this section](#our-struggles).

- Fixed [#32](https://github.com/ModernFlyouts-Community/ModernFlyouts/issues/32) - **Bug: ModernFlyouts gets hidden behind other topmost windows and not shown in Lockscreen**. This issue happens due to the restrictions laid down by Windows. Windows doesn't allow regular top-most top-level windows to be shown on top of several other windows (some full-screen apps, Start menu, Always-On-Top Task Manager and the lock screen). We had to do some dark magic to get this thing to show on-top of every window which the native flyout could. Thanks to [@ADeltaX](https://github.com/ADeltaX) for discovering the [CreateWindowInBand](https://blog.adeltax.com/window-z-order-in-windows-10/) private API and for making the dll injection to work properly. Also see [this section](#our-struggles-for-v0.9).

- Fixed [#184](https://github.com/ModernFlyouts-Community/ModernFlyouts/issues/184) - **Bug: Win32 apps' information are not shown in the media session controls**. Another issue caused by the Windows 10 WinRT API `GlobalSystemMediaTransportControlsSessionManager`.

- Fixed [#305]((https://github.com/ModernFlyouts-Community/ModernFlyouts/issues/305) - **Bug: Pressing and holding caps lock toggles flyout twice.**. Fixed as a part of [#306](https://github.com/ModernFlyouts-Community/ModernFlyouts/pull/306) by a community member [@Renzo904](https://github.com/Renzo904). Thank you [@Renzo904](https://github.com/Renzo904)! ❤❤❤

- Fixed [#312](https://github.com/ModernFlyouts-Community/ModernFlyouts/issues/312) - **Bug: Use mousewheel turn up volume will stuck at some value**. Fixed as a part of [#313](https://github.com/ModernFlyouts-Community/ModernFlyouts/pull/313). Thanks again [@Renzo904](https://github.com/Renzo904)!

### What's New

- Resolved [#327](https://github.com/ModernFlyouts-Community/ModernFlyouts/issues/327) - **Feature request: Multi-monitor support**. Now, you can select which monitor the flyout auto align to or be moved to (based on relative position).

- Resolved [#74](https://github.com/ModernFlyouts-Community/ModernFlyouts/issues/74) - **Feature Request: Offer some sensible default positions**. From now on, there are options to auto place the flyout to your desired alignment on your desired monitor.

- Resolved [#71](https://github.com/ModernFlyouts-Community/ModernFlyouts/issues/71) - **Feature Request: Add the ability to change whether the Media controls appears below or above the Volume controls and and the ability to control the expand direction**. Two new options called **"Content stacking direction"** and **"Expand direction"** (expand animation direction, to be precise) have been added.

- Resolved [#135](https://github.com/ModernFlyouts-Community/ModernFlyouts/issues/135) - **Feature Request: On Computers with multiple monitors, have a setting to change which monitor the flyout appears on.**. Related to [#327](https://github.com/ModernFlyouts-Community/ModernFlyouts/issues/327)

- Resolved [#136](https://github.com/ModernFlyouts-Community/ModernFlyouts/issues/136) - **Feature Request: Split the Media session Flyouts and volume flyouts as it was pre 0.5**

- Resolved [#97](https://github.com/ModernFlyouts-Community/ModernFlyouts/issues/97) - **Feature proposal: Add support for seeking or changing the progress of the media playback**. The timeline info progress bar has now been replaced by a slider. This change allows you to seek the playback position easily from ModernFlyouts without having to open the source application.

- Resolved [#114](https://github.com/ModernFlyouts-Community/ModernFlyouts/issues/114) - **Timeline info suggestion**. The timeline info controls have been moved into the main flyout instead of being in a separate flyout.

- Resolved [#224](https://github.com/ModernFlyouts-Community/ModernFlyouts/issues/224) - **Feature Request: Option to toggle the album art background for media controls**. Fixed as a part of [#315](https://github.com/ModernFlyouts-Community/ModernFlyouts/pull/315) by another community member [@fheck](https://github.com/fheck). Thanks you too [@fheck](https://github.com/fheck)! ❤❤❤.

- Resolved [#183](https://github.com/ModernFlyouts-Community/ModernFlyouts/issues/183) - **Feature Request: Move flyout default position setting to personalisation page from general**

- Resolved [#69](https://github.com/ModernFlyouts-Community/ModernFlyouts/issues/69) - **Support for DPI Awareness**

- Resolved [#361](https://github.com/ModernFlyouts-Community/ModernFlyouts/issues/361) - Fixed an issue where Spotify's thumbnail were scaled improperly. We had to crop the extra padding and their branding from the thumbnails provided by Spotify. 

- Reworked topbar animations.

### Known issues

- Tooltips gets hidden behind the main flyout. I know! I worked 3 weeks with this! I am fully aware of this

- Flyout bounces indefinitely on hover sometimes when the topbar is set to auto-hide and the flyout is auto aligned to bottom.

- Users can't decide how close the flyout can get to the screen bounds (or can't set margin, to be precise) when it's auto placed. The ability is already added but due to time limitations this feature couldn't make it to this release.

### Our struggles for v0.9

#### NowPlayingSessionManager

**INowPlayingSessionManager** (INPSM) is a private Windows 10 API (since 1511) is the core API used by the native flyout and a Windows 10 WinRT API called **GSMTC** ([GlobalSystemMediaTransportControlsSessionManager](https://docs.microsoft.com/en-us/uwp/api/windows.media.control.globalsystemmediatransportcontrolssessionmanager)) (since Windows 10 1809). We were previously using **GSMTC** for our media controls. But it lead to some dead ends like [#29](https://github.com/ModernFlyouts-Community/ModernFlyouts/issues/29) & [#184](https://github.com/ModernFlyouts-Community/ModernFlyouts/issues/184). In order to overcome the issues laid down by the **GSMTC** APIs, [@ADeltaX](https://github.com/ADeltaX) suggested to use a private API found in Windows 10 reserved for internal usage called **INowPlayingSessionManager**. But there were plenty of problems while doing so. Since **INPSM** is reserved for internal usage by Microsoft, they didn't care about breaking and altering it in every Windows 10 build.

[@ADeltaX](https://github.com/ADeltaX) had to do all the hard work to get it running across all Windows 10 versions properly.
He wrote a wrapper called **NPSMLib** which will execute OS specific functions and work on all Windows 10 versions from 1511 to the latest insider builds.
He made a standalone [Nuget package called NPSMLib](https://www.nuget.org/packages/NPSMLib) so that others could be benefited too.

If you're interested in the **NPSMLib**, please check out its [GitHub repository](https://github.com/ADeltaX/NPSMLib).

#### Above Lockscreen & Topmost Flyout

Some users were struggling with the flyout getting hidden behind by some full-screen apps, Start Menu, Always-on-top Task Manager & the lock screen. While the native flyout had no problems with being topmost, we had no options other than doing some black magic.

[@ADeltaX](https://github.com/ADeltaX) [discovered and documented a private Windows API](https://blog.adeltax.com/window-z-order-in-windows-10/) called **`CreateWindowInBand`** which he made use of in his [AudioFlyout](https://github.com/ADeltaX/AudioFlyout) and [MobileShell](https://github.com/ADeltaX/MobileShell).

This **CreateWindowInBand** API is also reserved for internal usage by Microsoft and has plenty of restrictions in order to prevent it from being used by 3rd party applications. The **CreateWindowInBand** works fine if we target the normal window band but for higher bands MS has laid some serious restrictions.

> CreateWindowInBand/Ex **works ONLY if you pass ZBID_DEFAULT or ZBID_DESKTOP** as dwBand argument. Also **ZBID_UIACCESS is permitted only if the process has UIAccess token** (obtainable, for example, by setting `uiAccess=true` in `app.manifest`, more info [here](https://docs.microsoft.com/en-us/windows/win32/winauto/uiauto-securityoverview)). **Any other ZBID will fail with 0x5 (ACCESS DENIED)**.

> For CreateWindowInBand/Ex, to be able to use more ZBIDs, the program must have a special PE header, named **".imrsiv"** ( bss_seg), flagged with **IMAGE_DLLCHARACTERISTICS_FORCE_INTEGRITY** and be **signed with a Microsoft certificate "Microsoft Windows"**.

*Quoted from ADeltaX's blog*

So, to use **CreateWindowInBand** we need the executable to be either
- an **UIAccess** enabled process which indeed requires it to be **signed by a proper certificate** (could be obtained but costs a lump amount of money).
- an **immersive process** and be **signed with a Microsoft certificate "Microsoft Windows"**.

Both of them won't happen obviously. So, this is where the black magic kicks in.

So, what we need is a **surrogate host** which has the ability to be an **immersive process** and be **signed with a Microsoft certificate "Microsoft Windows"** and won't do anything other than hosting our application. And guess what [@ADeltaX](https://github.com/ADeltaX) strikes again! He found out the proper host for this purpose called the **"RuntimeBroker"** (an immersive, MS signed surrogate application found inside `C:\Windows\System32\`). He had to get 4 architecture specific **RuntimeBroker** executables from 4 different Windows 10 OSes (x86, x64, ARM and ARM64). He renamed it to **`Bro_(arch).exe`** for fun.

Using **RuntimeBroker** to host and execute our application was not a joke. It required us to do dll injection and host the .NET 5 **CoreCLR** inside of it. For further information see [this comment](https://github.com/ModernFlyouts-Community/ModernFlyouts/issues/32#issuecomment-735454172).

I can assure you this procedure is completely safe and secure. It won't affect your system and infringe your privacy. We guarantee you on that.

Somehow, we made it to work properly. We can't thank [@ADeltaX](https://github.com/ADeltaX) enough for all his help ❤. Attributing him is the only thing we can do in return.

#### Final Words

I [@ShankarBUS](https://github.com/ShankarBUS), the **author** and **lead developer** of this project will be leaving this team and this project in the hands of the other core team members [@Samuel12321](https://github.com/Samuel12321), [@Cyberdroid1](https://github.com/Cyberdroid1) and [@ADeltaX](https://github.com/ADeltaX). This will be the last release from myside.

I would like to give a shout out to
- [@ADeltaX](https://github.com/ADeltaX), for AudioFlyout and every single help from him.
- [@Samuel12321](https://github.com/Samuel12321) (co-owner, publisher and maintainer), for publishing this application, dealing with users and helping me maintain this repo since its early stages.
- [@Cyberdroid1](https://github.com/Cyberdroid1), for his ideas, suggestions, motivation and help & enthusiasm for this project.
- And finally all the community members (either on Discord, Telegram or GitHub), for all the contributions, motivations, suggestions, ideas and help and everything!

It means a lot to me.

My final words: ***Change da world… my final message. Goodb ye***.
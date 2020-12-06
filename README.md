# ModernFlyouts
### A modern **Fluent Design** replacement for the old **Metro themed** flyouts present in Windows since Windows 8

![ModernFlyouts](ModernFlyouts/Assets/Images/ModernFlyouts_128.png)

[Overview](#overview) | [Features](#features) | [System Requirements](#system-requirements) | [Installation](#installation) | [Screenshots](#screenshots)

## Overview

**Default Flyout** | **ModernFlyouts**

![Overview](docs/images/Overview.png)

This application aims to provide a **Fluent Design System** based replacement for the old, built-in, **Metro Design** based **Audio/Airplane mode/Brightness** flyouts present in **Windows** *(they are not updated since Windows 8 LOL)* which are shown while pressing the media or volume keys or even the brightness keys *(may be absent on Desktop PCs)* or when airplane/flight mode is toggled.

This project has its roots in [AudioFlyout](https://github.com/ADeltaX/AudioFlyout) by [@ADeltaX](https://github.com/ADeltaX/) with additional implementations for **"Airplane mode"**, **"Brightness"** and **"LockKeys"** *(includes Insert key, Caps, Num & Scroll lock keys)* flyouts. This project stands as a complete replacement for the built-in one.

> Note : The built-in flyout will not be permanently affected. It will be hidden temporarily while this application is running. So, no reason to fear breaking your system.
> For more information on how to recover the original flyouts, [check out this How To page](https://github.com/ShankarBUS/ModernFlyouts/wiki/How-To).

> Users are provided the freedom to choose between the Windows built-in flyouts or modern flyouts from ModernFlyouts or neither of them.

> It is impossible to have a flyout for the keyboard backlight brightness or for the function (Fn) key because they are not passed as keys but as hardware signals (which the OEMs decide).  Any OS can receive those signals if they have the required driver.

### Please check out the [Wiki](https://github.com/ShankarBUS/ModernFlyouts/wiki) for additional information, guides and how-tos.

## Features
- Fluent UI (similar to Windows 10X).
- Supports **Light**, **Dark** & **High contrast** themes.
- Media session controls have additional options such as **Shuffle**, **Repeat**, **Stop** and **Timeline Info**
- Redesigned audio and brightness flyouts along with additional flyouts for **Airplane mode**, **Lock keys** and **Insert/Overtype**
- Each module can be **disabled individually**.
- Flyout's TopBar can be pinned, unpinned or hidden.
- Flyout is **Draggable** and autosaves the position. It also has feature for default position. (Check out the settings)
- Smooth **Animations** & **Transitions**
- **Opacity** of the background of flyouts can be changed
- **Timeout** of flyouts can be changed

## System Requirements
- **Windows 10 1809 and above** (older versions such as v0.3 and below supports Windows downlevel to Windows 8, however v0.4 and above won't)

## Installation
Modern Flyouts is available for you to install via [GitHub](https://github.com/ShankarBUS/ModernFlyouts/releases/latest), **winget** and the [Microsoft Store](https://www.microsoft.com/store/apps/9MT60QV066RP).

All the distribution methods mentioned above are supported, however installing the app from the **Microsoft Store** is **recommended** as it is easier to install and will automatically remain up to date.

**Microsoft Store:**

<a href='https://www.microsoft.com/store/apps/9MT60QV066RP?ocid=badge'><img src='https://developer.microsoft.com/en-us/store/badges/images/English_get-it-from-MS.png' alt='Microsoft Store' width='160'/></a>

You can [sign up here](https://forms.office.com/Pages/ResponsePage.aspx?id=DQSIkWdsW0yxEjajBLZtrQAAAAAAAAAAAAMAALdxYU9UQU9GMzQ2Rk40MDJFSkU5UzRKTVg2Nk1PTy4u) for **Beta Builds** of ModernFlyouts which will include new experimental features

**GitHub:**

1. Go to the [latest release on the Releases page](https://github.com/ShankarBUS/ModernFlyouts/releases/latest).
2. Download the latest ***.msix** file from the assets.
3. Install the downloaded ***.msix** file and launch the app from the **Start Menu**.

**winget:**

_please note that due to how winget processes package updates, updates may take **longer** to be released to winget_

`winget install --id=ModernFlyouts.ModernFlyouts -e`

## NOTES

### Credits
First of all, we must thank our good friend **[@ADeltaX](https://github.com/ADeltaX/)** for one of his marvelous works **"[AudioFlyout](https://github.com/ADeltaX/AudioFlyout)"**. Since the project was stale for a while and it lacked support for brightness and airplane mode flyouts, this project was born. He not only let us use his source code but also helped us improve this app. And he still supports us 😄.
I must admit that this project wouldn't be here *how it is* without **[@ADeltaX](https://github.com/ADeltaX/)**. Our heartful thanks to him (and our contributors too obviously 😅) ❤.

### Our Team
- **[@ShankarBUS](https://github.com/ShankarBUS/)**.
- **[@Samuel12321](https://github.com/Samuel12321/)** - co-maintainer/co-owner of this repository and application publisher (including **Microsoft Store**).

### Our Contributors 💗💕

**Code:**
- [yume-chan](https://github.com/yume-chan) - Found and fixed an important bug which we couldn't even identify 😅 [#113](https://github.com/ShankarBUS/ModernFlyouts/pull/113).

**Translators (sorted old to new):**
- [yukiokun057](https://github.com/yukiokun057) - Russian translator.
- [knurzl](https://github.com/knurzl) - German translator.
- [lucasskluser](https://github.com/lucasskluser) - Portuguese translator.
- [Stealnoob](https://github.com/Stealnoob) - French translator.
- [imgradeone](https://github.com/imgradeone) - Chinese (Simplified) translator.
- [sewerynkalemba](https://github.com/sewerynkalemba) - Polish translators.
- [ArmasF31](https://github.com/ArmasF31) - German translator.
- [Tarik02](https://github.com/Tarik02) - Ukrainian translator.
- [Per-Terra](https://github.com/Per-Terra) - Japanese translator.

**Others:**
- [Cyberdroid1](https://github.com/Cyberdroid1) - Wiki.

### Dependencies and References
- [NAudio](https://github.com/naudio/NAudio)
- [ModernWpf](https://github.com/Kinnara/ModernWpf) (Inspired the name **Modern**Flyouts)
- [Hardcodet.NotifyIcon.Wpf](https://github.com/hardcodet/wpf-notifyicon)

## Screenshots

Coming back soon 😅.

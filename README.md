![ModernFlyouts](ModernFlyouts/Assets/Images/ModernFlyouts_128.png)
# ModernFlyouts

#### An open source, modern, and **Fluent Design-based** replacement for the old **Metro-themed** flyouts present in **Windows 10**.

[![Microsoft Store](https://img.shields.io/badge/Microsoft-Store-blue?style=flat&logo=microsoft)](https://www.microsoft.com/store/apps/9MT60QV066RP?ocid=badge)
[![Github All Releases](https://img.shields.io/github/downloads/ModernFlyouts-Community/ModernFlyouts/total.svg?style=flat&logo=github)](https://github.com/ModernFlyouts-Community/ModernFlyouts/releases)
[![GitHub release](https://img.shields.io/github/release/ModernFlyouts-Community/ModernFlyouts.svg?style=flat&logo=github)](https://github.com/ModernFlyouts-Community/ModernFlyouts/releases)
[![Telegram](https://img.shields.io/badge/Telegram-channel-blue?style=flat&logo=telegram)](https://t.me/modernflyouts)
[![Discord](https://discordapp.com/api/guilds/772367965307404298/widget.png)](https://discord.gg/TcYskeyaYE)


[Overview](#overview) • [Features](#features) • [System Requirements](#system-requirements) • [Installation](#installation) • [Connect with us](#connect-with-us) • [Contributing](#contributing) • [Screenshots](#screenshots)

## Overview

**Default Flyout** | **ModernFlyouts**

![Overview](docs/images/Overview.png)

This application aims to provide a **Fluent Design System** based replacement for the old, built-in, **Metro Design** based **Audio/Airplane mode/Brightness** flyouts present in **Windows** *(which haven't been updated since Windows 8 LOL)* which are shown while pressing the media or volume keys or even the brightness keys *(may be absent on Desktop PCs)* or when airplane/flight mode is toggled.

This project has its roots in [AudioFlyout](https://github.com/ADeltaX/AudioFlyout) by [@ADeltaX](https://github.com/ADeltaX/) with additional implementations for **"Airplane mode"**, **"Brightness"** and **"LockKeys"** *(includes Insert key, Caps, Num & Scroll lock keys)* flyouts. This project stands as a complete replacement for the built-in one.

> Note : The built-in flyout will not be permanently affected. It will be hidden temporarily while this application is running. So, no reason to fear breaking your system.
> For more information on how to recover the original flyouts, [check out this How To page](https://github.com/ModernFlyouts-Community/ModernFlyouts/wiki/How-To).

> Users are provided the freedom to choose between the Windows built-in flyouts or modern flyouts from ModernFlyouts or neither of them.

> It is impossible to have a flyout for the keyboard backlight brightness or for the function (Fn) key because they are not passed as keys but as hardware signals (which the OEMs decide).  Any OS can receive those signals if they have the required driver.

### Please check out the [Wiki](https://github.com/ModernFlyouts-Community/ModernFlyouts/wiki) for additional information, guides and how-tos.

### Please check out [this document](docs/GSMTC-Support-And-Popular-Apps.md) for support regarding your media player/browser.

## Features

- Fluent UI (similar to Windows 10X).
- Supports **Light**, **Dark** & **High contrast** themes.
- Media session controls have additional options such as **Shuffle**, **Repeat**, **Stop** and **Timeline Info**.
- Redesigned audio and brightness flyouts along with additional flyouts for **Airplane mode**, **Lock keys** and **Insert/Overtype**.
- Each module can be **disabled individually**.
- Flyout's TopBar can be pinned, unpinned or hidden.
- Flyout is **Draggable** and autosaves the position. It also has feature for default position. (Check out the settings)
- Smooth **Animations** & **Transitions**.
- **Opacity** of the background of flyouts can be changed.
- **Timeout** of flyouts can be changed.

## System Requirements

- **Windows 10 1809 and above** (older versions such as [v0.3](https://github.com/ModernFlyouts-Community/ModernFlyouts/releases/tag/v0.3.0) and below support Windows 10 1803 downlevel till Windows 8, however v0.4 and above won't).

## Installation

Modern Flyouts is available for you to install via [GitHub](https://github.com/ModernFlyouts-Community/ModernFlyouts/releases/latest), **winget** and the [Microsoft Store](https://www.microsoft.com/store/apps/9MT60QV066RP).

All the distribution methods mentioned above are supported, however installing the app from the **Microsoft Store** is **recommended** as it is easier to install and will automatically remain up to date.

**Microsoft Store:**

<a href='https://www.microsoft.com/store/apps/9MT60QV066RP?ocid=badge'><img src='https://developer.microsoft.com/en-us/store/badges/images/English_get-it-from-MS.png' alt='Microsoft Store' width='160'/></a>

**GitHub:**

1. Go to the [latest release on the Releases page](https://github.com/ModernFlyouts-Community/ModernFlyouts/releases/latest).
2. Download the latest ***.msixbundle** file from the assets.
3. Install the downloaded ***.msixbundle** file and launch the app from the **Start Menu**.

**winget:**

_please note that due to how winget processes package updates, updates may take **longer** to be released to winget_.

`winget install --id=ModernFlyouts.ModernFlyouts -e`

**Chocolatey**

ModernFlyouts is **Unofficially** available to install via Chocolatey [Here](https://chocolatey.org/packages/modernflyouts), however as this is not maintained by us we can not verify it's security or being up to date.

## Connect with us
You can join our [Discord Server](https://discord.gg/TcYskeyaYE) or [Telegram Group](https://t.me/ModernFlyouts) to connect with us. By doing so, we can have off-topic conversations, news about this app, development previews and providing & collecting feedback (people who don't have a GitHub account may be benefitted).

## Contributing

This project welcomes all types of contributions such help planning, design, documentation, finding bugs are ways everyone can help on top of coding features / bug fixes. We are excited to work with the community to make this project reach its goals and beyond.

We ask that **before you start work on a feature that you would like to contribute**, please read our [Contributor's Guide](CONTRIBUTING.md). We will be happy to work with you to figure out the best approach, provide guidance and mentorship throughout feature development, and help avoid any wasted or duplicate effort.

For guidance on developing for ModernFlyouts, please read the [developer guide](docs/developer_guide.md) for a detailed breakdown. This includes how to setup your computer to build and run the app.

## NOTES

### Credits

First of all, we must thank our good friend **[@ADeltaX](https://github.com/ADeltaX/)** for one of his marvelous works **"[AudioFlyout](https://github.com/ADeltaX/AudioFlyout)"**. Since the project was stale for a while and it lacked support for brightness and airplane mode flyouts, this project was born. He not only let us use his source code but also helped us improve this app. And he still supports us 😄.
I must admit that this project wouldn't be here *how it is* without **[@ADeltaX](https://github.com/ADeltaX/)**. Our heartful thanks to him ❤.

Next, we must thank **[@riverar](https://github.com/riverar)** for accepting our invite and allowing us to integrate parts of [EarTrumpet](https://github.com/File-New-Project/EarTrumpet) into ModernFlyouts.

### Our Team

- **[Samuel12321](https://github.com/Samuel12321/)** - Maintainer/co-owner of this repository and application publisher (including **Microsoft Store**).
- **[ShankarBUS](https://github.com/ShankarBUS/)** - Ex-Maintainer/co-owner & Developer.
- **[Cyberdroid1](https://github.com/Cyberdroid1)** - Maintainer.
- **[ADeltaX](https://github.com/ADeltaX/)** - Maintainer & Developer - Our savior! Helps us at critical times.


### Our Contributors 💗💕

**Code:**

- [yume-chan](https://github.com/yume-chan) - Found and fixed an important bug which we couldn't even identify 😅 [#113](https://github.com/ModernFlyouts-Community/ModernFlyouts/pull/113).
- [Renzo904](https://github.com/Renzo904) - Fixed some important and annoying bugs [#306](https://github.com/ModernFlyouts-Community/ModernFlyouts/pull/306) and [#313](https://github.com/ModernFlyouts-Community/ModernFlyouts/pull/313).
- [fheck](https://github.com/fheck) - Added the ability to toggle the usage of media session controls' thumbnail as background [#315](https://github.com/ModernFlyouts-Community/ModernFlyouts/pull/315).

**Translators :**

- [yukiokun057](https://github.com/yukiokun057) - Russian translator.
- [knurzl](https://github.com/knurzl) - German translator.
- [lucasskluser](https://github.com/lucasskluser) - Portuguese translator.
- [Stealnoob](https://github.com/Stealnoob) - French translator.
- [imgradeone](https://github.com/imgradeone) - Chinese (Simplified) translator.
- [sewerynkalemba](https://github.com/sewerynkalemba) - Polish translator.
- [ArmasF31](https://github.com/ArmasF31) - German translator.
- [ANT0x1](https://github.com/ANT0x1) - Russian translator.
- [Hymian7](https://github.com/Hymian7) - German translator.
- [TheAgamer554](https://github.com/TheAgamer554) - Spanish translator.
- [ShintakuNobuhiro](https://github.com/ShintakuNobuhiro) - Japanese translator.
- [blinchk](https://github.com/blinchk) - Russian translator.
- [TragicLifeHu](https://github.com/TragicLifeHu) - Chinese (Traditional) translator.
- [Tarik02](https://github.com/Tarik02) - Ukrainian translator.
- [MichelangeloDePascale02](https://github.com/MichelangeloDePascale02) - Italian translator.
- [Per-Terra](https://github.com/Per-Terra) - Japanese translator.
- [Renzo904](https://github.com/Renzo904) - Spanish translator.
- [Arno500](https://github.com/Arno500) - French translator.
- [Daxxxis](https://github.com/Daxxxis) - Polish translator.
- [OOBSoftInc](https://github.com/OOBSoftInc) - Italian translator.
- [VasilisPat](https://github.com/VasilisPat)
- [honorsea](https://github.com/honorsea) - Turkish translator.
- [MohammadShughri](https://github.com/MohammadShughri) - Arabic translator.
- [Slasar41](https://github.com/Slasar41) - Indoesian translator.
- [ShangJixin](https://github.com/ShangJixin) - Chinese (Simplified) translator.
- [mate131909](https://github.com/mate131909) - Korean translator.
- [nvi9](https://github.com/nvi9) - Hungarian translator.
- [Reset12138](https://github.com/Reset12138) - Chinese (Simplified) translator.
- [howon-kim](https://github.com/howon-kim) - Korean translator.
- [pklion](https://github.com/pklion) - Japanese translator.

**Others:**

- [Poopooracoocoo](https://github.com/Poopooracoocoo) - README.

### Dependencies and References

- [NAudio](https://github.com/naudio/NAudio)
- [ModernWpf](https://github.com/Kinnara/ModernWpf) (Inspired the name **Modern**Flyouts)
- [Hardcodet.NotifyIcon.Wpf](https://github.com/hardcodet/wpf-notifyicon)


## Beta Builds

The ability to signup for beta builds has been temporarily removed while we work on a better way to release and manage them. Beta builds will still occasionally be available to download fron the releases tab on github. Thankyou to everyone who helps us test new releases before they go public.

## Screenshots

Screenshots are temporarily removed 😅. They will be restored during v1.0 release cycle.

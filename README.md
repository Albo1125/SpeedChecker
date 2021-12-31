# SpeedChecker
SpeedChecker is a UK-based resource for FiveM by Albo1125 that provides speed laser gun & average speed check functionality. It is available at [https://github.com/Albo1125/SpeedChecker](https://github.com/Albo1125/SpeedChecker)

## Installation & Usage
1. Download the latest release.
2. Unzip the SpeedChecker folder into your resources folder on your FiveM server.
3. Add the following to your server.cfg file:
```text
ensure SpeedChecker
```
4. Optionally, download a [weaponmarksmanpistol.meta](https://www.gta5-mods.com/weapons/driveby-marksman-pistol) that allows you to use it from within a vehicle. Uncomment the two lines from the `fxmanifest.lua` to add it to the server.
5. Optionally, download a more appropriate model to replace the default marksmanpistol and stream it. For example, the [ProLaser 4](https://www.lcpdfr.com/downloads/gta5mods/misc/18506-prolaser-4-radar-gun/)
6. Optionally, customise the commands in `sv_SpeedChecker.lua`.
7. Optionally, enable and customise the whitelist in `vars.lua`.

## Commands & Controls
* /speedgun - Toggles the speed gun.
* PAGEUP - Turns on the average speed check interface while in a vehicle. Once the target vehicle passes a static object, press PAGEUP to start measuring the time (t). When you yourself pass that same static object, press PAGEUP again to start measuring the distance (d). When the target vehicle passes another static object, press PAGEUP to stop measuring the time. When you pass that same static object, press PAGEUP again to stop measuring the distance. You will then have a valid measurement. Press PAGEUP again to reset the interface.
* PAGEDOWN - Close the average speed check interface.


## Improvements & Licencing
Please view the license. Improvements and new feature additions are very welcome, please feel free to create a pull request. As a guideline, please do not release separate versions with minor modifications, but contribute to this repository directly. However, if you really do wish to release modified versions of my work, proper credit is always required and you should always link back to this original source and respect the licence.

## Libraries used (many thanks to their authors)
* [CitizenFX.Core.Client](https://www.nuget.org/packages/CitizenFX.Core.Client)

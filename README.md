# Heli Utils
Adjust Patrol Heli &amp; CH47 health values &amp; no. of crates dropped.

## Description
This plugin allows server administrators to configure the health and the number of crates dropped by CH47 and Patrol Helicopters in Rust. It uses a simple JSON configuration file that can be edited on-the-fly, providing enhanced flexibility in gameplay dynamics.

## Features
* Permissions System: Controls who can change the helicopter settings via commands.
* In-game Configuration Commands: Allows real-time adjustment of helicopter health and crate settings through chat commands.

## Configuration
The settings and options can be configured in the `HeliUtils.json` file under the `config` directory. The use of an editor and validator is recommended to avoid formatting issues and syntax errors.

```json
{
  "CH47Health": 4000,
  "PatrolHelicopterHealth": 10000,
  "CH47CrateCount": 1,
  "PatrolHelicopterCrateCount": 4
}
```

## Permissions
* `heliutils.sethealth` - Allows a user to set helicopter health.
* `heliutils.setcrate` - Allows a user to set the number of crates dropped.

e.g. `oxide.grant group admin heliutils.sethealth`

## Commands
Use the following commands in the chat to manage settings:

* `/heliutils sethealth [type] [value]` - Sets the health for the specified helicopter type (ch47 or patrol).
* `/heliutils setcrates [type] [number]` - Sets the number of crates dropped by the specified helicopter type.

## Installation
1. Download the `HeliUtils.cs` file.
2. Place it in the `oxide/plugins` directory of your Rust server.
3. Restart the server or use the `oxide.reload HeliUtils` command to load the plugin.
4. Edit the `HeliUtils.json` in the `oxide/config` directory to set the desired health and crate values.

## Support
For support, feature requests, or bug reports, please visit comment in the help thread.

## Version History

* 1.1.0: Refactor
* 1.0.0: Initial release.

# MapCycle Plugin for CounterStrikeSharp

<!-- Langue: English -->
## <u>Overview</u>
MapCycle is a plugin designed for CounterStrikeSharp. This plugin enables server administrators to automate the rotation of a predefined list of maps. It's compatible with both standard and workshop maps.

## <u>Donate</u>
I dedicate a significant part of my free time to coding and developing meaningful plugins for CS2. If you appreciate my work and would like to support me, please consider making a donation through PayPal. Your support helps me continue coding CS2 plugins. Thank you!
[![PayPal](https://www.paypalobjects.com/en_US/i/btn/btn_donate_LG.gif)](https://www.paypal.com/donate/?hosted_button_id=MVCFKC7V772WS)

## <u>Features</u>
- **Automatic Map Rotation**: Rotates maps according to a configurable list (random order possible).
- **Support for Multiple Map Types**: Compatible with both standard and workshop maps.
- **Simple Configuration**: Uses a JSON file for easy setup.
- **Chat Notifications**: Informs players about the upcoming map.
- **Rtv commands**: Triggers a vote directly by players or changes the map. This can depend on the configured options, such as ratio or not.
- **Config hotreloading**: Allows changing options without rebooting the server.
- **Add and remove map from the cycle directly in game**: Allows adding/removing maps on the fly without restarting the server.

## <u>Compatibility</u>
- [CS2 Simple Admin](https://github.com/daffyyyy/CS2-SimpleAdmin)

## <u>Commands</u>
| Command | Usage Example | Explanation | Accessibility |
|---------|---------------|-------------|---------------|
| `!addmap` | `!addmap cs_assault Assault 3070594412` | Adds a new map to the cycle without manually editing the configuration file | Admin |
| `!removemap` | `!removemap cs_assault` | Removes a map from the cycle directly in-game or from the console | Admin |
| `!keepmap` | `!keepmap [optional]mapDisplayName` | Adds the current map to the cycle | Admin |
| `!goto` | `!goto cs_assault` | Allows direct access to a chosen map in the cycle, bypassing the need to wait for the current match to end | Admin |
| `!go` | `!go` | Immediately transitions to the next map | Admin |
| `!cfgr` | `!cfgr` | Changes a value in the plugin configuration without restarting the server | Admin |
| `!resetrtv` | `!resetrtv` | Resets the Rock The Vote (RTV) | Admin |
| `!nextmap` | `!nextmap` / `!nextmap de_dust2` | Displays the next map or sets a different next map | Player/Admin |
| `!rtv` | `!rtv` | Initiates a vote or triggers a map change based on a ratio | Player |


## <u>Installation</u>
- Download the latest release from [here](https://github.com/RonanLOUARN/Map-Cycle/releases).
- Unzip the folder named `MapCycle`.
- Place it in the directory: `game/csgo/addons/counterstrikesharp/plugins/`.
- You can start your server with the two default maps, or add new maps to the configuration file, which will be automatically generated upon the first startup. 
- This plugins needs you to have access of the flag `@css/changemap` in `game/csgo/addons/counterstrikesharp/configs/admins.json` 

## <u>Admin config</u>
Go to `game/csgo/addons/counterstrikesharp/configs/admins.json`. Create the file if needed.
Write the configuration like the following one.
```json
{
  "NANOR": {
    "identity": "STEAM_0:0:123123123",
    "flags": ["@css/changemap"]
  }
}

```

More informations [here](https://docs.cssharp.dev/docs/admin-framework/defining-admins.html)

## <u>Configuration</u>
The configuration file is automatically generated in `game/csgo/addons/counterstrikesharp/configs/plugins/MapCycle`, initially containing two default maps.

**JSON attributes**

- `MapCycle`: An object containing parameters for map rotation.
  - `RandomOrder`: Enables or disables random map rotation.
  - `MapChangeAtTheEndOfMatchDelay`: Delay for map change once the match is finished (when we reach the win panel)

- `Rtv`: An object containing parameters for Rock The Vote (RTV) voting.
  - `Enabled`: Enables or disables RTV.
  - `AutoVoteEnabled`: Enables or disables automatic triggering of the vote.
  - `AutoVoteTimeStartInSeconds`: Triggers an auto vote after the configured time in this option. Overrides `AutoVoteRoundStart` and `AutoVoteStartAtTheEndOfMatch`. Set to 0 to disable.
  - `AutoVoteRoundStart`: The round number when the auto vote is triggered.
  - `AutoVoteStartAtTheEndOfMatch`: Enables voting at the end of the match. Overrides `AutoVoteRoundStart`.
  - `PlayerCommandEnabled`: Enables or disables the !rtv command for players.
  - `PlayerCommandTriggerAVote`: Triggers a vote when players type !rtv instead of changing the map directly.
  - `PlayerCommandRatioEnabled`: Enables the use of `PlayerCommandRatio`.
  - `PlayerCommandRatio`: From 0.0 to 1.0. This ratio triggers the action of the !rtv command only if the ratio of players who have typed !rtv is reached.
  - `PlayerCommandChangeTheMapDirectlyAfterVote`: Triggers the map change as soon as the vote is finished.
  - `VoteMapCount`: The number of maps proposed in the vote.
  - `VoteDurationInSeconds`: The duration of the vote in seconds.
  - `VoteRatioEnabled`: Enables the use of `VoteRatio`.
  - `VoteRatio`: From 0.0 to 1.0. The minimum ratio of players for the vote to be valid, otherwise a map will be chosen by the map cycle.
  - `ExtendMap`: Ajoute une option de vote pour continuer sur la map en cours

Each map in the configuration file includes the following attributes:
- `Name`: The actual name of the map (e.g., `de_dust2`, `de_cbble`).
- `DisplayName`: The name that is displayed in the chat. If this field is empty, it will display the name of the map (de_dust2, etc..).
- `Id`: The workshop ID, or the map name again if it's an official map.
- `Workshop`: Indicates whether the map is from the workshop (`true` or `false`).

#### <u>Example Configuration</u>
```json
{
  "MapCycle": {
    "RandomOrder": false,
    "MapChangeAtTheEndOfMatchDelay": 19
  },
  "Maps": [
    {
      "Name": "de_dust2",
      "DisplayName": "Dust 2",
      "Id": "de_dust2",
      "Workshop": false
    },
    {
      "Name": "de_aztec",
      "DisplayName": "Aztec",
      "Id": "3070960099",
      "Workshop": true
    }
  ],
  "Rtv": {
    "Enabled": true,
    "AutoVoteEnabled": true,
    "AutoVoteTimeStartInSeconds": 50,
    "AutoVoteRoundStart": 1,
    "AutoVoteStartAtTheEndOfMatch": true,
    "PlayerCommandEnabled": false,
    "PlayerCommandTriggerAVote": false,
    "PlayerCommandRatioEnabled": false,
    "PlayerCommandRatio": 1,
    "PlayerCommandChangeTheMapDirectlyAfterVote": false,
    "VoteMapCount": 5,
    "VoteDurationInSeconds": 30,
    "VoteRatioEnabled": true,
    "VoteRatio": 0.5,
    "ExtendMap": false
  },
  "ConfigVersion": 2
}

```

## <u>Troubleshooting</u>
#### <u>If the Cycle doesn't work as expected and it restarts to the first map</u>
1) **Check if the JSON config is correctly formated.** You can use this tool to verify that: https://jsonformatter.curiousconcept.com/
If it's correctly formated then you'll have a green screen
If not, you'll have a red screen with explanations:

2) **Check if the map name is correct.**
Set the map on your server with `host_workshop_map 123123123` and when the map has started you verify the name directly on the steam server browser.

## <u>Author</u>
- ModuleName: MapCycle
- ModuleAuthor: NANOR
- ModuleVersion: 1.4.3

## <u>Support</u>
For assistance, please raise an issue on the GitHub repository of the project.

---

**Note:** It is essential to ensure that CounterStrikeSharp and all necessary dependencies are properly installed and configured on your server to guarantee the effective functioning of the MapCycle plugin.


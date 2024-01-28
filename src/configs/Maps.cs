using CounterStrikeSharp.API.Core;
using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Modules.Commands;
using Microsoft.Extensions.Localization;

namespace MapCycle
{ 
    public partial class ConfigGen : BasePluginConfig
    {
        [JsonPropertyName("Maps")]
        public List<MapItem> Maps { get; set; } = new List<MapItem>
        {
            new MapItem { Name = "test_official_map", DisplayName = "Offi (REMOVE IT)", Id = "test_official_map", Workshop = false },
            new MapItem { Name = "test_workshop", DisplayName = "WS (REMOVE IT)", Id = "123123123", Workshop = true }
        };


        public void AddMap(string mapName, string displayName, string id, bool workshop, CommandInfo info, IStringLocalizer localizer)
        {
            if (Maps.Any(x => x.Name == id || x.Id == id || x.Name == mapName))
            {
                info.ReplyLocalized(localizer, "AlreadyExistingMap", mapName);
                return;
            }

            Maps.Add(new MapItem { Name = mapName, DisplayName = displayName, Id = id, Workshop = workshop });
            RewriteConfig();
            info.ReplyLocalized(localizer, "MapAdded", mapName);
        }

        public void RemoveMap(CCSPlayerController player, string mapName, CommandInfo info, IStringLocalizer localizer)
        {
            if (!Maps.Any(x => x.Name == mapName))
            {
                info.ReplyLocalized(localizer, "NotExistingMap", mapName);
                return;
            }

            Maps.RemoveAll(x => x.Name == mapName);
            RewriteConfig();
            info.ReplyLocalized(localizer, "MapRemoved", mapName);
        }

        public void ChangeMapDisplayName(CCSPlayerController player, string mapName, string displayName)
        {
            var map = Maps.FirstOrDefault(x => x.Name == mapName);
            if (map != null)
            {
                map.DisplayName = displayName;
                RewriteConfig();
            }
        }
    }
}
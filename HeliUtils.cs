using Oxide.Core;
using Oxide.Core.Plugins;
using Oxide.Core.Libraries.Covalence;
using System.Collections.Generic;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("HeliUtils", "Myles", "1.1.0")]
    public class HeliUtils : RustPlugin
    {
        private ConfigData configData;

        class ConfigData
        {
            public float CH47Health;
            public float PatrolHelicopterHealth;
            public int CH47CrateCount;
            public int PatrolHelicopterCrateCount;
        }

        private const string permissionHealth = "heliutils.sethealth";
        private const string permissionCrate = "heliutils.setcrate";

        protected override void LoadDefaultConfig()
        {
            PrintWarning("Creating a new configuration file.");
            configData = new ConfigData
            {
                CH47Health = 4000f,
                PatrolHelicopterHealth = 10000f,
                CH47CrateCount = 1,
                PatrolHelicopterCrateCount = 4
            };
            SaveConfig();  // Save default configuration
        }

        protected override void SaveConfig()
        {
            Config.WriteObject(configData, true); // Ensure config data is saved properly
        }

        private void LoadConfigVariables()
        {
            configData = Config.ReadObject<ConfigData>(); // Read the configuration into configData
        }

        void Init()
        {
            LoadConfigVariables();  // Load the configuration on initialization
            permission.RegisterPermission(permissionHealth, this);
            permission.RegisterPermission(permissionCrate, this);
            AddCovalenceCommand("heliutils", "HandleHeliCommand");

            // Register messages for localization
            lang.RegisterMessages(new Dictionary<string, string>
            {
                ["NoPermission"] = "You do not have permission to use this command.",
                ["InvalidValue"] = "Invalid value.",
                ["HealthSet"] = "{0} helicopter health set to {1}.",
                ["CrateCountSet"] = "{0} helicopter number of crates set to {1}.",
                ["Usage"] = "Usage: /heliutils sethealth [type] [value] or /heliutils setcrates [type] [number]",
                ["InvalidType"] = "Invalid helicopter type specified. Use 'ch47' or 'patrol'."
            }, this);
        }

        void OnEntitySpawned(BaseEntity entity)
        {
            if (entity is CH47Helicopter ch47)
            {
                ch47.InitializeHealth(configData.CH47Health, configData.CH47Health);
            }
            else if (entity is PatrolHelicopter heli)
            {
                heli.InitializeHealth(configData.PatrolHelicopterHealth, configData.PatrolHelicopterHealth);
            }
        }

        void OnEntityDeath(BaseCombatEntity entity, HitInfo info)
        {
            if (entity is CH47Helicopter ch47)
            {
                for (int i = 0; i < configData.CH47CrateCount; i++)
                {
                    DropCrate(ch47.transform.position);
                }
            }
            else if (entity is PatrolHelicopter heli)
            {
                for (int i = 0; i < configData.PatrolHelicopterCrateCount; i++)
                {
                    DropCrate(heli.transform.position);
                }
            }
        }

        void DropCrate(Vector3 position)
        {
            var crate = GameManager.server.CreateEntity("assets/prefabs/npc/patrol helicopter/heli_crate.prefab", position, Quaternion.identity) as LootContainer;
            crate.Spawn();
        }

        void HandleHeliCommand(IPlayer player, string command, string[] args)
        {
            if (args.Length < 3)
            {
                player.Reply(lang.GetMessage("Usage", this, player.Id));
                return;
            }

            string heliType = args[1].ToLower();
            string action = args[0].ToLower();
            string value = args[2];

            // Check for valid helicopter type
            if (heliType != "ch47" && heliType != "patrol")
            {
                player.Reply(lang.GetMessage("InvalidType", this, player.Id));
                return;
            }

            // Process commands based on action
            switch (action)
            {
                case "sethealth":
                    if (!player.HasPermission(permissionHealth))
                    {
                        player.Reply(lang.GetMessage("NoPermission", this, player.Id));
                        return;
                    }
                    SetHealth(player, heliType, value);
                    break;
                case "setcrates":
                    if (!player.HasPermission(permissionCrate))
                    {
                        player.Reply(lang.GetMessage("NoPermission", this, player.Id));
                        return;
                    }
                    SetCrates(player, heliType, value);
                    break;
                default:
                    player.Reply(lang.GetMessage("InvalidCommand", this, player.Id));
                    break;
            }
        }

        void SetHealth(IPlayer player, string heliType, string value)
        {
            if (float.TryParse(value, out float newHealth))
            {
                if (heliType == "ch47")
                {
                    configData.CH47Health = newHealth;
                }
                else if (heliType == "patrol")
                {
                    configData.PatrolHelicopterHealth = newHealth;
                }
                SaveConfig();
                player.Reply(string.Format(lang.GetMessage("HealthSet", this, player.Id), heliType.ToUpper(), newHealth));
            }
            else
            {
                player.Reply(lang.GetMessage("InvalidValue", this, player.Id));
            }
        }

        void SetCrates(IPlayer player, string heliType, string value)
        {
            if (int.TryParse(value, out int newCrateCount))
            {
                if (heliType == "ch47")
                {
                    configData.CH47CrateCount = newCrateCount;
                }
                else if (heliType == "patrol")
                {
                    configData.PatrolHelicopterCrateCount = newCrateCount;
                }
                SaveConfig();
                player.Reply(string.Format(lang.GetMessage("CrateCountSet", this, player.Id), heliType.ToUpper(), newCrateCount));
            }
            else
            {
                player.Reply(lang.GetMessage("InvalidValue", this, player.Id));
            }
        }

        void ProcessCommand(IPlayer player, string heliType, string action, string value)
        {
            // Handle each action
            switch (action)
            {
                case "sethealth":
                    if (float.TryParse(value, out float newHealth))
                    {
                        if (heliType == "ch47")
                        {
                            configData.CH47Health = newHealth;
                        }
                        else if (heliType == "patrol")
                        {
                            configData.PatrolHelicopterHealth = newHealth;
                        }
                        else
                        {
                            player.Reply(lang.GetMessage("InvalidType", this, player.Id));
                            return;
                        }
                        SaveConfig();
                        player.Reply(string.Format(lang.GetMessage("HealthSet", this, player.Id), heliType.ToUpper(), newHealth));
                    }
                    else
                    {
                        player.Reply(lang.GetMessage("InvalidValue", this, player.Id));
                    }
                    break;

                case "setcrates":
                    if (int.TryParse(value, out int newCrateCount))
                    {
                        if (heliType == "ch47")
                        {
                            configData.CH47CrateCount = newCrateCount;
                        }
                        else if (heliType == "patrol")
                        {
                            configData.PatrolHelicopterCrateCount = newCrateCount;
                        }
                        else
                        {
                            player.Reply(lang.GetMessage("InvalidType", this, player.Id));
                            return;
                        }
                        SaveConfig();
                        player.Reply(string.Format(lang.GetMessage("CrateCountSet", this, player.Id), heliType.ToUpper(), newCrateCount));
                    }
                    else
                    {
                        player.Reply(lang.GetMessage("InvalidValue", this, player.Id));
                    }
                    break;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace TICMod
{
    public static partial class CommandHandler
    {
        private static CommandResponse ParseTrigger(string command, string[] args, bool execute, int i, int j)
        {
            CommandResponse resp = new CommandResponse(false, $"Unknown Command '{command}'");
            command = command.ToLower();

            switch (command)
            {
                case "time":
                    resp = TriggerTime(args, resp, execute, i, j);
                    break;

                case "playerdeath":
                    resp = TriggerPlayerDeath(args, resp, execute, i, j);
                    break;
            }

            return resp;
        }


        private static CommandResponse TriggerTime(string[] args, CommandResponse resp, bool execute, int i, int j)
        {
            TICWorld world = ModContent.GetInstance<TICWorld>();
            TICWorld.Data data = null;
            if (execute)
            {
                data = world.data[(i, j)];
            }

            if (args.Length != 1)
            {
                resp.response = $"Takes 1 parameter; Time to activate at";
                return resp;
            }

            var ret = ParseTime(args[0], resp);
            resp = ret.Item2;

            if (!resp.valid)
            {
                return resp;
            }

            string givenTime = args[0];
            bool triggered = false;
            if (execute)
            {
                data.trigger = () =>
                {
                    string currenttime = Utilities.GetTimeAsString(Main.time);


                    if (data.enabled && currenttime == givenTime && !triggered)
                    {
                        ModContent.GetInstance<ExtraWireTrips>().AddWireUpdate(i, j - 1);
                        triggered = true;

                        if (data.chatOutput)
                        {
                            world.SendChatMsg($"Reached time {currenttime}, triggering.", i, j);
                        }
                    }
                    else if (currenttime != givenTime && triggered)
                    {
                        triggered = false;
                    }
                };
            }

            resp.valid = true;
            return resp;
        }

        private static CommandResponse TriggerPlayerDeath(string[] args, CommandResponse resp, bool execute, int i, int j)
        {
            TICWorld world = ModContent.GetInstance<TICWorld>();
            TICWorld.Data data = null;
            if (execute)
            {
                data = world.data[(i, j)];
            }

            string storeName = null;
            if (args.Length > 1)
            {
                resp.response = "Takes 1 optional parameter; Datastore name";
                return resp;
            }

            if (args.Length == 1)
            {
                storeName = args[0];
            }

            TICMod mod = ModContent.GetInstance<TICMod>();
            
            List<Player> triggeredPlayers = new List<Player>();
            if (execute)
            {
                data.trigger = () =>
                {
                    foreach (var player in Main.player)
                    {
                        if (player.dead && data.enabled && !triggeredPlayers.Contains(player))
                        {
                            triggeredPlayers.Add(player);
                            mod.playerDataStore.AddItem(storeName, player);
                            ModContent.GetInstance<ExtraWireTrips>().AddWireUpdate(i, j - 1);

                            if (data.chatOutput)
                                world.SendChatMsg($"{player.name} died, triggering.", i, j);
                        }
                        else if (!player.dead && triggeredPlayers.Contains(player))
                        {
                            triggeredPlayers.Remove(player);
                        }
                    }
                };
            }

            resp.valid = true;
            return resp;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace TICMod.Commands.Triggers
{
    class PlayerDeath : Trigger
    {
        protected string storeName = "";

        private HashSet<string> aliases_ = new HashSet<string>() { "death", "playerdeath" };
        protected override HashSet<string> aliases { get { return aliases_; } }

        public override bool ParseArguments(string[] args, out string err)
        {
            err = "";

            if (args.Length > 1)
            {
                err = "Takes 1 optional parameter; Datastore name";
                return false;
            }

            if (args.Length == 1)
            {
                storeName = args[0];
            }

            return true;
        }

        public override Action GetTrigger()
        {
            TICSystem.Data data = this.data;
            List<Player> triggeredPlayers = new List<Player>();

            return () =>
            {
                foreach (var player in Main.player)
                {
                    if (player.dead && data.enabled && !triggeredPlayers.Contains(player))
                    {
                        triggeredPlayers.Add(player);
                        modSystem.playerDataStore.AddItem(storeName, player);
                        ModContent.GetInstance<ExtraWireTrips>().AddWireUpdate(data.x, data.y - 1);

                        if (data.chatOutput)
                        {
                            world.SendChatMsg($"{player.name} died, triggering.", data.x, data.y);
                        }
                    }
                    else if (!player.dead && triggeredPlayers.Contains(player))
                    {
                        triggeredPlayers.Remove(player);
                    }
                }
            };
        }
    }
}

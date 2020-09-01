using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace TICMod.Commands.Triggers
{
    class Time : Trigger
    {
        protected string givenTime;
        protected bool triggered = false;

        private HashSet<string> aliases_ = new HashSet<string>() { "time", "istime" };
        protected override HashSet<string> aliases { get { return aliases_; } }

        public override bool ParseArguments(string[] args, out string err)
        {
            err = "";

            if (args.Length != 1)
            {
                err = $"Takes 1 parameter; Time to activate at";
                return false;
            }

            bool valid = ParseTime(args[0], out List<uint> time, out err);
            if (!valid)
            {
                return false;
            }
            givenTime = args[0];

            return true;
        }

        public override Action GetTrigger()
        {
            bool triggered = this.triggered;
            string givenTime = this.givenTime;
            TICWorld.Data data = this.data;

            return () =>
            {
                string currenttime = Utils.GetTimeAsString(Main.time);

                if (data.enabled && currenttime == givenTime && !triggered)
                {
                    ModContent.GetInstance<ExtraWireTrips>().AddWireUpdate(data.x, data.y - 1);
                    triggered = true;

                    if (data.chatOutput)
                    {
                        world.SendChatMsg($"Reached time {currenttime}, triggering.", data.x, data.y);
                    }
                }
                else if (currenttime != givenTime && triggered)
                {
                    triggered = false;
                }
            };
        }
    }
}

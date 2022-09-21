using IL.Terraria.GameContent.UI.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace TICMod.Commands.Triggers
{
    public class Timer : TriggerCommand
    {
        protected bool triggered = false;
        protected int seconds;
        protected DateTime lastTriggered;

        private HashSet<string> aliases_ = new HashSet<string>() { "timer", "repeat" };
        protected override HashSet<string> aliases { get { return aliases_; } }

        public override bool ParseArguments(string[] args, out string err)
        {
            err = "";

            if (args.Length != 1)
            {
                err = $"Takes 1 parameter; Interval (seconds)";
                return false;
            }

            bool valid = ParseInt(args[0], out seconds, out err, 0);
            if (!valid)
            {
                return false;
            }

            lastTriggered = DateTime.Now;

            return true;
        }

        public override Action GetTrigger()
        {
            bool triggered = this.triggered;
            int seconds = this.seconds;
            TICSystem.Data data = this.data;
            lastTriggered = DateTime.Now;

            return () =>
            {
                string currenttime = Utils.GetTimeAsString(Main.time);

                if (!data.enabled)
                {
                    lastTriggered = DateTime.Now;
                }

                if (data.enabled && DateTime.Compare(lastTriggered.AddSeconds(seconds), DateTime.Now) <= 0 && !triggered)
                {
                    ModContent.GetInstance<ExtraWireTrips>().AddWireUpdate(data.x, data.y - 1);
                    triggered = true;
                    lastTriggered = DateTime.Now;

                    if (data.chatOutput)
                    {
                        world.SendChatMsg($"{seconds} seconds passed since last activation, triggering.", data.x, data.y);
                    }
                }
                else if (DateTime.Compare(lastTriggered.AddSeconds(seconds), DateTime.Now) >= 0)
                {
                    triggered = false;
                }
            };
        }
    }
}

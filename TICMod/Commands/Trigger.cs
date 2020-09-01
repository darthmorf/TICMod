using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TICMod.Commands
{
    public abstract class Trigger : Command
    {
        protected TICWorld.Data data;
        protected TICWorld world;

        public virtual string Execute(int i, int j, TICWorld world)
        {
            this.world = world;
            this.data = world.data[(i, j)];
            this.data.trigger = GetTrigger();

            return "";
        }

        public override string Execute()
        {
            throw new Exception("Triggers must only be executed with passed position and data");
        }

        public abstract Action GetTrigger();
    }
}

/*        
        protected bool triggered = false;

        private HashSet<string> aliases_ = new HashSet<string>() { "" };
        protected override HashSet<string> aliases { get { return aliases_; } }

        public override bool ParseArguments(string[] args, out string err)
        {
            err = "";

            return true;
        }

        public override Action GetTrigger()
        {
            bool triggered = this.triggered;
            TICWorld.Data data = this.data;

            return () =>
            {
                       

                if (condition && !triggered)
                {
                    
                }
                else if (triggered)
                {

                    triggered = false;
                }
            };
        }
*/
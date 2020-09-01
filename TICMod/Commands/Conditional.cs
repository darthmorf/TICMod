﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TICMod.Commands.Conditionals
{
    public abstract class Conditional : Command
    {
        // Nothing here for now - might need it in the future

        /*

        private HashSet<string> aliases_ = new HashSet<string>() { "" };
        protected override HashSet<string> aliases { get { return aliases_; } }

        public override bool ParseArguments(string[] args, out string err)
        {
            err = "";

            
        }

        *
        public override string Execute(out bool conditionMet)
        {
                        
        }
         
        */
        public abstract string Execute(out bool conditionMet);

        public override string Execute()
        {
            return Execute(out bool conditionMet);
        }
    }
}
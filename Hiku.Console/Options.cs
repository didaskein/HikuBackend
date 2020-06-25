using Hiku.Framework.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hiku.Cmd
{
    /// <summary>
    /// Options in order to Run the cmd 
    /// </summary>
    public class Options
    {
        /// <summary>
        /// Set environment : DEV, UAT, PRE, PRD
        /// </summary>
        public EnumEnvironment Environment { get; set; }

        /// <summary>
        /// Set Console : true (for dev), false (on devops)
        /// </summary>
        public bool Console { get; set; }
    }
}

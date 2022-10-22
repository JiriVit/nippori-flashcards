using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nippori.Controls
{
    public class ButtonClickEventArgs : EventArgs
    {
        /// <summary>
        /// Gets command associated with the button.
        /// </summary>
        public ButtonCommands Command { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="ButtonClickEventArgs"/>.
        /// </summary>
        /// <param name="command">Command associated with the button.</param>
        public ButtonClickEventArgs(ButtonCommands command)
        {
            Command = command;
        }
    }
}

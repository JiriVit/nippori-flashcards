using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nippori.Controls
{
    /// <summary>
    /// Enumerates commands supported by buttons of a <see cref="VocableField"/>.
    /// </summary>
    public enum ButtonCommands
    {
        /// <summary>
        /// Speak the expression using speech synthesis.
        /// </summary>
        Speak,
        /// <summary>
        /// Look up translation on DeepL website.
        /// </summary>
        DeepL,
        /// <summary>
        /// Look up the expression on Google Images.
        /// </summary>
        GoogleImages,
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nippori.Enums
{
    public enum VocableStatus
    {
        /// <summary>
        /// Vocable is inactive and shall not be examined.
        /// </summary>
        Inactive = 0,
        /// <summary>
        /// Vocable is active and shall be always examined.
        /// </summary>
        Active = 1,
        /// <summary>
        /// Vocable is considered easy and should not be examined unless explicitly requested.
        /// </summary>
        Easy = 2,
        /// <summary>
        /// Vocable is considered difficult.
        /// </summary>
        Difficult = 3,
    }
}

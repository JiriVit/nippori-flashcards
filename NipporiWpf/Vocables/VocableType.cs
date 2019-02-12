using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NipporiWpf.Vocables
{
    /// <summary>
    /// Reprezentuje typ slovíčka.
    /// </summary>
    public class VocableType
    {
        public string Name { get; set; }
        public int InputColumn { get; set; }
        public int[] OutputColumns { get; set; }

        public override string ToString()
        {
            return String.Format("{0} | {1} | {2}", Name, InputColumn,
                String.Join(",", OutputColumns.Select(x => x.ToString())));
        }
    }
}

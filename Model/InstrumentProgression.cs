using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MusicBand_Manager.Model
{
    public class InstrumentProgression
    {
        public int Id { get; set; }
        public string Instrument { get; set; }
        public float? Progression { get; set; }
        public string? Notes { get; set; }
        public string? TutoLink { get; set; }
        public Member? Member { get; set; }
    }
}

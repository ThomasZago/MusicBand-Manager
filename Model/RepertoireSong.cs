using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace MusicBand_Manager.Model
{
       public class RepertoireSong
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Style { get; set; }
        public string OriginalComposer { get; set; }
        public string? Lyrics { get; set; }
        public List<InstrumentProgression> InstrumentProgressions { get; set; }
    }
}

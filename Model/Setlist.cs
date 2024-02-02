using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicBand_Manager.Model
{
    public class Setlist
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public Dictionary<RepertoireSong, int> SetlistSongs { get; set; }
    }
}

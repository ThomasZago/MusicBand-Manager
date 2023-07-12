using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicBand_Manager.Model
{
    public class Member
    {
        public int Id {  get; set; }
        public string FullName { get; set; }
        public DateTime ArrivalDate { get; set; }
        public DateTime? DepartureDate { get; set; }
    }
}

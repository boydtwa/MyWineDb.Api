using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyWineDb.Api.Models
{
    public class VineyardVarietalDetail
    {
        public string VineyardName { get; set; }
        public string Region { get; set; }
        public string VineyardAppellation {get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string GeoCoordinates { get; set; }

        public string CloneName { get; set; }
        public string VineyardPercentage { get; set; }

        public IEnumerable<NoteDetailModel> Notes { get; set; }

        public bool HasNotes()
        {
            return (Notes == null || Notes.Count() == 0) ? false : true;
        }
    }
}

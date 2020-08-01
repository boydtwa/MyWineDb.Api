using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MyWineDb.Api.Models
{
    public class WineMetaDetail
    {
        public bool IsEstate { get; set; }

        public string WineProductLine { get; set; }

        public IEnumerable<WineVarietalDetailModel> VarietalDetails { get; set; }

        public IEnumerable<NoteDetailModel> Notes { get; set; }

        public bool HasNotes()
        {
            return (Notes == null || Notes.Count() == 0) ? false : true;
        }
    }
}

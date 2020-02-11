using System;
using System.Collections.Generic;

namespace MoneyManagerUI
{
    public partial class Tads
    {
        public Tads()
        {
            RecordsTags = new HashSet<RecordsTags>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<RecordsTags> RecordsTags { get; set; }
    }
}

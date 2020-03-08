using System;
using System.Collections.Generic;

namespace MoneyManagerUI
{
    public partial class Tags
    {
        public Tags()
        {
            RecordsTags = new HashSet<RecordsTags>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<RecordsTags> RecordsTags { get; set; }
    }
}

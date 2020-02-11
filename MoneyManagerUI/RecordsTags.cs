using System;
using System.Collections.Generic;

namespace MoneyManagerUI
{
    public partial class RecordsTags
    {
        public int Id { get; set; }
        public int RecordId { get; set; }
        public int TadId { get; set; }

        public virtual Records Record { get; set; }
        public virtual Tads Tad { get; set; }
    }
}

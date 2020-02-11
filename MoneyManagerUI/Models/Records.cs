using System;
using System.Collections.Generic;

namespace MoneyManagerUI
{
    public partial class Records
    {
        public Records()
        {
            RecordsTags = new HashSet<RecordsTags>();
        }

        public int Id { get; set; }
        public decimal Sum { get; set; }
        public int CategoryId { get; set; }
        public int SubcategoryId { get; set; }
        public DateTime Date { get; set; }

        public virtual Categories Category { get; set; }
        public virtual Subcategories Subcategory { get; set; }
        public virtual ICollection<RecordsTags> RecordsTags { get; set; }
    }
}

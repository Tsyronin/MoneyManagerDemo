using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoneyManagerUI
{
    public partial class Records
    {
        public Records()
        {
            RecordsTags = new HashSet<RecordsTags>();
        }

        public int Id { get; set; }
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Sum { get; set; }
        public int CategoryId { get; set; }
        public int SubcategoryId { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        public virtual Categories Category { get; set; }
        public virtual Subcategories Subcategory { get; set; }
        public virtual ICollection<RecordsTags> RecordsTags { get; set; }
    }
}

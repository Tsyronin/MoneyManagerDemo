using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MoneyManagerUI
{
    public partial class Subcategories
    {
        public Subcategories()
        {
            Records = new HashSet<Records>();
        }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public int CatedoryId { get; set; }

        public virtual Categories Catedory { get; set; }
        public virtual ICollection<Records> Records { get; set; }
    }
}

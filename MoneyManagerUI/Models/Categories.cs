using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MoneyManagerUI
{
    public partial class Categories
    {
        public Categories()
        {
            Records = new HashSet<Records>();
            Subcategories = new HashSet<Subcategories>();
        }

        public int Id { get; set; }
        [Display(Name = "Category")]
        [Required(ErrorMessage = "You must enter the category name")]
        public string Name { get; set; }

        public virtual ICollection<Records> Records { get; set; }
        public virtual ICollection<Subcategories> Subcategories { get; set; }
    }
}

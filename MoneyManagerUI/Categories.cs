using System;
using System.Collections.Generic;

namespace MoneyManagerUI
{
    //comment
    public partial class Categories
    {
        public Categories()
        {
            Records = new HashSet<Records>();
            Subcategories = new HashSet<Subcategories>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Records> Records { get; set; }
        public virtual ICollection<Subcategories> Subcategories { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace MoneyManagerUI.ViewModel
{
    public class RecordViewModel
    {
        public int Id { get; set; }

        [Required]
        public int Sum { get; set; }

        public int CategoryId { get; set; }

        [Display(Name = "Subcategory")]
        public int SubcategoryId { get; set; }

        public int[] TagIds { get; set; } = new int[] { };
        public MultiSelectList Tags { get; set; }
    }
}

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
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Sum { get; set; }

        public int CategoryId { get; set; }

        public int SubcategoryId { get; set; }

        public int[] TagIds { get; set; }
        public MultiSelectList Tags { get; set; }
    }
}

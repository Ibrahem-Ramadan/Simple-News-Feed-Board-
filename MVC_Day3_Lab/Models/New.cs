namespace MVC_Day3_Lab.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("New")]
    public partial class New
    {
        public int NewId { get; set; }

        [Display(Name ="Title")]
        [Required(ErrorMessage ="Required Field")]
        public string NewTitle { get; set; }

        [Display(Name = "Breif")]
        [Required(ErrorMessage = "Required Field")]
        public string NewBreif { get; set; }

        [Display(Name = "Description")] 
        [Required(ErrorMessage = "Required Field")]
        public string NewDesc { get; set; }

        [StringLength(50)]
        [Display(Name = "Upload Photo")]
        public string NewPhoto { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Posted Time")]
        public DateTime? DateTime { get; set; }

        public int? UserId { get; set; }

        [Display(Name = "Catalog")]
        public int? CatId { get; set; }

        public virtual Catalog Catalog { get; set; }

        public virtual User User { get; set; }
    }
}

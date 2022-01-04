namespace MVC_Day3_Lab.Models
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            News = new HashSet<New>();
            Skills = new List<Skill>();
        }

        public int UserId { get; set; }

        [Required(ErrorMessage = "Required Field")]
        [StringLength(50)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Required Field")]
        [StringLength(50)]
        public string Password { get; set; }

        [NotMapped]
        [Display(Name = "Confirm Password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Password Not Matching")]
        public string ConfPassword { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "Required Field")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Invalid Email")]
        //[Remote("emailCheck","User",ErrorMessage ="this Email Already Excist")]
        public string Email { get; set; }

        [Range(18, 30, ErrorMessage = "Invalid Age")]

        public int? Age { get; set; }

        [StringLength(50)]
        [Display(Name = "Upload Your Photo")]
        public string Photo { get; set; }

        [StringLength(50)]
        [Display(Name = "Upload Your CV")]
        public string CV { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<New> News { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual List<Skill> Skills { get; set; }
    }
}

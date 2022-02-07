using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OSwan_TheatreApp.Models.ViewModels
{
    public class EditRegisteredUserViewModel
    {
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public string Street { get; set; }

        public string City { get; set; }

        [Display(Name = "Post Code")]
        public string PostCode { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Trusted User?")]
        public TrustedUser TrustedUser { get; set; }

        [Required]
        [Display(Name = "Suspended")]
        public bool IsSuspended { get; set; }
    }
}
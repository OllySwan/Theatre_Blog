using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OSwan_TheatreApp.Models
{
    public class Admin : User
    {
        [Display(Name ="Status")]
        public AdminStatus AdminStatus { get; set; }
    }

    //Admin Status is fairly self explanatory here

    public enum AdminStatus
    {
        FullTime,
        PartTime
    }
}
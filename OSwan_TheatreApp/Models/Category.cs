using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OSwan_TheatreApp.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Display(Name ="Category")]
        public string Name { get; set; }

        //Navigational Properties

        public virtual List<Post> Posts { get; set; }//Represents the many
    }
}
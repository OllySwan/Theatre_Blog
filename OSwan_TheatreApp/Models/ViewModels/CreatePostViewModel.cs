using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OSwan_TheatreApp.Models.ViewModels
{
    public class CreatePostViewModel
    {
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        public string MainBody { get; set; }

        public string ImageUrl { get; set; }

        public string Category { get; set; }

        public ICollection<SelectListItem> Categories { get; set; }
    }
}
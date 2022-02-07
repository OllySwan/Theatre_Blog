﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OSwan_TheatreApp.Models.ViewModels
{
    public class ChangeViewModel
    {
        public string UserName { get; set; }

        public string OldRole { get; set; }

        public ICollection<SelectListItem> Roles { get; set; }

        [Required, Display(Name = "Role")]
        public string Role { get; set; }
    }
}
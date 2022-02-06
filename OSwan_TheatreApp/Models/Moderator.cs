using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OSwan_TheatreApp.Models
{
    public class Moderator : User
    {
        public ModType ModType { get; set; }
    }

    //Jr is a new mod
    //Sr is a mod that has been helping with the site for a long time

    public enum ModType
    {
        Jr,
        Sr
    }
}
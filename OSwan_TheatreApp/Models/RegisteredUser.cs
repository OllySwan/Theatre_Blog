using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OSwan_TheatreApp.Models
{
    public class RegisteredUser : User
    {
        public TrustedUser TrustedUser { get; set; }
    }

    //Trusted vs not trusted will indicate if the user has been suspended before

    public enum TrustedUser
    {
        Trusted,
        NotTrusted
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OSwan_TheatreApp.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }

        [Required]
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        public string MainBody { get; set; }

        public string ImageUrl { get; set; }

        [Display(Name ="Date Posted")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:d}")] //Short date, may change later
        public DateTime DatePosted { get; set; }

        public ApprovalStatus ApprovalStatus { get; set; }


        //Navigational Properties
        //User
        [ForeignKey("User")]
        public string UserId { get; set; }
        public User User { get; set; }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        public Category Category { get; set; }//Representing the one


        //Will need to store comments
    }

    //Enum to handle approval status of post
    public enum ApprovalStatus
    {
       TBC,
       Approved,
       Declined
    }

}
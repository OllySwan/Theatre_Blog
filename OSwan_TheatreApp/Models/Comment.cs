using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OSwan_TheatreApp.Models
{
    public class Comment
    {
        public Comment()
        {
            this.Date = DateTime.Now;
        }

        public int CommentId { get; set; }

        [Required]
        public string Text { get; set; }

        public string CommentAuthor { get; set; }

        public DateTime Date { get; set; }

        public commentApprovalStatus commentApprovalStatus { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }

        public User User { get; set; }

        [ForeignKey("Post")]
        public int PostId { get; set; }
        public virtual Post Post { get; set; }
    }

    //Enum to handle approval status of post
    public enum commentApprovalStatus
    {
        TBC,
        Approved,
        Declined
    }
}
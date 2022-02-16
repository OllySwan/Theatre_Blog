using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OSwan_TheatreApp.Models.ViewModels
{
    public class PostCommentViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Text { get; set; }

        public string CommentAuthor { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogSystem.MvcUI.Models
{
    public class EditArticleViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public Guid[] CategoryIds { get; set; }
    }
}
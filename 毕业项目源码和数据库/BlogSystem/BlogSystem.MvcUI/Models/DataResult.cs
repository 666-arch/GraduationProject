using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogSystem.MvcUI.Models
{
    public class DataResult
    {
        public string Token { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
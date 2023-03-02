using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App2.Models
{
    public class Token
    {
        public string client_id { set; get; }
        public string client_secret { set; get;}
        public string grant_type { set; get; }
    }
}
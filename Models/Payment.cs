using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App2.Models
{
    public class Payment
    {
        public int msisdn { set; get; }
        public int reference { set; get; }
        public string pin { set; get; }
        public int amount { set; get; }
        public string id { set; get; }


    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace CommentEngine.Models
{
    public class AdminKomentar
    {
        public string Vijest { get; set; }
        public string Ime { get; set; }        
        public string  Komentar { get; set; }
        public int Odobren { get; set; }
        public int Zloupotreba { get; set; }
        public string Username { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public DateTime Datum { get; set; }
        public int Id { get; set; }
    }
}

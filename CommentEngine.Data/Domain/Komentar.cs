using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommentEngine.Data.Domain
{
    public class Komentar
    {
        public virtual int id { get; set; }
        public virtual int id_vijesti { get; set; }
        public virtual string username { get; set; }
        public virtual string ime { get; set; }
        public virtual string tekst { get; set; }
        public virtual DateTime datum { get; set; }
        public virtual int odobren { get; set; }
        public virtual int zloupotreba { get; set; }
        public virtual int? id_parent { get; set; }
        public virtual string odgovara { get; set; }
        public virtual int likes { get; set; }
        public virtual int dislikes { get; set; }
        public virtual string nivo { get; set; }
    }
}

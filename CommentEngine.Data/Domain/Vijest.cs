using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommentEngine.Data.Domain
{
    public class Vijest
    {
        public virtual int id { get; set; }
        public virtual string naslov { get; set; }
        public virtual string tekst { get; set; }
        public virtual DateTime datum { get; set; }
        public virtual string username { get; set; }
        public virtual IList<Komentar> komentari { get; set; }
    }
}

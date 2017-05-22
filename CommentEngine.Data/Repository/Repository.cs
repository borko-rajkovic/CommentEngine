using CommentEngine.Data.Domain;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Linq;
using CommentEngine;

namespace CommentEngine.Data.Repository
{
    public class Repository
    {
        public static List<Vijest> GetAllVijesti()
        {
            using (ISession session = NHHelper.OpenSession())
            {
                List<Vijest> result = session.Query<Vijest>().Select(x => x).Fetch(x => x.komentari).ToList();
                
                return result;
            }
        }

        public static Komentar GetKomentarById(int id_komentara)
        {
            using (ISession session = NHHelper.OpenSession())
            {
                Komentar result = session.Query<Komentar>().Where(x => x.id == id_komentara).FirstOrDefault();

                return result;
            }
        }

        public static List<Komentar> GetKomentari(int? id_vijesti = null)
        {
            using (ISession session = NHHelper.OpenSession())
            {
                List<Komentar> result;

                var test = session.QueryOver<Vijest>()
                                .JoinQueryOver<Komentar>(x => x.komentari)
                                .List();

                if (id_vijesti == null)
                {
                    result = session.Query<Komentar>().ToList();
                }
                else
                {
                    result = session.Query<Komentar>().Where(x => x.odobren == 1 && x.id_vijesti == id_vijesti).OrderBy(x => x.nivo).ToList();
                }                
                return result;
            }
        }

        public static int BrojKomentaraZaVijest(int id_vijesti)
        {
            using (ISession session = NHHelper.OpenSession())
            {
                int result = session.Query<Komentar>().Where(x => x.odobren == 1 && x.id_vijesti == id_vijesti).Count();
                return result;
            }
        }

        public static bool KomentarPrijavljen(int id)
        {
            using (ISession session = NHHelper.OpenSession())
            {
                bool result = session.Query<Komentar>().Where(x => x.zloupotreba == 1 && x.id == id).Any();
                return result;
            }
        }

        public static void PrijaviKomentar(int id)
        {
            using (ISession session = NHHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    Komentar result = session.Query<Komentar>().Where(x => x.id == id).FirstOrDefault();
                    result.zloupotreba = 1;
                    session.Update(result);
                    transaction.Commit();
                }
            }
        }

        public static int LajkKomentar(int id)
        {
            int likes = 0;
            using (ISession session = NHHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    Komentar result = session.Query<Komentar>().Where(x => x.id == id).FirstOrDefault();
                    result.likes += 1;
                    likes = result.likes;
                    session.Update(result);
                    transaction.Commit();
                }
            }
            return likes;
        }

        public static int DislikeKomentar(int id)
        {
            int dislikes = 0;
            using (ISession session = NHHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    Komentar result = session.Query<Komentar>().Where(x => x.id == id).FirstOrDefault();
                    result.dislikes += 1;
                    dislikes = result.dislikes;
                    session.Update(result);
                    transaction.Commit();
                }
            }
            return dislikes;
        }

        public static int PromijeniOdobrenjeKomentara(int id, bool odobri)
        {
            using (ISession session = NHHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    Komentar result = session.Query<Komentar>().Where(x => x.id == id).FirstOrDefault();

                    int brojOdobrenihKomentara = session.Query<Komentar>().Where(x => x.id_vijesti == result.id_vijesti && x.odobren == 1).Count()+(odobri?1:-1);

                    result.odobren = odobri?1:0;

                    session.Update(result);
                    transaction.Commit();

                    return brojOdobrenihKomentara;
                }
            }
        }

        public static string GetNaslov(int id)
        {
            string Naslov = "";            
            using (ISession session = NHHelper.OpenSession())
            {
                Naslov = session.Query<Vijest>().Where(x => x.id == id).Select(x => x.naslov).FirstOrDefault();
            }
            return Naslov;
        }

        public static int GetVijestiID(int id_komentara)
        {
            int idVijesti;
            using (ISession session = NHHelper.OpenSession())
            {
                idVijesti = session.Query<Komentar>().Where(x => x.id == id_komentara).Select(x => x.id_vijesti).FirstOrDefault();
            }
            return idVijesti;
        }

        public static void UnesiKomentar(string ime, string tekst, int? id_parent, int id_vijesti, string odgovara)
        {
            using (ISession session = NHHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    Komentar result = new Komentar() { ime=ime, tekst=tekst,id_parent=id_parent,id_vijesti=id_vijesti,datum=DateTime.Now, odgovara = odgovara };
                    session.Save(result);
                    transaction.Commit();
                }
            }
        }
    }
}

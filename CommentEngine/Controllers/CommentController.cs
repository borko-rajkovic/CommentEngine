using CommentEngine.Data.Domain;
using CommentEngine.Data.Repository;
using CommentEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommentEngine.Hubs;
using System.Net;
using CommentEngine.GooglePlusOAuth;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace CommentEngine.Controllers
{
    public class CommentController : Controller
    {
        #region Metode koje vracaju View

            public ActionResult Vijesti()
            {
                GoogleAccountCheck();

                List<Vijest> vijesti = Repository.GetAllVijesti();

                return View(vijesti);
            }

            public ActionResult Administracija()
            {
                ViewBag.KoristiKendo = 0;
                return View(adminKomentari());
            }

            public ActionResult Administracija2()
            {
                ViewBag.KoristiKendo = 1;
                return View("Administracija", adminKomentari());
            }

            public ActionResult KomentariZaVijest(int id)
            {
                List<Komentar> komentariZaVijest = Repository.GetKomentari(id);

                ViewBag.id_vijesti = id;
                ViewBag.VijestNaslov = Repository.GetNaslov(id);
                ViewBag.broj_komentara = komentariZaVijest.Count();

                return View(komentariZaVijest);
            }
            
        #endregion

        #region Metode koje vracaju Partial View
            public ActionResult KomentarById(int id)
            {
                Komentar komentar = Repository.GetKomentarById(id);

                return View(komentar);
            }

        #endregion

        #region Pomocne metode

            public List<AdminKomentar> adminKomentari()
            {
                List<Vijest> vijesti = Repository.GetAllVijesti();
                List<Komentar> komentari = Repository.GetKomentari();

                var spojeno = (from vijest in vijesti
                               join komentar in komentari
                               on vijest.id equals komentar.id_vijesti
                               select new AdminKomentar()
                               {
                                   Vijest = vijest.naslov,
                                   Ime = komentar.ime,
                                   Komentar = komentar.tekst,
                                   Odobren = komentar.odobren,
                                   Zloupotreba = komentar.zloupotreba,
                                   Username = komentar.username ?? "Neregistrovan korisnik",
                                   Likes = komentar.likes,
                                   Dislikes = komentar.dislikes,
                                   Datum = komentar.datum,
                                   Id = komentar.id
                               }).OrderByDescending(x => x.Id);

                List<AdminKomentar> adminKomentari = spojeno.ToList();
                return adminKomentari;
            }

            [HttpPost]
            public ActionResult adminKomentariDS()
            {
                return Json(adminKomentari());
            }

            public ActionResult ImaLiPromjena(int id_vijesti, int broj_komentara)
            {
                string rezultat = "false";

                if (Repository.BrojKomentaraZaVijest(id_vijesti) > broj_komentara)
                {
                    rezultat = "true";
                }

                return Content(rezultat);
            }

            public ActionResult PrijaviKomentar(int id)
            {
                string rezultat = "false";

                if (!Repository.KomentarPrijavljen(id))
                {
                    Repository.PrijaviKomentar(id);
                    rezultat = "true";
                }

                return Content(rezultat);
            }

            public ActionResult LajkKomentar(int id)
            {
                string rezultat = Repository.LajkKomentar(id).ToString();

                return Content(rezultat);
            }

            public ActionResult DislikeKomentar(int id)
            {
                string rezultat = Repository.DislikeKomentar(id).ToString();

                return Content(rezultat);
            }

            [HttpPost]
            public ActionResult PromijeniOdobrenjeKomentara(int id_komentara, bool odobri)
            {
                int brojOdobrenihKomentara;
                brojOdobrenihKomentara = Repository.PromijeniOdobrenjeKomentara(id_komentara, odobri);

                return Json(new List<int> { Repository.GetVijestiID(id_komentara), brojOdobrenihKomentara });
            }

            public ActionResult UnesiKomentar(string ime, string tekst, int id_vijesti, int? id_parent = null, string odgovara = null)
            {
                Repository.UnesiKomentar(ime, tekst, id_parent, id_vijesti, odgovara);

                return Content("true");
            }

            #region Google OAuth

                protected string googleplus_client_id = "561824198459-ee3iee4f9vgde2k1e75ouiqfang26neu.apps.googleusercontent.com";
                protected string googleplus_client_sceret = "oESh0CjhT1dG4CRZnYd-xyUW";
                protected string googleplus_redirect_url = "http://localhost/CommentEngine/Comment/Vijesti/";
                protected string Parameters;

                public void GoogleAccountCheck() {

                        if (Session.Contents.Count > 0)
                        {
                            if (Session["loginWith"] != null && Session["loginWith"].ToString() == "google")
                            {
                                try
                                {
                                    var url = Request.Url.Query;
                                    if (url != "")
                                    {
                                        string queryString = url.ToString();
                                        char[] delimiterChars = { '=' };
                                        string[] words = queryString.Split(delimiterChars);
                                        string code = words[1];

                                        if (code != null)
                                        {
                                            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("https://accounts.google.com/o/oauth2/token");
                                            webRequest.Method = "POST";
                                            Parameters = "code=" + code + "&client_id=" + googleplus_client_id + "&client_secret=" + googleplus_client_sceret + "&redirect_uri=" + googleplus_redirect_url + "&grant_type=authorization_code";
                                            byte[] byteArray = Encoding.UTF8.GetBytes(Parameters);
                                            webRequest.ContentType = "application/x-www-form-urlencoded";
                                            webRequest.ContentLength = byteArray.Length;
                                            Stream postStream = webRequest.GetRequestStream();

                                            postStream.Write(byteArray, 0, byteArray.Length);
                                            postStream.Close();

                                            WebResponse response = webRequest.GetResponse();
                                            postStream = response.GetResponseStream();
                                            StreamReader reader = new StreamReader(postStream);
                                            string responseFromServer = reader.ReadToEnd();

                                            GooglePlusAccessToken serStatus = JsonConvert.DeserializeObject<GooglePlusAccessToken>(responseFromServer);

                                            if (serStatus != null)
                                            {
                                                string accessToken = string.Empty;
                                                accessToken = serStatus.access_token;

                                                if (!string.IsNullOrEmpty(accessToken))
                                                {
                                                    try
                                                    {
                                                        WebClient client = new WebClient();
                                                        var urlProfile = "https://www.googleapis.com/oauth2/v1/userinfo?access_token=" + accessToken;
                                                        string downloadString = client.DownloadString(urlProfile);
                                                        GoogleUserOutputData userData = JsonConvert.DeserializeObject<GoogleUserOutputData>(downloadString);
                                                        Session["user"] = userData.name;
                                                    }
                                                    catch (Exception) {}
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception) {}
                            }
                        }
                    }

                public void GoogleLogIn()
                {
                    var Googleurl = "https://accounts.google.com/o/oauth2/auth?response_type=code&redirect_uri=" + googleplus_redirect_url + "&scope=https://www.googleapis.com/auth/userinfo.email%20https://www.googleapis.com/auth/userinfo.profile&client_id=" + googleplus_client_id;
                    Session["loginWith"] = "google";
                    Response.Redirect(Googleurl);
                }

            #endregion

    #endregion

    }
}
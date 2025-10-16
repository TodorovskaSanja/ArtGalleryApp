using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ArtGalleryApp.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace ArtGalleryApp.Controllers
{
    public class ArtistsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Artists
        public ActionResult Index()
        {
            var email = User.Identity.GetUserName();
            var list = new List<int>();
            if (User.IsInRole("User"))
            {
                var usr = db.Users.Where(m => m.Email == email).Include(m => m.FollowedArtists).FirstOrDefault();

                foreach (Artist a in usr.FollowedArtists)
                {
                    list.Add(a.Id);
                }
            }
            Session["arts"] = list;

            return View(db.Artists.Include(m => m.Artworks).Include(m => m.Events).ToList());
        }

        // GET: Artists/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Artist artist = db.Artists.Where(m => m.Id == id).Include(m => m.Artworks).Include(m => m.Events).FirstOrDefault();
            if (artist == null)
            {
                return HttpNotFound();
            }
            var email = User.Identity.GetUserName();
            var list = new List<int>();
            var favs = new List<int>();
            if (User.IsInRole("User"))
            {
                var usr = db.Users.Where(m => m.Email == email).Include(m => m.FollowedArtists).Include(m => m.FavoriteArtworks).FirstOrDefault();

                foreach (Artist a in usr.FollowedArtists)
                {
                    list.Add(a.Id);
                }
                foreach (Artwork a in usr.FavoriteArtworks)
                {
                    favs.Add(a.Id);
                }
            }
            Session["arts"] = list;
            Session["fav-arts"] = favs;

            foreach (var ev in artist.Events)
            {
                if (ev.Status == "Pending" && !(DateTime.Parse(ev.Date) > DateTime.Now.Date || (DateTime.Parse(ev.Date) == DateTime.Now.Date && @DateTime.Parse(ev.Time).TimeOfDay > @DateTime.Now.TimeOfDay)))
                {
                    ev.Status = "Denied";
                    ev.Comment = "We weren't holding events at that time.";
                    db.SaveChanges();
                }
            }

            return View(artist);
        }

        // GET: Artists/Create
        public ActionResult Create(String email)
        {
            Artist artist = new Artist();
            artist.Email = Session["email"].ToString();
            artist.NumFollowers = 0;
            return View(artist);
        }

        // POST: Artists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Surname,Email,Biography,ImageURL,NumFollowers")] Artist artist)
        {
            if (ModelState.IsValid)
            {
                if (artist.ImageURL == null)
                    artist.ImageURL = "~/Content/Images/Profile-PNG-File.png";
                db.Artists.Add(artist);
                db.SaveChanges();
                Session["profile-picture"] = artist.ImageURL;
                return RedirectToAction("AddUserToRole", "Account");
            }

            return View(artist);
        }

        // GET: Artists/Edit/5
        [Authorize(Roles = "Artist")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Artist artist = db.Artists.Find(id);
            if (artist == null)
            {
                return HttpNotFound();
            }
            return View(artist);
        }

        // POST: Artists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Artist")]
        public ActionResult Edit([Bind(Include = "Id,Name,Surname,Email,Biography,ImageURL,NumFollowers")] Artist artist)
        {
            if (ModelState.IsValid)
            {
                if (artist.ImageURL == null)
                    artist.ImageURL = "~/Content/Images/Profile-PNG-File.png";
                db.Entry(artist).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = artist.Id });
            }
            return View(artist);
        }

        // GET: Artists/Delete/5
        [Authorize(Roles = "Admin, Artist")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Artist artist = db.Artists.Find(id);
            if (artist == null)
            {
                return HttpNotFound();
            }
            return View(artist);
        }

        // POST: Artists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Artist")]
        public ActionResult DeleteConfirmed(int id)
        {
            var artistId = User.Identity.GetUserId();
            var identityUser = UserManager.FindById(artistId);

            if (identityUser != null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

                var domainUser = db.Artists.Include(m => m.Artworks).Include(m => m.Events).FirstOrDefault(u => u.Email == identityUser.Email);
                if (domainUser != null)
                {
                    foreach (var art in domainUser.Artworks)
                    {
                        art.ArtistId = null;
                    }

                    foreach (var ev in domainUser.Events)
                    {
                        ev.ArtistId = null;
                    }

                    db.Artists.Remove(domainUser);
                    db.SaveChanges();
                }

                UserManager.Delete(identityUser);
            }
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [Authorize(Roles = "User")]
        public ActionResult FollowArtist(int id)
        {
            var email = User.Identity.GetUserName();
            var followed = db.Artists.Find(id);
            if (User.IsInRole("User"))
            {
                var usr = db.Users.Include(m => m.FollowedArtists).Where(m => m.Email == email).First();
                usr.FollowedArtists.Add(followed);
                followed.NumFollowers += 1;
                db.SaveChanges();
                List<int> tmp = (List<int>)Session["arts"];
                tmp.Add(id);
                Session["arts"] = tmp;
            }
            return RedirectToAction("Details", new { id = id });
        }

        [Authorize(Roles = "User")]
        public ActionResult UnfollowArtist(int id)
        {
            var email = User.Identity.GetUserName();
            var followed = db.Artists.Find(id);
            if (User.IsInRole("User"))
            {
                var usr = db.Users.Include(m => m.FollowedArtists).Where(m => m.Email == email).First();
                usr.FollowedArtists.Remove(followed);
                followed.NumFollowers -= 1;
                db.SaveChanges();
                List<int> tmp = (List<int>)Session["arts"];
                tmp.Remove(id);
                Session["arts"] = tmp;
            }
            return RedirectToAction("Details", new { id = id });
        }
    }
}

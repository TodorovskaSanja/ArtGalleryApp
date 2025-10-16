using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security.AntiXss;
using ArtGalleryApp.Models;
using Microsoft.AspNet.Identity;

namespace ArtGalleryApp.Controllers
{
    public class ArtworksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Artworks
        public ActionResult Index()
        {
            var email = User.Identity.GetUserName();
            var list = new List<int>();
            var cart = new List<int>();
            if (User.IsInRole("User"))
            {
                var usr = db.Users.Where(m => m.Email == email).Include(m => m.FavoriteArtworks).Include(m => m.Cart).FirstOrDefault();

                foreach (Artwork a in usr.FavoriteArtworks)
                {
                    list.Add(a.Id);
                }
                foreach (Artwork a in usr.Cart)
                {
                    cart.Add(a.Id);
                }
            }
            Session["fav-arts"] = list;
            Session["cart"] = cart;

            var artworks = db.Artworks.Include(a => a.Artist);
            artworks = artworks.OrderBy(a => a.Name);
            return View(artworks.ToList());
        }

        // GET: Artworks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Artwork artwork = db.Artworks.Where(m => m.Id == id).Include(a => a.Artist).Include(a => a.Artist.Artworks).FirstOrDefault();
            if (artwork == null)
            {
                return HttpNotFound();
            }

            List<int> list = new List<int>();
            List<int> cart = new List<int>();
            var email = User.Identity.GetUserName();

            if (User.IsInRole("User"))
            {
                var usr = db.Users.Where(m => m.Email == email).Include(m => m.FavoriteArtworks).Include(m => m.Cart).FirstOrDefault();
                foreach (Artwork a in usr.FavoriteArtworks)
                {
                    list.Add(a.Id);
                }

                foreach (Artwork a in usr.Cart)
                {
                    cart.Add(a.Id);
                }
            }
            ViewBag.favs = list;
            ViewBag.cart = cart;

            return View(artwork);
        }

        // GET: Artworks/Create
        [Authorize(Roles = "Artist")]
        public ActionResult Create()
        {
            if (User.IsInRole("Artist"))
            {
                var email = User.Identity.GetUserName();
                Artist artist = db.Artists.Where(m => m.Email == email).FirstOrDefault();

                Artwork model = new Artwork();
                model.ArtistId = artist.Id;

                return View(model);
            }
            return HttpNotFound();
        }

        // POST: Artworks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Artist")]
        public ActionResult Create([Bind(Include = "Id,Name,ArtistId,Category,Description,Year,ImageURL,Status,Price,ArtistName")] Artwork artwork)
        {
            if (ModelState.IsValid && User.IsInRole("Artist"))
            {
                var email = User.Identity.GetUserName();
                Artist artist = db.Artists.Where(m => m.Email == email).FirstOrDefault();
                artwork.ArtistName = artist.Name + " " + artist.Surname;
                if (artwork.Status == "Not for sale")
                    artwork.Price = 0;

                db.Artworks.Add(artwork);
                artist.Artworks.Add(artwork);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = artwork.Id });
            }

            return View(artwork);
        }

        // GET: Artworks/Edit/5
        [Authorize(Roles = "Artist")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Artwork artwork = db.Artworks.Include(a => a.Artist).FirstOrDefault(a => a.Id == id);
            if (artwork == null)
            {
                return HttpNotFound();
            }

            return View(artwork);
        }

        // POST: Artworks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Artist")]
        public ActionResult Edit([Bind(Include = "Id,Name,ArtistId,Category,Description,Year,ImageURL,Status,Price,ArtistName")] Artwork artwork)
        {
            if (ModelState.IsValid)
            {
                if (artwork.Status == "Not for sale")
                    artwork.Price = 0;
                db.Entry(artwork).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = artwork.Id });
            }

            return View(artwork);
        }

        // GET: Artworks/Delete/5
        [Authorize(Roles = "Admin, Artist")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Artwork artwork = db.Artworks.Find(id);
            if (artwork == null)
            {
                return HttpNotFound();
            }
            return View(artwork);
        }

        // POST: Artworks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Artist")]
        public ActionResult DeleteConfirmed(int id)
        {
            Artwork artwork = db.Artworks.Find(id);
            db.Artworks.Remove(artwork);
            db.SaveChanges();
            return RedirectToAction("Index");
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
        public ActionResult Favorite(int id)
        {
            var email = User.Identity.GetUserName();
            var fav = db.Artworks.Find(id);
            if (User.IsInRole("User"))
            {
                var usr = db.Users.Include(u => u.FavoriteArtworks).FirstOrDefault(m => m.Email == email);
                usr.FavoriteArtworks.Add(fav);
                db.SaveChanges();
                List<int> tmp = (List<int>)Session["fav-arts"];
                tmp.Add(id);
                Session["fav-arts"] = tmp;
            }
            return RedirectToAction("Details", new { id = id });
        }

        [Authorize(Roles = "User")]
        public ActionResult Unfavorite(int id)
        {
            var email = User.Identity.GetUserName();
            var unfav = db.Artworks.Find(id);
            if (User.IsInRole("User"))
            {
                var usr = db.Users.Include(u => u.FavoriteArtworks).FirstOrDefault(m => m.Email == email);
                usr.FavoriteArtworks.Remove(unfav);
                db.SaveChanges();
                List<int> tmp = (List<int>)Session["fav-arts"];
                if (tmp != null)
                    tmp.Remove(id);
                Session["fav-arts"] = tmp;
            }
            return RedirectToAction("Details", new { id = id });
        }

        [Authorize(Roles = "User")]
        public ActionResult AddToCart(int id)
        {
            var email = User.Identity.GetUserName();
            var art = db.Artworks.Find(id);
            if (User.IsInRole("User"))
            {
                var usr = db.Users.Include(u => u.Cart).FirstOrDefault(m => m.Email == email);
                usr.Cart.Add(art);
                db.SaveChanges();
                List<int> tmp = (List<int>)Session["cart"];
                tmp.Add(id);
                Session["cart"] = tmp;
            }
            return RedirectToAction("Details", new { id = id });
        }

        [Authorize(Roles = "User")]
        public ActionResult RemoveFromCart(int id)
        {
            var email = User.Identity.GetUserName();
            var art = db.Artworks.Find(id);
            if (User.IsInRole("User"))
            {
                var usr = db.Users.Include(u => u.Cart).FirstOrDefault(m => m.Email == email);
                usr.Cart.Remove(art);
                db.SaveChanges();
                List<int> tmp = (List<int>)Session["cart"];
                if (tmp != null)
                    tmp.Remove(id);
                Session["cart"] = tmp;
            }
            return RedirectToAction("Details", new { id = id });
        }

        public ActionResult Filter(String category, String status, String searchString)
        {
            List<Artwork> list = null;
            if (String.IsNullOrEmpty(category))
                category = "All";

            if (String.IsNullOrEmpty(status))
                status = "All";
            if (category == "All")
                list = db.Artworks.Include(m => m.Artist).OrderBy(m => m.Name).ToList();
            else
                list = db.Artworks.Include(m => m.Artist).Where(m => m.Category == category).OrderBy(m => m.Name).ToList();

            ViewBag.searchCategory = category;
            ViewBag.searchStatus = status;

            status = status.Replace("-", " ");
            if (status != "All")
                list = list.Where(m => m.Status == status).ToList();

            if (!String.IsNullOrEmpty(searchString))
            {
                list = list.Where(m => m.Name.ToLower().Contains(searchString.ToLower()) || (m.ArtistName).ToLower().Contains(searchString.ToLower())).ToList();
                ViewBag.searchStr = searchString;
            }

            var email = User.Identity.GetUserName();
            var tmp = new List<int>();
            if (User.IsInRole("User"))
            {
                var usr = db.Users.Where(m => m.Email == email).Include(m => m.FavoriteArtworks).Include(m => m.Cart).FirstOrDefault();

                foreach (Artwork a in usr.FavoriteArtworks)
                {
                    tmp.Add(a.Id);
                }
            }
            Session["fav-arts"] = tmp;

            return View(list);
        }

        [Authorize(Roles = "Artist")]
        public ActionResult SellArtwork(int id, int price)
        {
            var model = db.Artworks.Find(id);
            model.Status = "For sale";
            model.Price = price;
            db.SaveChanges();
            return RedirectToAction("Details", new { id = id });
        }
    }
}

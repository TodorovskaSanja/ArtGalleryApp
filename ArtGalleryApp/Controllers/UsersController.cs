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
    public class UsersController : Controller
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

        // GET: Users
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Include(m => m.FollowedArtists.Select(a => a.Artworks)).Include(m => m.FavoriteArtworks).Include(m => m.Cart).Include(m => m.Tickets.Select(t => t.Event)).FirstOrDefault(m => m.Id == id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            User user = new User();
            user.Email = Session["email"].ToString();
            return View(user);
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Surname,Email,ImageURL")] User user)
        {
            if (ModelState.IsValid)
            {
                if (user.ImageURL == null)
                    user.ImageURL = "~/Content/Images/Profile-PNG-File.png";
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("AddUserToRole", "Account");
            }

            return View(user);
        }

        // GET: Users/Edit/5
        [Authorize(Roles = "User")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]
        public ActionResult Edit([Bind(Include = "Id,Name,Surname,Email,ImageURL")] User user)
        {
            if (ModelState.IsValid)
            {
                if (user.ImageURL == null)
                    user.ImageURL = "~/Content/Images/Profile-PNG-File.png";
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = user.Id });
            }
            return View(user);
        }

        // GET: Users/Delete/5
        [Authorize(Roles = "Admin, User")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, User")]
        public ActionResult DeleteConfirmed(int id)
        {
            var userId = User.Identity.GetUserId();
            var identityUser = UserManager.FindById(userId);

            if (identityUser != null)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

                var domainUser = db.Users.Include(m => m.FollowedArtists).Include(m => m.Orders).Include(m => m.Tickets).Include(m => m.Cart).Include(m => m.FavoriteArtworks).Include(m => m.PurchasedArtworks).FirstOrDefault(u => u.Email == identityUser.Email);
                if (domainUser != null)
                {
                    foreach (var art in domainUser.FollowedArtists)
                    {
                        art.NumFollowers = art.NumFollowers - 1;
                    }

                    foreach (var ord in domainUser.Orders)
                    {
                        ord.UserId = null;
                    }

                    foreach (var t in domainUser.Tickets)
                    {
                        t.UserId = null;
                    }

                    domainUser.Orders.Clear();
                    domainUser.Cart.Clear();
                    domainUser.FavoriteArtworks.Clear();
                    domainUser.PurchasedArtworks.Clear();
                    domainUser.Tickets.Clear();
                    domainUser.FollowedArtists.Clear();

                    db.Users.Remove(domainUser);
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
        public ActionResult Cart(int id)
        {
            User model = db.Users.Include(m => m.Cart.Select(c => c.Artist)).FirstOrDefault(m => m.Id == id);
            return View(model);
        }

        [Authorize(Roles = "User")]
        public ActionResult Favorites(int id)
        {
            User model = db.Users.Include(m => m.FavoriteArtworks.Select(c => c.Artist)).FirstOrDefault(m => m.Id == id);
            return View(model);
        }

        [Authorize(Roles = "User")]
        public ActionResult Checkout(int id)
        {
            User usr = db.Users.Include(m => m.Cart).FirstOrDefault(m => m.Id == id);
            int subtotal = 0;
            foreach (var item in usr.Cart)
            {
                subtotal += (int)item.Price;
            }
            ViewBag.subtotal = subtotal;
            ViewBag.usr = usr;

            Order model = new Order();
            model.UserId = usr.Id;
            model.Name = usr.Name;
            model.Surname = usr.Surname;
            model.Email = usr.Email;
            model.ShippingPrice = 0;
            model.Total = subtotal;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]
        public ActionResult Checkout([Bind(Include = "Id,UserId,Name,Surname,Email,PhoneNumber,Address,City,Country,ShippingMethod,ShippingPrice,Total,WhenOrdered")] Order order)
        {
            if (ModelState.IsValid)
            {
                var usr = db.Users.Include(u => u.Cart).FirstOrDefault(u => u.Id == order.UserId);
                order.WhenOrdered = DateTime.Now;
                foreach (var artwork in usr.Cart.ToList())
                {
                    order.Artworks.Add(artwork);
                    artwork.Status = "Purchased";
                    usr.PurchasedArtworks.Add(artwork);

                    order.OrderArtworks.Add(new OrderArtwork
                    {
                        ArtworkName = artwork.Name,
                        ArtworkArtistName = artwork.Artist != null ? artwork.Artist.Name + " " + artwork.Artist.Surname : "Unknown Artist",
                        ArtworkImageURL = artwork.ImageURL,
                        ArtworkPrice = artwork.Price
                    });
                }

                db.Orders.Add(order);
                usr.Cart.Clear();
                db.SaveChanges();

                TempData["alertMessage"] = "Thank you. Your order has been received and will be processed soon.";

                return Redirect("/Orders/Details/" + order.Id);
            }

            return View(order);
        }
    }
}

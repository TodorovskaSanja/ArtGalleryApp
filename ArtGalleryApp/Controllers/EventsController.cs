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

namespace ArtGalleryApp.Controllers
{
    public class EventsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Events
        public ActionResult Index()
        {
            var events = db.Events.Include(@e => @e.Artist);
            events = events.OrderBy(e => e.Time);
            events = events.OrderBy(e => e.Date);

            ViewBag.date = DateTime.Now.ToString("yyyy/MM/dd");
            ViewBag.time = DateTime.Now.ToString("HH:mm");

            return View(events.ToList());
        }

        // GET: Events/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Include(m => m.Artist).FirstOrDefault(m => m.Id == id);
            if (@event == null)
            {
                return HttpNotFound();
            }

            if (@event.Status == "Pending" && !(DateTime.Parse(@event.Date) > DateTime.Now.Date || (DateTime.Parse(@event.Date) == DateTime.Now.Date && @DateTime.Parse(@event.Time).TimeOfDay > @DateTime.Now.TimeOfDay)))
            {
                @event.Status = "Denied";
                @event.Comment = "We weren't holding events at that time.";
                db.SaveChanges();
            }

            return View(@event);
        }

        // GET: Events/Create
        [Authorize(Roles = "Artist")]
        public ActionResult Create()
        {
            if (User.IsInRole("Artist"))
            {
                var email = User.Identity.GetUserName();
                Artist artist = db.Artists.Where(m => m.Email == email).FirstOrDefault();

                Event model = new Event();
                model.ArtistId = artist.Id;
                model.Status = "Pending";

                //ViewBag.ArtistId = new SelectList(db.Artists, "Id", "Name");
                return View(model);
            }
            return HttpNotFound();
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Artist")]
        public ActionResult Create([Bind(Include = "Id,Name,Description,Date,Time,Duration,ImageURL,ArtistId,Status,TicketPrice,Comment,ArtistName")] Event @event)
        {
            if (ModelState.IsValid && User.IsInRole("Artist"))
            {
                var email = User.Identity.GetUserName();
                Artist artist = db.Artists.Where(m => m.Email == email).FirstOrDefault();
                @event.ArtistName = artist.Name + " " + artist.Surname;

                db.Events.Add(@event);
                artist.Events.Add(@event);
                db.SaveChanges();
                TempData["alertMessage"] = "Your event has been successfully sent for review. You can view its status on your profile.";
                return RedirectToAction("Details", new { id = @event.Id });
            }

            return View(@event);
        }

        // GET: Events/Edit/5
        [Authorize(Roles = "Artist")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }

            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Artist")]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,Date,Time,Duration,ImageURL,ArtistId,Status,TicketPrice,Comment,ArtistName")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Entry(@event).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = @event.Id });
            }

            return View(@event);
        }

        // GET: Events/Delete/5
        [Authorize(Roles = "Admin, Artist")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Artist")]
        public ActionResult DeleteConfirmed(int id)
        {
            Event @event = db.Events.Find(id);
            db.Events.Remove(@event);
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

        [Authorize(Roles = "Admin")]
        public ActionResult ApproveEvent(int id)
        {
            Event ev = db.Events.Find(id);
            ev.Status = "Approved";
            db.SaveChanges();
            return RedirectToAction("Details", new { id = id });
        }

        [Authorize(Roles = "Admin")]
        public ActionResult DenyEvent(int id, string comment)
        {
            Event ev = db.Events.Find(id);
            ev.Status = "Denied";
            ev.Comment = comment;
            db.SaveChanges();
            return RedirectToAction("Details", new { id = id });
        }
    }
}

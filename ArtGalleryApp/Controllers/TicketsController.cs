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
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tickets
        public ActionResult Index(int? id)
        {
            var tickets = db.Tickets.Include(t => t.Event).Include(t => t.User);
            if (id != null)
            {
                tickets = tickets.Where(m => m.UserId == id);
                ViewBag.usrId = id;
            }
            return View(tickets.ToList());
        }

        // GET: Tickets/Details/5
        [Authorize(Roles = "Admin, User")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Include(m => m.Event).Include(m => m.User).FirstOrDefault(m => m.Id == id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // GET: Tickets/Create
        [Authorize(Roles = "User")]
        public ActionResult Create(int id)
        {
            if (User.IsInRole("User"))
            {
                Ticket model = new Ticket();
                Event ev = db.Events.Find(id);
                model.EventId = ev.Id;
                model.Event = ev;
                var email = User.Identity.GetUserName();
                User usr = db.Users.FirstOrDefault(m => m.Email == email);
                model.NumTickets = 1;
                model.Price = ev.TicketPrice;
                model.UserId = usr.Id;
                model.User = usr;

                return View(model);
            }
            return HttpNotFound();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]
        public ActionResult Create([Bind(Include = "Id,EventId,UserId,NumTickets,Price,WhenBought")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                ticket.WhenBought = DateTime.Now;
                db.Tickets.Add(ticket);
                db.SaveChanges();

                TempData["alertMessage"] = "Your tickets have been successfully reserved.";

                return RedirectToAction("Details", new { id = ticket.Id});
            }

            ViewBag.EventId = new SelectList(db.Events, "Id", "Name", ticket.EventId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "Name", ticket.UserId);

            return View(ticket);
        }

        // GET: Tickets/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "Id,EventId,UserId,NumTickets,Price,WhenBought")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = ticket.Id });
            }
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
            db.SaveChanges();
            if (User.IsInRole("User"))
            {
                var email = User.Identity.GetUserName();
                var usrId = db.Users.FirstOrDefault(m => m.Email == email).Id;
                return RedirectToAction("Index", new { id = usrId });
            }
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
    }
}

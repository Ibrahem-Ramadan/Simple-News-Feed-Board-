using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_Day3_Lab.Models;
using PagedList;

namespace MVC_Day3_Lab.Controllers
{
    public class NewsController : Controller
    {
        // GET: News
        MVCLAB3Context db = new MVCLAB3Context();
        public ActionResult Index(int? pageNo , string sortAttribute , string search)
        {
            
            var tmp = int.Parse(Session["userId"].ToString());
            int page = pageNo == null ? 1 : (int)pageNo;
            var list = db.News.Where(n => n.UserId == tmp).OrderBy(n=>n.NewId);
            
            if (!String.IsNullOrEmpty(search))
            {

                list = list.Where(n => (n.NewTitle.Contains(search)==true) ||( n.NewBreif.Contains(search)==true)).OrderBy(n => n.UserId);
                List<New> ss = list.ToList();
            }

            switch (sortAttribute)
            {
                case "catName":
                    list = list.OrderBy(n => n.Catalog.CatName);
                    break;
                case "time":
                    list = list.OrderBy(n => n.DateTime);
                    break;
                default: 
                    break;
            }

            ViewBag.sort = sortAttribute;
            ViewBag.search = search;
            ViewBag.pageNo = page;
            ViewBag.cat = new SelectList(db.Catalogs.ToList(), "CatId", "CatName");
            
            return View(list.ToPagedList(page, 2));
            
        }

        public ActionResult Create()
        {
            ViewBag.cat = new SelectList(db.Catalogs.ToList(), "CatId", "CatName");
            return View();
        }

        [HttpPost]
        public ActionResult Create(New nw , HttpPostedFileBase NewPhoto)
        {
            if (ModelState.IsValid)
            {
                if (NewPhoto != null)
                {
                    NewPhoto.SaveAs(Server.MapPath($"~/Attachments/{NewPhoto.FileName}"));
                    nw.NewPhoto = NewPhoto.FileName;
                }

                nw.UserId = int.Parse(Session["userId"].ToString());

                if(nw.DateTime ==null)
                    nw.DateTime = DateTime.Now;

                db.News.Add(nw);
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            else
            {
                ViewBag.cat = new SelectList(db.Catalogs.ToList(), "CatId", "CatName");
                return View();
            }
          
        }

        public ActionResult Edit(int id)
        {
            New nw =  db.News.Find(id);
            ViewBag.cat = new SelectList(db.Catalogs.ToList(), "CatId", "CatName");
            return View(nw);
        }

        [HttpPost]
        public ActionResult Edit(New nw, HttpPostedFileBase NewPhoto)
        {
            New oldNw = db.News.Find(nw.NewId);
            oldNw.NewTitle = nw.NewTitle;
            oldNw.NewBreif = nw.NewBreif;
            oldNw.NewDesc = nw.NewDesc;
            oldNw.CatId = nw.CatId;

            if(nw.DateTime == null)
            {
                if(oldNw.DateTime == null)
                {
                    oldNw.DateTime = DateTime.Now;
                }
            }
            else
            {
                oldNw.DateTime = nw.DateTime;
            }

            if(NewPhoto != null)
            {
                NewPhoto.SaveAs(Server.MapPath($"~/Attachments/{NewPhoto.FileName}"));
                oldNw.NewPhoto = NewPhoto.FileName;
            }

            db.SaveChanges();
            ViewBag.cat = new SelectList(db.Catalogs.ToList(), "CatId", "CatName");
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int id)
        {
            db.News.Remove(db.News.Find(id));
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult NewsByCatalog(int? catId,int? pageNo)
        {
            int page = pageNo == null ? 1 : (int)pageNo;
            var tmp = int.Parse(Session["userId"].ToString());
            List<New> list;
            if (catId != null)
                list = db.News.Where(n => n.CatId == catId && n.UserId == tmp).ToList();
            else
                list = db.News.Where(n=> n.UserId == tmp).ToList();
            return PartialView(list.ToPagedList(page, 2));
        }

        public ActionResult Details(int id)
        {
            ViewBag.nw = db.News.Find(id);
            return PartialView();
        }
    }
}
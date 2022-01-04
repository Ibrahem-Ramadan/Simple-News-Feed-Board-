using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_Day3_Lab.Models;
using PagedList;

namespace MVC_Day3_Lab.Controllers
{
    public class UserController : Controller
    {
        // GET: User
         MVCLAB3Context db = new MVCLAB3Context();

        public ActionResult Home()
        {
            return View();
        }

        public ActionResult Register()
        {
            User user = new User();
            user.Skills = db.Skills.ToList();

            return View(user) ;
        }

        [HttpPost]  
        public ActionResult Register(User user ,HttpPostedFileBase CV, HttpPostedFileBase Photo )
        {
            if (ModelState.IsValid)
            {
                if(Photo != null)
                {
                    Photo.SaveAs(Server.MapPath($"~/Attachments/{Photo.FileName}"));
                    user.Photo = Photo.FileName;

                }
                if (CV != null)
                {
                    CV.SaveAs(Server.MapPath($"~/Attachments/{CV.FileName}"));
                    user.CV = CV.FileName;

                }

                List<Skill> skills = user.Skills.Where(s => s.IsSelected).ToList(); 
                user.Skills.Clear();
                db.Users.Add(user);
                foreach (Skill skill in skills)
                {
                    db.Skills.Attach(skill);
                    user.Skills.Add(skill);
                }
                db.SaveChanges();
            }
            return RedirectToAction("Login");
        }

        public ActionResult Login()
        {
            if(Request.Cookies["MVCLab4"] != null)
            {
                Session.Add("userId", Request.Cookies["MVCLab4"].Values["userId"]);
                return RedirectToAction("Profile");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(User user, bool remember)
        {
            User u = db.Users.Where(n=>n.Email == user.Email && n.Password == user.Password).FirstOrDefault();
            if (u != null)
            {
                if (remember)
                {
                    HttpCookie cookie = new HttpCookie("MVCLab4");
                    cookie.Values.Add("userId",u.UserId.ToString());
                    cookie.Values.Add("username",u.Username);
                    cookie.Expires = DateTime.Now.AddDays(15);
                    Response.Cookies.Add(cookie);
                }
                Session.Add("userId", u.UserId);
                return RedirectToAction("Profile");
            }
            else
            {
                ViewBag.error = "Invalid Email or Password";
                return View();
            }
        }

        public ActionResult Logout()
        {
            Session["userId"] = null;
            HttpCookie cookie = new HttpCookie("MVCLab4");
            cookie.Expires= DateTime.Now.AddDays(-1);
            Response.Cookies.Add(cookie);

            return RedirectToAction("Login");
        }

        public ActionResult ChangePassword()
        {
            int id = int.Parse(Session["userId"].ToString());
            return View(db.Users.Find(id));
        }

        [HttpPost]
        public ActionResult ChangePassword(User user , string oldPass)
        {
            User olduser = db.Users.Where(u => u.Password == oldPass && u.UserId == user.UserId).SingleOrDefault();
            if (olduser != null)
            {
                olduser.Password = user.Password;
                olduser.ConfPassword = user.ConfPassword;

                db.SaveChanges();
                return RedirectToAction("Profile");
            }
            else
            {
                ViewBag.error = "Invalid Password";
                return View();
            }
        }
        public ActionResult Profile()
        { 
            int id = int.Parse(Session["userId"].ToString());
            User user = db.Users.Find(id);
            return View(user);
        }
        
        public ActionResult allUsers(int? pageNo ,string sortAttribute , string Search )
        {
            int n = pageNo == null ? 1 : pageNo.Value;
            var users = db.Users.OrderBy(u => u.UserId);
             
            if (!String.IsNullOrEmpty(Search))
            {
                users = db.Users.Where(u=>u.Username.Contains(Search)).OrderBy(u=>u.UserId);
            }
           
            switch (sortAttribute)
            {
                case "username":
                    users = users.OrderBy(u => u.Username);
                    break;
                case "email":
                    users = users.OrderBy(u => u.Email);
                    break;
                case "age":
                    users = users.OrderBy(u => u.Age);
                    break;
                default :
                    break;
            }
            ViewBag.search = Search;
            ViewBag.sort = sortAttribute;
            return View(users.ToPagedList(n,3));
        }

        public ActionResult Edit(int id)
        {
            User user = db.Users.Find(id);
            List<Skill> userSkills = db.Users.Find(id).Skills.ToList();
            userSkills.ForEach(skill => skill.IsSelected = true);
            foreach (var skill in userSkills)
            {
                db.Skills.Find(skill.SkillId).IsSelected = true ;
            }
            user.Skills = db.Skills.ToList();
            return View(user);
        }

        [HttpPost]
        public ActionResult Edit(User user,HttpPostedFileBase Photo , HttpPostedFileBase CV)
        {
            User oldUser = db.Users.Find(user.UserId);
            oldUser.Username = user.Username;
            oldUser.Email = user.Email;
            oldUser.Age = user.Age;
            oldUser.ConfPassword = user.ConfPassword = user.Password;

            if(Photo != null)
            {
                Photo.SaveAs(Server.MapPath($"~/Attachments/{Photo.FileName}"));
                oldUser.Photo = Photo.FileName;
            }
            if (CV != null)
            {
                CV.SaveAs(Server.MapPath($"~/Attachments/{CV.FileName}"));
                oldUser.CV = CV.FileName;
            }
       
            //delete old skills
            for(int i= oldUser.Skills.Count-1; i >= 0;i--)
            {
                oldUser.Skills.Remove(oldUser.Skills[i]);
            }
            db.SaveChanges();


            List<Skill> newSkills = user.Skills.Where(s => s.IsSelected).ToList();
            //insert olduser to new skills
            for (int i = 0; i < newSkills.Count; i++)
            {
                var tmp = newSkills[i].SkillId;
                Skill userskill = db.Skills.Where(s => s.SkillId == tmp ).FirstOrDefault();
                userskill.Users.Add(oldUser);
                db.Users.Attach(oldUser);
            }
            db.SaveChanges();
            return RedirectToAction("Profile");
        }

        
        public ActionResult Delete(int id)
        {
            db.Users.Remove(db.Users.Find(id));
            db.SaveChanges();
            return RedirectToAction("allUsers");
        }

        public ActionResult emailCheck(string Email)
        {
            User user = db.Users.Where(u => u.Email == Email).FirstOrDefault();
            //email not excist (validation not fired)
            if (user != null)
                return Json(false,JsonRequestBehavior.AllowGet);
            //email excist (validation fired)
            else
                return Json(true,JsonRequestBehavior.AllowGet); 
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
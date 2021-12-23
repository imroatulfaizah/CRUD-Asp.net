using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using TheStartUpBuddy.Models;

namespace TheStartUpBuddy.Controllers
{
    public class DefaultController : Controller
    {
        
        DB_BuddyEntities context = new DB_BuddyEntities();

        public ActionResult Index()
        {
            return View();
        }

        #region get data select
        private List<Position> getPositions()
        {
            var list = (from x in context.MST_POSITION
                        select
                          new Position()
                          {
                              ID = x.ID,
                              PositionName = x.Position
                          }).Distinct().ToList();

            return list;
        }
        #endregion

        #region
        public ActionResult Create()
        {
            ViewBag.Position = getPositions();
            return View();
        }
        #endregion

        #region
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Users _usr, HttpPostedFileBase file, string Position, string Female, string Male)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    #region get data form

                    MST_USERS userdata_ = new MST_USERS();
                    userdata_.Name = _usr.Name;
                    userdata_.Email = _usr.Email;
                    userdata_.Gender = _usr.Gender;
                    userdata_.Age = _usr.Age;
                    userdata_.Address = _usr.Address;
                    userdata_.Position = Position;

                    if (file != null)
                    {
                        if (file.ContentLength > 0)
                        {
                            var fileName = Path.GetFileName(file.FileName);
                            var path = Path.Combine(Server.MapPath("~/App_Data/File"), fileName);
                            file.SaveAs(path);
                            userdata_.Profile_Photo = fileName;
                        }
                        else
                        {
                            userdata_.Profile_Photo = _usr.Profile_Photo;
                        }
                    }
                    context.MST_USERS.Add(userdata_);
                    context.SaveChanges();

                    #endregion

                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {

                ModelState.AddModelError("", "Unable to save changes");
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region load data
        public ActionResult LoadData()
        {
            try
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

                //Paging Size (10,20,50,100)    
               int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                // Getting all User data    
                var _Users = (from _user in context.MST_USERS
                              join _pst in context.MST_POSITION on _user.Position equals _pst.ID.ToString()
                              select
                           new Users()
                           {
                               Admin_ID = _user.Admin_ID,
                               Name = _user.Name,
                               Email = _user.Email,
                               Profile_Photo = _user.Profile_Photo,
                               Gender = _user.Gender,
                               Age = _user.Age,
                               Address = _user.Address,
                               Position = _pst.Position,
                           });
                //Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    _Users = _Users.OrderBy(sortColumn + " " + sortColumnDir);
                }
                //Search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    _Users = _Users.Where(m => m.Name.Contains(searchValue) || m.Email.Contains(searchValue) || m.Profile_Photo.Contains(searchValue)
                    || m.Gender.Contains(searchValue) || m.Age.Contains(searchValue) || m.Address.Contains(searchValue) || m.Position.Contains(searchValue));
                }   
                //total number of rows count     
                recordsTotal = _Users.Count();
                var data = _Users.Skip(skip).Take(pageSize).ToList(); 
  
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });

            }
            catch (Exception)
            {
                throw;
            }

        }

        #endregion

        #region edit
        [HttpGet]
        public ActionResult Edit(int? ID)
        {
            ViewBag.id = ID;
            ViewBag.Position = getPositions();

            try
            {
                var _User = new MST_USERS();
                _User = (context.MST_USERS).Where(a => a.Admin_ID == ID).FirstOrDefault();

                ViewBag.SelectedPosition = _User.Position;

                return View(_User);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region update
        [HttpPost]
        public ActionResult Update(Users u, int? id, HttpPostedFileBase file, string Position)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    #region Ambil data form

                    var user = new MST_USERS();
                    user = (context.MST_USERS).Where(a => a.Admin_ID == id).FirstOrDefault();
                    user.Name = u.Name;
                    user.Email = u.Email;
                    if (file != null)
                    {
                        if (file.ContentLength > 0)
                        {
                            var fileName = Path.GetFileName(file.FileName);
                            var path = Path.Combine(Server.MapPath("~/App_Data/File"), fileName);
                            file.SaveAs(path);
                            user.Profile_Photo = fileName;
                        }
                        else
                        {
                            user.Profile_Photo = u.Profile_Photo;
                        }
                    }
                   
                    user.Gender = u.Gender;
                    user.Age = u.Age;
                    user.Address = u.Address;
                    user.Position = Position;

                    context.SaveChanges();

                    #endregion

                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {

                ModelState.AddModelError("", "Unable to save changes.");
            }
            return View();
        }

        #endregion

        #region delete
        [HttpPost]
        public JsonResult Delete(int? ID)
        {
            var customer = context.MST_USERS.Find(ID);
            if (ID == null)
                return Json(data: "Not Deleted", behavior: JsonRequestBehavior.AllowGet);
            context.MST_USERS.Remove(customer);
            context.SaveChanges();

            return Json(data: "Deleted", behavior: JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}


﻿using Crud_app_with_file_upload.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Crud_app_with_file_upload.Controllers
{
    public class HomeController : Controller
    {
        CrudAppDBEntities db = new CrudAppDBEntities();
        // GET: Home
        public ActionResult Index()
        {
            var data = db.Employees.ToList();
            return View(data);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Employee e)
        {
            if(ModelState.IsValid == true)
            {
                string fileName = Path.GetFileNameWithoutExtension(e.ImageFile.FileName);
                string extension = Path.GetExtension(e.ImageFile.FileName);

                HttpPostedFileBase postedFile = e.ImageFile;
                int length = postedFile.ContentLength;

                if(extension.ToLower() == ".jpg" || extension.ToLower() == ".png")
                {
                    if(length < 1000000)
                    {
                        fileName = fileName + extension;
                        e.Img_path = "~/images/" + fileName;
                        fileName = Path.Combine(Server.MapPath("~/images/"), fileName);
                        e.ImageFile.SaveAs(fileName);
                        db.Employees.Add(e);
                        int res = db.SaveChanges();

                        if(res > 0)
                        {
                            TempData["CreateMessage"] = "<p class=\"alert alert-success text-bold\">Data was inserted</p>";
                            ModelState.Clear();
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            TempData["SizeMessage"] = "<p class=\"alert alert-danger text=bold\">The uploaded file should be smaller than 1MB</p>";
                        }
                    }
                    else
                    {
                        TempData["ExtensionMessage"] = "<p class=\"alert alert-danger\">The uploaded file should be smaller than 1MB</p>";
                    }
                }
                else
                {
                    TempData["ExtensionMessage"] = "<p class=\"alert alert-danger\">The uploaded file format is not supported</p>";
                }
            }
            return View();
        }

        public ActionResult Edit(int id)
        {
            var Emp = db.Employees.Where(model => model.Id == id).FirstOrDefault();
            Session["Image"] = Emp.Img_path;
            return View(Emp);
        }

        [HttpPost]
        public ActionResult Edit(Employee e)
        {
            if(ModelState.IsValid == true)
            {
                if(e.ImageFile != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(e.ImageFile.FileName);
                    string extension = Path.GetExtension(e.ImageFile.FileName);

                    HttpPostedFileBase postedFile = e.ImageFile;
                    int length = postedFile.ContentLength;

                    if (extension.ToLower() == ".jpg" || extension.ToLower() == ".png")
                    {
                        if (length < 1000000)
                        {
                            fileName = fileName + extension;
                            e.Img_path = "~/images/" + fileName;
                            fileName = Path.Combine(Server.MapPath("~/images/"), fileName);
                            e.ImageFile.SaveAs(fileName);
                            db.Entry(e).State = EntityState.Modified;
                            int res = db.SaveChanges();

                            if (res > 0)
                            {
                                TempData["UpdateMessage"] = "<p class=\"alert alert-success text-bold\">Data was updated</p>";
                                ModelState.Clear();
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                TempData["UpdateMessage"] = "<p class=\"alert alert-danger text=bold\">Data was not updated</p>";
                            }
                        }
                        else
                        {
                            TempData["SizeMessage"] = "<p class=\"alert alert-danger\">The uploaded file should be smaller than 1MB</p>";
                        }
                    }
                    else
                    {
                        TempData["ExtensionMessage"] = "<p class=\"alert alert-danger\">The uploaded file format is not supported</p>";
                    }
                }
                else
                {
                    e.Img_path = Session["Image"].ToString();
                    db.Entry(e).State = EntityState.Modified;
                    int res = db.SaveChanges();

                    if (res > 0)
                    {
                        TempData["UpdateMessage"] = "<p class=\"alert alert-success text-bold\">Data was updated</p>";
                        ModelState.Clear();
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["UpdateMessage"] = "<p class=\"alert alert-danger text=bold\">Data was not updated</p>";
                    }
                }
            }
            return View();
        }

        public ActionResult Delete(int id)
        {
            if(id > 0)
            {
                var Emp = db.Employees.Where(model => model.Id == id).FirstOrDefault();
                if(Emp != null)
                {
                    db.Entry(Emp).State = EntityState.Deleted;
                    int res = db.SaveChanges();

                    if(res > 0)
                    {
                        TempData["DeletedMessage"] = "<p class=\"alert alert-danger text-bold\">Data deleted successfully</p>";
                    }
                    else
                    {
                        TempData["DeletedMessage"] = "<p class=\"alert alert-warning text-bold\">Data deleted successfully</p>";
                    }
                }
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
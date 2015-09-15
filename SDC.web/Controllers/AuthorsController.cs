﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using SDC.web.ViewModels;
using SDC.data;
using SDC.data.Entity;
using SDC.data.Entity.Profile;

namespace SDC.web.Controllers
{
    public class AuthorsController : Controller
    {
        // GET: Authors
        /// <summary>
        /// will display a list with all authors.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            UserProfile profile = ((UserProfile)Session["ProfileInfo"]);
            if (profile == null || profile.Role == RolesCustom.USER)
            {
                ViewBag.CanEdit = false;
            }
            else
            {
                ViewBag.CanEdit = true;
            }

            return View();
        }

        /// <summary>
        /// shows details of an author
        /// </summary>
        /// <param name="id">id of author</param>
        /// <returns>view</returns>
        [HttpGet]
        public ActionResult ViewAuthor(int id)
        {
            using (var db = new SDCContext())
            {
                var author = db.Authors
                    .Include(a => a.AddedBy)
                    .Include(a => a.LastModifiedBy)
                    .Include(a => a.Books)
                    .First(a => a.Id == id);

                var model = AutoMapper.Mapper.Map<AuthorViewModel>(author);
                return View(model);
            }
        }
    }
}
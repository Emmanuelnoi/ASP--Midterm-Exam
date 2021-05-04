using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList.Models;

namespace ToDoList.Controllers
{
    public class HomeController : Controller
    {
        private ToDoContext context;
        public HomeController(ToDoContext ctx) => context = ctx;

        public IActionResult Index(string id, string sortOrder)
        {

            var filters = new Filters(id);
            ViewBag.Filters = filters;
            ViewBag.Categories = context.Categories.ToList();
            ViewBag.Statuses = context.Statuses.ToList();
            ViewBag.DueFilters = Filters.DueFilterValues;


            ViewBag.DescriptionSort = String.IsNullOrEmpty(sortOrder) ? "task_desc" : "";
            ViewBag.CategorySort = String.IsNullOrEmpty(sortOrder) ? "task_categ" : "";
            ViewBag.DueDateSort = String.IsNullOrEmpty(sortOrder) ? "task_date" : "";
            ViewBag.StatusSort = String.IsNullOrEmpty(sortOrder) ? "task_status" : "";


            var tasks = from td in context.ToDos select td;

            switch (sortOrder)
            {
                case "task_desc":
                    tasks = tasks.OrderByDescending(td => td.Description);

                    break;
                case "task_categ":
                    tasks = tasks.OrderByDescending(td => td.Category);

                    break;
                case "task_date":
                    tasks = tasks.OrderByDescending(td => td.DueDate);

                    break;
                case "task_status":
                    tasks = tasks.OrderByDescending(td => td.Status);

                    break;
                default:
                    tasks = tasks.OrderBy(td => td.Id);
                    break;

            }

            return View(tasks);
        }

        //public IActionResult Index(string id)
        //{
        //    // load current filters and data needed for filter drop downs in ViewBag
        //    var filters = new Filters(id);
        //    ViewBag.Filters = filters;
        //    ViewBag.Categories = context.Categories.ToList();
        //    ViewBag.Statuses = context.Statuses.ToList();
        //    ViewBag.DueFilters = Filters.DueFilterValues;

        //    // get ToDo objects from database based on current filters
        //    IQueryable<ToDo> query = context.ToDos
        //        .Include(t => t.Category).Include(t => t.Status);
        //    if (filters.HasCategory)
        //    {
        //        query = query.Where(t => t.CategoryId == filters.CategoryId);
        //    }
        //    if (filters.HasStatus)
        //    {
        //        query = query.Where(t => t.StatusId == filters.StatusId);
        //    }
        //    if (filters.HasDue)
        //    {
        //        var today = DateTime.Today;
        //        if (filters.IsPast)
        //            query = query.Where(t => t.DueDate < today);
        //        else if (filters.IsFuture)
        //            query = query.Where(t => t.DueDate > today);
        //        else if (filters.IsToday)
        //            query = query.Where(t => t.DueDate == today);
        //    }
        //    var tasks = query.OrderBy(t => t.DueDate).ToList();
        //    return View(tasks);
        //}
        public IActionResult Add()
        {
            ViewBag.Action = "Add";
            ViewBag.Categories = context.Categories.ToList();
            ViewBag.Statuses = context.Statuses.ToList();
            return View("Add", new ToDo());
        }

        [HttpPost]
        public IActionResult Add(ToDo task)
        {
            if (ModelState.IsValid)
            {
                context.ToDos.Add(task);
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Categories = context.Categories.ToList();
                ViewBag.Statuses = context.Statuses.ToList();
                return View(task);
            }
        }

        [HttpPost]
        public IActionResult Filter(string[] filter)
        {
            string id = string.Join('-', filter);
            return RedirectToAction("Index", new { ID = id });
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            ViewBag.Action = "update";
            ViewBag.Categories = context.Categories.ToList();
            ViewBag.Statuses = context.Statuses.ToList();
            var existingData = context.ToDos.Find(id);
            return View("Add", existingData);

        }
        [HttpPost]
        public IActionResult Update(ToDo item )
        {
            if(ModelState.IsValid)
            {
                context.ToDos.Update(item);
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Action = "update";
                ViewBag.Categories = context.Categories.ToList();
                ViewBag.Statuses = context.Statuses.ToList();
                var existingData = context.ToDos.Find(item.Id);
                return View("Add", existingData);
            }
            

        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewBag.Action = "Edit";
            ViewBag.Categories = context.Categories.ToList();
            ViewBag.Statuses = context.Statuses.ToList();
            var todo = context.ToDos.Find(id);
            return View(todo);
        }

        [HttpPost]
        public IActionResult Edit([FromRoute]string id, ToDo selected)
        {
            if (selected.StatusId == null) {
                context.ToDos.Remove(selected);
            }
            else {
                string newStatusId = selected.StatusId;
                selected = context.ToDos.Find(selected.Id);
                selected.StatusId = newStatusId;
                context.ToDos.Update(selected);
            }
            context.SaveChanges();

            return RedirectToAction("Index", new { ID = id });
        }
        public IActionResult Admin()
        {
            return View();
        }




    }
}
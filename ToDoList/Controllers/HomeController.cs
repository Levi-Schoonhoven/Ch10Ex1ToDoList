using System;
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

        public IActionResult Index()
        {
            // load current filters and data needed for filter drop downs in ViewBag
            ToDoViewModel toDoViewModel = new ToDoViewModel();
            //var filters = new Filters(id);
            toDoViewModel.Categories = context.Categories.ToList();
            toDoViewModel.Statuses = context.Statuses.ToList();
            toDoViewModel.Tasks = context.Tasks.ToList();
          
            toDoViewModel.DueFilters = new System.Collections.Generic.Dictionary<string, string> {
                {"today", "Today" },
                {"week", "Next 7 days" },
                {"month", "Next 30 days" }
            
            };
            return View(toDoViewModel);
            /*
            var filters = new Filters(id);
            ViewBag.Filters = filters;
            ViewBag.Categories = context.Categories.ToList();
            ViewBag.Statuses = context.Statuses.ToList();
            ViewBag.DueFilters = Filters.DueFilterValues;
            */
            /*
            // get ToDo objects from database based on current filters
            IQueryable<ToDo> query = context.ToDos
                .Include(t => t.Category).Include(t => t.Status);
            if (filters.HasCategory) {
                query = query.Where(t => t.CategoryId == filters.CategoryId);
            }
            if (filters.HasStatus) {
                query = query.Where(t => t.StatusId == filters.StatusId);
            }
            if (filters.HasDue) {
                var today = DateTime.Today;
                if (filters.IsPast)
                    query = query.Where(t => t.DueDate < today);
                else if (filters.IsFuture)
                    query = query.Where(t => t.DueDate > today);
                else if (filters.IsToday)
                    query = query.Where(t => t.DueDate == today);
            }
            var tasks = query.OrderBy(t => t.DueDate).ToList();
            */
           
        }

        public IActionResult Add()
        {
            ToDoViewModel toDoView = new ToDoViewModel
            {
                CurrentTask = new ToDo(),
                Categories = context.Categories.ToList(),
                Statuses = context.Statuses.ToList(),
                
                /*
                ViewBag.Categories = context.Categories.ToList();
                ViewBag.Statuses = context.Statuses.ToList();
                */
            };
            toDoView.DueFilters = new System.Collections.Generic.Dictionary<string, string>
            {
                {"Today","Due Today"},
                {"Tomorrow","Due Tomorrow" },
                {"Future", "Due in the Future"},
                {"Past","Past Due" },
                {"None", "No Due DAte" }

            };
            return View(toDoView);
        }

        [HttpPost]
        public IActionResult Add(ToDoViewModel model)
        {
            if (ModelState.IsValid)
            {
                context.Tasks.Add(model.CurrentTask);
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                model.Categories = context.Categories.ToList();
                model.Statuses = context.Statuses.ToList();
                
                model.DueFilters = new System.Collections.Generic.Dictionary<string, string>
                {
                {"Today","Due Today"},
                {"Tomorrow","Due Tomorrow" },
                {"Future", "Due in the Future"},
                {"Past","Past Due" },
                {"None", "No Due DAte" }

                };
                /*
                ViewBag.Categories = context.Categories.ToList();
                ViewBag.Statuses = context.Statuses.ToList();
                */
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult Filter(string[] filter)
        {
            string id = string.Join('-', filter);
            return RedirectToAction("Index", new { ID = id });
        }

        [HttpPost]
        public IActionResult Edit([FromRoute]string id, ToDo selected)
        {
            if (selected.StatusId == null) {
                context.Tasks.Remove(selected);
            }
            else {
                string newStatusId = selected.StatusId;
                selected = context.Tasks.Find(selected.Id);
                selected.StatusId = newStatusId;
                context.Tasks.Update(selected);
            }
            context.SaveChanges();

            return RedirectToAction("Index", new { ID = id });
        }
    }
}
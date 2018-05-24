using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MasterDetailEditing.Models;
using DevExpress.AspNetCore;

namespace MasterDetailEditing.Controllers {
    public class HomeController : Controller {
        NorthwindContext _northwindContext { get; }
        public HomeController(NorthwindContext northwindContext) {
            _northwindContext = northwindContext;
        }
        public IActionResult Index() {
            return View(_northwindContext.Categories);
        }
        #region MasterGrid
        public IActionResult MasterDetailView() {
            return PartialView("MasterGridView", _northwindContext.Categories);
        }
        public IActionResult MasterAddAction(Category category) {
            if (ModelState.IsValid)
                try {
                    category.CategoryID = _northwindContext.Categories.Max(c => c.CategoryID) + 1;
                    _northwindContext.Categories.Add(category);
                    _northwindContext.SaveChanges();
                } catch (Exception e) {
                    ViewData["ErrorMessage"] = e.Message;
                }
            return PartialView("MasterGridView", _northwindContext.Categories);
        }
        public IActionResult MasterUpdateRowAction(Category category) {
            if (ModelState.IsValid)
                try {
                    _northwindContext.Categories.Update(category);
                    _northwindContext.SaveChanges();
                } catch (Exception e) {
                    ViewData["ErrorMessage"] = e.Message;
                }
            return PartialView("MasterGridView", _northwindContext.Categories);
        }
        public IActionResult MasterDeleteRowAction(int categoryID = -1) {
            var category = _northwindContext.Categories.FirstOrDefault(c => c.CategoryID == categoryID);
            try {
                _northwindContext.Categories.Remove(category);
                _northwindContext.SaveChanges();
            } catch (Exception e) {
                ViewData["error"] = e.Message;
            }
            return PartialView("MasterGridView", _northwindContext.Categories);
        }
        #endregion

        #region Detail Grid
        public IActionResult DetailGridView(int categoryID) {
            ViewData["categoryID"] = categoryID;
            return PartialView("DetailGridView", _northwindContext.Products.Where(p => p.CategoryID == categoryID).ToList());
        }
        public IActionResult DetailAddAction(Product product, int categoryID) {
            try {
                if (ModelState.IsValid) {
                    product.ProductID = _northwindContext.Products.Max(p => p.ProductID) + 1;
                    _northwindContext.Products.Add(product);
                    _northwindContext.SaveChanges();
                }
            } catch (Exception e) {
                ViewData["error"] = e.Message;
            }

            ViewData["categoryID"] = categoryID;
            return PartialView("DetailGridView", _northwindContext.Products.Where(p => p.CategoryID == categoryID));
        }
        public IActionResult DetailUpdateRowAction(Product product, int categoryID) {
            try {
                if (ModelState.IsValid) {
                    _northwindContext.Products.Update(product);
                    _northwindContext.SaveChanges();
                }
            } catch (Exception e) {
                ViewData["error"] = e.Message;
            }
            ViewData["categoryID"] = categoryID;
            return PartialView("DetailGridView", _northwindContext.Products.Where(p => p.CategoryID == categoryID));
        }
        public IActionResult DetailDeleteRowAction(int categoryID, int productID = -1) {
            var product = _northwindContext.Products
                .FirstOrDefault(i => i.ProductID == productID);
            try {
                _northwindContext.Products.Remove(product);
                _northwindContext.SaveChanges();
            } catch (Exception e) {
                ViewData["error"] = e.Message;
            }
            ViewData["categoryID"] = categoryID;
            return PartialView("DetailGridView", _northwindContext.Products.Where(p => p.CategoryID == categoryID));
        }
        #endregion
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

}
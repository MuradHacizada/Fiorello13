﻿using Fiorello.DAL;
using Fiorello.Helper;
using Fiorello.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Fiorello.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {

        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        public ProductsController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            List<Product> products = await _db.Products.Include(x => x.Category).ToListAsync();
            return View(products);
        }
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _db.Categories.ToListAsync();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, int catId)
        {
            ViewBag.Categories = await _db.Categories.ToListAsync();
            #region Exist Item
            bool isExist = await _db.Products.AnyAsync(x => x.Name == product.Name);
            if (isExist)
            {
                ModelState.AddModelError("Name", "This Product is already exist !");
                return View();
            }
            #endregion

            #region Save Image
            if (product.Photo == null)
            {
                ModelState.AddModelError("Photo", "Image can not be null");
                return View();
            }
            if (!product.Photo.IsImage())
            {
                ModelState.AddModelError("Photo", "Pls select image type");
                return View();
            }
            if (product.Photo.IsOlderMb())
            {
                ModelState.AddModelError("Photo", "max 1 mb");
                return View();
            }
            string folder = Path.Combine(_env.WebRootPath, "img");
            product.Image = await product.Photo.SaveFileAsync(folder);
            #endregion

            product.CategoryId = catId;
            await _db.Products.AddAsync(product);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
                return NotFound();
            Product dbproduct = await _db.Products.Include(x => x.ProductDetail).FirstOrDefaultAsync(x => x.Id == id);
            if (dbproduct == null)
                return BadRequest();
            ViewBag.Categories = await _db.Categories.ToListAsync();
            return View(dbproduct);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Product product, int catId)
        {
            if (id == null)
                return NotFound();
            Product dbproduct = await _db.Products.Include(x=>x.ProductDetail).FirstOrDefaultAsync(x => x.Id == id);
            if (dbproduct == null)
                return BadRequest();
            ViewBag.Categories = await _db.Categories.ToListAsync();

            #region Exist Item
            bool isExist = await _db.Products.AnyAsync(x => x.Name == product.Name && x.Id != id);
            if (isExist)
            {
                ModelState.AddModelError("Name", "This Product is already exist !");
                return View();
            }
            #endregion

            #region Save Image
            if (product.Photo != null)
            {
                if (!product.Photo.IsImage())
                {
                    ModelState.AddModelError("Photo", "Pls select image type");
                    return View();
                }
                if (product.Photo.IsOlderMb())
                {
                    ModelState.AddModelError("Photo", "max 1 mb");
                    return View();
                }
                string folder = Path.Combine(_env.WebRootPath, "img");
                dbproduct.Image = await product.Photo.SaveFileAsync(folder);
            }

            #endregion

            dbproduct.Name = product.Name;
            dbproduct.Price = product.Price;
            dbproduct.CategoryId = catId;
            dbproduct.ProductDetail.Description = product.ProductDetail.Description;

            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}

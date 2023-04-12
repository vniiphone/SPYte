using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SPYte.Data;
using SPYte.Models;

namespace SPYte.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsAdminController : Controller
    {
        private readonly IWebHostEnvironment _env;

        private readonly SPYteDBContext _context;

        public ProductsAdminController(SPYteDBContext context, IWebHostEnvironment env)
        {
            _env = env;
            _context = context;
        }

        // GET: Admin/ProductsAdmin
        public async Task<IActionResult> Index()
        {
            var sPYteDBContext = _context.Products.Include(p => p.Brand).Include(p => p.Cat);
            return View(await sPYteDBContext.ToListAsync());
        }

        // GET: Admin/ProductsAdmin/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Cat)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name");
            ViewData["CatId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price,Stock,CreatedDate,UpdatedDate,DeletedDate,CatId,BrandId,Unit")] Product product, IEnumerable<IFormFile> files)
        {
            ModelState.Remove(nameof(product.Cat));
            ModelState.Remove(nameof(product.Brand));
           if(ModelState.IsValid) {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                string[] filenameList = new string[20];
                var proDir = Path.Combine(_env.WebRootPath, "files/images");
                foreach (var file in files)
                {
                    if (file.Length > 5242880)
                    {
                        return View(product);
                        //StatusMessage = "Error: file too big (only allow 5Mb or lower)";
                        //return Page();
                    }
                    var imgName = Guid.NewGuid().ToString() + file.FileName;
                    var path = Path.Combine(proDir, imgName);
                    if (!Directory.Exists(proDir))
                    {
                        Directory.CreateDirectory(proDir);
                    }
                    using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write))
                    {
                        file.CopyTo(stream);
                    }
                    Image img = new Image();
                    img.ProductId = product.Id;
                    img.Url = imgName;
                    _context.Images.Add(img);
                    await _context.SaveChangesAsync();
                }
                
                return RedirectToAction(nameof(Index));
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name", product.BrandId);
            ViewData["CatId"] = new SelectList(_context.Categories, "Id", "Name", product.CatId);
            return View(product);
        }
        // GET: Admin/ProductsAdmin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name,Description,Price,Stock,CreatedDate,UpdatedDate,DeletedDate,CatId,BrandId,Unit")] Product product, IEnumerable<IFormFile> files)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(product.Cat));
            ModelState.Remove(nameof(product.Brand));

            if (ModelState.IsValid)
            {
                try
                {
                    // Get the product from the database
                    var dbProduct = await _context.Products.FindAsync(id);

                    // Update the properties of the product
                    dbProduct.Name = product.Name;
                    dbProduct.Description = product.Description;
                    dbProduct.Price = product.Price;
                    dbProduct.Stock = product.Stock;
                    dbProduct.CreatedDate = product.CreatedDate;
                    dbProduct.UpdatedDate = product.UpdatedDate;
                    dbProduct.DeletedDate = product.DeletedDate;
                    dbProduct.CatId = product.CatId;
                    dbProduct.BrandId = product.BrandId;
                    dbProduct.Unit = product.Unit;

                    // Delete the existing images associated with the product
                    var images = _context.Images.Where(i => i.ProductId == id).ToList();
                    foreach (var image in images)
                    {
                        _context.Images.Remove(image);
                    }

                    // Add the new images
                    if (files != null && files.Any())
                    {
                        foreach (var file in files)
                        {
                            var fileName = Path.GetFileName(file.FileName);
                            var imagePath = Path.Combine(_env.WebRootPath, "images", fileName);
                            using (var fileStream = new FileStream(imagePath, FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                            }

                            var image = new Image
                            {
                                Name = fileName,
                                ProductId = dbProduct.Id
                            };

                            _context.Images.Add(image);
                        }
                    }

                    // Save changes to the database
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name", product.BrandId);
            ViewData["CatId"] = new SelectList(_context.Categories, "Id", "Name", product.CatId);
            return View(product);
        }




        //public async Task<IActionResult> Edit(long? id)
        //{
        //    if (id == null || _context.Products == null)
        //    {
        //        return NotFound();
        //    }

        //    var product = await _context.Products.FindAsync(id) ;
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }

        //    ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name", product.BrandId);
        //    ViewData["CatId"] = new SelectList(_context.Categories, "Id", "Name", product.CatId);
        //    return View(product);
        //}

        //// POST: Admin/ProductsAdmin/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(long id, [Bind("Id,Name,Description,Price,Stock,CreatedDate,UpdatedDate,DeletedDate,CatId,BrandId,Unit")] Product product, IEnumerable<IFormFile> files)
        //{
        //    if (id != product.Id)
        //    {
        //        return NotFound();
        //    }
        //    ModelState.Remove(nameof(product.Cat));
        //    ModelState.Remove(nameof(product.Brand));
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            // Lấy danh sách ảnh từ bảng ảnh
        //            var images = _context.Images.Where(i => i.ProductId == id).ToList();
        //            if (images == null)
        //            {
        //                images = new List<Image>();
        //            }
        //            ViewData["Images"] = images;

        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!ProductExists(product.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name", product.BrandId);
        //    ViewData["CatId"] = new SelectList(_context.Categories, "Id", "Name", product.CatId);
        //    return View(product);
        //}


        //public async Task<IActionResult> Edit(long id, [Bind("Id,Name,Description,Price,Stock,CreatedDate,UpdatedDate,DeletedDate,CatId,BrandId,Unit")] Product product, )
        //{
        //    if (id != product.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            if (ModelState.IsValid)
        //            {
        //                var productImages = await _context.Images.Where(i => i.ProductId == id).ToListAsync();

        //                string[] filenameList = new string[20];
        //                var proDir = Path.Combine(_env.WebRootPath, "files/images");
        //                foreach (var file in files)
        //                {
        //                    if (file.Length > 5242880)
        //                    {
        //                        return View(product);
        //                        //StatusMessage = "Error: file too big (only allow 5Mb or lower)";
        //                        //return Page();
        //                    }
        //                    var imgName = Guid.NewGuid().ToString() + file.FileName;

        //                    var path = Path.Combine(proDir, imgName);


        //                    if (!Directory.Exists(proDir))
        //                    {
        //                        Directory.CreateDirectory(proDir);
        //                    }

        //                    using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write))
        //                    {
        //                        file.CopyTo(stream);
        //                    }
        //                    Image img = new Image();
        //                    img.ProductId = product.Id;
        //                    img.Url = imgName;
        //                    _context.Images.Add(img);
        //                }
        //                await _context.SaveChangesAsync();
        //                return RedirectToAction(nameof(Index));
        //            }
        //            _context.Update(product);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!ProductExists(product.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name", product.BrandId);
        //    ViewData["CatId"] = new SelectList(_context.Categories, "Id", "Name", product.CatId);
        //    var images = await _context.Images.Where(x => x.ProductId == id).ToListAsync();
        //    ViewBag.Images = images;

        //    return View(product);
        //}

        // GET: Admin/ProductsAdmin/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Cat)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/ProductsAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'SPYteDBContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(long id)
        {
          return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

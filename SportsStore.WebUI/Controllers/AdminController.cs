using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;

namespace SportsStore.WebUI.Controllers
{
	[Authorize]
	public class AdminController : Controller
    {
		private IProductsRepository repository;

		public AdminController (IProductsRepository repo)
		{
			repository = repo;
		}
		public ViewResult Index()
		{
			return View(repository.Products);
		}

		public ViewResult Edit(int productId)
		{
			Product product = repository.Products.FirstOrDefault(p => p.ProductID == productId);
			return View(product);
		}

		[HttpPost]
		public ActionResult Edit(Product product, HttpPostedFileBase image)
		{
			if (ModelState.IsValid)
			{
				if (image != null)
				{
					product.ImageMimeType = image.ContentType;
					product.ImageData = new byte[image.ContentLength];
					image.InputStream.Read(product.ImageData, 0, image.ContentLength);
				}
				repository.SaveProduct(product);
				TempData["message"] = string.Format("{0} был сохранен", product.Name);
				return RedirectToAction("Index");
			}
			else
			{
				return View(product);
			}
		}

		public ViewResult Create()
		{
			return View("Edit", new Product());
		}

		[HttpPost]
		public ActionResult Delete(int productId)
		{
			Product deletedProduct = repository.DeleteProduct(productId);
			if (deletedProduct != null)
			{
				TempData["message"] = string.Format("{0} был удален", deletedProduct.Name);
			}
			return RedirectToAction("Index");
		}
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Models;

namespace SportsStore.WebUI.Controllers
{
    public class CartController : Controller
    {
		// GET: Cart
		private IProductsRepository repository;
		private IOrderProcessor orderProcessor;
		public CartController(IProductsRepository repo, IOrderProcessor proc)
		{
			repository = repo;
			orderProcessor = proc;
		}

		public ViewResult Index(Cart cart, string returnUrl)
		{
			return View(new CartIndexViewModel
			{
				Cart = cart,
				ReturnUrl = returnUrl
			});
		}

		public RedirectToRouteResult AddToCart(Cart cart, int productId, string returnUrl)
		{
			Product product = repository.Products.FirstOrDefault(p => p.ProductID == productId);

			if (product!= null)
			{
				cart.AddItem(product, 1);
			}
			return RedirectToAction("Index", new { returnUrl });
		}

		public RedirectToRouteResult RemoveFromCart(Cart cart ,int productId, string returnUrl)
		{
			Product product = repository.Products.FirstOrDefault(p => p.ProductID == productId);
			if (product != null)
			{
				cart.RemoveLine(product);
			}

			return RedirectToAction("Index", new { returnUrl });
		}

		private Cart GetCart()
		{
			Cart cart = (Cart)Session["Cart"];
			if (cart == null)
			{
				cart = new Cart();
				Session["Cart"] = cart;
			}
			return cart;
		}

		public PartialViewResult Summary(Cart cart)
		{
			return PartialView(cart);
		}

		[HttpPost]
		public ViewResult Checkout(Cart cart, ShippingDetails shippingDetails)
		{
			if (cart.Lines.Count() == 0)
			{
				ModelState.AddModelError("", "Извините, ваша корзина пуста!");
			}
			if (ModelState.IsValid)
			{
				orderProcessor.ProcessOrder(cart, shippingDetails);
				cart.Clear();
				return View("Завершено");
			}
			else
			{
				return View(shippingDetails);
			}
		}

		public ViewResult Checkout()
		{
			return View(new ShippingDetails());
		}
    }
}
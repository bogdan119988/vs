using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SportsStore.WebUI.Controllers;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using SportsStore.WebUI.Models;
using SportsStore.WebUI.HtmlHelpers;
using SportsStore.WebUI.Infrastructure.Abstract;

namespace SportsStore.UnitTests
{
	[TestClass]
	public class AdminSecurityTests
	{
		[TestMethod]
		public void Can_Login_With_Valid_Credentials()
		{
			Mock<IAuthProvider> mock = new Mock<IAuthProvider>();
			mock.Setup(m => m.Authenticate("admin", "admin")).Returns(true);

			LoginViewModel model = new LoginViewModel
			{
				UserName = "admin",
				Password = "admin"
			};

			AccountController target = new AccountController(mock.Object);

			ActionResult result = target.Login(model, "/MyURL");
			Assert.IsInstanceOfType(result, typeof(RedirectResult));
			Assert.AreEqual("/MyURL", ((RedirectResult)result).Url);
		}

		[TestMethod]
		public void Cannot_Login_With_Invalid_Credentials()
		{
			Mock<IAuthProvider> mock = new Mock<IAuthProvider>();
			mock.Setup(m => m.Authenticate("badUser", "badPass")).Returns(false);

			LoginViewModel model = new LoginViewModel
			{
				UserName = "badUser",
				Password = "badPass"
			};

			AccountController target = new AccountController(mock.Object);

			ActionResult result = target.Login(model, "/MyURL");
			Assert.IsInstanceOfType(result, typeof(ViewResult));
			Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
		}
	}

	[TestClass]
	public class AdminTests
	{
		[TestMethod]
		public void Can_Delete_Valid_Products()
		{
			Product prod = new Product { ProductID = 2, Name = "Test" };

			Mock<IProductsRepository> mock = new Mock<IProductsRepository>();

			mock.Setup(m => m.Products).Returns(new Product[]
			{
				new Product { ProductID = 1, Name = "P1"},
				prod,
				new Product { ProductID = 3, Name = "P3"}
			}.AsQueryable());

			AdminController target = new AdminController(mock.Object);

			target.Delete(prod.ProductID);

			mock.Verify(m => m.DeleteProduct(prod.ProductID));
		}

		[TestMethod]
		public void Can_Save_Valid_Changes()
		{
			Mock<IProductsRepository> mock = new Mock<IProductsRepository>();

			AdminController target = new AdminController(mock.Object);

			Product product = new Product { Name = "Test" };

			ActionResult resilt = target.Edit(product, null);

			mock.Verify(m => m.SaveProduct(product));

			Assert.IsNotInstanceOfType(resilt, typeof(ViewResult));
		}

		[TestMethod]
		public void Cannot_Save_Invalid_Chages()
		{
			Mock<IProductsRepository> mock = new Mock<IProductsRepository>();

			AdminController target = new AdminController(mock.Object);

			Product product = new Product { Name = "Test" };

			target.ModelState.AddModelError("error", "error");

			ActionResult resilt = target.Edit(product, null);

			mock.Verify(m => m.SaveProduct(It.IsAny<Product>()), Times.Never());

			Assert.IsNotInstanceOfType(resilt, typeof(ViewResult));
		}

		[TestMethod]
		public void Index_Contains_All_Products()
		{
			Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
			mock.Setup(m => m.Products).Returns(new Product[]{
				new Product {ProductID = 1, Name = "P1"},
				new Product {ProductID = 2, Name = "P2"},
				new Product {ProductID = 3, Name = "P3"},
			}.AsQueryable());

			AdminController target = new AdminController(mock.Object);

			Product[] result = ((IEnumerable<Product>)target.Index().ViewData.Model).ToArray();

			Assert.AreEqual(result.Length, 3);
			Assert.AreEqual("P1", result[0].Name);
			Assert.AreEqual("P2", result[1].Name);
			Assert.AreEqual("P3", result[2].Name);
		}

		[TestMethod]
		public void Can_Edit_Product()
		{
			Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
			mock.Setup(m => m.Products).Returns(new Product[]
			{
				new Product { ProductID = 1, Name = "P1"},
				new Product { ProductID = 2, Name = "P2"},
				new Product { ProductID = 3, Name = "P3"}
			}.AsQueryable());

			AdminController target = new AdminController(mock.Object);

			Product p1 = target.Edit(1).ViewData.Model as Product;
			Product p2 = target.Edit(2).ViewData.Model as Product;
			Product p3 = target.Edit(3).ViewData.Model as Product;

			Assert.AreEqual(1, p1.ProductID);
			Assert.AreEqual(2, p2.ProductID);
			Assert.AreEqual(3, p3.ProductID);
		}

		[TestMethod]
		public void Cannot_Edit_Nonexistent_Product()
		{
			Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
			mock.Setup(m => m.Products).Returns(new Product[]
			{
				new Product { ProductID = 1, Name = "P1"},
				new Product { ProductID = 2, Name = "P2"},
				new Product { ProductID = 3, Name = "P3"}
			}.AsQueryable());

			AdminController target = new AdminController(mock.Object);

			Product result = (Product)target.Edit(4).ViewData.Model;

			Assert.IsNull(result);
		}
		
	}

	[TestClass]
	class UnitTest1
	{
		[TestMethod]
		public void Can_Paginate()
		{
			Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
			mock.Setup(m => m.Products).Returns(new Product[]
			{
				new Product {ProductID = 1, Name = "P1"},
				new Product {ProductID = 2, Name = "P2"},
				new Product {ProductID = 3, Name = "P3"},
				new Product {ProductID = 4, Name = "P4"},
				new Product {ProductID = 5, Name = "P5"}
			}.AsQueryable());

			ProductController controller = new ProductController(mock.Object);
			controller.PageSize = 3;
			ProductsListViewModel result = (ProductsListViewModel)controller.List(null, 2).Model;

			Product[] prodArray = result.Products.ToArray();
			Assert.IsTrue(prodArray.Length == 2);
			Assert.AreEqual(prodArray[0].Name, "P4");
			Assert.AreEqual(prodArray[1].Name, "P5");
		}

		[TestMethod]
		public void Can_Generate_Page_Links()
		{
			HtmlHelper myHelper = null;

			PagingInfo pagingInfo = new PagingInfo
			{
				CurrentPage = 2,
				TotalItems = 28,
				ItemPerPage = 10
			};

			Func<int, string> pageUrlDelegate = i => "Page" + i;

			MvcHtmlString result = myHelper.PageLinks(pagingInfo, pageUrlDelegate);

			Assert.AreEqual(result.ToString(), @"<a href=""Page1"">1</a>" + @"<a class=""selected"" href=""Page2"">2</a>" + @"<a href=""Page3"">3</a>");
		}

		[TestMethod]
		public void Can_Send_Pagination_View_model()
		{
			Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
			mock.Setup(m => m.Products).Returns(new Product[]
			{
				new Product {ProductID = 1, Name = "P1"},
				new Product {ProductID = 2, Name = "P2"},
				new Product {ProductID = 3, Name = "P3"},
				new Product {ProductID = 4, Name = "P4"},
				new Product {ProductID = 5, Name = "P5"},
			}.AsQueryable());

			ProductController controller = new ProductController(mock.Object);
			controller.PageSize = 3;

			ProductsListViewModel result = (ProductsListViewModel)controller.List(null,2).Model;

			PagingInfo pageInfo = result.PagingInfo;
			Assert.AreEqual(pageInfo.CurrentPage, 2);
			Assert.AreEqual(pageInfo.ItemPerPage, 3);
			Assert.AreEqual(pageInfo.TotalItems, 5);
			Assert.AreEqual(pageInfo.TotalPages, 2);

		
		}

		[TestMethod]
		public void Can_Filter_Products()
		{
			Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
			mock.Setup(m => m.Products).Returns(new Product[]
			{
				new Product {ProductID = 1, Name = "P1"},
				new Product {ProductID = 2, Name = "P2"},
				new Product {ProductID = 3, Name = "P3"},
				new Product {ProductID = 4, Name = "P4"},
				new Product {ProductID = 5, Name = "P5"},
			}.AsQueryable());
			ProductController controller = new ProductController(mock.Object);
			controller.PageSize = 3;

			Product[] result = ((ProductsListViewModel)controller.List("Cat2", 1).Model).Products.ToArray();

			Assert.AreEqual(result.Length, 2);
			Assert.IsTrue(result[0].Name == "P2" && result[0].Category == "Cat2");
			Assert.IsTrue(result[1].Name == "P4" && result[1].Category == "Cat2");
		}

		[TestMethod]
		public void Can_Create_Categories()
		{
			Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
			mock.Setup(m => m.Products).Returns(new Product[]
			{
				new Product {ProductID = 1, Name = "P1", Category = "Apples"},
				new Product {ProductID = 2, Name = "P2", Category = "Apples"},
				new Product {ProductID = 3, Name = "P3", Category = "Plums"},
				new Product {ProductID = 4, Name = "P4", Category = "Oranges"}
			}.AsQueryable());
			NavController target = new NavController(mock.Object);

			string[] result = ((IEnumerable<string>)target.Menu().Model).ToArray();

			Assert.AreEqual(result.Length, 3);
			Assert.AreEqual(result[0], "Apples");
			Assert.AreEqual(result[1], "Oranges");
			Assert.AreEqual(result[2], "Plums");
		}

		[TestMethod]
		public void Indicates_Selected_Category()
		{
			Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
			mock.Setup(m => m.Products).Returns(new Product[]
			{
				new Product{ ProductID = 1, Name = "P2", Category = "Apples"},
				new Product{ ProductID = 4, Name = "P2", Category = "Oranges"},
			}.AsQueryable());

			NavController target = new NavController(mock.Object);

			string categoryToSelect = "Apples";

			string result = target.Menu(categoryToSelect).ViewBag.SelectedCategory;

			Assert.AreEqual(categoryToSelect, result);
		}

		[TestMethod]
		public void Generate_Category_Specific_Product_Count()
		{
			Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
			mock.Setup(m => m.Products).Returns(new Product[]
			{
				new Product {ProductID = 1, Name = "P1", Category = "Cat1"},
				new Product {ProductID = 2, Name = "P2", Category = "Cat2"},
				new Product {ProductID = 3, Name = "P3", Category = "Cat1"},
				new Product {ProductID = 4, Name = "P4", Category = "Cat2"},
				new Product {ProductID = 5, Name = "P5", Category = "Cat3"},
			}.AsQueryable());

			ProductController target = new ProductController(mock.Object);
			target.PageSize = 3;

			int res1 = ((ProductsListViewModel)target.List("Cat1").Model).PagingInfo.TotalItems;
			int res2 = ((ProductsListViewModel)target.List("Cat2").Model).PagingInfo.TotalItems;
			int res3 = ((ProductsListViewModel)target.List("Cat3").Model).PagingInfo.TotalItems;
			int resAll = ((ProductsListViewModel)target.List(null).Model).PagingInfo.TotalItems;

			Assert.AreEqual(res1, 2);
			Assert.AreEqual(res2, 2);
			Assert.AreEqual(res3, 1);
			Assert.AreEqual(resAll, 5);
		}
	}

		[TestClass]
		public class CartTests
		{
			[TestMethod]
			public void Can_Add_New_Lines()
			{
				Product p1 = new Product { ProductID = 1, Name = "P1" };
				Product p2 = new Product { ProductID = 2, Name = "P2" };

				Cart target = new Cart();

				target.AddItem(p1, 1);
				target.AddItem(p2, 1);
			
				Cart.CartLine[] results = target.Lines.ToArray();

				Assert.AreEqual(results.Length, 2);
				Assert.AreEqual(results[0].Product, p1);
				Assert.AreEqual(results[1].Product, p2);
			}
			
			[TestMethod]
			public void Can_Add_Quantity_For_Existing_Lines()
			{
				Product p1 = new Product { ProductID = 1, Name = "P1" };
				Product p2 = new Product { ProductID = 2, Name = "P2" };

				Cart target = new Cart();

				target.AddItem(p1, 1);
				target.AddItem(p2, 1);
				target.AddItem(p1, 10);

				Cart.CartLine[] results = target.Lines.OrderBy( c => c.Product.ProductID).ToArray();

				Assert.AreEqual(results.Length, 2);
				Assert.AreEqual(results[0].Quantity, 11);
				Assert.AreEqual(results[1].Quantity, 1);
		}

		[TestMethod]
		public void Can_Remove_Lines()
		{
			Product p1 = new Product { ProductID = 1, Name = "P1" };
			Product p2 = new Product { ProductID = 2, Name = "P2" };
			Product p3 = new Product { ProductID = 3, Name = "P3" };

			Cart target = new Cart();

			target.AddItem(p1, 1);
			target.AddItem(p2, 3);
			target.AddItem(p3, 5);
			target.AddItem(p2, 1);

			target.RemoveLine(p2);

			Assert.AreEqual(target.Lines.Where(c => c.Product == p2).Count(), 2);
			Assert.AreEqual(target.Lines.Count(), 2);
		}

		[TestMethod]
		public void Calculate_Cart_Total()
		{
			Product p1 = new Product { ProductID = 1, Name = "P1", Price = 100M };
			Product p2 = new Product { ProductID = 2, Name = "P2", Price = 50M };

			Cart target = new Cart();

			target.AddItem(p1, 1);
			target.AddItem(p2, 1);
			target.AddItem(p1, 3);
			decimal result = target.ComputeTotalValue();

			Assert.AreEqual(result, 450M);
		}

		[TestMethod]
		public void Can_Clear_Contents()
		{
			Product p1 = new Product { ProductID = 1, Name = "P1", Price = 100M };
			Product p2 = new Product { ProductID = 2, Name = "P2", Price = 50M };

			Cart target = new Cart();

			target.AddItem(p1, 1);
			target.AddItem(p2, 1);

			target.Clear();

			Assert.AreEqual(target.Lines.Count(), 0);
		}	 
	}

	[TestClass]
	public class CatrTests
	{
		[TestMethod]
		public void Can_Add_To_Cart()
		{
			Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
			mock.Setup(m => m.Products).Returns(new Product[]
			{
				new Product {ProductID = 1, Name = "P1", Category = "Apples"},
			}.AsQueryable());

			Cart cart = new Cart();

			CartController target = new CartController(mock.Object, null);

			target.AddToCart(cart, 1, null);

			Assert.AreEqual(cart.Lines.Count(), 1);
			Assert.AreEqual(cart.Lines.ToArray()[0].Product.ProductID, 1);
		}

		[TestMethod]
		public void Adding_Product_To_Cart_Goes_To_Cart_Screen()
		{
			Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
			mock.Setup(m => m.Products).Returns(new Product[]
			{
				new Product {ProductID = 1, Name = "P1", Category = "Apples"},
			}.AsQueryable());

			Cart cart = new Cart();

			CartController target = new CartController(mock.Object, null);

			RedirectToRouteResult result = target.AddToCart(cart, 2, "myUrl");

			Assert.AreEqual(result.RouteValues["action"], "Index");
			Assert.AreEqual(result.RouteValues["returnUrl"], "myUrl");

		}

		[TestMethod]
		public void Cant_View_Cart_Contents()
		{
			Cart cart = new Cart();

			CartController target = new CartController(null, null);

			CartIndexViewModel result = (CartIndexViewModel)target.Index(cart, "myUrl").ViewData.Model;

			Assert.AreSame(result.Cart, cart);
			Assert.AreEqual(result.ReturnUrl, "myUrl");
		}

		[TestMethod]
		public void Cannot_Checkout_Empty_Cart()
		{
			Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();

			Cart cart = new Cart();

			ShippingDetails shippingDetails = new ShippingDetails();

			CartController target = new CartController(null, mock.Object);

			ViewResult result = target.Checkout(cart, shippingDetails);

			mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never());

			Assert.AreEqual("", result.ViewData.ModelState.IsValid);
		}

		[TestMethod]
		public void Cannot_Checkout_Invalid_ShippingDetails()
		{
			Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();

			Cart cart = new Cart();
			cart.AddItem(new Product(), 1);

			CartController target = new CartController(null, mock.Object);

			target.ModelState.AddModelError("error", "error");

			ViewResult result = target.Checkout(cart, new ShippingDetails());

			mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never());

			Assert.AreEqual("", result.ViewName);

			Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
		}

		[TestMethod]
		public void Can_Checkout_And_Submit_Order()
		{
			Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();

			Cart cart = new Cart();
			cart.AddItem(new Product(), 1);

			CartController target = new CartController(null, mock.Object);

			ViewResult result = target.Checkout(cart, new ShippingDetails());

			mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Once());

			Assert.AreEqual("Завершено", result.ViewName);

			Assert.AreEqual(true, result.ViewData.ModelState.IsValid);
		}

	}
}

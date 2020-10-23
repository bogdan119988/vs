using System.Collections.Generic;
using System.Linq;



namespace SportsStore.Domain.Entities
{
	public class Cart
	{
		private List<CartLine> lineColection = new List<CartLine>();

		public void AddItem(Product product, int quantity)
		{
			CartLine line = lineColection.Where(p => p.Product.ProductID == product.ProductID).FirstOrDefault();

			if (line == null)
			{
				lineColection.Add(new CartLine
				{
					Product = product,
					Quantity = quantity
				});
			}
			else
			{
				line.Quantity += quantity;
			}
		}

		public void RemoveLine(Product product)
		{
			lineColection.RemoveAll(l => l.Product.ProductID == product.ProductID);
		}

		public decimal ComputeTotalValue()
		{
			return lineColection.Sum(e => e.Product.Price * e.Quantity);
		}

		public void Clear()
		{
			lineColection.Clear();
		}

		public IEnumerable<CartLine> Lines
		{
			get { return lineColection; }
		}

		public class CartLine
		{
			public Product Product { get; set; }
			public int Quantity { get; set; }
		}
	}

	
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SportsStore.Domain.Entities
{
	public class Product
	{
		[HiddenInput(DisplayValue = false)]
		public int ProductID { get; set; }

		[Display(Name ="Название")]
		[Required(ErrorMessage = "Пожалуйста введите название продукта")]
		public string Name { get; set; }

		[DataType(DataType.MultilineText),
			Display(Name ="Описание")]
		[Required(ErrorMessage = "Пожалуйста введите описание")]
		public string Description { get; set; }

		[Display(Name ="Цена")]
		[Required]
		[Range(0.01, double.MaxValue, ErrorMessage = "Пожалуйста введите положительную цену")]
		public decimal Price { get; set; }

		[Display(Name = "Категория")]
		[Required(ErrorMessage = "Пожалуйста введите категорию")]
		public string Category { get; set; }

		public byte[] ImageData { get; set; }

		[HiddenInput(DisplayValue = false)]
		public string ImageMimeType { get; set; }
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SportsStore.Domain.Entities
{
	public class ShippingDetails
	{
		[Required(ErrorMessage = "Пожалуйста, введита имя")]
		public string Name { get; set; }

		[Required(ErrorMessage = "Пожалуйста, заполните первую адресную строку")]
		public string Line1 { get; set; }
		public string Line2 { get; set; }
		public string Line3 { get; set; }

		[Required(ErrorMessage = "Пожалуйста, введите название города")]
		public string City { get; set; }

		[Required(ErrorMessage = "Пожалуйста, введите название области или республики")]
		public string State { get; set; }
		public string Zip { get; set; }

		[Required(ErrorMessage = "Пожалуйста, введите название страны")]
		public string Country { get; set; }
		public bool GiftWrap { get; set; }
	}
}

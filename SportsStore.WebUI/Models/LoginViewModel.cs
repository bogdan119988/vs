using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SportsStore.WebUI.Models
{
	public class LoginViewModel
	{
		[Required]
		[Display(Name = "Логин")]
		public string UserName { get; set; }
		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Пароль")]
		public string Password { get; set; }
	}
}
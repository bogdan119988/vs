using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SportsStore.Domain.Entities;
using SportsStore.Domain.Abstract;
using System.Net;
using System.Net.Mail;

namespace SportsStore.Domain.Concrete
{
	public class EmailSettings
	{
		public string MailToAdress = "orders@example.com";
		public string MailFromAddress = "sportsstore@example.com";
		public bool UseSsl = true;
		public string Username = "MySmtpUsername";
		public string Password = "MySmtpPassword";
		public string ServerName = "smtp.example.com";
		public int ServerPort = 587;
		public bool WriteAsFile = false;
		public string FileLocation = @"C:\Users\007MatvychukBr\source\repos\SportsStore\sports_store_emails";
	}

	public class EmailOrderProcessor : IOrderProcessor
	{
		private EmailSettings emailSettings;
		public EmailOrderProcessor(EmailSettings settings)
		{
			emailSettings = settings;
		}

		public void ProcessOrder(Cart cart, ShippingDetails shippingInfo)
		{
			using (var smtpClient = new SmtpClient())
			{
				smtpClient.EnableSsl = emailSettings.UseSsl;
				smtpClient.Host = emailSettings.ServerName;
				smtpClient.Port = emailSettings.ServerPort;
				smtpClient.UseDefaultCredentials = false;
				smtpClient.Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password);

				if (emailSettings.WriteAsFile)
				{
					smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
					smtpClient.PickupDirectoryLocation = emailSettings.FileLocation;
					smtpClient.EnableSsl = false;
				}

				StringBuilder body = new StringBuilder().AppendLine("Отправлен новый заказ").AppendLine("---").AppendLine("Количество:");

				foreach (var line in cart.Lines)
				{
					var subtotal = line.Product.Price * line.Quantity;
					body.AppendFormat("{0} x {1} (subtotal^ {2:c}", line.Quantity, line.Product.Name, subtotal);
				}

				body.AppendFormat("Общая стоимость заказа: {0:c}", cart.ComputeTotalValue()).AppendLine("---")
					.AppendLine("Заказчик:").AppendLine(shippingInfo.Name).AppendLine(shippingInfo.Line1).AppendLine(shippingInfo.Line2 ?? "")
					.AppendLine(shippingInfo.Line3 ?? "").AppendLine(shippingInfo.Country).AppendLine(shippingInfo.City).AppendLine(shippingInfo.State ?? "")
					.AppendLine(shippingInfo.Zip).AppendLine("---").AppendFormat("Подарочная упаковка: {0}", shippingInfo.GiftWrap ? "Да" : "Нет");

				MailMessage mailMessage = new MailMessage(
					emailSettings.MailFromAddress,
					emailSettings.MailToAdress,
					"Отправлен новый заказ!",
					body.ToString());
				if (emailSettings.WriteAsFile)
				{
					mailMessage.BodyEncoding = Encoding.UTF8;
				}
				smtpClient.Send(mailMessage);
			}
		}
	}
}

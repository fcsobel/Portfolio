using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Portfolio.Data.Model
{
	public enum ExternalProviderEnum { Facebook, Google, Local };

	public partial class LoginProfile
	{
		public virtual Guid UserId { get; set; }
		public string ProviderName { get; set; }
		public string ProviderKey { get; set; }
		public string UserName { get; set; }
		public int LoginCount { get; set; }
		public DateTime? LoginDate { get; set; }
		public string Url { get; set; }
		public string Picture { get; set; }
		public DateTime Created { get; set; }

		public User User { get; set; }

		[NotMapped]
		public ExternalProviderEnum Provider
		{
			get { return GetProvider(this.ProviderName); }
			set { this.ProviderName = GetProvider(value); }
		}

		// helper methods
		public static string GetProvider(ExternalProviderEnum value) { return value.ToString(); }
		public static ExternalProviderEnum GetProvider(string value)
		{
			if (string.IsNullOrWhiteSpace(value)) return ExternalProviderEnum.Local;
			ExternalProviderEnum type;
			if (Enum.TryParse(value, out type))
			{
				return type;
			}
			else
			{
				return ExternalProviderEnum.Local;
			}
		}

		public LoginProfile()
		{
			this.Created = DateTime.Now;
		}
	}
}

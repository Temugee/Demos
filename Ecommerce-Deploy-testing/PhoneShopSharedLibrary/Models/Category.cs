using System;
using System.ComponentModel.DataAnnotations;

namespace PhoneShopSharedLibrary.Models
{
	public class Category
	{
		public int Id { get; set; }
		[Required]
		public string? Name { get; set; }

		// Relationship one to many
		public List<Product>? Products { get; set; }
	}
}


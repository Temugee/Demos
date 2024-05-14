using System;
using System.ComponentModel.DataAnnotations;

namespace PhoneShopSharedLibrary.DTOs
{
	public class UserDTO
	{
		public int Id { get; set; }
		public string? Name { get; set; }
		public string? Email { get; set; }
		public string? Password { get; set; }
		[Required, DataType(DataType.Password), Compare(nameof(Password))]
		public string? ConfirmPassword { get; set; }
	}
}


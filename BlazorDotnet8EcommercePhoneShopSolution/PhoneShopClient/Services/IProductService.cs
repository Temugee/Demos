using System;
using PhoneShopSharedLibrary.Models;
using PhoneShopSharedLibrary.Responses;

namespace PhoneShopClient.Services
{
	public interface IProductService
	{
		Action? ProductAction { get; set; }
		Task<ServiceResponse> AddProduct(Product model);
		Task GetAllProducts(bool featuredProducts);
		List<Product> AllProducts { get; set; }
        List<Product> FeaturedProducts { get; set; }
		List<Product> ProductByCategory { get; set; }
		Task GetProductsByCategory(int categoryId);
		Product GetRandomProduct();
		bool isVisible { get; set; }
    }
}


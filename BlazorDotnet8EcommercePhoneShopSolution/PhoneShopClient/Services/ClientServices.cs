using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using PhoneShopClient.Authentication;
using PhoneShopClient.PrivateModels;
using PhoneShopSharedLibrary.DTOs;
using PhoneShopSharedLibrary.Models;
using PhoneShopSharedLibrary.Responses;

namespace PhoneShopClient.Services
{
    public class ClientServices : IProductService, ICategoryService, IUserAccountService, ICart
    {
        private const string ProductBaseUrl = "api/product";
        private const string CategoryBaseUrl = "api/category";
        private const string AuthenticationBaseUrl = "api/account";
        private readonly HttpClient httpClient;
        private readonly AuthenticationService authenticationService;
        private readonly ILocalStorageService locaStorageService;
        public Action? CategoryAction { get; set; }
        public List<Category> AllCategories { get; set; }
        public Action? ProductAction { get; set; }
        public List<Product> AllProducts { get; set; }
        public List<Product> FeaturedProducts { get; set; }
        public List<Product> ProductByCategory { get; set; }
        public bool isVisible { get; set; }
        public Action? CartAction { get; set; }
        public int CartCount { get; set; }
        public bool IsCartLoaderVisible { get; set; }

        public ClientServices(HttpClient httpClient, AuthenticationService authenticationService, ILocalStorageService locaStorageService)
        {
            this.httpClient = httpClient;
            this.authenticationService = authenticationService;
            this.locaStorageService = locaStorageService;
        }

        //Product

        public async Task<ServiceResponse> AddProduct(Product model)
        {
            await authenticationService.GetUserDetails();
            var privateHttpClient = await authenticationService.AddHeaderToHttpClient();
            var response = await privateHttpClient.PostAsync(ProductBaseUrl, General.GenerateStringContent(General.SerializeObj(model)));
            //Read Response
            var result = CheckResponse(response);
            if (!result.Flag)
                return result;
            var apiResponse = await ReadContent(response);
            
            var data = General.DeserializeJsonString<ServiceResponse>(apiResponse);
            if (!data.Flag) return data;
            await ClearAndGetAllProducts();
            return data;
        }
        private async Task ClearAndGetAllProducts()
        {
            bool featuredProduct = true;
            bool allProduct = false;
            AllProducts = null!;
            FeaturedProducts = null!;
            await GetAllProducts(featuredProduct);
            await GetAllProducts(allProduct);
        }

        public async Task GetAllProducts(bool featuredProducts)
        {
            if (featuredProducts && FeaturedProducts is null)
            {
                isVisible = true;
                FeaturedProducts = await GetProducts(featuredProducts);
                isVisible = false;
                ProductAction?.Invoke();
                return;
            }
            else
            {
                if (!featuredProducts && AllProducts is null)
                {
                    isVisible = true;
                    AllProducts = await GetProducts(featuredProducts);
                    isVisible = false; 
                    ProductAction?.Invoke();
                    return;
                }
            }
            
            

        }
        private async Task<List<Product>> GetProducts(bool featured)
        {
            var response = await httpClient.GetAsync($"{ProductBaseUrl}?featured={featured}");
            var (flag, _) = CheckResponse(response);
            if (!flag) return null!;
            var result = await ReadContent(response);
            return General.DeserializeJsonStringList<Product>(result).ToList();
            
        }
        public async Task GetProductsByCategory(int categoryId)
        {
            bool featured = true;
            await GetAllProducts(featured);
            ProductByCategory = AllProducts.Where(_ => _.CategoryId == categoryId).ToList();
            ProductAction?.Invoke();
        }
        public Product GetRandomProduct()
        {
            if (FeaturedProducts == null || FeaturedProducts.Count == 0)
                return null!;

            Random RandomNumbers = new();
            int minimumNumber = FeaturedProducts.Min(_ => _.Id);
            int maximumNumber = FeaturedProducts.Max(_ => _.Id) + 1;

            if (minimumNumber == maximumNumber)
                return FeaturedProducts.First();

            int result = RandomNumbers.Next(minimumNumber, maximumNumber);
            return FeaturedProducts.FirstOrDefault(_ => _.Id == result)!;
        }


        //Categories

        public async Task<ServiceResponse> AddCategory(Category model)
        {
            await authenticationService.GetUserDetails();
            var privateHttpClient = await authenticationService.AddHeaderToHttpClient();
            var response = await privateHttpClient.PostAsync(CategoryBaseUrl, General.GenerateStringContent(General.SerializeObj(model)));
            //Read Response
            var result = CheckResponse(response);
            if (!result.Flag)
                return result;
            var apiResponse = await ReadContent(response);
            
            var data = General.DeserializeJsonString<ServiceResponse>(apiResponse);
            if (!data.Flag) return data;
            await ClearAndGetAllCategories();
            return data;
        }

        public async Task GetAllCategories()
        {
            if (AllCategories is null)
            {
                var response = await httpClient.GetAsync($"{CategoryBaseUrl}");
                var (flag, _) = CheckResponse(response);
                if (!flag) return;

                var result = await ReadContent(response);
                AllCategories  = General.DeserializeJsonStringList<Category>(result).ToList();
                CategoryAction?.Invoke();
            }
            
            
        }

        private async Task ClearAndGetAllCategories()
        {
            AllCategories = null!;
            await GetAllCategories();
        }
        //General methods
        private static async Task<string> ReadContent(HttpResponseMessage response) => await response.Content.ReadAsStringAsync();
        private static ServiceResponse CheckResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
                return new ServiceResponse(false, "Error occurred. Try again later...");
            return new ServiceResponse(true, null!);
        }

        //authentication
        public async Task<ServiceResponse> Register(UserDTO model)
        {
            var response = await httpClient.PostAsync($"{AuthenticationBaseUrl}/register", General.GenerateStringContent(General.SerializeObj(model)));
            var result = CheckResponse(response);
            if (!result.Flag)
                return result;
            var apiResponse = await ReadContent(response);
            return General.DeserializeJsonString<ServiceResponse>(apiResponse);
        }

        public async Task<LoginResponse> Login(LoginDTO model)
        {
            var response = await httpClient.PostAsync($"{AuthenticationBaseUrl}/login", General.GenerateStringContent(General.SerializeObj(model)));

            if (!response.IsSuccessStatusCode)
                return new LoginResponse(false, "Error occured", null!, null!);

            var apiResponse = await ReadContent(response);
            return General.DeserializeJsonString<LoginResponse>(apiResponse);
        }



        // Cart Service
        public async Task GetCartCount()
        {
            string cartString = await GetCartFromLocalStorage();
            if (string.IsNullOrEmpty(cartString))
                CartCount = 0;
            else
                CartCount = General.DeserializeJsonStringList<StorageCart>(cartString).Count;
            CartAction?.Invoke();
        }

        public async Task<ServiceResponse> AddToCart(Product model, int updateQuantity = 1)
        {
            string message = string.Empty;
            var MyCart = new List<StorageCart>();
            var getCartFromStorage = await GetCartFromLocalStorage();
            if (!string.IsNullOrEmpty(getCartFromStorage))
            {
                MyCart = (List<StorageCart>)General.DeserializeJsonStringList<StorageCart>(getCartFromStorage);
                var checkIfAddedAlready = MyCart.FirstOrDefault(_ => _.ProductId == model.Id);
                if(checkIfAddedAlready is null)
                {
                    MyCart.Add(new StorageCart() { ProductId = model.Id, Quantity = 1 });
                    message = "Product Added to Cart";
                }
                else
                {
                    var updatedProduct = new StorageCart() { Quantity = updateQuantity, ProductId = model.Id };
                    MyCart.Remove(checkIfAddedAlready!);
                    MyCart.Add(updatedProduct);
                    message = "Product Updated";
                }
            }
            else
            {
                MyCart.Add(new StorageCart() { ProductId = model.Id, Quantity = 1 });
                message = "Product Added to Cart";
            }
            await RemoveCartFromLocalStorage();
            await SetCartToLocalStorage(General.SerializeObj(MyCart));
            await GetCartCount();
            return new ServiceResponse(true, message);
        }

        public async Task<List<Order>> MyOrders()
        {
            IsCartLoaderVisible = true;
            var cartList = new List<Order>();
            string myCartString = await GetCartFromLocalStorage();
            if (string.IsNullOrEmpty(myCartString)) return null!;
            var myCartList = General.DeserializeJsonStringList<StorageCart>(myCartString);
            await GetAllProducts(false);
            foreach(var cartItem in myCartList)
            {
                var product = AllProducts.FirstOrDefault(_ => _.Id == cartItem.ProductId);
                cartList.Add(new Order()
                {
                    Id = product!.Id,
                    Name = product.Name,
                    Quantity = cartItem.Quantity,
                    Price = product.Price,
                    Image = product.Base64Img,
                });
            }
            IsCartLoaderVisible = false;
            await GetCartCount();
            return cartList;
        }

        public async Task<ServiceResponse> DeleteCart(Order cart)
        {
            var myCartList = General.DeserializeJsonStringList<StorageCart>(await GetCartFromLocalStorage());
            if (myCartList is null) return new ServiceResponse(false, "Product not found");
            myCartList.Remove(myCartList.FirstOrDefault(_ => _.ProductId == cart.Id)!);
            await RemoveCartFromLocalStorage();
            await SetCartToLocalStorage(General.SerializeObj(myCartList));
            await GetCartCount();
            return new ServiceResponse(true, "Product removed successfully");
        }
        private async Task<string> GetCartFromLocalStorage() => await locaStorageService.GetItemAsStringAsync("cart");
        private async Task SetCartToLocalStorage(string cart) => await locaStorageService.SetItemAsStringAsync("cart", cart);
        private async Task RemoveCartFromLocalStorage() => await locaStorageService.RemoveItemAsync("cart");
    }
}

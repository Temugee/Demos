using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace mauiworkshop
{
    public class Item
    {
        public string PkId { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Barcode { get; set; }
        public decimal Price { get; set; }
    }

    public class ItemData
    {
        private const string ApiEndpoint = "http://localhost:5070/api/info/GetItems";

        private async Task<ObservableCollection<Item>> GetItemsFromApi()
        {
            ObservableCollection<Item> items = new ObservableCollection<Item>();

            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    HttpResponseMessage response = await httpClient.GetAsync(ApiEndpoint);

                    if (response.IsSuccessStatusCode)
                    {
                        using var responseStream = await response.Content.ReadAsStreamAsync();
                        items = await JsonSerializer.DeserializeAsync<ObservableCollection<Item>>(responseStream) ?? new ObservableCollection<Item>();
                    }
                    else
                    {
                        Console.WriteLine($"Failed to retrieve items. Status Code: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving items: {ex.Message}");
            }

            return items;
        }

        public ObservableCollection<Item> Itemees { get; private set; }

        public ItemData()
        {
            // Initialize the collection
            Itemees = new ObservableCollection<Item>();

            // Fetch items from the API and update the collection
            Task.Run(async () =>
            {
                ObservableCollection<Item> apiItems = await GetItemsFromApi();
                    foreach (var item in apiItems)
                    {
                        Itemees.Add(item);
                    }
                
            });
        }
    }
}

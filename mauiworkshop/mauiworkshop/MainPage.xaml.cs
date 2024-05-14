namespace mauiworkshop
{
    using System;
    using System.Net.Http; // Add this line
    using System.Net.Http.Json;
    using DevExpress.iOS.Navigation;
    using DevExpress.Maui.DataGrid;
    using Microsoft.Maui.Controls;
    using ObjCRuntime;
    using static System.Net.WebRequestMethods;

    public partial class MainPage : ContentPage
    {
        private HttpClient httpClient = new HttpClient();

        public MainPage()
        {
            InitializeComponent();
        }

        private async void Swipe_Delete(object sender, SwipeItemTapEventArgs e)
        {
            grid.DeleteRow(e.RowHandle);

            if (e.Item is Item itemToDelete)
            {
                // itemToDelete now contains the data of the item being deleted
                Console.WriteLine($"Deleting item with Id: {itemToDelete.Id}");

                using HttpClient httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.DeleteAsync("http://localhost:5070/api/info/DeleteItems?id=" + itemToDelete.Id);

                string res = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response from server: {res}");
            }
        }

        private async void Grid_ValidateCell(object sender, ValidateCellEventArgs e)
        {
            if (e.Item is Item editedItem)
            {
                Console.WriteLine($"Edited row Id: {editedItem.Id}");
                Console.WriteLine($"Edited row FieldName: {e.FieldName}");
                Console.WriteLine($"Edited row new value: {e.NewValue}");

                // Assuming EditModel class exists and you create an instance named editModel
                Item editModel = new Item
                {
                    PkId = editedItem.PkId,
                    Id = editedItem.Id,
                    Name = editedItem.Name,
                    Barcode = editedItem.Barcode,
                    Price = editedItem.Price
                };

                // Update the corresponding property based on the edited field
                switch (e.FieldName)
                {
                    case nameof(Item.Name):
                        editModel.Name = (string)e.NewValue;
                        break;

                    case nameof(Item.Barcode):
                        editModel.Barcode = (string)e.NewValue;
                        break;

                    case nameof(Item.Price):
                        editModel.Price = Convert.ToDecimal(e.NewValue);
                        break;

                    // Add other cases for additional fields as needed

                    default:
                        // Handle other fields or log a warning
                        Console.WriteLine($"Warning: Unexpected field name - {e.FieldName}");
                        break;
                }

                // Assuming you have a PostAsJsonAsync method for HttpClient
                HttpResponseMessage updateResponse = await httpClient.PostAsJsonAsync("http://localhost:5070/api/info/updateitem", editModel);

                string updateRes = await updateResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"Update response from server: {updateRes}");
            }
        }

    }
}

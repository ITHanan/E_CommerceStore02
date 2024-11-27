using Spectre.Console;
using System.Text.Json;

namespace E_CommerceStore02
{
    public class Customer : IIdentifiable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public List<Order> OrderHistory { get; set; }
        public List<Product> products { get; set; }
        public object product { get; }

        public Customer(int id, string name, string email, string address, List<Order> orderHistory )
        {
            Id = id;
            Name = name;
            Email = email;
            Address = address;
            OrderHistory = new List<Order>();
        }


        public void ViewOrderHistory()
        {
            foreach (var order in OrderHistory)
            {
                Console.WriteLine($"Order ID: {order.Id}, Total Amount: {order.TotalAmount:C}, Order Date: {order.OrderDate}");
            }
        }



        public void AddProductToCart(MyDB myDB)
        {
            try
            {
                var productRepo = new GenericClass<Product>();

                // Add products to the generic repository
                foreach (var product in myDB.AllProductDatafromEHandelsButikDataJSON)
                {
                    productRepo.AddTo(product);
                }

                // Display available products
                AnsiConsole.Markup("[bold green]=== Available Products ===[/]\n");
                foreach (var product in productRepo.GetAll())
                {
                    AnsiConsole.MarkupLine($"[cyan]ID: {product.Id} | Name: {product.Name} | Price: {product.Price:C} | Stock: {product.Stock}[/]");
                }

                // Prompt user for product ID
                AnsiConsole.Markup("[bold blue]Enter the Product ID to add to the cart: [/]");
                if (!int.TryParse(Console.ReadLine(), out int productId))
                {
                    AnsiConsole.MarkupLine("[bold red]Invalid Product ID![/]");
                    return;
                }

                // Find the selected product
                var selectedProduct = productRepo.GetByID(productId);
                if (selectedProduct == null)
                {
                    AnsiConsole.MarkupLine($"[bold red]Product with ID {productId} not found.[/]");
                    return;
                }

                // Check stock availability
                if (selectedProduct.Stock <= 0)
                {
                    AnsiConsole.MarkupLine("[bold red]Sorry, this product is out of stock.[/]");
                    return;
                }

                // Prompt user for quantity
                AnsiConsole.Markup("[bold blue]Enter the quantity to add to the cart: [/]");
                if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0)
                {
                    AnsiConsole.MarkupLine("[bold red]Invalid quantity![/]");
                    return;
                }

                if (quantity > selectedProduct.Stock)
                {
                    AnsiConsole.MarkupLine("[bold red]Not enough stock available.[/]");
                    return;
                }

                // Add to cart
                productRepo.AddTo(selectedProduct);
                myDB.AllProductDatafromEHandelsButikDataJSON.Add(selectedProduct);

                // Update stock using the generic repository
                selectedProduct.Stock -= quantity;
                productRepo.Updater(myDB.AllProductDatafromEHandelsButikDataJSON, selectedProduct, p => p.Id);

                // Success message
                AnsiConsole.MarkupLine($"[bold green]{quantity} unit(s) of {selectedProduct.Name} added to your cart successfully![/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[bold red]An error occurred: {ex.Message}[/]");
            }
            SaveToFile(myDB);
        }


        public void UpdateAddress(MyDB myDB)
        {
            try
            {
                // Step 1: Ask the user for the Customer ID
                AnsiConsole.Markup("[bold blue]Enter Customer ID to update address: [/]");
                if (!int.TryParse(Console.ReadLine(), out int customerId))
                {
                    AnsiConsole.MarkupLine("[bold red]Invalid Customer ID![/]");
                    return;
                }

                // Step 2: Search for the customer in the database using the GenericClass
                var customerRepo = new GenericClass<Customer>();
                foreach (var customer in myDB.AllCustomerDatafromEHandelsButikDataJSON)
                {
                    customerRepo.AddTo(customer);
                }

                var customerToUpdate = customerRepo.GetByID(customerId);
                if (customerToUpdate == null)
                {
                    AnsiConsole.MarkupLine("[bold red]Customer not found![/]");
                    return;
                }

                // Step 3: Display current address
                AnsiConsole.MarkupLine($"[bold green]Current Address:[/] {customerToUpdate.Address}");

                // Step 4: Prompt for new address
                AnsiConsole.Markup("[bold blue]Enter the new address: [/]");
                string newAddress = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(newAddress))
                {
                    AnsiConsole.MarkupLine("[bold red]Address cannot be empty![/]");
                    return;
                }

                // Step 5: Validate new address (You could add further validation here)
                if (newAddress.Length < 5)
                {
                    AnsiConsole.MarkupLine("[bold red]Address is too short. Please provide a valid address.[/]");
                    return;
                }

                // Step 6: Update the address
                customerToUpdate.Address = newAddress;

                // Step 7: Save the changes back to the repository (using the generic class)
                customerRepo.Updater(myDB.AllCustomerDatafromEHandelsButikDataJSON, customerToUpdate, c => c.Id);

                // Step 8: Save the updated database to the JSON file
                SaveToFile(myDB);

                // Step 9: Success message
                AnsiConsole.MarkupLine("[bold green]Customer address updated successfully![/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[bold red]An error occurred: {ex.Message}[/]");
            }

            SaveToFile(myDB);
        }

        public void SaveToFile(MyDB myDB)
        {
            string dataJsonFilePath = "EShoppingStore.json";

            string updatedBankDB = JsonSerializer.Serialize(myDB, new JsonSerializerOptions { WriteIndented = true });


            File.WriteAllText(dataJsonFilePath, updatedBankDB);

            MirrorChangesToProjectRoot("EShoppingStore.json");

        }

        static void MirrorChangesToProjectRoot(string fileName)
        {
            // Get the path to the output directory
            string outputDir = AppDomain.CurrentDomain.BaseDirectory;

            // Get the path to the project root directory
            string projectRootDir = Path.Combine(outputDir, "../../../");

            // Define paths for the source (output directory) and destination (project root)
            string sourceFilePath = Path.Combine(outputDir, fileName);
            string destFilePath = Path.Combine(projectRootDir, fileName);

            // Copy the file if it exists
            if (File.Exists(sourceFilePath))
            {
                File.Copy(sourceFilePath, destFilePath, true); // true to overwrite
                Console.WriteLine($"{fileName} has been mirrored to the project root.");
            }
            else
            {
                Console.WriteLine($"Source file {fileName} not found.");
            }
        }

    }
}

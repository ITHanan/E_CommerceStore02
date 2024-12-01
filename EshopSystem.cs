

using Spectre.Console;
using System.Text.Json;

namespace E_CommerceStore02
{
    public class EshopSystem
    {


        public void DisplayProduct(MyDB myDB)


        {
            var table = new Table()
                .Border(TableBorder.Rounded)
                 .AddColumn("[yellow]ID[/]")
                .AddColumn("[yellow]Name[/]")
                .AddColumn("[yellow]Description[/]")
                .AddColumn("[yellow]Price[/]")
                .AddColumn("[yellow]Stock[/]");

            foreach (var product in myDB.AllProductDatafromEHandelsButikDataJSON)
            {
                table.AddRow(
                    product.Id.ToString(),
                    product.Name,
                    product.Description,
                    product.Price.ToString("C"),
                    product.Stock.ToString()
                );

            }

            AnsiConsole.Write(table);


            Console.WriteLine("\n--------------------------------------------");
        }


        public void AddToCart(MyDB myDB, int customerId, int productId, int quantity)
        {
            try
            {

                // Display header
                Console.WriteLine("=== Add to Cart ===");
                Console.WriteLine("Available Products:");

                // Display available products
                DisplayProduct(myDB);

                // Ask user for product ID
                Console.Write("Enter the Product ID to add to the cart: ");
                if (!int.TryParse(Console.ReadLine(), out int Id))
                {
                    Console.WriteLine("Invalid Product ID!");
                    return;
                }

                // Find the slescted product
                var selectedProduct = myDB.AllProductDatafromEHandelsButikDataJSON.FirstOrDefault(p => p.Id == productId);
                if (selectedProduct == null)
                {
                    Console.WriteLine($"Product with ID {productId} not found.");
                    return;
                }

                // Check stock availability
                if (selectedProduct.Stock <= 0)
                {
                    Console.WriteLine("Sorry, this product is out of stock!");
                    return;
                }

                // Ask for quantity
                Console.Write("Enter quantity to add to the cart: ");
                if (!int.TryParse(Console.ReadLine(), result: out int stock) || quantity <= 0) //stock insted of quantity
                {
                    Console.WriteLine("Invalid quantity!");
                    return;
                }

                if (quantity > selectedProduct.Stock)
                {
                    Console.WriteLine("Not enough stock available!");
                    return;
                }

                //find or create the customer's cart 
                var cart = myDB.AllCartDatafromEHandelsButikDataJSON.FirstOrDefault(cart => cart.Id == productId);

                if (cart == null)
                {
                    cart = new Cart { Id = customerId, Products = new List<Product>() };
                    myDB.AllCartDatafromEHandelsButikDataJSON.Add(cart);

                }



                // Add product to cart


                var cartProduct = cart.Products.FirstOrDefault(p => p.Id == productId);


                if (cartProduct == null)
                {
                    cartProduct = new Product
                    {
                        Id = selectedProduct.Id,
                        Name = selectedProduct.Name,
                        Description = selectedProduct.Description,
                        Price = selectedProduct.Price,
                        Stock = quantity // Treat stock as Quantity in the cast 

                    };
                    cart.Products.Add(cartProduct);
                }


                //   myDB.AllProductDatafromEHandelsButikDataJSON.Add(selectedProduct);

                // Update stock
                selectedProduct.Stock -= quantity;

                // Success message
                Console.WriteLine($"{quantity} unit(s) of {selectedProduct.Name} added to your cart successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            SaveDataToJson(myDB);
        }




        public void ViewCart(MyDB myDB, int customerId)
        {
            try
            {
                // find the cart based on customer knowledge
                var cart = myDB.AllCartDatafromEHandelsButikDataJSON.FirstOrDefault(c => c.Id == customerId);

                if (cart == null || !cart.Products.Any())
                {
                    // if the basket is empty or there is no basket for this customer
                    AnsiConsole.MarkupLine("[bold yellow]Your cart is empty. Add products to your cart before proceeding.[/]");
                    return;
                }

                // show cart contents 
                var table = new Table()
                    .Border(TableBorder.Rounded)
                    .AddColumn("[yellow]Product ID[/]")
                    .AddColumn("[yellow]Product Name[/]")
                    .AddColumn("[yellow]Description[/]")
                    .AddColumn("[yellow]Quantity[/]")
                    .AddColumn("[yellow]Price[/]")
                    .AddColumn("[yellow]Total[/]");

                // calculate the total for the basket
                decimal totalAmount = 0;

                foreach (var product in cart.Products)
                {
                    decimal itemTotal = product.Price * product.Stock; // Stock represents quantity in the cart
                    table.AddRow(
                        product.Id.ToString(),
                        product.Name,
                        product.Description,
                        product.Stock.ToString(),
                        product.Price.ToString("C"),
                        itemTotal.ToString("C")
                    );
                    totalAmount += itemTotal;
                }

                // Display the table and total amount
                AnsiConsole.Write(table);
                AnsiConsole.MarkupLine($"[bold cyan]Total Amount: {totalAmount:C}[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[bold red]An error occurred while viewing your cart: {ex.Message}[/]");
            }
        }




        public void ViewAllCustomers(MyDB myDB)
        {
            try
            {
                if (myDB.AllCustomerDatafromEHandelsButikDataJSON == null || !myDB.AllCustomerDatafromEHandelsButikDataJSON.Any())
                {
                    AnsiConsole.MarkupLine("[bold yellow]No customers found in the database.[/]");
                    return;
                }

                // Create a table for displaying customer information
                var table = new Table()
                    .Border(TableBorder.Rounded)
                    .AddColumn("[yellow]Customer ID[/]")
                    .AddColumn("[yellow]Name[/]")
                    .AddColumn("[yellow]Email[/]")
                    .AddColumn("[yellow]Address[/]")
                    .AddColumn("[yellow]Order Count[/]");

                // Populate the table with customer data
                foreach (var customer in myDB.AllCustomerDatafromEHandelsButikDataJSON)
                {
                    table.AddRow(
                        customer.Id.ToString(),
                        customer.Name,
                        customer.Email,
                        customer.Address,
                        customer.OrderHistory?.Count.ToString() ?? "0"
                    );
                }

                // Display the table
                AnsiConsole.Write(table);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[bold red]An error occurred while viewing customers: {ex.Message}[/]");
            }
        }

        public void Checkout(MyDB myDB, int customerID)
        {
            try
            {
                // Login check
                Console.WriteLine("=== Checkout ===");

                ViewAllCustomers(myDB);

                if (!Login(myDB, out Customer customer))
                {
                    Console.WriteLine("Login required to proceed with checkout.");
                    return;
                }

                int customerId = customer.Id;

                // Retrieve the customer's cart
                var cart = myDB.AllCartDatafromEHandelsButikDataJSON
                               .FirstOrDefault(c => c.Id == customerId);

                if (cart == null || !cart.Products.Any())
                {
                    AnsiConsole.MarkupLine("[bold yellow]Your cart is empty. Add products to your cart before checking out.[/]");
                    return;
                }

                // Calculate total amount
                decimal totalAmount = cart.Products.Sum(p => p.Price * p.Stock);



                // Display order summary
                AnsiConsole.MarkupLine("[bold green]=== Order Summary ===[/]");
                foreach (var product in cart.Products)
                {
                    AnsiConsole.MarkupLine($"- [cyan]{product.Name}[/] | Quantity: [yellow]{product.Stock}[/] | Total: [green]{product.Price * product.Stock:C}[/]");
                }
                AnsiConsole.MarkupLine($"[bold cyan]Total Amount: [green]{totalAmount:C}[/]");

                // Confirm checkout
                AnsiConsole.Markup("[bold yellow]Do you want to proceed with checkout? (yes/no): [/] ");
                var confirmation = Console.ReadLine()?.Trim().ToLower();

                if (confirmation != "yes")
                {
                    AnsiConsole.MarkupLine("[bold red]Checkout canceled.[/]");
                    return;
                }

                // Deduct product stock
                foreach (var product in cart.Products)
                {
                    var inventoryProduct = myDB.AllProductDatafromEHandelsButikDataJSON
                                               .FirstOrDefault(p => p.Id == product.Id);

                    if (inventoryProduct != null)
                    {
                        if (inventoryProduct.Stock < product.Stock)
                        {
                            AnsiConsole.MarkupLine($"[bold red]Not enough stock for product: {product.Name}[/]");
                            return;
                        }
                        inventoryProduct.Stock -= product.Stock;
                    }
                }

                // Create a new order
                var newOrder = new Order(
                    id: myDB.AllOrderDatafromEHandelsButikDataJSON.Count + 1,
                    customer: customer,
                    orderItems: new List<Product>(cart.Products),
                    totalAmount: totalAmount,
                    orderDate: DateTime.Now
                );

                // Add the order to the database
                myDB.AllOrderDatafromEHandelsButikDataJSON.Add(newOrder);

                // Clear the customer's cart
                cart.Products.Clear();

                // Success message
                AnsiConsole.MarkupLine($"[bold green]Checkout successful! Your order ID is {newOrder.Id}.[/]");
                AnsiConsole.MarkupLine($"[bold cyan]Thank you for shopping with us, {customer.Name}![/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[bold red]An error occurred during checkout: {ex.Message}[/]");
            }

            SaveDataToJson(myDB);
        }


        public bool Login(MyDB myDB, out Customer authenticatedCustomer)
        {
            Console.Write("Enter your email: ");
            string email = Console.ReadLine()!;

            Console.Write("Enter your password: ");
            string password = Console.ReadLine()!;

            // Find customer by email and password
            authenticatedCustomer = myDB.AllCustomerDatafromEHandelsButikDataJSON
                                       .FirstOrDefault(c => c.Email == email && c.Password == password)!;

            if (authenticatedCustomer == null)
            {
                Console.WriteLine("Invalid email or password. Please try again.");
                return false;
            }

            Console.WriteLine($"Welcome back, {authenticatedCustomer.Name}!");
            return true;
        }




        static void SaveDataToJson(MyDB myDB)
        {
            try
            {
                string dataJsonFilePath = "EShoppingStore.json";

                string updatedEShopDB = JsonSerializer.Serialize(myDB, new JsonSerializerOptions { WriteIndented = true });


                File.WriteAllText(dataJsonFilePath, updatedEShopDB);

                MirrorChangesToProjectRoot("EShoppingStore.json");


                AnsiConsole.MarkupLine("[bold green]Data saved successfully to JSON file.[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[bold red]An error occurred while saving data: {ex.Message}[/]");
            }
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

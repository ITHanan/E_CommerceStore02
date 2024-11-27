

using Spectre.Console;

namespace E_CommerceStore02
{
    public class Cart
    {
        public int Id { get; set; }
        public List<Product> CartItem { get; set; }

        public Customer Customer { get; set; }
        public decimal TotalAmount { get; set; }

        public Cart(int id, Customer customer , List<Product> products)
        {
            Customer = customer;
            CartItem = new List<Product>();
            TotalAmount = 0;
        }



        public void AddToCart(MyDB myDB)
        {
            try
            {

                // Display header
                Console.WriteLine("=== Add to Cart ===");
                Console.WriteLine("Available Products:");

                // Display available products
                foreach (var product in myDB.AllProductDatafromEHandelsButikDataJSON)
                {
                    Console.WriteLine($"ID: {product.Id} | Name: {product.Name} - {product.Description} | Price: {product.Price:C} | Stock: {product.Stock}");
                }

                // Ask user for product ID
                Console.Write("Enter the Product ID to add to the cart: ");
                if (!int.TryParse(Console.ReadLine(), out int productId))
                {
                    Console.WriteLine("Invalid Product ID!");
                    return;
                }

                // Find product
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
                if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0)
                {
                    Console.WriteLine("Invalid quantity!");
                    return;
                }

                if (quantity > selectedProduct.Stock)
                {
                    Console.WriteLine("Not enough stock available!");
                    return;
                }

                // Add to cart
                myDB.AllProductDatafromEHandelsButikDataJSON.Add(selectedProduct);

                // Update stock
                selectedProduct.Stock -= quantity;

                // Success message
                Console.WriteLine($"{quantity} unit(s) of {selectedProduct.Name} added to your cart successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        public void AddToCart2(MyDB myDB)
        {
            try
            {
                // Prompt user for Customer ID
                AnsiConsole.Markup("[bold blue]Enter your Customer ID to add a product to your cart: [/]");
                if (!int.TryParse(Console.ReadLine(), out int customerId))
                {
                    AnsiConsole.MarkupLine("[bold red]Invalid Customer ID![/]");
                    return;
                }

                // Find the customer cart, or create a new cart if it doesn't exist
                var customerCart = myDB.AllCartDatafromEHandelsButikDataJSON
                                       .FirstOrDefault(c => c.Customer.Id == customerId);

                if (customerCart == null)
                {
                    // If no cart found for this customer, create a new one
                    var customer = myDB.AllCustomerDatafromEHandelsButikDataJSON
                                       .FirstOrDefault(c => c.Id == customerId);

                    if (customer == null)
                    {
                        AnsiConsole.MarkupLine("[bold red]Customer not found![/]");
                        return;
                    }

                    customerCart = new Cart(myDB.AllCartDatafromEHandelsButikDataJSON.Count + 1, customer, CartItem);
                    myDB.AllCartDatafromEHandelsButikDataJSON.Add(customerCart);
                    AnsiConsole.MarkupLine("[bold green]New cart created for the customer![/]");
                }

                // Prompt for Product ID to add to the cart
                AnsiConsole.Markup("[bold blue]Enter Product ID to add to your cart: [/]");
                if (!int.TryParse(Console.ReadLine(), out int productId))
                {
                    AnsiConsole.MarkupLine("[bold red]Invalid Product ID![/]");
                    return;
                }

                // Find the product in the inventory
                var product = myDB.AllProductDatafromEHandelsButikDataJSON
                                   .FirstOrDefault(p => p.Id == productId);

                if (product == null)
                {
                    AnsiConsole.MarkupLine("[bold red]Product not found in the inventory![/]");
                    return;
                }

                // Prompt for the quantity of the product to add to the cart
                AnsiConsole.Markup("[bold blue]Enter the quantity: [/]");
                if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0)
                {
                    AnsiConsole.MarkupLine("[bold red]Invalid quantity! Please enter a positive number.[/]");
                    return;
                }

                // Check if the requested quantity is available in stock
                if (product.Stock < quantity)
                {
                    AnsiConsole.MarkupLine("[bold red]Not enough stock available for the requested quantity![/]");
                    return;
                }

                // Add the product to the cart (check if it's already in the cart)
                var cartProduct = myDB.AllProductDatafromEHandelsButikDataJSON.FirstOrDefault(p => p.Id == productId);

                if (cartProduct != null)
                {
                    // If product is already in cart, update the quantity
                    cartProduct.Stock += quantity;
                    AnsiConsole.MarkupLine($"[bold green]Updated the quantity of {product.Name} in your cart to {cartProduct.Stock}.[/]");
                }
                else
                {
                    // If product is not in the cart, add it
                    var productCopy = new Product(product.Id, product.Name, product.Description ,product.Price, quantity);
                    myDB.AllProductDatafromEHandelsButikDataJSON.Add(productCopy);
                    AnsiConsole.MarkupLine($"[bold green]Added {quantity} unit(s) of {product.Name} to your cart![/]");
                }

                // Update the inventory stock after adding to cart
                product.Stock -= quantity;
                AnsiConsole.MarkupLine($"[bold yellow]Inventory updated! Stock left for {product.Name}: {product.Stock}.[/]");

            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[bold red]An error occurred: {ex.Message}[/]");
            }
        }


    }
}



using Spectre.Console;
using System.Text.Json.Serialization;

namespace E_CommerceStore02
{
    public class Order : IIdentifiable
    {

        [JsonPropertyName("OrderId")]
        public int Id { get; set; }

        [JsonPropertyName("CustomerId")]
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        [JsonPropertyName("OrderItems")]
        public List<Product> OrderItems { get; set; }

        [JsonPropertyName("TotalAmount")]
        public decimal TotalAmount { get; set; }

        [JsonPropertyName("OrderDate")]
        public DateTime OrderDate { get; set; }

        [JsonPropertyName("Status")]
        public string Status { get; set; } = "Pending";

        public Order(int id, Customer customer, List<Product> orderItems, decimal totalAmount, DateTime orderDate)
        {
            Id = id;
            Customer = customer;
            OrderItems = orderItems;
            TotalAmount = totalAmount;
            OrderDate = orderDate;
        }


        public void ViewOrderHistory(MyDB myDB)
        {
            try
            {
                // Prompt user for Customer ID
                AnsiConsole.Markup("[bold blue]Enter your Customer ID to view order history: [/]");

                if (!int.TryParse(Console.ReadLine(), out int customerId))
                {
                    AnsiConsole.MarkupLine("[bold red]Invalid Customer ID![/]");
                    return;
                }

                // Find the customer in the database
                var customer = myDB.AllCustomerDatafromEHandelsButikDataJSON
                                   .FirstOrDefault(c => c.Id == customerId);

                if (customer == null)
                {
                    AnsiConsole.MarkupLine("[bold red]Customer not found![/]");
                    return;
                }

                // Retrieve the orders associated with this customer
                var customerOrders = myDB.AllOrderDatafromEHandelsButikDataJSON
                                         .Where(o => o.Customer != null && o.Customer.Id == customerId)
                                         .ToList();

                if (customerOrders.Count == 0)
                {
                    AnsiConsole.MarkupLine("[bold yellow]No order history found for this customer.[/]");
                    return;
                }

                // Display the order history
                AnsiConsole.MarkupLine("\n[bold green]=== Order History ===[/]");
                foreach (var order in customerOrders)
                {
                    AnsiConsole.MarkupLine($"[bold cyan]Order ID:[/] {order.Id}");
                    AnsiConsole.MarkupLine($"[bold cyan]Order Date:[/] {order.OrderDate.ToShortDateString()}");
                    AnsiConsole.MarkupLine($"[bold cyan]Total Amount:[/] {order.TotalAmount:C}");
                    AnsiConsole.MarkupLine($"[bold cyan]Ordered Items:[/]");

                    // Display the ordered products
                    foreach (var product in order.OrderItems)
                    {
                        AnsiConsole.MarkupLine($"  - [green]{product.Name}[/], [yellow]Quantity: {product.Stock}[/], [cyan]Price: {product.Price:C}[/]");
                    }

                    AnsiConsole.MarkupLine("[gray]-------------------------------------[/]");
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[bold red]An error occurred: {ex.Message}[/]");
            }
        }


        public void Checkout(MyDB myDB)
        {
            try
            {
                // Find the customer
                var customer = myDB.AllCustomerDatafromEHandelsButikDataJSON
                                   .FirstOrDefault(c => c.Id == Id);

                if (customer == null)
                {
                    AnsiConsole.MarkupLine("[bold red]Customer not found![/]");
                    return;
                }

                // Retrieve the customer's cart
                var cart = myDB.AllCartDatafromEHandelsButikDataJSON
                               .FirstOrDefault(c => c.Id == Id);

                if (cart == null || !myDB.AllCartDatafromEHandelsButikDataJSON.Any())
                {
                    AnsiConsole.MarkupLine("[bold yellow]Your cart is empty. Add products to your cart before checking out.[/]");
                    return;
                }

                // Calculate total amount
                decimal totalAmount = myDB.AllProductDatafromEHandelsButikDataJSON.Sum(p => p.Price * p.Stock);

                // Display order summary
                AnsiConsole.MarkupLine("[bold green]=== Order Summary ===[/]");
                foreach (var product in myDB.AllProductDatafromEHandelsButikDataJSON)
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

           Customer. SaveToFile(myDB);
        }




    }
}

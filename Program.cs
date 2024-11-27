using Spectre.Console;
using Figgle;
using System.Text.Json;
using E_CommerceStore02;

class Program
{
    static void Main(string[] args)
    {
        string dataJsonFilePath = "EShoppingStore.json";

        // Display the welcome banner using Figgle
        AnsiConsole.Write(new FigletText("E-Shopping Store").Color(Color.Green));

        // Load data from JSON file
        MyDB myDB;
        if (File.Exists(dataJsonFilePath))
        {
            string allDataAsJson = File.ReadAllText(dataJsonFilePath);
            myDB = JsonSerializer.Deserialize<MyDB>(allDataAsJson)!;
            AnsiConsole.MarkupLine("[bold green]Data loaded successfully from JSON file![/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[bold red]JSON file not found. Exiting application.[/]");
            return;
        }

        // Application Menu
        bool running = true;
        while (running)
        {
            AnsiConsole.MarkupLine("\n[bold green]===[/] [yellow]E-Shopping Store Menu[/] [bold green]===\n[/]");
            AnsiConsole.MarkupLine("[bold cyan]1.[/] [white]View Products[/]");
            AnsiConsole.MarkupLine("[bold cyan]2.[/] [white]Add to Cart[/]");
            AnsiConsole.MarkupLine("[bold cyan]3.[/] [white]View Cart[/]");
            AnsiConsole.MarkupLine("[bold cyan]4.[/] [white]Checkout[/]");
            AnsiConsole.MarkupLine("[bold cyan]5.[/] [white]View Order History[/]");
            AnsiConsole.MarkupLine("[bold cyan]6.[/] [white]Exit[/]");

            var choice = AnsiConsole.Prompt(
                new TextPrompt<string>("[bold blue]Enter your choice:[/] ")
                    .PromptStyle("green")
                    .Validate(input =>
                        input is "1" or "2" or "3" or "4" or "5" or "6" ? ValidationResult.Success() : ValidationResult.Error("[red]Invalid choice![/]")));

            switch (choice)
            {
                case "1":
                    // Display all products
                    AnsiConsole.MarkupLine("\n[bold green]=== Available Products ===[/]");
                    foreach (var product in myDB.AllProductDatafromEHandelsButikDataJSON)
                    {
                        AnsiConsole.MarkupLine($"[cyan]ID: {product.Id} | Name: {product.Name} | Price: {product.Price:C} | Stock: {product.Stock}[/]");
                    }
                    break;

                case "2":
                    // Add to cart
                    var cart = myDB.AllCartDatafromEHandelsButikDataJSON.FirstOrDefault(); // Replace with customer retrieval logic
                    if (cart == null)
                    {
                        cart = new Cart(myDB.AllCartDatafromEHandelsButikDataJSON.Count + 1, customer, new List<Product>());
                        myDB.AllCartDatafromEHandelsButikDataJSON.Add(cart);
                    }
                    cart.AddToCart(myDB);
                    break;

                case "3":
                    // View cart
                    AnsiConsole.MarkupLine("\n[bold green]=== Your Cart ===[/]");
                    foreach (var cartItem in myDB.AllCartDatafromEHandelsButikDataJSON.FirstOrDefault()?.CartItem ?? new List<Product>())
                    {
                        var product = myDB.AllProductDatafromEHandelsButikDataJSON.First(p => p.Id == cartItem.Id);
                        AnsiConsole.MarkupLine($"[yellow]- {product.Name}[/], [cyan]Quantity: {cartItem.Stock}[/], [green]Price: {product.Price:C}[/]");
                    }
                    break;

                case "4":
                    // Checkout
                    var order = new Order(0, null, null, 0, DateTime.Now); // Replace nulls with customer and cart data
                    order.Checkout(myDB);
                    break;

                case "5":
                    // View order history
                    var viewOrder = new Order(0, null, null, 0, DateTime.Now); // Placeholder instance
                    viewOrder.ViewOrderHistory(myDB);
                    break;

                case "6":
                    // Exit the program
                    running = false;
                    AnsiConsole.MarkupLine("[bold yellow]Thank you for using E-Shopping Store! Goodbye![/]");
                    break;
            }

            // Save data back to JSON file after each operation
            SaveDataToJson(myDB, dataJsonFilePath);
        }
    }

    static void SaveDataToJson(MyDB myDB, string dataJsonFilePath)
    {
        try
        {
            string jsonData = JsonSerializer.Serialize(myDB, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(dataJsonFilePath, jsonData);
            AnsiConsole.MarkupLine("[bold green]Data saved successfully to JSON file.[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[bold red]An error occurred while saving data: {ex.Message}[/]");
        }
    }
}


using Spectre.Console;
using Figgle;
using System.Text.Json;
using E_CommerceStore02;

class Program
{
    private static Customer customer;
    static void Main(string[] args)
    {
        EshopSystem eShopSystem = new EshopSystem();

        string dataJsonFilePath = "EShoppingStore.json";
        string allDataAsJson = File.ReadAllText(dataJsonFilePath);
        MyDB myDB = JsonSerializer.Deserialize<MyDB>(allDataAsJson)!;

        // Display the welcome banner using Figgle
        AnsiConsole.Write(new FigletText("E-Shopping Store").Color(Color.Green));



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

                    eShopSystem.DisplayProduct(myDB);

                    break;

                case "2":
                    // Add to cart
                    var cart = myDB.AllCartDatafromEHandelsButikDataJSON.FirstOrDefault(); // Replace with customer retrieval logic
                    if (cart == null)
                    {

                        cart = new Cart(myDB.AllCartDatafromEHandelsButikDataJSON.Count + 1, customer, new List<Product>());
                        myDB.AllCartDatafromEHandelsButikDataJSON.Add(cart);
                    }
                    eShopSystem.AddToCart(myDB,1,1,2);// start to fix from here 
                    break;

                case "3":
                    // View cart
                    AnsiConsole.MarkupLine("\n[bold green]=== Your Cart ===[/]");

                    Console.WriteLine("Enter customer ID:");
                    int customerId = Convert.ToInt32(Console.ReadLine());
                    eShopSystem.ViewCart(myDB, customerId);
                    break;

                case "4":
                    // Checkout
                    // var order = new Order(0, null, null, 0, DateTime.Now); // Replace nulls with customer and cart data
                    Console.WriteLine("Enter customer ID: ");
                    customerId = Convert.ToInt32(Console.ReadLine());
                    eShopSystem.Checkout(myDB, customerId);
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
        }
    }

}


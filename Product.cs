

namespace E_CommerceStore02
{
    public class Product : IIdentifiable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }

        public Product(int id, string name, string description, decimal price, int stock)
        {
            Id = id;
            Name = name;
            Description = description;
            Price = price;
            Stock = stock;
        }

        public void DisplayProduct()
        {
            Console.WriteLine($"[ID: {Id}] {Name}");
            Console.WriteLine($"Description: {Description}");
            Console.WriteLine($"Price: {Price:C}");
            Console.WriteLine($"Stock: {Stock} available");
            Console.WriteLine("--------------------------------------------");
        }

        public bool UpdateStock(int quantity)
        {
            if (Stock >= quantity)
            {
                Stock -= quantity;
                return true;
            }
            else
            {
                Console.WriteLine($"[Error] Insufficient stock for {Name}. Only {Stock} left.");
                return false;
            }
        }

    }
}

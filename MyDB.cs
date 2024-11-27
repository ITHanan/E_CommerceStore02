

using System.Text.Json.Serialization;

namespace E_CommerceStore02
{
    public class MyDB
    {
        [JsonPropertyName("Products")]
        public List<Product> AllProductDatafromEHandelsButikDataJSON { get; set; } = new List<Product>();

        [JsonPropertyName("Customers")]
        public List<Customer> AllCustomerDatafromEHandelsButikDataJSON { get; set; } = new List<Customer>();

        [JsonPropertyName("Orders")]
        public List<Order> AllOrderDatafromEHandelsButikDataJSON { get; set; } = new List<Order>();

        [JsonPropertyName("Carts")]
        public List<Cart> AllCartDatafromEHandelsButikDataJSON { get; set; } = new List<Cart>();
    }
}

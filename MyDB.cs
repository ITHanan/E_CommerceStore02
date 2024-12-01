

using System.Text.Json.Serialization;

namespace E_CommerceStore02
{
    public class MyDB
    {
        [JsonPropertyName("products")]
        public List<Product> AllProductDatafromEHandelsButikDataJSON { get; set; } = new List<Product>();

        [JsonPropertyName("customers")]
        public List<Customer> AllCustomerDatafromEHandelsButikDataJSON { get; set; } = new List<Customer>();

        [JsonPropertyName("orders")]
        public List<Order> AllOrderDatafromEHandelsButikDataJSON { get; set; } = new List<Order>();

        [JsonPropertyName("carts")]
        public List<Cart> AllCartDatafromEHandelsButikDataJSON { get; set; } = new List<Cart>();
    }
}

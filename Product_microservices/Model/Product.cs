namespace Product_microservices.Model
{
    public class Product
    {
        public int Id { get; set; }          // Unique identifier
        public string Name { get; set; }     // Product name
        public string Category { get; set; } // Category (e.g., Electronics, Clothing)
        public decimal Price { get; set; }   // Price of product
        public int Stock { get; set; }       // Available quantity
    }
}

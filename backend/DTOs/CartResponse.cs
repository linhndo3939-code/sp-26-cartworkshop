namespace backend.Dtos;

public class CartResponse
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;

    public List<CartItemResponse> Items { get; set; } = new();

    // COMPUTED: sum of all item quantities (Wednesday, Slide 12)
    public int TotalItems => Items.Sum(i => i.Quantity);

    // COMPUTED: sum of (Price x Quantity) for all items
    public decimal Subtotal => Items.Sum(i => i.LineTotal);

    public decimal Total => Subtotal;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
using System.Collections.Concurrent;

namespace TP_Product.Models.Help
{
    public class CartLine
{
    public Product Prod { get; set; } = null!;
    public int quantite { get; set; }
    public decimal UnitPrice => Convert.ToDecimal(Prod.Price);
    public decimal TotalPrice => UnitPrice * quantite;
}

    public class UserCart
    {
        // Un panier par clé (id utilisateur, ou id de session pour un visiteur)
        private static readonly ConcurrentDictionary<string, UserCart> Carts = new();

        public static UserCart For(string key) => Carts.GetOrAdd(key, _ => new UserCart());

        public List<CartLine> Items { get; } = new();

        // Retourne false si le stock est insuffisant
        public bool AddItem(Product p)
        {
            var line = Items.FirstOrDefault(i => i.Prod.ProductId == p.ProductId);
            int actuel = line?.quantite ?? 0;
            if (actuel + 1 > p.QteStock) return false;

            if (line == null) Items.Add(new CartLine { Prod = p, quantite = 1 });
            else line.quantite++;
            return true;
        }

        public void SetLessOneItem(Product p)
        {
            var line = Items.FirstOrDefault(i => i.Prod.ProductId == p.ProductId);
            if (line == null) return;
            line.quantite--;
            if (line.quantite <= 0) Items.Remove(line);
        }

        public void RemoveItem(Product p)
            => Items.RemoveAll(i => i.Prod.ProductId == p.ProductId);

        public decimal GetSubTotal() => Items.Sum(i => i.TotalPrice);

        public void Clear() => Items.Clear();
    }
}
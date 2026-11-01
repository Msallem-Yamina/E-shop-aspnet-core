namespace TP_Product.Models.Help
{
    public class ListeCart
    {
        public List<Item> Items { get; private set; }
        public static readonly ListeCart Instance;

        static ListeCart()
        {
            Instance = new ListeCart();
            Instance.Items = new List<Item>();
        }

        protected ListeCart() { }

        // Ajoute un produit (ou +1 s'il est déjà dans le panier)
        public void AddItem(Product prod)
        {
            foreach (Item a in Items)
            {
                if (a.Prod.ProductId == prod.ProductId)
                {
                    a.quantite++;
                    return;
                }
            }
            Item newItem = new Item(prod);
            newItem.quantite = 1;
            Items.Add(newItem);
        }

        // Retire 1 unité ; supprime la ligne si la quantité tombe à 0
        public void SetLessOneItem(Product prod)
        {
            foreach (Item a in Items)
            {
                if (a.Prod.ProductId == prod.ProductId)
                {
                    a.quantite--;
                    if (a.quantite <= 0)
                        RemoveItem(a.Prod);
                    return;
                }
            }
        }

        public void SetItemQuantity(Product prod, int quantity)
        {
            if (quantity == 0)
            {
                RemoveItem(prod);
                return;
            }
            foreach (Item a in Items)
            {
                if (a.Prod.ProductId == prod.ProductId)
                {
                    a.quantite = quantity;
                    return;
                }
            }
        }

        public void RemoveItem(Product prod)
        {
            Item t = Items.FirstOrDefault(a => a.Prod.ProductId == prod.ProductId);
            if (t != null)
                Items.Remove(t);
        }

        public float GetSubTotal()
        {
            float subTotal = 0;
            foreach (Item i in Items)
                subTotal += i.TotalPrice;
            return subTotal;
        }
    }
}
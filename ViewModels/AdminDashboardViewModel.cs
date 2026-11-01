using TP_Product.Models;

namespace TP_Product.ViewModels
{
    public class StatLigne
    {
        public string Label { get; set; }
        public int Valeur { get; set; }
    }

    public class AdminDashboardViewModel
    {
        public int NbArticles { get; set; }
        public int NbCategories { get; set; }
        public int NbUtilisateurs { get; set; }
        public int NbCommandes { get; set; }
        public float ChiffreAffaires { get; set; }
        public float ValeurStock { get; set; }
        public int NbRupture { get; set; }
        public int NbStockFaible { get; set; }

        public List<StatLigne> ArticlesParCategorie { get; set; }
        public List<StatLigne> UtilisateursParRole { get; set; }
        public List<StatLigne> ArticlesLesPlusVendus { get; set; }
        public List<Product> ArticlesAReapprovisionner { get; set; }
        public List<Order> DernieresCommandes { get; set; }
    }
}

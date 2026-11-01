using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TP_Product.Models.Repositories;
using TP_Product.ViewModels;

namespace TP_Product.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private const int SeuilStockFaible = 5;

        readonly IProductRepository productRepository;
        readonly ICategorieRepository categorieRepository;
        readonly IOrderRepository orderRepository;
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public DashboardController(IProductRepository productRepository,
            ICategorieRepository categorieRepository,
            IOrderRepository orderRepository,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            this.productRepository = productRepository;
            this.categorieRepository = categorieRepository;
            this.orderRepository = orderRepository;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            if (!User.IsInRole("Admin"))
                return View("Espace");

            var produits = productRepository.GetAll();
            var categories = categorieRepository.GetAll();
            var commandes = orderRepository.GetAll();

            var model = new AdminDashboardViewModel
            {
                NbArticles = produits.Count,
                NbCategories = categories.Count,
                NbUtilisateurs = userManager.Users.Count(),
                NbCommandes = commandes.Count,
                ChiffreAffaires = commandes.Sum(c => c.TotalAmount),
                ValeurStock = produits.Sum(p => p.Price * p.QteStock),
                NbRupture = produits.Count(p => p.QteStock <= 0),
                NbStockFaible = produits.Count(p => p.QteStock > 0 && p.QteStock <= SeuilStockFaible),

                ArticlesParCategorie = categories
                    .Select(c => new StatLigne
                    {
                        Label = c.CategoryName,
                        Valeur = produits.Count(p => p.CategoryId == c.CategoryId)
                    })
                    .OrderByDescending(l => l.Valeur)
                    .ToList(),

                UtilisateursParRole = await CompterUtilisateursParRole(),

                ArticlesLesPlusVendus = commandes
                    .SelectMany(c => c.Items)
                    .GroupBy(i => i.ProductName)
                    .Select(g => new StatLigne { Label = g.Key, Valeur = g.Sum(i => i.Quantity) })
                    .OrderByDescending(l => l.Valeur)
                    .Take(5)
                    .ToList(),

                ArticlesAReapprovisionner = produits
                    .Where(p => p.QteStock <= SeuilStockFaible)
                    .OrderBy(p => p.QteStock)
                    .Take(5)
                    .ToList(),

                DernieresCommandes = commandes.Take(5).ToList()
            };

            return View(model);
        }

        private async Task<List<StatLigne>> CompterUtilisateursParRole()
        {
            var lignes = new List<StatLigne>();
            var idsAvecRole = new HashSet<string>();

            foreach (var role in roleManager.Roles.ToList())
            {
                var membres = await userManager.GetUsersInRoleAsync(role.Name);
                idsAvecRole.UnionWith(membres.Select(u => u.Id));
                lignes.Add(new StatLigne { Label = role.Name, Valeur = membres.Count });
            }

            lignes.Add(new StatLigne
            {
                Label = "Sans rôle",
                Valeur = userManager.Users.Count() - idsAvecRole.Count
            });

            return lignes.OrderByDescending(l => l.Valeur).ToList();
        }
    }
}

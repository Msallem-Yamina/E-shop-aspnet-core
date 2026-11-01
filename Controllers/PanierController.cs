using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TP_Product.Models;
using TP_Product.Models.Help;
using TP_Product.Models.Repositories;
using TP_Product.ViewModels;

namespace TP_Product.Controllers
{
    public class PanierController : Controller
    {
        readonly IProductRepository productRepository;
        readonly IOrderRepository orderRepository;
        private readonly UserManager<IdentityUser> userManager;

        public PanierController(IProductRepository productRepository,
                                IOrderRepository orderRepository,
                                UserManager<IdentityUser> userManager)
        {
            this.productRepository = productRepository;
            this.orderRepository = orderRepository;
            this.userManager = userManager;
        }

        // Panier propre à l'utilisateur connecté (ou au visiteur via la session)
        private UserCart Cart
        {
            get
            {
                var guestId = HttpContext.Session.GetString("CartId");

                if (User.Identity?.IsAuthenticated == true)
                {
                    var userCart = UserCart.For("user_" + userManager.GetUserId(User));

                    // Fusion du panier invité dans le panier du compte
                    if (guestId != null)
                    {
                        var guestCart = UserCart.For("guest_" + guestId);
                        foreach (var line in guestCart.Items.ToList())
                        {
                            var existing = userCart.Items
                                .FirstOrDefault(i => i.Prod.ProductId == line.Prod.ProductId);
                            if (existing == null)
                                userCart.Items.Add(line);
                            else
                                existing.quantite = Math.Min(existing.quantite + line.quantite,
                                                             line.Prod.QteStock);
                        }
                        guestCart.Clear();
                        HttpContext.Session.Remove("CartId");
                    }
                    return userCart;
                }

                if (guestId == null)
                {
                    guestId = Guid.NewGuid().ToString();
                    HttpContext.Session.SetString("CartId", guestId);
                }
                return UserCart.For("guest_" + guestId);
            }
        }
        // GET: /Panier
        public ActionResult Index()
        {
            ViewBag.Liste = Cart.Items;
            ViewBag.total = Cart.GetSubTotal();
            return View();
        }

        // GET: /Panier/AddProduct/5
        public ActionResult AddProduct(int id)
        {
            Product pp = productRepository.GetById(id);
            if (pp != null && !Cart.AddItem(pp))
            {
                TempData["Error"] = $"Stock insuffisant pour « {pp.Name} ».";
                return RedirectToAction("Index", "Product");
            }
            return RedirectToAction("Index");
        }

        // GET: /Panier/Count (nombre d'articles pour le badge de la navbar)
        [HttpGet]
        public ActionResult Count()
        {
            int count = Cart.Items.Sum(i => i.quantite);
            return Json(new { count });
        }

        [HttpPost]
        public ActionResult PlusProduct(int id)
        {
            Product pp = productRepository.GetById(id);
            bool ok = Cart.AddItem(pp);
            var trouve = Cart.Items.FirstOrDefault(a => a.Prod.ProductId == id);

            return Json(new
            {
                ct = 1,
                ok,
                total = Cart.GetSubTotal(),
                quantite = trouve?.quantite ?? 0,
                totalRow = trouve?.TotalPrice ?? 0
            });
        }

        [HttpPost]
        public ActionResult MinusProduct(int id)
        {
            Product pp = productRepository.GetById(id);
            Cart.SetLessOneItem(pp);
            var trouve = Cart.Items.FirstOrDefault(a => a.Prod.ProductId == id);

            return Json(new
            {
                ct = trouve != null ? 1 : 0,
                total = Cart.GetSubTotal(),
                quantite = trouve?.quantite ?? 0,
                totalRow = trouve?.TotalPrice ?? 0
            });
        }

        [HttpPost]
        public ActionResult RemoveProduct(int id)
        {
            Product pp = productRepository.GetById(id);
            Cart.RemoveItem(pp);

            return Json(new { total = Cart.GetSubTotal() });
        }

        // GET: /Panier/Checkout
        [Authorize]
        [HttpGet]
        public IActionResult Checkout()
        {
            var cartItems = Cart.Items.ToList();
            if (!cartItems.Any())
                return RedirectToAction("Index");

            var viewModel = new OrderViewModel
            {
                CartItems = cartItems.Select(item => new CartItemViewModel
                {
                    ProductName = item.Prod.Name,
                    Quantity = item.quantite,
                    Price = item.Prod.Price
                }).ToList(),
                TotalAmount = (float)Cart.GetSubTotal()
            };
            return View(viewModel);
        }

        // POST: /Panier/Checkout
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(OrderViewModel model)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account", new { returnUrl = Request.Path });

            // On recharge toujours le panier côté serveur (on ne fait pas confiance au formulaire)
            var cartItems = Cart.Items.ToList();
            if (!cartItems.Any())
                return RedirectToAction("Index");

            model.CartItems = cartItems.Select(item => new CartItemViewModel
            {
                ProductName = item.Prod.Name,
                Quantity = item.quantite,
                Price = item.Prod.Price
            }).ToList();
            model.TotalAmount = (float)Cart.GetSubTotal();

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Une erreur est survenue. Veuillez vérifier les informations.";
                return View(model);
            }

            var order = new Order
            {
                CustomerName = user.UserName,
                Email = user.Email,
                Address = model.Address,
                TotalAmount = model.TotalAmount,
                OrderDate = DateTime.Now,
                UserId = user.Id,
                Items = model.CartItems.Select(item => new OrderItem
                {
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    Price = item.Price
                }).ToList()
            };

            orderRepository.Add(order);
            Cart.Clear();
            TempData["SuccessMessage"] = "Votre commande a été passée avec succès.";
            return RedirectToAction("Confirmation", new { orderId = order.Id });
        }

        // GET: /Panier/Confirmation
        [Authorize]
        public async Task<IActionResult> Confirmation(int orderId)
        {
            var order = orderRepository.GetById(orderId);
            if (order == null)
                return NotFound();

            // Un utilisateur ne peut voir que ses propres commandes
            var user = await userManager.GetUserAsync(User);
            if (order.UserId != user?.Id)
                return Forbid();

            return View(order);
        }

        [Authorize]
        public async Task<IActionResult> MesCommandes()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account", new { returnUrl = Request.Path });

            var commandes = orderRepository.GetByUserId(user.Id);
            return View(commandes);
        }
    }
}
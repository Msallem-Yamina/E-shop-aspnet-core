using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TP_Product.Models;
using TP_Product.Models.Repositories;

namespace TP_Product.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IProductRepository _productRepository;
    private readonly ICategorieRepository _categorieRepository;

    public HomeController(ILogger<HomeController> logger,
        IProductRepository productRepository,
        ICategorieRepository categorieRepository)
    {
        _logger = logger;
        _productRepository = productRepository;
        _categorieRepository = categorieRepository;
    }

    public IActionResult Index(int? categoryId, int page = 1)
    {
        int pageSize = 4; // Nombre de produits par page

        var produits = _productRepository.GetAllProducts();

        if (categoryId.HasValue)
            produits = produits.Where(p => p.CategoryId == categoryId);

        // Pagination
        var totalProducts = produits.Count();
        var totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

        if (page > totalPages) page = totalPages;
        if (page < 1) page = 1;

        ViewBag.Categories = _categorieRepository.GetAll();
        ViewBag.CategoryId = categoryId;
        ViewBag.TotalProducts = totalProducts;
        ViewBag.TotalPages = totalPages;
        ViewBag.CurrentPage = page;

        return View(produits.Skip((page - 1) * pageSize).Take(pageSize).ToList());
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

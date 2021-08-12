using AutoMapper;
using ElasticSearchCase.Business.Abstract;
using ElasticSearchCase.Business.Dtos;
using ElasticSearchCase.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearchCase.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        public HomeController(ILogger<HomeController> logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            //var model = new ProductListViewModel
            //{
            //    Products = await _productService.GetAll()
            //};
            //return View(model);

            return View();
        }

        public async Task<PartialViewResult> SearchWithQuery(string query)
        {
            var model = new ProductListViewModel
            {
                Products = await _productService.GetElasticSearchDocumentByQuery(query)
            };
             return PartialView("Partial/_AddedProductList", model);
        }
        public JsonResult AddProduct(ProductDto product)
        {
            try
            {
                _productService.Add(product);
                return Json(new { status = true });
            }
            catch (Exception ex)
            {

                return Json(new { status = false, message = ex.Message });
            }
        }
        public async Task<PartialViewResult> GetProductList()
        {
            var model = new ProductListViewModel
            {
                Products = await _productService.GetAll()
            };
            return PartialView("Partial/_AddedProductList", model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

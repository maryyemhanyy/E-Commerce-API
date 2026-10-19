using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Products;
using E_Commerce.Application.Params;
using E_Commerce.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    public class ProductsController(IProductServices productServices) : ApiBaseController
    {
        [HttpGet]
        [Authorize]
        [RedisCacheAttribure(1000)]
        public async Task<ActionResult<PaginationResult<ProductDto>>> GetProducts([FromQuery] ProductQueryParams queryParams, CancellationToken ct)
        {
            var products = await productServices.GetAllProductsAsync(queryParams, ct);
            return ToActionResult(products);
        }
        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<BrandDto>>> GetBrands(CancellationToken ct)
        {
            var brands = await productServices.GetAllBrandsAsync(ct);
            return ToActionResult(brands);
        }
        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<TypeDto>>> GetTypes(CancellationToken ct)
        {
            var types = await productServices.GetAllTypesAsync(ct);
            return ToActionResult(types);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProductById(int id, CancellationToken ct)
        {
            var product = await productServices.GetByIdAsync(id, ct);
            return ToActionResult(product);
        }
    }
}

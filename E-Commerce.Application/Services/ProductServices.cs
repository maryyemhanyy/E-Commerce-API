using AutoMapper;
using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Products;
using E_Commerce.Application.Params;
using E_Commerce.Application.Specifications;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services
{
    public class ProductServices : IProductServices
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public ProductServices(IUnitOfWork unitOfWork , IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        public async Task<Result<IReadOnlyList<BrandDto>>> GetAllBrandsAsync(CancellationToken ct = default)
        {
            var brands = await unitOfWork.GetRepositoryAsync<ProductBrand,int>().GetAllAsync(ct);

            var mappedBrands = mapper.Map<IReadOnlyList<BrandDto>>(brands);

            return Result<IReadOnlyList<BrandDto>>.Ok(mappedBrands);
        }

        public async Task<Result<PaginationResult<ProductDto>>> GetAllProductsAsync(ProductQueryParams queryParams, CancellationToken ct = default)
        {
            var spec = new ProductSpecifications(queryParams);

            var products = await unitOfWork.GetRepositoryAsync<Product,int>().GetAllWithSpecificationsAsync(spec, ct);

            var mappedProducts = mapper.Map<IReadOnlyList<ProductDto>>(products);

            var countSpec = new ProductCountSpecifications(queryParams);

            var totalCount = await unitOfWork.GetRepositoryAsync<Product, int>().GetProductCountWithSpecificationsAsync(countSpec, ct);

            return Result<PaginationResult<ProductDto>>
                .Ok(new PaginationResult<ProductDto>(mappedProducts , queryParams.PageIndex ,  mappedProducts.Count , totalCount));
        }

        public async Task<Result<IReadOnlyList<TypeDto>>> GetAllTypesAsync(CancellationToken ct = default)
        {
            var types = await unitOfWork.GetRepositoryAsync<ProductType, int>().GetAllAsync(ct);

            var mappedtypes = mapper.Map<IReadOnlyList<TypeDto>>(types);

            return Result<IReadOnlyList<TypeDto>>.Ok(mappedtypes);
        }

        public async Task<Result<ProductDto>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var spec = new ProductSpecifications(id);

            var product = await unitOfWork.GetRepositoryAsync<Product, int>().GetByIdWithSpecificationsAsync(spec, ct);

            if (product is null) return Result<ProductDto>.Fail(Error.NotFound("Product not found", $"Product with id:{id} not found"));

            var mappedProduct = mapper.Map<ProductDto>(product);

            return Result<ProductDto>.Ok(mappedProduct);
        }
    }
}

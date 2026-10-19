using AutoMapper;
using AutoMapper.Execution;
using E_Commerce.Application.DTOs.Orders;
using E_Commerce.Domain.Entities.Order;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Profiles
{
    public class OrderItemPictureUrlResolver(IOptions<UrlSettings> options) : IValueResolver<OrderItem, OrderItemDto, string>
    {
        private readonly UrlSettings urlSettings = options.Value;
        public string Resolve(OrderItem source, OrderItemDto destination, string destMember, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(source.ItemOrdered.PictureUrl)) return string.Empty;

            return $"{urlSettings.BaseUrl}/Files/{source.ItemOrdered.PictureUrl}";
        }
    }
}

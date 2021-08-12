using AutoMapper;
using ElasticSearchCase.Business.Dtos;
using ElasticSearchCase.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace ElasticSearchCase.Business.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDto>();
            CreateMap<Product, ProductElasticSearchIndexDto>();
            CreateMap<ProductDto, Product>();
            CreateMap<ProductElasticSearchIndexDto, Product>();
        }
    }
}

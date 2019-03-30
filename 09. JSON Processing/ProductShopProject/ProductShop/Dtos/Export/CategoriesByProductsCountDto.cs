using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductShop.Dtos.Export
{
    public class CategoriesByProductsCountDto
    {
        [JsonProperty("category")]
        public string Name { get; set; }

        [JsonProperty("productsCount")]
        public int ProductsCount { get; set; }

        [JsonProperty("averagePrice")]
        public decimal AveragePrice { get; set; }

        [JsonProperty("totalRevenue")]
        public decimal TotalPriceSum { get; set; }
    }
}

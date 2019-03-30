using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductShop.Dtos.Export
{
    public class CategoriesByProductsCountDto
    {
        [JsonProperty(PropertyName = "category")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "productsCount")]
        public int ProductsCount { get; set; }

        [JsonProperty(PropertyName = "averagePrice")]
        public decimal AveragePrice { get; set; }

        [JsonProperty(PropertyName = "totalRevenue")]
        public decimal TotalPriceSum { get; set; }
    }
}

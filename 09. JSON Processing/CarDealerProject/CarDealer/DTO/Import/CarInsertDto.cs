﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CarDealer.DTO.Import
{
    public class CarInsertDto
    {
        public string Make { get; set; }

        public string Model { get; set; }

        public long TravelledDistance { get; set; }

        public int[] PartsId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace P03_FootballBetting.Data.Models
{
    public class Town
    {
        public Town()
        {
            this.Teams = new List<Team>();
        }

        public int TownId { get; set; }

        public int CountryId { get; set; }
        public Country Country { get; set; }

        public string Name { get; set; }

        public ICollection<Team> Teams { get; set; }
    }
}

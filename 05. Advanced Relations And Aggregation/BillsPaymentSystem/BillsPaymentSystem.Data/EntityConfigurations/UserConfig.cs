﻿using BillsPaymentSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillsPaymentSystem.Data.EntityConfigurations
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(f => f.FirstName)
                   .HasMaxLength(50)
                   .IsUnicode()
                   .IsRequired();

            builder.Property(l => l.LastName)
                   .HasMaxLength(50)
                   .IsUnicode()
                   .IsRequired();

            builder.Property(e => e.Email)
                   .HasMaxLength(80)
                   .IsRequired();

            builder.Property(p => p.Password)
                   .HasMaxLength(25)
                   .IsRequired();
        }
    }
}

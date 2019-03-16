using BillsPaymentSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillsPaymentSystem.Data.EntityConfigurations
{
    public class BankAccountConfig : IEntityTypeConfiguration<BankAccount>
    {
        public void Configure(EntityTypeBuilder<BankAccount> builder)
        {
            builder.Property(b => b.BankName)
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(s => s.SWIFT)
                   .HasMaxLength(20)
                   .IsUnicode(false)
                   .IsRequired();

        }
    }
}

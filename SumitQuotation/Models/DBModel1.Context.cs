﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SumitQuotation.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class QuotationDBEntities : DbContext
    {
        public QuotationDBEntities()
            : base("name=QuotationDBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<tbl_Customer> tbl_Customer { get; set; }
        public virtual DbSet<tbl_Item> tbl_Item { get; set; }
        public virtual DbSet<tbl_Quotation> tbl_Quotation { get; set; }
        public virtual DbSet<tbl_QuotationDetail> tbl_QuotationDetail { get; set; }
        public virtual DbSet<Quotation_View> Quotation_View { get; set; }
    }
}

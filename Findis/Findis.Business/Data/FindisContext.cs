/********************************************************************************
 Copyright (C) 2015 Eric Bataille <e.c.p.bataille@gmail.com>

 This program is free software; you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation; either version 2 of the License, or
 (at your option) any later version.

 This program is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 GNU General Public License for more details.

 You should have received a copy of the GNU General Public License
 along with this program; if not, write to the Free Software
 Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307, USA.
********************************************************************************/


using System;
using System.Data.Entity;
using Findis.Business.Data.Entity;

namespace Findis.Business.Data
{
    internal class FindisContext : DbContext
    {
        #region Constructors
        
        public FindisContext() : base("FindisContext") { }
        
        #endregion Constructors

        static FindisContext()
        {
            // Database initialize
            Database.SetInitializer(new CreateDatabaseIfNotExists<FindisContext>());
            using (var db = new FindisContext())
                db.Database.Initialize(false);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Types().Configure(c => c.ToTable(GetTableName(c.ClrType)));

            modelBuilder.Entity<ContributionEntity>()
                .HasRequired(c => c.Currency).WithMany().WillCascadeOnDelete(false);

            modelBuilder.Entity<CurrencyEntity>().Property(c => c.ExchangeRate).HasPrecision(18, 9);
        }

        private static string GetTableName(Type type)
        {
            if (!type.Name.EndsWith("Entity")) return type.Name;

            if (type.Name.Length < 7) throw new ArgumentException("Cannot have a table named Entity");
            return type.Name.Substring(0, type.Name.Length - 6);
        }

        #region Tables

        public DbSet<PersonEntity> Persons { get; set; }

        public DbSet<EventPersonEntity> EventPersons { get; set; }

        public DbSet<EventEntity> Events { get; set; }
        
        public DbSet<TransactionEntity> Transactions { get; set; }

        public DbSet<ExtraParticipantEntity> ExtraParticipants { get; set; }

        public DbSet<ExcludedParticipantEntity> ExcludedParticipants { get; set; }

        public DbSet<ContributionEntity> Contributions { get; set; }

        public DbSet<CurrencyEntity> Currencies { get; set; }
        
        #endregion Tables
    }
}

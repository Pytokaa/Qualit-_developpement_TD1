﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TD1.Models;

namespace TD1.Models.EntityFramework;

public partial class ProduitDbContext : DbContext
{
    public DbSet<Produit> Produits { get; set; }
    
    public ProduitDbContext()
    {
    }

    public ProduitDbContext(DbContextOptions<ProduitDbContext> options)
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("produit");
        
        modelBuilder.Entity<Produit>(e =>
        {
            e.HasKey(p => p.IdProduit);
            
            e.HasOne(p => p.MarqueNavigation)
                .WithMany(m => m.Produits)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_produits_marque");
            
            e.HasOne(p => p.TypeProduitNavigation)
                .WithMany(m => m.Produits)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_produits_type_produit");
        });
        modelBuilder.Entity<TypeProduit>(e =>
        {
            e.HasKey(p => p.IdTypeProduit);
            
            e.HasMany(p => p.Produits)
                .WithOne(m => m.TypeProduitNavigation)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_produits_type_produit");
        });
        
        modelBuilder.Entity<Marque>(e =>
        {
            e.HasKey(p => p.IdMarque);
            
            e.HasMany(p => p.Produits)
                .WithOne(m => m.MarqueNavigation)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_produits_marque");
        });
        
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TD1.Models.EntityFramework;


namespace TD1.Models;

[Table("t_e_marque_mar")]
public class Marque : IEntity
{
    [Key]
    [Column("id_marque")]
    public int IdMarque { get; set; }
    
    [Column("nom_marque")]
    public string NomMarque { get; set; }
    
    
    //relations avec les autres tables
    
    [InverseProperty(nameof(Product.MarqueNavigation))]
    public virtual ICollection<Product> Produits { get; set; } = new List<Product>();
    
    public int GetId() => IdMarque;
    public string GetName() =>  NomMarque;
}
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TD1.Models.EntityFramework;


namespace TD1.Models;

[Table("t_e_Marque_mar")]
public class Marque : IEntity
{
    [Key]
    [Column("id_marque")]
    public int IdMarque { get; set; }
    
    [Column("nom_marque")]
    public string NomMarque { get; set; }
    
    
    //relations avec les autres tables
    
    [InverseProperty(nameof(Produit.MarqueNavigation))]
    public virtual ICollection<Produit> Produits { get; set; } = new List<Produit>();
    
    public int GetId() => IdMarque;
    public string GetName() =>  NomMarque;
}
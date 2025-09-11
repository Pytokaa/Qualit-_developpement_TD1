using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;



namespace TD1.Models;

[Table("t_e_Marque_mar")]
public partial class Marque
{
    [Key]
    [Column("id_marque")]
    public int IdMarque { get; set; }
    
    [Column("nom_marque")]
    public string NomMarque { get; set; }
    
    
    //relations avec les autres tables
    
    [InverseProperty(nameof(Produit.MarqueNavigation))]
    public virtual ICollection<Produit> Produits { get; set; } = new List<Produit>();
}
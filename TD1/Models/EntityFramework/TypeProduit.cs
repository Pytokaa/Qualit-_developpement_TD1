using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace TD1.Models;


[Table("t_e_typeproduit_typ")]
public partial class TypeProduit
{
    [Key]
    [Column("id_type_produit")]
    public int IdTypeProduit { get; set; }
    
    
    [Column("nom_type_produit")]
    public string NomTypeProduit { get; set; }
    
    //relation avec les autres tables 
    
    [InverseProperty(nameof(Produit.TypeProduitNavigation))]
    public ICollection<Produit>? Produits { get; set; } = new List<Produit>();
    
}
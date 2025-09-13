using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;


namespace TD1.Models;


[Table("t_e_typeproduit_typ")]
public partial class TypeProduit
{
    [Key]
    [Column("id_type_produit")]
    public int IdTypeProduit { get; set; }
    
    [Required(ErrorMessage ="This property must be filled")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "The string length must be between 2 and 50 characters")]
    [Column("nom_type_produit")]
    public string NomTypeProduit { get; set; }
    
    //relation avec les autres tables 
    
    [InverseProperty(nameof(Produit.TypeProduitNavigation))]
    public ICollection<Produit>? Produits { get; set; } = new List<Produit>();
    
}
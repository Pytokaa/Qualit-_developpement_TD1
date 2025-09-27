using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;
using TD1.Models.EntityFramework;


namespace TD1.Models;


[Table("t_e_typeproduit_typ")]
public class TypeProduit : IEntity
{
    [Key]
    [Column("id_type_produit")]
    public int IdTypeProduit { get; set; }
    
    [Required(ErrorMessage ="This property must be filled")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "The string length must be between 2 and 50 characters")]
    [Column("nom_type_produit")]
    public string NomTypeProduit { get; set; }
    
    //relation avec les autres tables 
    
    [InverseProperty(nameof(Product.TypeProduitNavigation))]
    public ICollection<Product>? Produits { get; set; } = new List<Product>();


    public int GetId() => IdTypeProduit;
    public string GetName() => NomTypeProduit;

}
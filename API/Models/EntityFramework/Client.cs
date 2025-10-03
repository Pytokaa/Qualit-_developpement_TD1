using System.ComponentModel.DataAnnotations.Schema;

namespace TD1.Models.EntityFramework;

[Table("t_e_client_cli")]
public class Client : IEntityWithNavigation
{
    int IdClient { get; set; }
    string NomClient { get; set; }
    string PrenomClient { get; set; }

    public string GetName() => NomClient;

    public int GetId() =>  IdClient;
}
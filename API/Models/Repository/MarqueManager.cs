using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TD1.Models;
using TD1.Models.EntityFramework;


namespace TD1.Repository;

public class MarqueManager : GenericManager<Marque>
{
    public MarqueManager(AppDbContext context) : base(context){}
}
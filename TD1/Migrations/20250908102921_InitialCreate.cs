using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TD1.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "produit");

            migrationBuilder.CreateTable(
                name: "t_e_Marque_mar",
                schema: "produit",
                columns: table => new
                {
                    id_marque = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nom_marque = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_Marque_mar", x => x.id_marque);
                });

            migrationBuilder.CreateTable(
                name: "t_e_typeproduit_typ",
                schema: "produit",
                columns: table => new
                {
                    id_type_produit = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nom_type_produit = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_typeproduit_typ", x => x.id_type_produit);
                });

            migrationBuilder.CreateTable(
                name: "t_e_produit_pro",
                schema: "produit",
                columns: table => new
                {
                    id_produit = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nom_produit = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    nom_photo = table.Column<string>(type: "text", nullable: false),
                    uri_photo = table.Column<string>(type: "text", nullable: false),
                    id_type_produit = table.Column<int>(type: "integer", nullable: true),
                    id_marque = table.Column<int>(type: "integer", nullable: true),
                    stock_reel = table.Column<int>(type: "integer", nullable: false),
                    stock_min = table.Column<int>(type: "integer", nullable: false),
                    stock_max = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_e_produit_pro", x => x.id_produit);
                    table.ForeignKey(
                        name: "FK_produits_marque",
                        column: x => x.id_marque,
                        principalSchema: "produit",
                        principalTable: "t_e_Marque_mar",
                        principalColumn: "id_marque");
                    table.ForeignKey(
                        name: "FK_produits_type_produit",
                        column: x => x.id_type_produit,
                        principalSchema: "produit",
                        principalTable: "t_e_typeproduit_typ",
                        principalColumn: "id_type_produit");
                });

            migrationBuilder.CreateIndex(
                name: "IX_t_e_produit_pro_id_marque",
                schema: "produit",
                table: "t_e_produit_pro",
                column: "id_marque");

            migrationBuilder.CreateIndex(
                name: "IX_t_e_produit_pro_id_type_produit",
                schema: "produit",
                table: "t_e_produit_pro",
                column: "id_type_produit");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_e_produit_pro",
                schema: "produit");

            migrationBuilder.DropTable(
                name: "t_e_Marque_mar",
                schema: "produit");

            migrationBuilder.DropTable(
                name: "t_e_typeproduit_typ",
                schema: "produit");
        }
    }
}

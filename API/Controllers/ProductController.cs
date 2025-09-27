using Microsoft.AspNetCore.Mvc;
using TD1.DTO;
using TD1.Models;
using TD1.Repository;
using AutoMapper;

namespace TD1.Controllers;


[Microsoft.AspNetCore.Components.Route("api/[controller]")]
[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IDataRepository<Product> _productManager;

    public ProductController(IDataRepository<Product> manager, IMapper mapper)
    {
        _productManager = manager;
        _mapper = mapper;
    }
    // GET: api/Produits
    /// <summary>
    /// Get the list of products
    /// </summary>
    /// <returns>An HTTP response containing the list of products.</returns>
    /// <response code="200">The request was successful and returned a list of products</response>
    /// <response code="500">An internal server error occurred.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProduitDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ProduitDTO>>> GetAll()
    {
        var produits = (await _productManager.GetAllAsync()).Value;
        var productsDTO = _mapper.Map<IEnumerable<ProduitDTO>>(produits);

        return new ActionResult<IEnumerable<ProduitDTO>>(productsDTO);
    }
    // GET: api/Produits/id/5
    /// <summary>
    /// Retrieves a specific product by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the product to retrieve.</param>
    /// <returns>An HTTP response containing the product corresponding to the identifier.</returns>
    /// <response code="200">The product was found and is returned.</response>
    /// <response code="404">No product was found with the specified identifier.</response>
    /// <response code="500">An internal server error occurred.</response>
    [HttpGet("id/{id}")]
    [ProducesResponseType(typeof(ProduitDetailDTO),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProduitDetailDTO>> GetById(int id)
    {
        var product = await _productManager.GetByIdAsync(id);
        if (product.Value == null)
            return NotFound();
        
        var productDTO = _mapper.Map<ProduitDetailDTO>(product.Value);
        return productDTO;
    }
    // GET: api/Produits/id/{id}
    /// <summary>
    /// Retrieves a specific product by its name.
    /// </summary>
    /// <param name="name">The name of the product to retrieve.</param>
    /// <returns>An HTTP response containing the product corresponding to the name.</returns>
    /// <response code="200">The product was found and is returned.</response>
    /// <response code="404">No product was found with the specified name.</response>
    /// <response code="500">An internal server error occurred.</response>
    [HttpGet("name/{name}")]
    [ProducesResponseType(typeof(ProduitDetailDTO),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProduitDetailDTO>> GetByName(string name)
    {
        var produit = await _productManager.GetByStringAsync(name);
        if (produit.Value == null)
        {
            return NotFound();
        }
        ProduitDetailDTO productDTO = _mapper.Map<ProduitDetailDTO>(produit.Value);
        return productDTO;
    }
    // POST: api/Produits
    /// <summary>
    /// Adds a new product to the database.
    /// </summary>
    /// <param name="product">The product data to add.</param>
    /// <returns>An HTTP response containing the created product.</returns>
    /// <response code="201">The product was successfully created.</response>
    /// <response code="400">The provided data is invalid or incomplete.</response>
    /// <response code="500">An internal server error occurred.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Product), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProduitDetailDTO>> AddProduit(ProduitDetailDTO productDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var product =  _mapper.Map<Product>(productDto);
        await _productManager.AddAsync(product);
        var resultDto = _mapper.Map<ProduitDetailDTO>(product);
        return CreatedAtAction( nameof(GetById), new { id = product.IdProduit }, resultDto);
    }
    // PUT: api/Produits/id/{id}
    /// <summary>
    /// Updates an existing product by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the product to update.</param>
    /// <param name="product">The new data for the product.</param>
    /// <returns>An HTTP response indicating the result of the operation.</returns>
    /// <response code="204">The product was successfully updated.</response>
    /// <response code="400">The provided data is invalid or the identifier does not match the product.</response>
    /// <response code="404">No product was found with the specified identifier.</response>
    /// <response code="500">An internal server error occurred.</response>

    [HttpPut("id/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PutProduit(int id, [FromBody] ProduitDetailDTO productDto)
    {
        if (id != productDto.IdProduit)
        {
            return BadRequest();
        }
        ActionResult<Product?> prodToUpdate = await _productManager.GetByIdAsync(id);

        if (prodToUpdate.Value == null)
        {
            return NotFound();
        }
        var product = _mapper.Map<Product>(productDto);
        await _productManager.UpdateAsync(prodToUpdate.Value, product);
        return NoContent();
    }
    // DELETE: api/Produits/{id}
    /// <summary>
    /// Deletes an existing product by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the product to delete.</param>
    /// <returns>An HTTP response indicating the result of the operation.</returns>
    /// <response code="204">The product was successfully deleted.</response>
    /// <response code="404">No product was found with the specified identifier.</response>
    /// <response code="500">An internal server error occurred.</response>
    [HttpDelete("id/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteProduit(int id) 
    {
        ActionResult<Product?> produitToDelete = await _productManager.GetByIdAsync(id);
        if (produitToDelete.Value == null)
        {
            return NotFound();
        }
        await _productManager.DeleteAsync(produitToDelete.Value);
        return NoContent();
    }
    // GET: api/Produits/brand/brandname
    /// <summary>
    /// Retrieves specifics products by there brand name
    /// </summary>
    /// <param name="brand">The name of the brand of the product to retrieve.</param>
    /// <returns>An HTTP response containing the product corresponding to the identifier.</returns>
    /// <response code="200">The product was found and is returned.</response>
    /// <response code="404">No product was found with the specified brand name.</response>
    /// <response code="500">An internal server error occurred.</response>
    [HttpGet("brand/{brand}")]
    [ProducesResponseType(typeof(IEnumerable<ProduitDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ProduitDTO>>> GetAllProductFilterdByBrand(string brand)
    {
        var produits = (await _productManager.GetListByProperty(p => p.MarqueNavigation.NomMarque == brand)).Value;
        var productsDTO = _mapper.Map<IEnumerable<ProduitDTO>>(produits);

        return new ActionResult<IEnumerable<ProduitDTO>>(productsDTO);
    }
    // GET: api/Produits/productType/producttypename
    /// <summary>
    /// Retrieves a specific product by product type name
    /// </summary>
    /// <param name="productType">The name of the product type to retrieve.</param>
    /// <returns>An HTTP response containing the product corresponding to the product type name.</returns>
    /// <response code="200">The product was found and is returned.</response>
    /// <response code="404">No product was found with the specified identifier.</response>
    /// <response code="500">An internal server error occurred.</response>
    [HttpGet("productType/{productType}")]
    [ProducesResponseType(typeof(IEnumerable<ProduitDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ProduitDTO>>> GetAllProductFilterdByProductType(string productType)
    {
        var produits = (await _productManager.GetListByProperty(p => p.TypeProduitNavigation.NomTypeProduit == productType)).Value;
        var productsDTO = _mapper.Map<IEnumerable<ProduitDTO>>(produits);

        return new ActionResult<IEnumerable<ProduitDTO>>(productsDTO);
    }
    
    
    [HttpGet("productListByName/{name}")]
    [ProducesResponseType(typeof(IEnumerable<ProduitDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ProduitDTO>>> GetAllProductByName(string name)
    {
        var produits = (await _productManager.GetListByProperty(p => p.NomProduit.Contains(name))).Value;
        var productsDTO = _mapper.Map<IEnumerable<ProduitDTO>>(produits);

        return new ActionResult<IEnumerable<ProduitDTO>>(productsDTO);
    }
}




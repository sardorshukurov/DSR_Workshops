using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using NetSchool.Services.Books.Books;
using NetSchool.Services.Books.Books.Models;
using NetSchool.Services.Logger;

namespace NetSchool.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "Product")]
[Route("v{version:apiVersion}/[controller]")]
public class BookController : ControllerBase
{
    private readonly IAppLogger _logger;
    private readonly IBookService _bookService;

    public BookController(IAppLogger logger, IBookService bookService)
    {
        _logger = logger;
        _bookService = bookService;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _bookService.GetAll();
        
        if (result == null)
            return NotFound();
        
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var result = await _bookService.GetById(id);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost("")]
    public async Task<BookModel> Create(CreateBookModel model)
    {
        var result = await _bookService.Create(model);

        return result;
    }

    [HttpPut("{id:guid}")]
    public async Task Update([FromRoute] Guid id, UpdateBookModel model)
    {
        await _bookService.Update(id, model);
    }

    [HttpDelete("{id:guid}")]
    public async Task Delete([FromRoute] Guid id)
    {
        await _bookService.Delete(id);
    }
}
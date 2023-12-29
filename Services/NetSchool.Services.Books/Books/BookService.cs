using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using NetSchool.Common.Validator;
using NetSchool.Context;
using NetSchool.Context.Entities;
using NetSchool.Services.Actions;
using NetSchool.Services.Books.Books.Models;

namespace NetSchool.Services.Books.Books;

public class BookService : IBookService
{
    private readonly IDbContextFactory<MainDbContext> _dbContextFactory;
    private readonly IMapper _mapper;
    private readonly IAction _action;
    private readonly IModelValidator<CreateBookModel> _createValidator;
    private readonly IModelValidator<UpdateBookModel> _updateValidator;
    
    public BookService(
        IDbContextFactory<MainDbContext> dbContextFactory, 
        IMapper mapper,
        IAction action,
        IModelValidator<CreateBookModel> createValidator,
        IModelValidator<UpdateBookModel> updateValidator
        )
    {
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
        _action = action;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }
    
    public async Task<IEnumerable<BookModel>> GetAll()
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();

        var books = await context.Books
            .Include(book => book.Author).ThenInclude(author => author.Detail)
            .Include(book => book.Categories)
            .ToListAsync();
        var result = _mapper.Map<IEnumerable<BookModel>>(books);

        return result;
    }

    public async Task<BookModel> GetById(Guid id)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();

        var book = await context.Books
            .Include(book => book.Author).ThenInclude(author => author.Detail)
            .Include(book => book.Categories)
            .FirstOrDefaultAsync(x => x.Uid == id);
        
        var result = _mapper.Map<BookModel>(book);
        
        return result;
    }

    public async Task<BookModel> Create(CreateBookModel model)
    {
        await _createValidator.CheckAsync(model);
        
        using var context = await _dbContextFactory.CreateDbContextAsync();

        var book = _mapper.Map<Book>(model);

        await context.Books.AddAsync(book);

        await context.SaveChangesAsync();

        await _action.PublicateBook(new PublicateBookModel()
        {
            Id = book.Id,
            Title = book.Title,
            Description = book.Description
        });
        
        return _mapper.Map<BookModel>(book);
    }

    public async Task Update(Guid id, UpdateBookModel model)
    {
        await _updateValidator.CheckAsync(model);
        
        using var context = await _dbContextFactory.CreateDbContextAsync();

        var book = await context.Books.Where(x => x.Uid == id).FirstOrDefaultAsync();
        
        book = _mapper.Map(model, book);
        
        context.Books.Update(book); 

        await context.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        throw new NotImplementedException();
    }
}
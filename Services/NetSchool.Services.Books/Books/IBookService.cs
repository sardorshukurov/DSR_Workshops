using NetSchool.Services.Books.Books.Models;

namespace NetSchool.Services.Books.Books;

public interface IBookService
{
    Task<IEnumerable<BookModel>> GetAll();
    Task<BookModel> GetById(Guid id);
    Task<BookModel> Create(CreateBookModel model);
    Task Update(Guid id, UpdateBookModel model);
    Task Delete(Guid id);
}
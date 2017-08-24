using System.Collections.Generic;
using System.Linq;
using AuthorBookGraphQLAPI.Data;
using AuthorBookGraphQLAPI.Models;

namespace AuthorBookGraphQLAPI.Repositories
{
    public class LibraryRepository : ILibraryRepository
    {
        private LibraryContext _context;

        public LibraryRepository(LibraryContext context)
        {
            _context = context;
        }

        public void AddAuthor(Author author)
        {
            _context.Authors.Add(author);

            // the repository fills the id (instead of using identity columns)
            if (!author.Books.Any()) return;
            foreach (var book in author.Books)
            {
                AddBookForAuthor(author.AuthorId, book);
            }
        }

        public void AddBookForAuthor(int authorId, Book book)
        {
            var author = GetAuthor(authorId);
            author?.Books.Add(book);
        }

        public bool AuthorExists(int authorId)
        {
            return _context.Authors.Any(a => a.AuthorId == authorId);
        }

        public void DeleteAuthor(Author author)
        {
            _context.Authors.Remove(author);
        }

        public void DeleteBook(Book book)
        {
            _context.Books.Remove(book);
        }

        public Author GetAuthor(int authorId)
        {
            return _context.Authors.FirstOrDefault(a => a.AuthorId == authorId);
        }

        public IEnumerable<Author> GetAuthors()
        {
            return _context.Authors.OrderBy(a => a.Name);
        }

        public IEnumerable<Author> GetAuthors(IEnumerable<int> authorIds)
        {
            return _context.Authors.Where(a => authorIds.Contains(a.AuthorId))
                .OrderBy(a => a.Name)
                .ToList();
        }

        public void UpdateAuthor(Author author)
        {
            _context.Update(author);
            _context.Entry(author).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        }

        public Book GetBookForAuthor(int authorId, int bookId)
        {
            return _context.Books.FirstOrDefault(b => b.AuthorId == authorId && b.BookId == bookId);
        }

        public IEnumerable<Book> GetBooksForAuthor(int authorId)
        {
            return _context.Books
                        .Where(b => b.AuthorId == authorId).OrderBy(b => b.Title).ToList();
        }

        public void UpdateBookForAuthor(Book book)
        {
            _context.Attach(book);
            _context.Entry(book).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}
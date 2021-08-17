using BookLibrary.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookLibrary.Services
{
    public class BookService
    {
        private readonly IMongoCollection<Book> _books;

        public BookService(IBooksLibraryDBSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var db = client.GetDatabase(settings.DBName);
            _books = db.GetCollection<Book>(settings.BooksCollectionName);
        }

        public async Task<List<Book>> GetAsync(string owner) => await _books.Find(bk => bk.Owner == owner).ToListAsync();

        public async Task<Book> GetByIdAsync(string id, string owner) => await _books.Find(bk => bk.Id == id && bk.Owner == owner).FirstOrDefaultAsync();

        public async Task<List<Book>> GetAsync(string genre, string owner) => await _books.Find(b => b.Genres.Any(g => g == genre) && b.Owner == owner).ToListAsync();
        public async Task<string> AddAsync(Book book)
        {
            await _books.InsertOneAsync(book);
            return book.Id;
        }

        public async Task UpdateAsync(string id, string owner, Book book) => await _books.ReplaceOneAsync(bk => bk.Owner == owner && bk.Id == id, book);

        public async Task DeleteAsync(string id, string owner) => await _books.DeleteOneAsync(bk => bk.Id == id && bk.Owner == owner);

        public bool BookIsRight(Book book) => book.Genres != null && book.Author != null & book.BookName != null;
    }
}

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

        public async Task<List<Book>> GetAsync() => await _books.Find(_ => true).ToListAsync();

        public async Task<Book> GetByIdAsync(string id) => await _books.Find(bk => bk.Id == id).FirstOrDefaultAsync();

        public async Task<List<Book>> GetAsync(string genre) => await _books.Find(b => b.Genres.Any(g => g == genre)).ToListAsync();
        public async Task<string> AddAsync(Book book)
        {
            await _books.InsertOneAsync(book);
            return book.Id;
        }

        public async Task UpdateAsync(string id, Book book) => await _books.ReplaceOneAsync(bk => bk.Id == id, book);

        public async Task DeleteAsync(string id) => await _books.DeleteOneAsync(bk => bk.Id == id);

        public bool BookIsRight(Book book) => book.Genres != null && book.Author != null & book.BookName != null;
    }
}

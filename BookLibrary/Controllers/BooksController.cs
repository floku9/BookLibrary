using BookLibrary.Models;
using BookLibrary.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookLibrary.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class BooksController : Controller
    {
        
        private readonly BookService _bookService;
        private readonly string _ownerName;

        public BooksController(BookService bookService, IHttpContextAccessor contextAccessor)
        {
            _bookService = bookService;
            _ownerName = contextAccessor.HttpContext.User.Identity.Name;
        }

        /// <summary>
        /// Получить список книг. Если указан жанр, то список книг будет по заданному жанру
        /// </summary>
        /// <param name="genre">Жанр книги. По умолчанию null</param>
        /// <returns></returns>

        [HttpGet]
        [Route("getbooks")]
        public async Task<IActionResult> GetBooks(string genre = null)
        {
           
            List<Book> books = genre == null ? await _bookService.GetAsync(_ownerName) : await _bookService.GetAsync(genre, _ownerName);

            if (books == null)
            {
                return NotFound(new Response()
                {
                    Error = new SimpleError { Message = "По заданным критериями книги не были найдены" }
                });
            }
            return Ok(new Response()
            {
                Result = books
            });
        }
        /// <summary>
        /// Получить информацию о книге по ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getbookinfo")]
        public async Task<IActionResult> GetBookInfo(string id)
        {
            try
            {
                Book book = await _bookService.GetByIdAsync(id, _ownerName);
                if (book == null)
                {
                    return NotFound(new Response
                    {
                        Error = new BookNotFoundByIdError()
                    });
                }
                return Ok(new Response()
                {
                    Result = book
                });
            }
            catch (FormatException)
            {
                return BadRequest(new Response
                {
                    Error = new SimpleError() { Message = "Id книги имеет неправильный формат" }
                });
            };
        }
        /// <summary>
        /// Добавить новую книгу
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        [HttpPost] 
        [Route("add")]
        public async Task<IActionResult> Add(Book book)
        {
            book.Owner = _ownerName;
            if (!_bookService.BookIsRight(book))
                return BadRequest(new Response()
                {
                    Error = new SimpleError() { Message = "Вы заполнили не все необходимые поля книги" }
                });

            var bookId = await _bookService.AddAsync(book);
            return Ok(new Response() { Result = bookId });
        }

        /// <summary>
        /// Обновление книги по ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="book"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update(string id, Book book)
        {
            try
            {
                var findedBook = await _bookService.GetByIdAsync(id, _ownerName);

                if (findedBook == null)
                {
                    return NotFound(new Response()
                    {
                        Error = new BookNotFoundByIdError()
                    });
                }

                else if (!_bookService.BookIsRight(book))
                {
                    return BadRequest(new Response()
                    {
                        Error = new SimpleError() { Message = "Вы заполнили не все необходимые поля книги" }
                    });
                }

                await _bookService.UpdateAsync(id, _ownerName, book);
                return Ok(new Response()
                {
                    Result = "Книга успешно обновлена"
                });
            }
            catch (FormatException)
            {

                return BadRequest(new Response
                {
                    Error = new SimpleError() { Message = "Id книги имеет неправильный формат" }
                });
            }
        }

        /// <summary>
        /// Удалить книгу по ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> Delete(string id)
        {
            var findedBook = await _bookService.GetByIdAsync(id, _ownerName);

            if (findedBook == null)
            {
                return NotFound(new Response()
                {
                    Error = new BookNotFoundByIdError()
                });
            }

            await _bookService.DeleteAsync(id, _ownerName);

            return Ok(new Response()
            {
                Result = "Книга успешно удалена"
            });
        }
    }
}


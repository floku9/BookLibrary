using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookLibrary;
using BookLibrary.Controllers;
using BookLibrary.Models;
using BookLibrary.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace BookLibrary.Test
{
    public class BookLibraryTesting
    {
        public string BookId { get; set; } //���������� ��� ���������� ����������� � ���������� ������ � �����
        BooksLibraryDBSettings _settings = new BooksLibraryDBSettings()
        {
            BooksCollectionName = "Books",
            UsersCollectionName = "Users",
            ConnectionString = "mongodb://localhost:27017",
            DBName = "BooksLibraryDb"
        };
        AuthController _authController;
        BooksController _booksController;

        public BookLibraryTesting()
        {
            _authController = new AuthController(new AuthService(_settings));
            _booksController = new BooksController(new BookService(_settings));
        }

        /// <summary>
        /// ������������ ����������� �� ������������ ������������
        /// </summary>
        [Fact]
        public async Task RightAuthorizeTest()
        {
            //Arrange
            string username = "User123";
            string password = "qweasd123";

            //Act
            var actionResult = await _authController.Login(username, password);

            //Assert
            var okObjResult = actionResult as OkObjectResult;
            Assert.NotNull(okObjResult);

            var model = okObjResult.Value as Response;
            Assert.NotNull(model);
        }

        /// <summary>
        /// ������������ ����������� ������������, �������� ��� � ��
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task NotAuthorizeTest()
        {
            //Arrange
            string username = "foo";
            string password = "foo";

            //Act
            var actionResult = await _authController.Login(username, password);

            //Assert
            Assert.IsType<BadRequestObjectResult>(actionResult);
        }

        /// <summary>
        /// ���� ������ GetBooks
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetBooksTest()
        {
            //Act
            var actionResult = await _booksController.GetBooks();

            //Assert
            Assert.IsType<OkObjectResult>(actionResult);
        }

        /// <summary>
        /// ���� ������ GetBooks � ������
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetBooksByGenreTest()
        {
            //Act
            var actionResult = await _booksController.GetBooks("Novel");

            //Assert
            Assert.IsType<OkObjectResult>(actionResult);
        }

        /// <summary>
        /// ������������ ������ ��������� ����� �� ID, ���������� � ��
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetBookByID()
        {
            //Act
            var actionResult = await _booksController.GetBookInfo("61164a66bcebdc788222203a");

            //Assert
            Assert.IsType<OkObjectResult>(actionResult);
        } 
        
        /// <summary>
        /// ������������ ������ ��������� ����� �� ID, �� ���������� � ��
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetWrongBookByID()
        {
            //Act
            var actionResult = await _booksController.GetBookInfo("61164a66bcebdc788222514f");

            //Assert
            Assert.IsType<NotFoundObjectResult>(actionResult);
        }

        /// <summary>
        /// ������������ ���������� �����
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddBook()
        {
            //Arrange
            Book book = new Book()
            {
                Author = "Anthony Burgess",
                BookName = "A Clockwork Orange",
                Genres = new List<string>() {"Novel", "Satire"}
            };

            //Act
            var okObjectResult = await _booksController.Add(book) as OkObjectResult;

            //Assert
            Assert.NotNull(okObjectResult);

            var model = okObjectResult.Value as Response;
            Assert.NotNull(model.Result);
            BookId = model.Result.ToString();
        }

        /// <summary>
        /// ������������ ���������� ������ �����
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddEmptyBook()
        {
            //Arrange
            Book book = new Book(){};

            //Act
            var actionResult = await _booksController.Add(book);

            //Assert
            Assert.IsType<BadRequestObjectResult>(actionResult);
        }

        /// <summary>
        /// ���� ��� �������� ���������� �����
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task UpdateBook()
        {
            //Arrange
            Book bookToUpdate = new Book
            {
                Author = "Anthony Burgess",
                BookName = "Mozart and the Wolf Gang",
                Genres = new List<string>() { "Novel", "Satire" }
            };

            //Act
            var actionResult = await _booksController.Update(BookId, bookToUpdate);

            //
            Assert.IsType<OkObjectResult>(actionResult);
        }

        /// <summary>
        /// ���� ������������� ���������� �����
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task WrongUpdateBook()
        {
            //Arrange
            Book bookToUpdate = new Book();

            //Act
            var wrongIdActionResult = await _booksController.Update("61164a66bcebdc788222514f", bookToUpdate);
            var wrongBookActionResult = _booksController.Update(BookId, bookToUpdate);

            //Assert
            Assert.IsType<NotFoundObjectResult>(wrongIdActionResult);
            Assert.IsType<BadRequestObjectResult>(wrongBookActionResult);
        } 

        /// <summary>
        /// ���� �������� �����
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task DeleteBook()
        {
            //Act
            var actionResult = await _booksController.Delete(BookId);

            //Assert
            Assert.IsType<OkObjectResult>(actionResult);
        }

        /// <summary>
        /// ���� ������������� �������� �����
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task DeleteWrongBook()
        {
            //Act
            var actionResult = await _booksController.Delete("61164a66bcebdc788222514f");

            //Assert
            Assert.IsType<BadRequestObjectResult>(actionResult);
        }
    }
}

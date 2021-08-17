using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookLibrary.Models
{
    public class BookNotFoundByIdError : IError
    {
        public string Message { get; set; } = "Книги с данным ключом нет в библиотеке";
    }
}

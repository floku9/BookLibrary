using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookLibrary.Models
{
    /// <summary>
    /// Простая ошибка, в которой содержится только сообщение
    /// </summary>
    public class SimpleError : IError
    {
        public string Message { get; set; }
    }
}

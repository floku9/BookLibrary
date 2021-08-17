using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookLibrary.Models
{
    /// <summary>
    /// Формат ответа, который будет передаваться пользователю
    /// </summary>
    public class Response
    {
        public IError Error { get; set; }
        public object Result { get; set; }
    }
}

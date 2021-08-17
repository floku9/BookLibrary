using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookLibrary.Models
{
    public interface IError
    {
        public string Message { get; set; }
    }
}

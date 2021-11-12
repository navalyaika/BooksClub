using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace booksClub.Models
{
    public class UserBook
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public int IdBook { get; set; }
    }
}

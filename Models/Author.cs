using System;
using System.Collections.Generic;

namespace AuthorBookGraphQLAPI.Models
{
    public class Author
    {
        public int AuthorId { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; } = DateTime.Now;
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
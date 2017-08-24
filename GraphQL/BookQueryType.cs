using AuthorBookGraphQLAPI.Models;
using GraphQL.Language.AST;
using GraphQL.Types;

namespace AuthorBookGraphQLAPI.GraphQL
{
    public class BookQueryType : ObjectGraphType<Book>
    {
        public BookQueryType()
        {
            Field(x => x.BookId).Description("The ID of the book");
            Field(x => x.Title).Description("The title of the book");
            Field<AuthorQueryType>("author");
        }
    }
}
using AuthorBookGraphQLAPI.Models;
using GraphQL.Types;

namespace AuthorBookGraphQLAPI.GraphQL
{
    public class AuthorQueryType : ObjectGraphType<Author>
    {
        public AuthorQueryType()
        {
            Field(x => x.AuthorId).Description("The ID of the Author");
            Field(x => x.Name).Description("The name of the author");
            Field(x => x.BirthDate).Description("The author's birth date");
            Field<ListGraphType<BookQueryType>>("books", resolve: context => new Book[] { });
        }
    }
}
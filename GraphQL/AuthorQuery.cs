using AuthorBookGraphQLAPI.Models;
using AuthorBookGraphQLAPI.Repositories;
using GraphQL.Types;

namespace AuthorBookGraphQLAPI.GraphQL
{
    public class AuthorQuery : ObjectGraphType
    {
        public AuthorQuery(ILibraryRepository repo)
        {
            Field<AuthorQueryType>("author",
                arguments: new QueryArguments(
                    new QueryArgument<IntGraphType>() {Name = "authorId"}),
                resolve: context =>
                {
                    var id = context.GetArgument<int>("authorId");
                    return repo.GetAuthor(id);
                });
            Field<ListGraphType<AuthorQueryType>>("authors",
                resolve: context => repo.GetAuthors());
        }
    }
}
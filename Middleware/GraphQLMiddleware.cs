using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AuthorBookGraphQLAPI.GraphQL;
using AuthorBookGraphQLAPI.Repositories;
using GraphQL;
using GraphQL.Http;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace AuthorBookGraphQLAPI.Middleware
{
    /// <summary>
    /// Class to be used in Startup.cs
    /// app.UseGraphQL in Configure
    /// </summary>
    public static class GraphQlMiddlewareExtensions
    {
        public static IApplicationBuilder UseGraphQL(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GraphQLMiddleware>();
        }
    }
    
    public class GraphQLMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILibraryRepository _repo;

        public GraphQLMiddleware(RequestDelegate next, ILibraryRepository repo)
        {
            _next = next;
            _repo = repo;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var sent = false;
            if (httpContext.Request.Path.StartsWithSegments("/graph"))
            {
                using (var sr = new StreamReader(httpContext.Request.Body))
                {
                    var query = await sr.ReadToEndAsync();
                    if (!String.IsNullOrWhiteSpace(query))
                    {
                        var schema = new Schema {Query = new AuthorQuery(_repo)};

                        var result = await new DocumentExecuter()
                            .ExecuteAsync(opts =>
                            {
                                opts.Schema = schema;
                                opts.Query = query;
                            }).ConfigureAwait(false);
                        
                        CheckForErrors(result);

                        await WriteResult(httpContext, result);

                        sent = true;
                    }
                }
                if (!sent)
                {
                    await _next(httpContext);
                }
            }
        }
        
        private async Task WriteResult(HttpContext httpContext, ExecutionResult result)
        {
            var json = new DocumentWriter(indent: true).Write(result);

            httpContext.Response.StatusCode = 200;
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsync(json);
        }

        private void CheckForErrors(ExecutionResult result)
        {
            if (result.Errors?.Count > 0)
            {
                var errors = new List<Exception>();
                foreach (var error in result.Errors)
                {
                    var ex = new Exception(error.Message);
                    if (error.InnerException != null)
                    {
                        ex = new Exception(error.Message, error.InnerException);
                    }
                    errors.Add(ex);
                }
                throw new AggregateException(errors);
            }
        }
    }
}
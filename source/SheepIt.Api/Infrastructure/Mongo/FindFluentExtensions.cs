using System;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace SheepIt.Api.Infrastructure.Mongo
{
    public static class FindFluentExtensions
    {
        public static IFindFluent<TDocument, TDocument> Sort<TDocument>(this IFindFluent<TDocument, TDocument> findFluent,
            Func<SortDefinitionBuilder<TDocument>, SortDefinition<TDocument>> buildSortDefinition)
        {
            var sortDefinitionBuilder = Builders<TDocument>.Sort;

            var sortDefinition = buildSortDefinition(sortDefinitionBuilder);

            return findFluent.Sort(sortDefinition);
        }

        public static async Task<TDocument[]> ToArray<TDocument>(this IFindFluent<TDocument, TDocument> findFluent)
        {
            var list = await findFluent.ToListAsync();

            // todo: doesn't seem very efficient
            return list.ToArray();
        }
    }
}
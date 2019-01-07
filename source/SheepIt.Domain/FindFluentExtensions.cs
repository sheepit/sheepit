using System;
using MongoDB.Driver;

namespace SheepIt.Domain
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

        public static TDocument[] ToArray<TDocument>(this IFindFluent<TDocument, TDocument> findFluent)
        {
            // todo: well, that doesn't seem very fast
            return findFluent
                .ToList()
                .ToArray();
        }
    }
}
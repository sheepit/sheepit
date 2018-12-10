using System;
using System.Collections.Generic;
using System.Linq;

namespace SheepIt.ConsolePrototype
{
    public class ConsoleTable
    {
        public static Builder<T> Containing<T>(IEnumerable<T> data)
        {
            return new Builder<T>(data.ToArray());
        }

        public class Builder<T>
        {
            private readonly T[] _items;

            private readonly List<Column> _columns = new List<Column>();

            public Builder(T[] items)
            {
                _items = items;
            }

            public Builder<T> WithColumn(string header, Func<T, string> valueSelector)
            {
                var values = _items
                    .Select(valueSelector)
                    .ToArray();

                var maxItemWidth = values.Any()
                    ? values.Max(str => str.Length)
                    : 0;

                var maxColumnWidth = Math.Max(header.Length, maxItemWidth);

                _columns.Add(new Column
                {
                    Header = header,
                    Values = values,
                    Width = maxColumnWidth
                });

                return this;
            }

            public void Show()
            {
                foreach (var line in GetLines())
                {
                    Console.WriteLine(line);
                }
            }

            private IEnumerable<string> GetLines()
            {
                if (!_items.Any())
                {
                    yield return "There are no elements.";
                    yield break;
                }

                yield return _columns
                    .Select(column => column.Header.PadRight(column.Width))
                    .JoinWith(" ║ ");

                yield return _columns
                    .Select(column => new string('═', column.Width))
                    .JoinWith("═╬═");

                for (var rowIndex = 0; rowIndex < _items.Length; rowIndex++)
                {
                    // ReSharper disable once AccessToModifiedClosure - it will be executed immediately
                    yield return _columns
                        .Select(column => column.Values[rowIndex].PadRight(column.Width))
                        .JoinWith(" ║ ");
                }
            }

            class Column
            {
                public string Header { get; set; }
                public string[] Values { get; set; }
                public int Width { get; set; }
            }
        }
    }

    public static class StringExtensions
    {
        public static string JoinWith(this IEnumerable<string> items, string separator)
        {
            return string.Join(separator, items);
        }
    }
}
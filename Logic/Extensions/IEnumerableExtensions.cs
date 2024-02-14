namespace Logic.Extensions
{
    internal static class IEnumerableExtensions
    {
        public static Random Rng = new();

        public static TSource? Random<TSource>(this IEnumerable<TSource> source)
        {
            if (!source.Any())
            {
                return default;
            }

            return source.ElementAt(Rng.Next(source.Count()));
        }
    }
}

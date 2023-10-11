using WebAPIUserApp.Models;

namespace WebAPIUserApp.Domain
{
    /// <summary>
    /// Extension class
    /// </summary>
    public static class Extension
    {
        /// <summary>
        /// Extension for LINQ query for User class. It orders the query by the selected property
        /// in the selected direction
        /// </summary>
        /// <param name="query">Query to order</param>
        /// <param name="property">Property name</param>
        /// <param name="direction">Ordering direction</param>
        /// <returns>Ordered query</returns>
        public static IQueryable<User> ApplyOrder(this IQueryable<User> query, string property, Direction direction)
        {
            switch((property.ToLower(), direction))
            {
                case ("id", Direction.asc):
                    return query.OrderBy(x => x.Id);
                case ("id", Direction.desc):
                    return query.OrderByDescending(x => x.Id);
                case ("name", Direction.asc):
                    return query.OrderBy(x => x.Name.ToLower());
                case ("name", Direction.desc):
                    return query.OrderByDescending(x => x.Name.ToLower());
                case ("email", Direction.asc):
                    return query.OrderBy(x => x.Email.ToLower());
                case ("email", Direction.desc):
                    return query.OrderByDescending(x => x.Email.ToLower());
                case ("age", Direction.asc):
                    return query.OrderBy(x => x.Age);
                case ("age", Direction.desc):
                    return query.OrderByDescending(x => x.Age);
                default:
                    throw new Exception("There is no such field to order");
            }
        }
    }
}

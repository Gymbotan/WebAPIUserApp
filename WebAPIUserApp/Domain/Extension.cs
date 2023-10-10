using WebAPIUserApp.Models;

namespace WebAPIUserApp.Domain
{
    public static class Extension
    {
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

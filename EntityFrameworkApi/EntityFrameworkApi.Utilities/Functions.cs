using EntityFrameworkApi.Utilities.Model;
using System.Security.Claims;

namespace EntityFrameworkApi.Utilities
{
    public static class Functions
    {
        public static TokenModel ParseTokenWithoutAdmin(IEnumerable<Claim> Claims, long userId)
        {
            string? id = Claims.Where(x => x.Type == "id").FirstOrDefault()?.Value;
            string? name = Claims.Where(x => x.Type == "name").FirstOrDefault()?.Value;

            if (!long.TryParse(id, out var uid))
                throw new ArgumentNullException();

            if (uid <= 0)
                throw new ArgumentNullException();

            if (uid != userId)
                throw new ArgumentNullException();

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException();

            TokenModel model = new TokenModel()
            {
                Id = uid,
                Name = name
            };

            return model;
        }

        public static TokenModel ParseToken(IEnumerable<Claim> Claims)
        {
            string? id = Claims.Where(x => x.Type == "id").FirstOrDefault()?.Value;
            string? name = Claims.Where(x => x.Type == "name").FirstOrDefault()?.Value;

            if (!long.TryParse(id, out var uid))
                throw new ArgumentNullException();

            if (uid <= 0)
                throw new ArgumentNullException();

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException();

            TokenModel model = new TokenModel()
            {
                Id = uid,
                Name = name
            };

            return model;
        }
    }
}

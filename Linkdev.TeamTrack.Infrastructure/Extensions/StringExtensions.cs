using System.Text.Json;

namespace Linkdev.TeamTrack.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        public static string ToCamelCase(this string input, bool applyDot = true)
        {
            if (applyDot)
            {
                return input.Split('.').Select(x => x.ToCamelCase(false)).Aggregate((x, y) => $"{x}.{y}");
            }
            return JsonNamingPolicy.CamelCase.ConvertName(input);
        }
    }
}

namespace MusicPlayerMcp.Core
{
    public class ValidationUtils
    {
        public static bool IsValidUri(string value)
        {
            return Uri.TryCreate(value, UriKind.Absolute, out var uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }
    }
}

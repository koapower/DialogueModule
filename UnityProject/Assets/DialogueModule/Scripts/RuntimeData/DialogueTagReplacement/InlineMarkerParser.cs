using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DialogueModule
{
    public static class InlineMarkerParser
    {
        private static readonly Regex markerRegex = new Regex(@"<(speed|fx|sfx)=([^>]+)>", RegexOptions.Compiled);

        public static string Parse(string input, out List<InlineMarker> markers)
        {
            markers = new List<InlineMarker>();
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            var output = new StringBuilder();
            int lastIndex = 0;

            foreach (Match match in markerRegex.Matches(input))
            {
                output.Append(input, lastIndex, match.Index - lastIndex);
                lastIndex = match.Index + match.Length;

                var kind = match.Groups[1].Value switch
                {
                    "speed" => MarkerKind.Speed,
                    "fx" => MarkerKind.Fx,
                    "sfx" => MarkerKind.Sfx,
                    _ => MarkerKind.Speed,
                };

                markers.Add(new InlineMarker
                {
                    charIndex = output.Length,
                    kind = kind,
                    value = match.Groups[2].Value,
                });
            }

            output.Append(input, lastIndex, input.Length - lastIndex);
            return output.ToString();
        }
    }
}

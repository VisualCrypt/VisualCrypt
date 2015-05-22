using System.Text.RegularExpressions;

namespace VisualCrypt.Desktop.Features.FindReplace
{
    public static class SearchStrategy
    {
        /// <summary>
        /// OK
        /// </summary>
        public static SearchResult? Search(string source, string what, int startThisSearchAt, SearchOptions searchOptions)
        {
            var regEx = CreateRegEx(what, searchOptions);

            Match match = regEx.Match(source, startThisSearchAt);
            if (!match.Success)
                return null;

            var searchResult = new SearchResult
            {
                Value = match.Value,
                Index = match.Index,
                Lenght = match.Length,
            };
            return searchResult;
        }


        static Regex CreateRegEx(string what, SearchOptions searchOptions)
        {
            var options = RegexOptions.Compiled;
            string expression;

            if (searchOptions.UseRegEx)
                expression = what;
            else
                expression = Regex.Escape(what);

            if (!searchOptions.MatchCase)
                options |= RegexOptions.IgnoreCase;

            if (searchOptions.MatchWholeWord)
                expression = "\\b" + expression + "\\b";

            if (searchOptions.SearchUp)
                options |= RegexOptions.RightToLeft;


            var regEx = new Regex(expression, options);
            return regEx;
        }
    }
}

using System.Collections.Generic;

namespace Core.StopWords
{
    public static class StopWordsConstants
    {
        public static Dictionary<string, string> StopWords => new Dictionary<string, string>
        {
            {"ar", "arabic"},
            {"bg", "bulgarian"},
            {"ca", "catalan"},
            {"cs", "czech"},
            {"da", "danish"},
            {"nl", "dutch"},
            {"en", "english"},
            {"fi", "finnish"},
            {"fr", "french"},
            {"de", "german"},
            {"gu", "gujarati"},
            {"he", "hebrew"},
            {"hi", "hindi"},
            {"hu", "hungarian"},
            {"id", "indonesian"},
            {"ms", "malaysian"},
            {"it", "italian"},
            {"nb", "norwegian"},
            {"pl", "polish"},
            {"pt", "portuguese"},
            {"ro", "romanian"},
            {"ru", "russian"},
            {"sk", "slovak"},
            {"es", "spanish"},
            {"sv", "swedish"},
            {"tr", "turkish"},
            {"uk", "ukrainian"},
            {"vi", "vietnamese"}
        };
    }
}
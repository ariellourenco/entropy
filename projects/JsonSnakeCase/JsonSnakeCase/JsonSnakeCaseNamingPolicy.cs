using System.Buffers;

namespace System.Text.Json
{
    public sealed class JsonSnakeCaseNamingPolicy : JsonNamingPolicy
    {
        private const char Separator = '_';

        public override string ConvertName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;

            if (name.Length == 1)
                return name.ToLowerInvariant();

            // The worst-case is we need to insert a separator between every char in the appx (s.Length * 2).
            var result = new StringBuilder(2 * (name.Length - 1));
            var wroteUnderscorePreviously = false;

            for (int i = 0; i < name.Length; i++)
            {
                var current = name[i];

                if (i > 0 && i < name.Length - 1 && char.IsLetter(current))
                {
                    // Text somewhere in the middle of the string.
                    var previous = name[i - 1];
                    var next = name[i + 1];

                    if (char.IsLetter(previous) && char.IsLetter(next))
                    {
                        // In the middle of a bit of text
                        var previousUpper = char.IsUpper(previous);
                        var currentUpper = char.IsUpper(current);
                        var nextUpper = char.IsUpper(next);

                        switch ((previousUpper, currentUpper, nextUpper))
                        {
                            case (false, false, false): // aaa
                            case (true,  true,  true): // AAA
                            case (true, false, false): // Aaa
                            {
                                // same word
                                result.Append(char.ToLowerInvariant(current));
                                wroteUnderscorePreviously = false;
                                break;
                            }

                            case (false, false,  true): // aaA
                            case ( true, false,  true): // AaA
                            {
                                // end of word
                                result.Append(char.ToLowerInvariant(current));
                                result.Append(Separator);
                                wroteUnderscorePreviously = true;
                                break;
                            }

                            case (false,  true,  true): // aAA
                            case ( true,  true, false): // AAa
                            case (false,  true, false): // aAa
                            {
                                // beginning of word
                                if (!wroteUnderscorePreviously)
                                {
                                    result.Append(Separator);
                                }
                                result.Append(char.ToLowerInvariant(current));
                                wroteUnderscorePreviously = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        // Beginning or end of text
                        result.Append(char.ToLowerInvariant(current));
                        wroteUnderscorePreviously = false;
                    }
                }
                else if (char.IsLetter(current))
                {
                    // Text at the beginning or the end of the string
                    result.Append(char.ToLowerInvariant(current));
                    wroteUnderscorePreviously = false;
                }
                else if (char.IsNumber(current))
                {
                    // A number at any point in the string
                    if (i > 0 && !wroteUnderscorePreviously)
                        result.Append(Separator);

                    result.Append(current);
                    wroteUnderscorePreviously = false;
                    
                    if (i < name.Length - 1)
                    {
                        result.Append(Separator);
                        wroteUnderscorePreviously = true;
                    }
                }
                else if (!wroteUnderscorePreviously)
                {
                    // Any punctuation at any point in the string
                    result.Append(Separator);
                    wroteUnderscorePreviously = true;
                }
            }

            return result.ToString();
        }
    }
}
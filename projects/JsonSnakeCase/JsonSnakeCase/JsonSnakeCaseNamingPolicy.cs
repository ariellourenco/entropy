using System.Buffers;

namespace System.Text.Json
{
    /// <summary>
    /// Implements the Snake Case naming policy used to convert a string-based name to compound words or phrases that are
    /// separated by an underscore instead of by spaces.
    /// <para>Basic Snake Case Capitalization Rules:</para>
    /// <list type="bullet">
    /// <item><description>Ignores leading/trailing whitespace.</description></item>
    /// <item><description>Collapse multiple underscores into one.</description></item>
    /// <item><description>Numbers are only treated as separate words if followed by capital letter.</description></item>
    /// <item><description>Sequence of consecutive capital letter is considered one "word".</description></item>
    /// <item><description>Ignore punctuation.</description></item>
    /// </list>
    /// </summary>
    public sealed class JsonSnakeCaseNamingPolicy : JsonNamingPolicy
    {
        private const char Separator = '_';

        public override string ConvertName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;

            if (name.Length == 1)
                return name.ToLowerInvariant();

            // The worst-case is we need to insert a separator between every char in
            // the appx (s.Length * 2).
            char[] arr = ArrayPool<char>.Shared.Rent(2 * (name.Length - 1));
            bool wroteUnderscorePreviously = false;
            int position = 0;

            for (int i = 0; i < name.Length; i++)
            {
                var current = name[i];

                if (i > 0 && i < name.Length - 1 && char.IsLetter(current))
                {
                    // Char somewhere in the middle of the string.
                    var previous = name[i - 1];
                    var next = name[i + 1];

                    if (char.IsLetter(previous) && char.IsLetter(next))
                    {
                        switch ((char.IsUpper(previous), char.IsUpper(current), char.IsUpper(next)))
                        {
                            case (false, false, false): // aaa
                            case (true, true, true):  // AAA
                            case (true, false, false):  // Aaa
                            {
                                // same word
                                arr[position++] = char.ToLowerInvariant(current);
                                wroteUnderscorePreviously = false;
                                break;
                            }

                            case (false, false, true): // aaA
                            case (true, false, true): // AaA
                            {
                                // end of word
                                arr[position++] = char.ToLowerInvariant(current);
                                arr[position++] = Separator;
                                wroteUnderscorePreviously = true;
                                break;
                            }

                            case (false, true, true): // aAA
                            case (true, true, false): // AAa
                            case (false, true, false): // aAa
                            {
                                // beginning of word
                                if (!wroteUnderscorePreviously)
                                    arr[position++] = Separator;

                                arr[position++] = char.ToLowerInvariant(current);
                                wroteUnderscorePreviously = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        // Beginning or end of text
                        arr[position++] = char.ToLowerInvariant(current);
                        wroteUnderscorePreviously = false;
                    }
                }
                else if (char.IsLetter(current))
                {
                    // Char at the beginning or the end of the string
                    arr[position++] = char.ToLowerInvariant(current);
                    wroteUnderscorePreviously = false;
                }
                else if (char.IsNumber(current))
                {
                    // A number at any point in the string
                    if (i > 0 && !wroteUnderscorePreviously)
                        arr[position++] = Separator;

                    arr[position++] = current;
                    wroteUnderscorePreviously = false;

                    if (i < name.Length - 1)
                    {
                        arr[position++] = Separator;
                        wroteUnderscorePreviously = true;
                    }
                }
                else if (!wroteUnderscorePreviously)
                {
                    // Collapse multiple underscores/punctuation/whitespces into one at
                    // any point in the string.
                    arr[position++] = Separator;
                    wroteUnderscorePreviously = true;
                }
            }

            ArrayPool<char>.Shared.Return(arr);

            return new string(arr, 0, position);
        }
    }
}

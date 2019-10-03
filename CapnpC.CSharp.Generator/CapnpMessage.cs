using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

[assembly: InternalsVisibleTo("CapnpC.CSharp.Generator.Tests")]

namespace CapnpC.CSharp.Generator
{
    /// <summary>
    /// Represents a capnp.exe output message
    /// </summary>
    public class CapnpMessage
    {
        // capnp outputs look like this:
        //   empty.capnp:1:1: error: File does not declare an ID.  I've generated one for you.  Add this line to your file: @0xc82955a0c779197d;
        //   f:\code\invalid.capnp:9:7-8: error: Ordinal @0 originally used here.
        // Parsing them is harder than it seems because the colon may be part of the file name (as in the example above).
        // And it becomes even worse! NTFS has a rarely used feature called "alternate data streams", identified by a colon:
        //   f:\code\somefile:stream.capnp:9:7-8: error: Ordinal @0 originally used here.
        // What about a name which looks like a line number? (Hint: the 10 denotes the alternate data stream)
        //   f:\code\somefile:10:9:7-8: error: Ordinal @0 originally used here.
        // Watching for the *last* colon as message separator does not work either, either. See first example.
        // Strategy: Watch out for the *last* occurence of pattern :[line]:[column]

        static readonly Regex LineColumnRegex = new Regex(@":(?<Line>\d+):(?<Column>\d+)(-(?<EndColumn>\d+))?:", RegexOptions.Compiled | RegexOptions.RightToLeft);

        /// <summary>
        /// Constructs an instance from given message
        /// </summary>
        /// <param name="fullMessage">output message (one line)</param>
        public CapnpMessage(string fullMessage)
        {
            FullMessage = fullMessage;

            var match = LineColumnRegex.Match(fullMessage);

            if (match.Success)
            {
                IsParseSuccess = true;
                FileName = fullMessage.Substring(0, match.Index);
                var lineMatch = match.Groups["Line"];
                if (lineMatch.Success)
                {
                    int.TryParse(lineMatch.Value, out int value);
                    Line = value;
                }
                var columnMatch = match.Groups["Column"];
                if (columnMatch.Success)
                {
                    int.TryParse(columnMatch.Value, out int value);
                    Column = value;
                }
                var endColumnMatch = match.Groups["EndColumn"];
                if (endColumnMatch.Success)
                {
                    int.TryParse(endColumnMatch.Value, out int value);
                    EndColumn = value;
                }

                int restIndex = match.Index + match.Length;
                int bodyIndex = fullMessage.IndexOf(':', restIndex);

                if (bodyIndex >= 0)
                {
                    Category = fullMessage.Substring(restIndex, bodyIndex - restIndex).Trim();
                    MessageText = fullMessage.Substring(bodyIndex + 1).Trim();
                }
                else
                {
                    // Never observed "in the wild", just in case...
                    Category = string.Empty;
                    MessageText = fullMessage.Substring(restIndex).Trim();
                }
            }
        }

        /// <summary>
        /// The original message
        /// </summary>
        public string FullMessage { get; }

        /// <summary>
        /// Whether the message could be decompsed into [filename]:[line]:[column]: [category]: [text]
        /// </summary>
        public bool IsParseSuccess { get; }

        /// <summary>
        /// Parsed file name (null iff not IsParseSuccess)
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Parsed line (0 if not IsParseSuccess)
        /// </summary>
        public int Line { get; }

        /// <summary>
        /// Parsed column (0 if not IsParseSuccess)
        /// </summary>
        public int Column { get; }

        /// <summary>
        /// Parsed end column (0 if there is none)
        /// </summary>
        public int EndColumn { get; }

        /// <summary>
        /// Parsed category (e.g. "error", null iff not IsParseSuccess)
        /// </summary>
        public string Category { get; }

        /// <summary>
        /// Parsed message body text (0 if not IsParseSuccess)
        /// </summary>
        public string MessageText { get; }
    }
}

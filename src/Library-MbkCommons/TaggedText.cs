/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  MbkCommons Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  Mbk.Commons
 *  File:       TaggedText.cs
 *  Created:    2011-04-26
 *  Modified:   2011-04-29
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;
using System.Text;

namespace Mbk.Commons
{
    /// <summary>
    /// Specifies how an object or text is horizontally aligned.
    /// </summary>
    /// 
    public enum TextAlign : int
    {
        Left = 0,
        Center,
        Right
    }

    /// <summary>
    /// Represents immutable Tagged Text association between some object, text and 
    /// string[] fields. It is commonly used to tag some text with an object 
    /// (or vice versa).
    /// </summary>
    /// 
    public struct TaggedText
    {
        /// <summary>
        /// An empty tagged object. Association between empty ("") string and
        /// null object.
        /// </summary>
        /// 
        public static readonly TaggedText Empty = new TaggedText( string.Empty );

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Properties ]

        /// <summary>
        /// Gets the text part of this TaggedText.
        /// </summary>
        /// 
        public string Text { get; private set; }

        /// <summary>
        /// Gets the object tag of this TaggedText.
        /// </summary>
        /// 
        public object Tag { get; private set; }

        /// <summary>
        /// Gets the fields string[] of the TaggedText.
        /// </summary>
        /// 
        public string[] Fields { get; private set; }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constructor ]

        /// <summary>
        /// Initializes a new instance of the TaggedText class with the specified
        /// text, object tag and fields. 
        /// </summary>
        /// 
        public TaggedText( string text, object tag = null, string[] fields = null )
            : this ()
        {
            this.Text = text;
            this.Tag  = tag;
            this.Fields = fields;
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Public Methods ]

        /// <summary>
        /// Returns a new TaggedText in which the text part is replaced with a new text.
        /// </summary>
        /// 
        public TaggedText Replace( string newText )
        {
            return new TaggedText( newText, this.Tag, this.Fields );
        }

        /// <summary>
        /// Returns a new TaggedText in which the tag is replaced with a new tag.
        /// </summary>
        /// 
        public TaggedText UpdateTag( object newTag )
        {
            return new TaggedText( this.Text, newTag, this.Fields );
        }

        /// <summary>
        /// Returns a new TaggedText in which the fields is replaced with new fields.
        /// </summary>
        /// 
        public TaggedText UpdateFields( string[] newFields )
        {
            return new TaggedText( this.Text, this.Tag, newFields );
        }

        /// <summary>
        /// Returns a new TaggedText in which the text of the left and right
        /// TaggedText are concatenated.
        /// </summary>
        /// 
        public static TaggedText operator + ( TaggedText left, TaggedText right )
        {
            return new TaggedText( left.Text + right.Text, left.Tag, left.Fields );
        }

        /// <summary>
        /// Returns a new TaggedText in which the text of the left TaggedText and right
        /// string are concatenated.
        /// </summary>
        /// 
        public static TaggedText operator + ( TaggedText left, string right )
        {
            return new TaggedText( left.Text + right, left.Tag, left.Fields );
        }

        /// <summary>
        /// Returns a new TaggedText constructed from a string value.
        /// </summary>
        /// 
        public static explicit operator TaggedText( string value )
        {
            return new TaggedText( value );
        }

        /// <summary>
        /// Returns the text part of the TaggedText
        /// </summary>
        /// 
        public override string ToString ()
        {
            return this.Text;
        }

        /////////////////////////////////////////////////////////////////////////////////

        public string AlignedText( int maxWidth, 
            TextAlign textAlign = TextAlign.Left, int padLeft = 0, int padRight = 0
            )
        {
            return AlignedText( this.Text, maxWidth, textAlign, padLeft, padRight );
        }

        /////////////////////////////////////////////////////////////////////////////////

        public static TaggedTextCollection SplitTextInLines( string text, int splitAt = 0 )
        {
            return SplitTextInLines( text, ref splitAt );
        }

        /////////////////////////////////////////////////////////////////////////////////

        public static TaggedTextCollection SplitTextInLines( string text, 
            ref int maxLineWidth )
        {
            int lineWrapAt = maxLineWidth;

            maxLineWidth = 0;
            TaggedTextCollection lines = new TaggedTextCollection ();

            if ( text == null )
            {
                return lines;
            }

            // Add CRLF delimitted chunks of text to lines collection.
            // 'start' points to beginning of next line
            // 'end' points to next CRLF or end of text

            const string delimiter = "\n";
            int start = 0;

            while( start < text.Length )
            {
                // Find next CRLF (if any)
                //
                int end = text.IndexOf( delimiter, start );
                int skipChars = delimiter.Length;

                // Calculate length of the chunk between line 'start' and next CRLF
                // (or text length if CRLF is not found).
                //
                int len = end < 0 ? text.Length - start : end - start;

                if ( lineWrapAt > 0 && len > lineWrapAt )
                {
                    skipChars = 0;

                    // If line should wrap, find space between words
                    //
                    for ( len = lineWrapAt; len > 0; --len )
                    {
                        char c = text[ start + len ];

                        // Skip all (found) spaces (between words) backwards
                        //
                        if ( char.IsWhiteSpace( c ) )
                        {
                            ++skipChars;

                            while( len > 0
                                && char.IsWhiteSpace( text[ start + len - 1 ] ) )
                            {
                                ++skipChars;
                                --len;
                            }
                        }

                        // If found space between words or other delimiter character
                        //
                        if ( skipChars > 0 || c == '\\' || c == '/' )
                        {
                            break;
                        }
                    }

                    if ( len <= 0 ) // not found any space between words
                    {
                        len = lineWrapAt; // reset to original 'wrap at' position
                    }
                }

                maxLineWidth = Math.Max( len, maxLineWidth );

                // Add chunk to lines colletion
                //
                lines.Add( TaggedText.ClearFromControlCharacters( 
                    text.Substring( start, len ) ) );

                // Move 'start' at the beginning of next line (i.e. skip CRLF)
                //
                start += len + skipChars;
            }

            return lines;
        }

        /////////////////////////////////////////////////////////////////////////////////

        public static TaggedText ClearFromControlCharacters( 
            string text, object tag = null )
        {
            StringBuilder sb = new StringBuilder ();

            foreach( char c in text )
            {
                if ( c == '\t' )
                {
                    sb.Append( ' ', 4 - ( sb.Length % 4 ) );
                }
                else if ( c == '\r' )
                {
                    /* ignore carriage-returns */
                }
                else if ( char.IsWhiteSpace( c ) )
                {
                    sb.Append( ' ' );
                }
                else if ( ! char.IsControl( c ) )
                {
                    sb.Append( c );
                }
            }
            return new TaggedText( sb.ToString (), tag );
        }

        /////////////////////////////////////////////////////////////////////////////////

        public static string Join( string delimiter, TaggedTextCollection lines )
        {
            StringBuilder sb = new StringBuilder ();

            foreach ( TaggedText t in lines )
            {
                if ( sb.Length == 0 ) 
                {
                    sb.Append( t.Text );
                }
                else
                {
                    sb.Append( delimiter ).Append( t.Text );
                }
            }

            return sb.ToString ();
        }

        /////////////////////////////////////////////////////////////////////////////////

        public static string AlignedText( string text, int maxWidth, 
            TextAlign textAlign = TextAlign.Left, int padLeft = 0, int padRight = 0
            )
        {
            StringBuilder sb = new StringBuilder ();

            if ( maxWidth <= 0 )
            {
                return sb.ToString ();
            }

            int reducedWidth = maxWidth - padLeft - padRight;

            if ( reducedWidth <= 0 || text == null )
            {
                sb.Append( ' ', maxWidth );
            }
            else if ( text.Length <= reducedWidth )
            {
                sb.Append( ' ', padLeft );

                switch( textAlign )
                {
                    case TextAlign.Left:
                        sb.Append( text );
                        sb.Append( ' ', reducedWidth - text.Length );
                        break;

                    case TextAlign.Center:
                        int left = ( reducedWidth - text.Length ) / 2;
                        sb.Append( ' ', left );
                        sb.Append( text );
                        sb.Append( ' ', reducedWidth - text.Length - left );
                        break;

                    case TextAlign.Right:
                        sb.Append( ' ', reducedWidth - text.Length );
                        sb.Append( text );
                        break;
                }

                sb.Append( ' ', padRight );
            }
            else // text.Lenth > reducedWidth
            {
                sb.Append( ' ', padLeft );

                switch( textAlign )
                {
                    case TextAlign.Left:
                        sb.Append( text, 0, reducedWidth );
                        break;

                    case TextAlign.Center:
                        int left = ( text.Length - reducedWidth ) / 2;
                        sb.Append( text, left, reducedWidth );
                        break;

                    case TextAlign.Right:
                        left = text.Length - reducedWidth;
                        sb.Append( text, left, reducedWidth );
                        break;
                }

                sb.Append( ' ', padRight );
            }

            return sb.ToString ();
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////
    }
}
/* --------------------------------------------------------------------------------------
 *  KTH ID132V Laboration 4
 *  Video Rental Outlet (VRO) Library
 * --------------------------------------------------------------------------------------
 *  Namespace:  VROLib.CustomerDatabase
 *  File:       CreditCard.cs
 *  Created:    2011-03-12
 *  Modified:   2011-05-01
 * --------------------------------------------------------------------------------------
 *  Author:     Mikica B Kocic
 *  License:    Creative Commons - GNU General Public License 
 *              http://creativecommons.org/licenses/GPL/2.0/
 * --------------------------------------------------------------------------------------
 */

using System;
using System.Text;
using Mbk.Commons;

namespace VROLib.CustomerDatabase
{
    /// <summary>
    /// CreditCard structure encapsulates immutable credit card information, like its 
    /// type, number and expiry date.
    /// </summary>
    /// 
    [Serializable]
    public struct CreditCard
    {
        /////////////////////////////////////////////////////////////////////////////////

        #region [ Properties ]

        /// <summary>
        /// Credit Card type like VISA, MasterCard etc.
        /// </summary>
        /// 
        public CreditCardType Type { get; private set; }

        /// <summary>
        /// Credit Card number: 12-16 digits depending on credit card type.
        /// </summary>
        /// 
        public string Number { get; private set; }

        /// <summary>
        /// The last date the credit card is valid thru.
        /// </summary>
        /// 
        public DateTime ValidThru { get; private set; }

        /// <summary>
        /// True if credit card has expired, i.e. 'now' has passed valid thru date.
        /// </summary>
        /// 
        public bool Expired
        { 
            get { return DateTime.Now > this.ValidThru; }
        }

        /// <summary>
        /// Returns string representation of valid thru date in ISO 'yyyy-MM' format.
        /// </summary>
        /// 
        public string VerboseValidThru
        {
            get { return ValidThru.ToString( "yyyy-MM" ); }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the CreditCard structure.
        /// </summary>
        /// <param name="type">Type of Credit Card (VISA etc.)</param>
        /// <param name="number">Credit Card number</param>
        /// <param name="expYear">Expiry year; two-digit years are assumed to belong to 
        /// 21st century.</param>
        /// <param name="expMonth">Expiry month; 1-12</param>
        /// 
        public CreditCard( CreditCardType type, string number, 
                int expYear, int expMonth )
            : this ()
        {
            string notValidInfo = Validate( type, number );

            if ( notValidInfo != null )
            {
                throw new ArgumentException( notValidInfo );
            }

            this.Type = type;
            this.Number = number;

            // Set last valid thru date (the last date of the expiry month). Valid thru
            // date is calculated as the beginning of the next month minus one day.
            // Two-digit years are assumed to belong to 21st century.
            //
            this.ValidThru = new DateTime( 
                    expYear < 2000 ? expYear + 2000 : expYear, expMonth, 1 
                ).AddMonths( 1 ).AddDays( -1 );
        }

        /// <summary>
        /// Initializes a new instance of the CreditCard structure.
        /// </summary>
        /// <param name="type">Type of Credit Card (VISA etc.)</param>
        /// <param name="number">Credit Card number</param>
        /// <param name="validThru">A string with date in ISO "yyyy-MM" format</param>
        /// 
        public CreditCard( CreditCardType type, string number, string validThru )
            : this ()
        {
            string notValidInfo = Validate( type, number );

            if ( notValidInfo != null )
            {
                throw new ArgumentException( notValidInfo );
            }

            if ( string.IsNullOrEmpty( validThru ) )
            {
                throw new ArgumentException( "Valid Thru must not be null" );
            }

            this.Type = type;
            this.Number = number;

            try
            {
                this.ValidThru = DateTime.ParseExact( validThru, "yyyy-M", 
                    System.Globalization.CultureInfo.InvariantCulture );
            }
            catch( Exception )
            {
                throw new ArgumentException( 
                    "Valid Thru must be a date in 'yyyy-MM' format" );
            }
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Credit Card Number Validation ]

        /// <summary>
        /// Validates credit card number against type of the credit card.
        /// Uses Luhn Algorithm to validate digits.
        /// </summary>
        /// <returns>True if credit card number is valid.</returns>
        /// 
        public static string Validate( CreditCardType cardType, string cardNumber )
        {
            if ( string.IsNullOrEmpty( cardNumber ) )
            {
                return "Credit Card number must not be null or empty";
            }

            // Validate card type and establish required length
            //
            int requiredLength = -1;
            switch ( cardType )
            {
                case CreditCardType.MasterCard:
                    requiredLength = 16;
                    break;

                case CreditCardType.BankCard:
                    requiredLength = 16;
                    break;

                case CreditCardType.VISA:
                    requiredLength = 16;
                    break;

                case CreditCardType.AmericanExpress:
                    requiredLength = 15;
                    break;

                case CreditCardType.Discover:
                    requiredLength = 16;
                    break;

                case CreditCardType.DinersClub:
                    requiredLength = 14;
                    break;

                case CreditCardType.JCB:
                    requiredLength = 16;
                    break;

                default:
                    return "Unknown Credit Card Type: " + cardType;
            }

            // Remove non-digits
            //
            int length = 0;
            int[] digits = new int[ requiredLength ];

            for ( int i = 0; i < cardNumber.Length; i++ )
            {
                if ( char.IsDigit( cardNumber, i ) )
                {
                    if ( length >= requiredLength )
                    {
                        return "Invalid number of digits in Credit Card Number.\n"
                            + "Maximum length is " + requiredLength + " digits.";
                    }

                    digits[ length++ ] = int.Parse( "" + cardNumber[ i ] );
                }
                else if ( cardNumber[ i ] != '-' )
                {
                    return "Invalid character in Credit Card Number.\n"
                        + "Allowed characters are digits and hyphen.";
                }
            }

            if ( requiredLength != length )
            {
                return "Invalid number of digits in Credit Card Number.\n"
                    + "Required length is " + requiredLength + " digits.";
            }

            // Use Luhn Algorithm to validate digits
            //
            int sum = 0;
            for(int i = length - 1; i >= 0; --i )
            {
                if ( i % 2 == length % 2 )
                {
                    int n = digits[i] * 2;
                    sum += (n / 10) + (n % 10);
                }
                else
                {
                    sum += digits[i];
                }
            }

            if ( sum % 10 != 0 )
            {
                return cardType.Verbose () + " Credit Card number " + cardNumber
                    + " has invalid checksum.";
            }

            return null;
        }

        #endregion 

        /////////////////////////////////////////////////////////////////////////////////

        #region [ Overriden Virtual Method ToString() ]

        /// <summary>
        /// Converts the value of this instance to its equivalent string representation.
        /// </summary>
        /// 
        public override string ToString ()
        {
            StringBuilder sb = new StringBuilder ();

            if ( this.Type != CreditCardType.None )
            {
                sb.Append( this.Type )
                  .Append( ": " )
                  .Append( this.Number )
                  .Append( ", Valid thru: " )
                  .Append( this.VerboseValidThru );
            }

            return sb.ToString ();
        }

        #endregion

        /////////////////////////////////////////////////////////////////////////////////
    }
}
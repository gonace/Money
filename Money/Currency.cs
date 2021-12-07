﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Money.Exceptions;

namespace Money
{
    public class Currency : IComparable
    {
        private static readonly ConcurrentDictionary<string, Currency> Currencies = new ConcurrentDictionary<string, Currency>();

        #region Currencies

        public static readonly Currency AED = new Currency(CultureInfo.GetCultureInfo("ar-AE"), 100, 784, "Fils", 100, 25, new[] { "DH", "Dhs" });
        public static readonly Currency AFN = new Currency(CultureInfo.GetCultureInfo("ps-AF"), 100, 971, "Pul", 100, 100, new[] { "Af", "Afs" });
        public static readonly Currency ALL = new Currency(CultureInfo.GetCultureInfo("sq-AL"), 100, 008, "Qintar", 100, 100, new[] { "Lek" });
        public static readonly Currency AMD = new Currency(CultureInfo.GetCultureInfo("hy-AM"), 100, 051, "Luma", 100, 10, new[] { "dram" });
        public static readonly Currency BHD = new Currency(CultureInfo.GetCultureInfo("ar-BH"), 100, 048, "Fils", 1000, 5, new[] { "BD" }, symbolFirst: true);
        public static readonly Currency CHF = new Currency(CultureInfo.GetCultureInfo("de-CH"), 100, 756, "Rappen", 100, 5, new [] { "SFr", "Fr" }, format: "%u%n", symbolFirst: true);
        public static readonly Currency DZD = new Currency(CultureInfo.GetCultureInfo("ar-DZ"), 100, 012, "Centime", 100, 100, new[] { "DA" });
        public static readonly Currency EGP = new Currency(CultureInfo.GetCultureInfo("ar-EG"), 100, 818, "Piastre", 100, 25, new[] { "LE", "E£", "L.E." }, symbolFirst: true);
        public static readonly Currency ETB = new Currency(CultureInfo.GetCultureInfo("am-ET"), 100, 230, "Santim", 100, 1);
        public static readonly Currency IQD = new Currency(CultureInfo.GetCultureInfo("ar-IQ"), 100, 368, "Fils", 1000, 50000);
        public static readonly Currency JOD = new Currency(CultureInfo.GetCultureInfo("ar-JO"), 100, 400, "Fils", 1000, 5, new[] { "JD" });
        public static readonly Currency KWD = new Currency(CultureInfo.GetCultureInfo("ar-KW"), 100, 414, "Fils", 1000, 5, new[] { "K.D." });
        public static readonly Currency LBP = new Currency(CultureInfo.GetCultureInfo("ar-LB"), 100, 422, "Piastre", 100, 25000, new[] { "£", "L£" }, symbolFirst: true);
        public static readonly Currency SEK = new Currency(CultureInfo.GetCultureInfo("sv-SE"), 100, 752, "Öre", 100, 100, new []{ ":-" });
        public static readonly Currency USD = new Currency(CultureInfo.GetCultureInfo("en-US"), 100, 840, "Cent", 100, 1, new []{ "US$" }, symbolFirst: true);
        public static readonly Currency ZAR = new Currency(CultureInfo.GetCultureInfo("af-ZA"), 100, 710, "Cent", 100, 10, symbolFirst: true);

        #endregion


        #region Properties

        public int Priority { get; }
        public string Code { get; }
        public decimal? Number { get; }
        public string Name { get; }
        public string Format { get; set; }
        public string[] Symbols { get; }
        public bool SymbolFirst { get; }

        public string Centesimal { get; }
        public int CentesimalConversion { get; }

        public string DecimalMark { get; }
        public string ThousandsSeparator { get; }
        public int SmallestDenomination { get; }
        public string HtmlEntity { get; }

        public string Symbol => Symbols.First();

        #endregion

        #region Constructors
        private Currency(
            CultureInfo culture,
            int priority,
            decimal number,
            string centesimal,
            int centesimalConversion,
            int smallestDenomination,
            IEnumerable<string> alternateSymbols = null,
            string format = null,
            bool symbolFirst = false,
            string htmlEntity = null)
        {
            var region = new RegionInfo(culture.Name);
            var symbols = new List<string> { culture.NumberFormat.CurrencySymbol };

            if (alternateSymbols != null && alternateSymbols.Any())
                symbols.AddRange(alternateSymbols);

            Priority = priority;
            Code = region.ISOCurrencySymbol;
            Number = number;
            Name = region.CurrencyEnglishName;
            Format = string.IsNullOrWhiteSpace(format) ? symbolFirst ? "%u %n" : "%n %u" : format;
            Symbols = symbols.ToArray();
            SymbolFirst = symbolFirst;
            Centesimal = centesimal;
            CentesimalConversion = centesimalConversion;
            DecimalMark = culture.NumberFormat.CurrencyDecimalSeparator;
            ThousandsSeparator = culture.NumberFormat.CurrencyGroupSeparator;
            SmallestDenomination = smallestDenomination;
            HtmlEntity = htmlEntity;

            Currencies.AddOrUpdate(Code, this, (k, v) => v);
        }

        private Currency(
            int priority,
            string code,
            decimal number,
            string name,
            string format,
            string centesimal,
            int centesimalConversion,
            string decimalMark,
            string thousandsSeparator,
            int smallestDenomination,
            IEnumerable<string> symbols,
            bool symbolFirst = false,
            string htmlEntity = null)
        {

            Priority = priority;
            Code = code;
            Number = number;
            Name = name;
            Format = format;
            Symbols = symbols.ToArray();
            SymbolFirst = symbolFirst;
            Centesimal = centesimal;
            CentesimalConversion = centesimalConversion;
            DecimalMark = decimalMark;
            ThousandsSeparator = thousandsSeparator;
            SmallestDenomination = smallestDenomination;
            HtmlEntity = htmlEntity;

            Currencies.AddOrUpdate(Code, this, (k, v) => v);
        }
        #endregion

        #region Methods
        /// <summary>
        ///  TODO
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static Currency Get(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new UnknownCurrencyException(code);

            if (!Currencies.ContainsKey(code))
                throw new UnknownCurrencyException(code);

            Currencies.TryGetValue(code, out var currency);

            return currency;
        }

        /// <summary>
        /// Lookup a currency with given +id+ an returns a +Currency+ instance on
        /// success, +null+ otherwise.
        ///
        /// @param [String, Symbol, #to_s] id Used to look into +table+ and
        /// retrieve the applicable attributes.
        ///
        /// @return [Money::Currency]
        ///
        /// @example
        ///   Money.Currency.Find("eur") #=> #<Money.Currency Id: "eur" ...>
        ///   Money.Currency.Find("foo") #=> null
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static Currency Find(string code)
        {
            try
            {
                return Get(code);
            }
            catch (UnknownCurrencyException)
            {
                return null;
            }
        }

        /// <summary>
        /// Lookup a currency with given +num+ as an ISO 4217 numeric and returns an
        /// +Currency+ instance on success, +null+ otherwise.
        ///
        /// @param [#to_s] num used to look into +table+ in +iso_numeric+ and find
        /// the right currency id.
        ///
        /// @return [Money.Currency]
        ///
        /// @example
        ///   Money.Currency.Find(978) #=> #<Money.Currency Id: "eur" ...>
        ///   Money.Currency.Find(51) #=> #<Money.Currency Id: "amd" ...>
        ///   Money.Currency.Find('001') #=> nil
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static Currency Find(int number)
        {
            try
            {
                var currency = Currencies.Values.FirstOrDefault(x => x.Number == number);

                if (currency == null)
                    throw new UnknownCurrencyException(number);

                return currency;
            }
            catch (UnknownCurrencyException)
            {
                return null;
            }
        }

        public static IEnumerable<Currency> All()
        {
            return Currencies.Values;
        }

        public int CompareTo(object obj)
        {
            var currency = (Currency)obj;
            return String.CompareOrdinal(Code, currency.Code);
        }

        /// <summary>
        /// Returns the relation between subunit and unit as a base 10 exponent.
        ///
        ///  Note that MGA and MRU are exceptions and are rounded to 1
        ///  @see https://en.wikipedia.org/wiki/ISO_4217#Active_codes
        /// </summary>
        /// <returns>[Integer]</returns>
        public int Exponent => (int)Math.Round(Math.Log10(CentesimalConversion), MidpointRounding.ToEven);

        /// <summary>
        /// Returns if a code currency is ISO.
        /// </summary>
        public bool IsIso => Number != null;
        #endregion

        #region Overrides
        public override int GetHashCode()
        {
            return Code.GetHashCode();
        }

        /// <summary>
        /// Compares +self+ with +other_currency+ and returns +true+ if the are the
        /// same or if their +id+ attributes match.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>[Boolean]</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Currency))
            {
                return false;
            }

            if (ReferenceEquals(obj, this))
            {
                return true;
            }

            var rhs = (Currency)obj;

            return Code.Equals(rhs.Code);
        }

        /// <summary>
        /// Returns a string representation corresponding to the upcase +id+
        /// attribute. Useful in cases where only implicit conversions are made.
        /// </summary>
        /// <returns>[String]</returns>
        public override string ToString()
        {
            return Code;
        }

        #endregion

        #region Operators
        /// <summary>
        /// Compares +self+ with +other_currency+ and returns +true+ if the are the
        /// same or if their +id+ attributes match.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>[Boolean]</returns>
        public static bool operator ==(Currency lhs, Currency rhs)
        {
            if (ReferenceEquals(lhs, null))
            {
                return ReferenceEquals(rhs, null);
            }
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Compares +self+ with +other_currency+ and returns +true+ if the are the
        /// same or if their +id+ attributes match.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>[Boolean]</returns>
        public static bool operator !=(Currency lhs, Currency rhs)
        {
            return !(lhs == rhs);
        }
        #endregion
    }
}

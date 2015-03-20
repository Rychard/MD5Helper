using System;
using System.Collections.Generic;
using System.Linq;

namespace HashHelper
{
    public static class FileSize
    {
        #region Static Constructor

        private static readonly List<FileSizeUnit> _units;
        static FileSize()
        {
            _units = new List<FileSizeUnit>
            {
                new FileSizeUnit(1, "Byte", "B"),
                new FileSizeUnit(1024, "Kilobyte", "KB"),
                new FileSizeUnit(1024*1024, "Megabyte", "MB"),
                new FileSizeUnit(1024*1024*1024, "Gigabyte", "GB"),
            };
        }

        #endregion

        private static FileSizeUnit GetSize(String unit)
        {
            Func<FileSizeUnit, Boolean> predicate = (obj => obj.ShortName == unit || obj.LongName == unit || obj.LongNamePluralized == unit);
            if (_units.Any(predicate))
            {
                if (_units.Count(predicate) == 1)
                {
                    var unitOfMeasure = _units.Single(predicate);
                    return unitOfMeasure;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the numeric value of size in the specified units.
        /// </summary>
        /// <param name="bytes">The amount of bytes.</param>
        /// <param name="unit">The short or long name of the unit that measurements will be returned in.</param>
        /// <param name="decimalPlaces">the amount of decimal places to round the resulting value to.  If set to <c>-1</c> the result will not be rounded.</param>
        /// <returns>
        /// If the specified unit is valid, returns the amount of units, else returns <c>-1</c>.
        /// </returns>
        public static Double GetSize(long bytes, String unit, int decimalPlaces = -1)
        {
            var unitOfMeasure = GetSize(unit);
            if (unitOfMeasure == null) { return -1; }

            Double result = (bytes / (Double)unitOfMeasure.Bytes);
            if (decimalPlaces > -1)
            {
                result = Math.Round(result, decimalPlaces, MidpointRounding.AwayFromZero);
            }
            return result;
        }

        /// <summary>
        /// Gets the string that represents the amount of the units specified.
        /// </summary>
        /// <param name="bytes">The amount of bytes.</param>
        /// <param name="unit">The short or long name of the unit that measurements will be returned in.</param>
        /// <param name="decimalPlaces">the amount of decimal places to round the resulting value to.  If set to <c>-1</c> the result will not be rounded.</param>
        /// <returns>
        /// If the specified unit is valid, returns the amount of units, else returns an <c>String.Empty</c>.
        /// </returns>
        public static String GetSizeString(long bytes, String unit, int decimalPlaces = -1)
        {
            var unitOfMeasure = GetSize(unit);
            if (unitOfMeasure == null) { return String.Empty; }
            var result = GetSize(bytes, unit, decimalPlaces);
            return String.Format("{0} {1}", result, unitOfMeasure.ShortName);
        }

        /// <summary>
        /// Represents a unit of measure for the size of digital information.
        /// </summary>
        private class FileSizeUnit
        {
            /// <summary>
            /// Gets the number of bytes required for a single unit of this measurement.
            /// </summary>
            public long Bytes { get; set; }

            /// <summary>
            /// Gets the number of bits required for a single unit of this measurement.
            /// </summary>
            public long Bits { get { return (this.Bytes * 8); } }

            /// <summary>
            /// Gets the long representation of this unit of measurement.
            /// </summary>
            public String LongName { get; set; }

            /// <summary>
            /// Gets the long representation of this unit of measurement.
            /// </summary>
            public String LongNamePluralized { get; set; }

            /// <summary>
            /// Gets the shortened representation of this unit of measurement, usually appended to the end of values.
            /// </summary>
            public String ShortName { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="FileSizeUnit"/> class.
            /// </summary>
            public FileSizeUnit()
            {
                this.Bytes = 1;
                this.LongName = "Byte";
                this.LongNamePluralized = "Bytes";
                this.ShortName = "B";
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="FileSizeUnit"/> class.
            /// </summary>
            /// <param name="bytes">The amount of bytes represented by this unit.</param>
            /// <param name="longName">The long representation of this unit.</param>
            /// <param name="shortName">The shortened representation of this unit.</param>
            public FileSizeUnit(long bytes, String longName, String shortName) : this(bytes, longName, shortName, String.Format("{0}s", longName)) { }

            /// <summary>
            /// Initializes a new instance of the <see cref="FileSizeUnit"/> class.
            /// </summary>
            /// <param name="bytes">The amount of bytes represented by this unit.</param>
            /// <param name="longName">The long representation of this unit.</param>
            /// <param name="shortName">The shortened representation of this unit.</param>
            /// <param name="longNamePluralized">The pluralized version of the long representation of this unit.</param>
            public FileSizeUnit(long bytes, String longName, String shortName, String longNamePluralized)
            {
                this.Bytes = bytes;
                this.LongName = longName;
                this.LongNamePluralized = longNamePluralized;
                this.ShortName = shortName;
            }
        }
    }
}

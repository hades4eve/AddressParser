using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressParser.Shared
{
    public class Address
    {
        #region Fixed Formatted Address
        /// <summary>
        /// Building value
        /// </summary>
        public string Building { set; get; }
        /// <summary>
        /// Sub Building value
        /// </summary>
        public string SubBuilding { set; get; }
        /// <summary>
        /// Street value
        /// </summary>
        public string Street { set; get; }
        /// <summary>
        /// Sub Street value
        /// </summary>
        public string SubStreet { set; get; }
        /// <summary>
        /// City value
        /// </summary>
        public string City { set; get; }
        /// <summary>
        /// Sub City value
        /// </summary>
        public string SubCity { set; get; }
        /// <summary>
        /// ZippostCode Value
        /// </summary>
        public string ZippostCode { set; get; }
        /// <summary>
        /// Selected Country
        /// </summary>
        public EnumCountry Country { set; get; }
        #endregion

        #region Free Formatted Addresses
        public string AddressLine1 { set; get; }
        public string AddressLine2 { set; get; }
        public string AddressLine3 { set; get; }
        public string AddressLine4 { set; get; }
        #endregion

        public string GetFreeFormattedAddress(string separator = ",")
        {
            string output = null;
            StringBuilder sb = new StringBuilder();

            if (separator == string.Empty || separator.Length > 1)
            {
                throw new ArgumentException("Separator cannot be NULL or Empty character. Separator only allowed 1 character.");
            }

            //To prevent double space as separator
            if (!separator.Equals(" "))
            {
                separator += " ";
            }

            if (!string.IsNullOrWhiteSpace(AddressLine1))
            {
                sb.Append(AddressLine1);
                sb.Append(separator);
            }

            if (!string.IsNullOrWhiteSpace(AddressLine2))
            {
                sb.Append(AddressLine2);
                sb.Append(separator);
            }

            if (!string.IsNullOrWhiteSpace(AddressLine3))
            {
                sb.Append(AddressLine3);
                sb.Append(separator);
            }

            if (!string.IsNullOrWhiteSpace(AddressLine4))
            {
                sb.Append(AddressLine4);
                sb.Append(separator);
            }

            return output.Trim().TrimEnd(new[] { separator[0] });
        }
    }
}

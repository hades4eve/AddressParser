using AddressParser.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AddressParser.Shared;
using System.Text.RegularExpressions;

namespace AddressParser.Country
{
    internal class BelgiumParserProviderv1 : IParserProvider
    {
        public Address ParseAddress(Address InputAddress)
        {
            Address OutputAddress = new Address();
            OutputAddress = InputAddress;

            const string regZipcode = @"\d{4}";
            const string regBuildingNumber = @"[\d]+";

            //For string manipulation to ensure no DUPLICATED set of numeric is identified as zippostcode and building number
            string carbonFullAddress = InputAddress.GetFreeFormattedAddress(" ");
            string originalFullAddress = InputAddress.GetFreeFormattedAddress(" ");

            string buildingNumber = string.Empty;
            string streetName = string.Empty;
            string zipCode = string.Empty;
            string city = string.Empty;

            //Flags to hold if Building / Zippostcode is supplied
            bool isBuildingFF = false;
            bool isZippostcodeFF = false;

            //If FF is not supplied
            if (!string.IsNullOrEmpty(carbonFullAddress))
            {
                //Check to parse Zippostcode if available.
                Match matchZipCode = Regex.Match(carbonFullAddress, regZipcode, RegexOptions.RightToLeft);

                if (matchZipCode.Groups.Count > 0)
                {
                    isZippostcodeFF = true;
                    //Zippostcode always the last set of 4 digits number
                    zipCode = matchZipCode.Groups[matchZipCode.Groups.Count - 1].Value;
                    carbonFullAddress = TrimSpace(carbonFullAddress.Replace(zipCode, ""));
                }
                //Check to parse first set of numeric to building numbers.
                Match matchBuildingNumber = Regex.Match(carbonFullAddress, regBuildingNumber, RegexOptions.CultureInvariant);

                if (matchBuildingNumber.Groups.Count > 0)
                {
                    isBuildingFF = true;
                    //Building number is first set of numeric by default.
                    buildingNumber = matchBuildingNumber.Groups[0].Value;
                }

                List<string> fullAddressParts = new List<string>();

                //if Zippostcode is supplied (Preferable)
                if (isZippostcodeFF)
                {
                    //Trim building from the full address string before splitting
                    if (!string.IsNullOrEmpty(buildingNumber))
                    {
                        originalFullAddress = originalFullAddress.Replace(buildingNumber, string.Empty);
                    }

                    fullAddressParts = originalFullAddress.Split(new string[] { zipCode }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                // if Building is supplied
                else if (isBuildingFF)
                    fullAddressParts = originalFullAddress.Split(new string[] { buildingNumber }, StringSplitOptions.RemoveEmptyEntries).ToList();

                if (fullAddressParts.Count > 0)
                {
                    //first part is STREET value
                    streetName = fullAddressParts[0].Trim();
                    fullAddressParts.RemoveAt(0);

                    //last part is CITY value
                    if (fullAddressParts.Count > 0)
                    {
                        city = fullAddressParts.Last();
                    }
                }
                else
                {
                    //Unable to perform string split between STREET and CITY
                    streetName = originalFullAddress;
                }

                if (string.IsNullOrEmpty(OutputAddress.Building.Trim()))
                    OutputAddress.Building = buildingNumber;

                if (string.IsNullOrEmpty(OutputAddress.Street.Trim()))
                    OutputAddress.Street = streetName;

                if (string.IsNullOrEmpty(OutputAddress.City.Trim()))
                    OutputAddress.City = city;

                if (string.IsNullOrEmpty(OutputAddress.ZippostCode.Trim()))
                    OutputAddress.ZippostCode = zipCode;

            }

            return OutputAddress;
        }

        private string TrimSpace(string input)
        {
            Regex ExtraSpace = new Regex(@"\s{2,}", RegexOptions.CultureInvariant);

            return ExtraSpace.Replace(input, "");
        }
    }
}

using AddressParser.Interface;
using AddressParser.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressParser
{
    public class AddressParser : MarshalByRefObject
    {
        private AddressParserFactory addressParserFactory = new AddressParserFactory();
        /// <summary>
        /// Execute Parse address method to break down the field from Free Format To Fixed Format
        /// </summary>
        /// <param name="address">Address with input fields</param>
        /// <param name="country">Country of the Parser Provider</param>
        /// <param name="version">Version of the Parser Provider</param>
        /// <returns></returns>
        public Address ExecuteParseAddress(Address address, EnumCountry country, string version)
        {
            Address result = default(Address);
            IParserProvider parserProvider = addressParserFactory.GetParserProvider(country, version);

            result = parserProvider.ParseAddress(address);


            return result;
        }
    }
}

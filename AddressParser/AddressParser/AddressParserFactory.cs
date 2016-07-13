using AddressParser.Country;
using AddressParser.Interface;
using AddressParser.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressParser
{
    internal class AddressParserFactory
    {
        /// <summary>
        /// Invoke correct Address Parser Provider
        /// </summary>
        /// <param name="country">Country of the Parser Provider</param>
        /// <param name="version">Version of the Parser Provider</param>
        /// <returns>Re</returns>
        public IParserProvider GetParserProvider(EnumCountry country, string version)
        {
            IParserProvider parserProvider;

            switch (country)
            {
                case EnumCountry.Belgium:
                    {
                        switch (version)
                        {
                            case "1.0":
                                parserProvider = new BelgiumParserProviderv1();
                                break;
                            default:
                                throw new NotImplementedException($"{country.ToString()} Parser Provider version {version} has not been implemented.");
                        }
                    }
                    break;
                case EnumCountry.France:
                    {
                        switch (version)
                        {
                            case "1.0":
                                parserProvider = new FranceParserProviderv1();
                                break;
                            default:
                                throw new NotImplementedException($"{country.ToString()} Parser Provider version {version} has not been implemented.");
                        }
                    }
                    break;
                case EnumCountry.NewZealand:
                    {
                        switch(version)
                        {
                            case "1.0":
                                parserProvider = new NewZealandParserProviderv1();
                                break;
                            default:
                                throw new NotImplementedException($"{country.ToString()} Parser Provider version {version} has not been implemented.");
                        }
                    }
                    break;
                default:
                    throw new NotImplementedException($"{country.ToString()} is not in coverage.");
            }

            return parserProvider;
        }
    }
}

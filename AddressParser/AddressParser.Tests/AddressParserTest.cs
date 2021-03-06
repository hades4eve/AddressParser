// <copyright file="AddressParserTest.cs">Copyright ©  2016</copyright>
using System;
using AddressParser;
using AddressParser.Shared;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AddressParser.Tests
{
    /// <summary>This class contains parameterized unit tests for AddressParser</summary>
    [PexClass(typeof(global::AddressParser.AddressParser))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class AddressParserTest
    {
        /// <summary>Test stub for ExecuteParseAddress(Address, EnumCountry, String)</summary>
        [PexMethod]
        public Address ExecuteParseAddressTest(
            [PexAssumeUnderTest]global::AddressParser.AddressParser target,
            Address address,
            EnumCountry country,
            string version
        )
        {
            Address result = target.ExecuteParseAddress(address, country, version);
            return result;
            // TODO: add assertions to method AddressParserTest.ExecuteParseAddressTest(AddressParser, Address, EnumCountry, String)
        }
    }
}

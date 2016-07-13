using AddressParser.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressParser.Interface
{
    internal interface IParserProvider
    {
        Address ParseAddress(Address InputAddress);
    }
}

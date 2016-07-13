using AddressParser.Interface;
using AddressParser.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AddressParser.Country
{
    internal class NewZealandParserProviderv1 : IParserProvider
    {
        public static readonly string[] CountryTrimEntities = { "newzealand", "new zealand", "nz", "n.z." };
        public static readonly string[] m_DataZoo_NZ_Abbreviation = { "Apartment", "Apt", "Flat", "Floor", "Kiosk", "Ksk", "Level", "Room", "Rm", "Shop", "Shp", "Suite", "Ste", "Unit", "Villa", "Vlla" };
        static Regex RegExtractNumber = new Regex(@"(\d+)", RegexOptions.Compiled);

        public Address ParseAddress(Address InputAddress)
        {
            Address OutputAddress = new Address();
            OutputAddress = InputAddress;

            string unitNo = string.Empty;
            string streetNo = string.Empty;
            string streetName = string.Empty;
            string suburb = string.Empty;
            string city = string.Empty;
            string zipPostCode = string.Empty;

            string _possibleCity = string.Empty;

            if (!String.IsNullOrWhiteSpace(InputAddress.AddressLine1)
                || !String.IsNullOrWhiteSpace(InputAddress.AddressLine2)
                || !String.IsNullOrWhiteSpace(InputAddress.AddressLine3)
                || !String.IsNullOrWhiteSpace(InputAddress.AddressLine4))
            {
                string fullAddress = String.Empty;

                #region Building full address string 
                StringBuilder sBuilder = new StringBuilder();

                if (!string.IsNullOrEmpty(InputAddress.AddressLine1.Trim()))
                {
                    sBuilder.Append(InputAddress.AddressLine1.Trim());
                    sBuilder.Append(",");
                }

                if (!string.IsNullOrEmpty(InputAddress.AddressLine2.Trim()))
                {
                    sBuilder.Append(InputAddress.AddressLine2.Trim());
                    sBuilder.Append(",");
                }

                if (!string.IsNullOrEmpty(InputAddress.AddressLine3.Trim()))
                {
                    sBuilder.Append(InputAddress.AddressLine3.Trim());
                    sBuilder.Append(",");
                }

                if (!string.IsNullOrEmpty(InputAddress.AddressLine4.Trim()))
                {
                    sBuilder.Append(InputAddress.AddressLine4.Trim());
                    sBuilder.Append(",");
                }

                fullAddress = sBuilder.ToString().Trim(','); //Trim both end comma (,)
                #endregion

                #region Get Address From Fix Format Address.
                suburb = InputAddress.SubCity.Trim();
                city = InputAddress.City.Trim();
                zipPostCode = InputAddress.ZippostCode.Trim();
                #endregion

                //Remove Country Elements from the list
                foreach (string v in CountryTrimEntities)
                {
                    fullAddress = Regex.Replace(fullAddress, v, "", RegexOptions.IgnoreCase);
                }

                List<string> addressElements = fullAddress.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                //Trim extra space (space may exist if user enter one of the line (<suburb><space>,<space><city>)
                for (int i = 0; i < addressElements.Count; i++)
                {
                    addressElements[i] = addressElements[i].Trim();
                }

                addressElements = addressElements.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();//Flush empty elements

                #region Free Format Address Get Zip Post Code.
                if (String.IsNullOrWhiteSpace(zipPostCode))
                {
                    string regZipCode = @"\d{4}"; // Postcode for New Zealand is 4 digits.

                    for (int i = addressElements.Count - 1; i >= 0; i--)
                    {
                        Match matchZipCode = Regex.Match(addressElements[i], regZipCode, RegexOptions.CultureInvariant);

                        if (matchZipCode.Success)
                        {
                            foreach (Group g in matchZipCode.Groups)
                            {
                                zipPostCode = g.Value;
                                break;
                            }

                            if (addressElements.Count - 1 != i)
                            {
                                string _s = string.Empty;
                                for (int zLE = i + 1; zLE < addressElements.Count; zLE++)
                                {
                                    _s += addressElements[zLE] + " ";

                                    addressElements[zLE] = "";
                                }

                                _possibleCity = _s;
                                _possibleCity = _possibleCity.Trim();
                            }

                            addressElements[i] = addressElements[i].Replace(zipPostCode, "").Trim();
                            break;
                        }
                    }
                }
                else
                {
                    if (addressElements.Count - 1 != addressElements.IndexOf(zipPostCode))
                    {
                        string _s = string.Empty;
                        for (int i = addressElements.IndexOf(zipPostCode) + 1; i < addressElements.Count; i++)
                        {
                            _s += addressElements[i] + " ";

                            addressElements[i] = "";
                        }

                        _possibleCity = _possibleCity.Trim();
                    }

                    for (int i = addressElements.Count - 1; i >= 0; i--)
                    {
                        addressElements[i] = addressElements[i].Replace(zipPostCode, "").Trim();
                    }
                }
                #endregion

                addressElements = addressElements.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();//Flush empty elements


                int? i_possibleStreetNotation = null;

                #region Get UnitNo

                //Unit No might be in Flat <digit> / Apt <digit> Floor <digit>
                //Therefore always have to get all the value and remove from the list
                if (m_DataZoo_NZ_Abbreviation != null && m_DataZoo_NZ_Abbreviation.Length > 0)
                {
                    string strRegCompiled = $@"(({string.Join("|", m_DataZoo_NZ_Abbreviation)})(\.?)(\s+|)\d+)";

                    Regex regUnitNo = new Regex(strRegCompiled, RegexOptions.IgnoreCase);

                    List<Tuple<string, int>> unitNoList = new List<Tuple<string, int>>();

                    for (int i = 0; i < addressElements.Count; i++)
                    {
                        if (regUnitNo.IsMatch(addressElements[i]))
                        {
                            MatchCollection collection = regUnitNo.Matches(addressElements[i]);

                            foreach (Match match in collection)
                            {
                                unitNoList.Add(new Tuple<string, int>(match.Value, i));
                            }
                        }
                    }

                    //Removing all values in front of unit no. (Deemed to be Shop Name, Place Name)
                    int? lastOccurence = null;

                    if (unitNoList != null && unitNoList.Count > 0)
                    {
                        lastOccurence = unitNoList.Max(x => x.Item2);
                    }

                    if (lastOccurence.HasValue && lastOccurence > 0)
                    {
                        for (int i = 0; i < lastOccurence.Value; i++)
                        {
                            addressElements.RemoveAt(i);
                        }
                    }

                    for (int i = 0; i < addressElements.Count; i++)
                    {
                        foreach (var unitElement in unitNoList)
                        {
                            Match m = Regex.Match(addressElements[i], unitElement.Item1, RegexOptions.IgnoreCase);

                            if (m.Length > 0)
                            {
                                addressElements[i] = Regex.Replace(addressElements[i], unitElement.Item1, "", RegexOptions.IgnoreCase).Trim();

                                //Example : Apt 30 James Hill Street
                                //After   : --- -- James Hill Street
                                //Therefore marked potential remainder as a street.
                                if (!string.IsNullOrWhiteSpace(addressElements[i]))
                                {
                                    i_possibleStreetNotation = i;
                                }
                            }
                        }
                    }

                    //Concat multiple elements to a single string of unit no.
                    foreach (var unitNoElement in unitNoList)
                    {
                        unitNo += unitNoElement.Item1 + " ";
                    }

                    unitNo = unitNo.Trim();
                }
                #endregion



                addressElements = addressElements.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();

                #region Regex Declaration
                Regex RegStreetnumStreetA = new Regex(@"(\d+)(\s+)[a-zA-Z ]+");
                Regex RegStreetnumStreetB = new Regex(@"(\d+)-(\d+)(\s+)[a-zA-Z ]+");
                Regex RegStreetnumStreetC = new Regex(@"\w+/\w+(\s+)[a-zA-Z ]+");
                Regex RegStreetnumStreetD = new Regex(@"[0-9]+[a-zA-Z](\s+)[a-zA-Z ]+");
                Regex RegStreetnumStreetE = new Regex(@"(\d+)(\s+)(\d+)(\s+)((?:[a-zA-Z ]+))");
                #endregion

                char? RegexMatch = null;
                int? indexOfElement = null;
                bool hasGotStreetElements = false;

                if (i_possibleStreetNotation.HasValue)
                {
                    if (RegStreetnumStreetB.IsMatch(addressElements[i_possibleStreetNotation.Value]))
                    {
                        RegexMatch = 'B';
                        indexOfElement = i_possibleStreetNotation.Value;
                        hasGotStreetElements = true;
                    }
                    else if (RegStreetnumStreetC.IsMatch(addressElements[i_possibleStreetNotation.Value]))
                    {
                        RegexMatch = 'C';
                        indexOfElement = i_possibleStreetNotation.Value;
                        hasGotStreetElements = true;
                    }
                    else if (RegStreetnumStreetD.IsMatch(addressElements[i_possibleStreetNotation.Value]))
                    {
                        RegexMatch = 'D';
                        indexOfElement = i_possibleStreetNotation.Value;
                        hasGotStreetElements = true;
                    }
                    else if (RegStreetnumStreetE.IsMatch(addressElements[i_possibleStreetNotation.Value]))
                    {
                        RegexMatch = 'E';
                        indexOfElement = i_possibleStreetNotation.Value;
                        hasGotStreetElements = true;
                    }
                }

                if (!hasGotStreetElements)
                {
                    for (int i = 0; i < addressElements.Count; i++)
                    {
                        if (RegStreetnumStreetB.IsMatch(addressElements[i]))
                        {
                            RegexMatch = 'B';
                            indexOfElement = i;
                            break;
                        }
                        else if (RegStreetnumStreetC.IsMatch(addressElements[i]))
                        {
                            RegexMatch = 'C';
                            indexOfElement = i;
                            break;
                        }
                        else if (RegStreetnumStreetD.IsMatch(addressElements[i]))
                        {
                            RegexMatch = 'D';
                            indexOfElement = i;
                            break;
                        }
                        else if (RegStreetnumStreetE.IsMatch(addressElements[i]))
                        {
                            RegexMatch = 'E';
                            indexOfElement = i;
                            break;
                        }
                    }
                }

                if (RegexMatch.HasValue)
                {
                    switch (RegexMatch.Value)
                    {
                        case 'B':
                            {
                                //Example : 121-123
                                Regex extractNum = new Regex(@"\d+-\d+");
                                Match match = extractNum.Match(addressElements[indexOfElement.Value]);
                                string[] rawStNum = match.Value.Trim().Split('-');

                                streetNo = rawStNum[0];
                                streetName = addressElements[indexOfElement.Value].Replace(match.Value, "").Trim();
                                addressElements.Remove(addressElements[indexOfElement.Value]);
                            }
                            break;
                        case 'C':
                            {
                                //Example : 12/123A
                                Regex extractNum1 = new Regex(@"\w+/\w+");

                                Match match = extractNum1.Match(addressElements[indexOfElement.Value]);
                                string[] rawStNum = match.Value.Trim().Split('/');
                                unitNo = rawStNum[0];

                                Match matchNumOnly = RegExtractNumber.Match(rawStNum[1]);
                                if (matchNumOnly.Success)
                                {
                                    streetNo = matchNumOnly.Value;
                                }

                                Regex regToRemoved = new Regex(unitNo + @"/" + streetNo + @".*?\s+");
                                streetName = addressElements[indexOfElement.Value].Replace(match.Value, "").Trim();
                                addressElements.Remove(addressElements[indexOfElement.Value]);
                            }
                            break;
                        case 'D':
                            {
                                //Example : 123A
                                Match match = RegExtractNumber.Match(addressElements[indexOfElement.Value]);

                                if (match.Success)
                                {
                                    string rawStNum = match.Value;
                                    streetNo = rawStNum;
                                }

                                Regex regToRemoved = new Regex(streetNo + @".*?\s+");
                                streetName = regToRemoved.Replace(addressElements[indexOfElement.Value], "").Trim();

                                addressElements.Remove(addressElements[indexOfElement.Value]);
                            }
                            break;
                        case 'E':
                            {
                                //Example 11 123
                                MatchCollection collection = RegExtractNumber.Matches(addressElements[indexOfElement.Value]);

                                if (collection != null && collection.Count > 0)
                                {
                                    unitNo = collection[0].Value;
                                    streetNo = collection[1].Value;

                                    addressElements[indexOfElement.Value] = addressElements[indexOfElement.Value].Replace(unitNo, "").Trim();
                                    addressElements[indexOfElement.Value] = addressElements[indexOfElement.Value].Replace(streetNo, "").Trim();
                                }
                            }
                            break;
                        default:
                            throw new NotImplementedException("New extraction algorithm not implemented.");
                    }

                }

                #region Assuming First Element is thoroughfare element (street)
                if (string.IsNullOrEmpty(streetName) && addressElements.Count > 0)
                {

                    //Everything else. Example : 12 Mill Street, Mill Street, 99 123 Mill Street
                    string[] streetElement = addressElements[0].Split(' ');
                    int? StreetNumber = null;
                    Regex isDigit = new Regex(@"\d+");
                    if (streetElement.Length > 0)
                    {
                        string streetN = String.Empty;

                        for (int j = streetElement.Length - 1; j >= 0; j--)
                        {
                            if (!isDigit.IsMatch(streetElement[j]))
                            {
                                streetN = streetElement[j] + " " + streetN;
                            }
                            else
                            {
                                StreetNumber = int.Parse(streetElement[j]);
                                break;
                            }
                        }

                        streetName = streetN;
                        addressElements.RemoveAt(0);
                        if (StreetNumber.HasValue)
                            streetNo = StreetNumber.Value.ToString();
                    }
                }
                #endregion

                if (addressElements.Count > 0)
                {
                    city = addressElements.Last();

                    addressElements.Remove(city);
                }
                else if (!String.IsNullOrWhiteSpace(_possibleCity))
                {
                    city = _possibleCity;
                }

                if (addressElements.Count > 0)
                {
                    suburb = addressElements[addressElements.Count - 1];
                    addressElements.Remove(suburb);
                }
            }

            #region Re-assigning to OutputAddress
        
            OutputAddress.Building = unitNo;
            OutputAddress.SubCity =  String.IsNullOrWhiteSpace(InputAddress.SubCity) ? suburb : InputAddress.SubCity.Trim();
            OutputAddress.City =  String.IsNullOrWhiteSpace(InputAddress.City) ? city : InputAddress.City.Trim();
            OutputAddress.ZippostCode =  String.IsNullOrWhiteSpace(InputAddress.ZippostCode) ? zipPostCode : InputAddress.ZippostCode.Trim();
            OutputAddress.Street  = String.IsNullOrWhiteSpace(InputAddress.Street) ? streetName : InputAddress.Street.Trim();
            #endregion

            return OutputAddress;
        }
    }
}

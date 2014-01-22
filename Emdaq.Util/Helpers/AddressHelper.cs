using System;
using System.Collections.Generic;
using System.Linq;
using Emdaq.Util.Extensions;

namespace Emdaq.Util.Helpers
{
    public class AddressHelper
    {
        #region singleton

        private static readonly Lazy<AddressHelper> Singleton = new Lazy<AddressHelper>(() => new AddressHelper());
        public static AddressHelper I { get { return Singleton.Value; } }

        #endregion

        public string FormatZipCode(string zip, bool requireUsZip = true, bool requireValid = true)
        {
            if (zip == null)
            {
                return null;
            }

            // no international validation
            if (!requireUsZip)
            {
                return zip;
            }

            var zipAsNumber = zip.RemoveNonNumeric().TryParse<int>();

            // zip must be a number
            if (zipAsNumber != null)
            {
                var cleanZip = zipAsNumber.Value.ToString();

                // length of 5 is valid
                if (cleanZip.Length == 5)
                {
                    return cleanZip;
                }

                // length of 9 is valid
                if (cleanZip.Length == 9)
                {
                    return cleanZip.Substring(0, 5) + '-' + cleanZip.Substring(5);
                }

                // length less than 5 means it must be missing 0s
                if (cleanZip.Length < 5)
                {
                    return cleanZip.PadLeft(5, '0');
                }

                // length greater than 5 and less than 9 means it must be missing 0s
                if (cleanZip.Length < 9)
                {
                    cleanZip = cleanZip.PadLeft(9, '0');
                    return cleanZip.Substring(0, 5) + '-' + cleanZip.Substring(5);
                }
            }

            // anything else is invalid
            return requireValid ? null : zip;
        }

        public bool AreAddressLinesEqual(string line1, string line2)
        {
            if (line1 == null && line2 == null)
            {
                return true;
            }

            if (line1 == null || line2 == null)
            {
                return false;
            }

            var cleanedLine1 = CleanAddressLine(line1);
            var cleanedLine2 = CleanAddressLine(line2);

            return string.Equals(cleanedLine1, cleanedLine2);
        }

        public string CleanAddressLine(string line)
        {
            if (line == null)
            {
                return null;
            }

            return string.Join(" ", line.RemovePunctuation().ToUpper()
                                        .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                        .Select(x => StreetTypes.ContainsKey(x) ? StreetTypes[x] : x));
        }

        #region street types, from USPS

        private static readonly Dictionary<string, string> StreetTypes = new Dictionary<string, string>
        {  
            {"ALLEE","ALY"},
            {"ALLEY","ALY"},
            {"ALLY","ALY"},
            {"ALY","ALY"},
            {"ANEX ","ANX"},
            {"ANNEX","ANX"},
            {"ANNX","ANX"},
            {"ANX","ANX"},
            {"ARC","ARC"},
            {"ARCADE","ARC"},
            {"AV","AVE"},
            {"AVE","AVE"},
            {"AVEN","AVE"},
            {"AVENU","AVE"},
            {"AVENUE","AVE"},
            {"AVN","AVE"},
            {"AVNUE","AVE"},
            {"BAYOO","BYU"},
            {"BAYOU","BYU"},
            {"BCH","BCH"},
            {"BEACH","BCH"},
            {"BEND","BND"},
            {"BND","BND"},
            {"BLF","BLF"},
            {"BLUF","BLF"},
            {"BLUFF","BLF"},
            {"BLUFFS","BLFS"},
            {"BOT","BTM"},
            {"BOTTM","BTM"},
            {"BOTTOM","BTM"},
            {"BTM","BTM"},
            {"BLVD","BLVD"},
            {"BOUL","BLVD"},
            {"BOULEVARD","BLVD"},
            {"BOULV","BLVD"},
            {"BR","BR"},
            {"BRANCH","BR"},
            {"BRNCH","BR"},
            {"BRDGE","BRG"},
            {"BRG","BRG"},
            {"BRIDGE","BRG"},
            {"BRK","BRK"},
            {"BROOK","BRK"},
            {"BROOKS","BRKS"},
            {"BURG","BG"},
            {"BURGS","BGS"},
            {"BYP","BYP"},
            {"BYPA","BYP"},
            {"BYPAS","BYP"},
            {"BYPASS","BYP"},
            {"BYPS","BYP"},
            {"CAMP","CP"},
            {"CMP","CP"},
            {"CP","CP"},
            {"CANYN","CYN"},
            {"CANYON","CYN"},
            {"CNYN","CYN"},
            {"CYN","CYN"},
            {"CAPE","CPE"},
            {"CPE","CPE"},
            {"CAUSEWAY","CSWY"},
            {"CAUSWAY","CSWY"},
            {"CSWY","CSWY"},
            {"CEN","CTR"},
            {"CENT","CTR"},
            {"CENTER ","CTR"},
            {"CENTR ","CTR"},
            {"CENTRE","CTR"},
            {"CNTER ","CTR"},
            {"CNTR ","CTR"},
            {"CTR ","CTR"},
            {"CENTERS ","CTRS"},
            {"CIR ","CIR"},
            {"CIRC ","CIR"},
            {"CIRCL ","CIR"},
            {"CIRCLE ","CIR"},
            {"CRCL ","CIR"},
            {"CRCLE ","CIR"},
            {"CIRCLES ","CIRS"},
            {"CLF ","CLF"},
            {"CLIFF ","CLF"},
            {"CLFS ","CLFS"},
            {"CLIFFS ","CLFS"},
            {"CLB ","CLB"},
            {"CLUB ","CLB"},
            {"COMMON ","CMN"},
            {"COR ","COR"},
            {"CORNER ","COR"},
            {"CORNERS ","CORS"},
            {"CORS ","CORS"},
            {"COURSE ","CRSE"},
            {"CRSE ","CRSE"},
            {"COURT ","CT"},
            {"CRT ","CT"},
            {"CT ","CT"},
            {"COURTS ","CTS"},
            {"COVE ","CV"},
            {"CV ","CV"},
            {"COVES ","CVS"},
            {"CK ","CRK"},
            {"CR ","CRK"},
            {"CREEK ","CRK"},
            {"CRK ","CRK"},
            {"CRECENT ","CRES"},
            {"CRES ","CRES"},
            {"CRESCENT ","CRES"},
            {"CRESENT ","CRES"},
            {"CRSCNT ","CRES"},
            {"CRSENT ","CRES"},
            {"CRSNT ","CRES"},
            {"CREST ","CRST"},
            {"CROSSING ","XING"},
            {"CRSSING ","XING"},
            {"CRSSNG ","XING"},
            {"XING ","XING"},
            {"CROSSROAD ","XRD"},
            {"CURVE ","CURV"},
            {"DALE","DL"},
            {"DL","DL"},
            {"DAM","DM"},
            {"DM ","DM"},
            {"DIV","DV"},
            {"DIVIDE","DV"},
            {"DV","DV"},
            {"DVD","DV"},
            {"DR","DR"},
            {"DRIV","DR"},
            {"DRIVE","DR"},
            {"DRV","DR"},
            {"DRIVES","DRS"},
            {"EST","EST"},
            {"ESTATE","EST"},
            {"ESTATES","ESTS"},
            {"ESTS","ESTS"},
            {"EXP","EXPY"},
            {"EXPR","EXPY"},
            {"EXPRESS","EXPY"},
            {"EXPRESSWAY","EXPY"},
            {"EXPW","EXPY"},
            {"EXPY","EXPY"},
            {"EXT","EXT"},
            {"EXTENSION","EXT"},
            {"EXTN","EXT"},
            {"EXTNSN","EXT"},
            {"EXTENSIONS","EXTS"},
            {"EXTS","EXTS"},
            {"FALL","FALL"},
            {"FALLS","FLS"},
            {"FLS","FLS"},
            {"FERRY","FRY"},
            {"FRRY","FRY"},
            {"FRY","FRY"},
            {"FIELD","FLD"},
            {"FLD","FLD"},
            {"FIELDS","FLDS"},
            {"FLDS","FLDS"},
            {"FLAT","FLT"},
            {"FLT","FLT"},
            {"FLATS","FLTS"},
            {"FLTS","FLTS"},
            {"FORD","FRD"},
            {"FRD","FRD"},
            {"FORDS","FRDS"},
            {"FOREST","FRST"},
            {"FORESTS","FRST"},
            {"FRST","FRST"},
            {"FORG","FRG"},
            {"FORGE","FRG"},
            {"FRG","FRG"},
            {"FORGES","FRGS"},
            {"FORK","FRK"},
            {"FRK","FRK"},
            {"FORKS","FRKS"},
            {"FRKS","FRKS"},
            {"FORT","FT"},
            {"FRT","FT"},
            {"FT","FT"},
            {"FREEWAY","FWY"},
            {"FREEWY","FWY"},
            {"FRWAY","FWY"},
            {"FRWY","FWY"},
            {"FWY","FWY"},
            {"GARDEN","GDN"},
            {"GARDN","GDN"},
            {"GDN","GDN"},
            {"GRDEN","GDN"},
            {"GRDN","GDN"},
            {"GARDENS","GDNS"},
            {"GDNS","GDNS"},
            {"GRDNS","GDNS"},
            {"GATEWAY","GTWY"},
            {"GATEWY","GTWY"},
            {"GATWAY","GTWY"},
            {"GTWAY","GTWY"},
            {"GTWY","GTWY"},
            {"GLEN","GLN"},
            {"GLN","GLN"},
            {"GLENS","GLNS"},
            {"GREEN","GRN"},
            {"GRN","GRN"},
            {"GREENS","GRNS"},
            {"GROV","GRV"},
            {"GROVE","GRV"},
            {"GRV","GRV"},
            {"GROVES","GRVS"},
            {"HARB","HBR"},
            {"HARBOR","HBR"},
            {"HARBR","HBR"},
            {"HBR","HBR"},
            {"HRBOR","HBR"},
            {"HARBORS","HBRS"},
            {"HAVEN","HVN"},
            {"HAVN","HVN"},
            {"HVN","HVN"},
            {"HEIGHT","HTS"},
            {"HEIGHTS","HTS"},
            {"HGTS","HTS"},
            {"HT","HTS"},
            {"HTS","HTS"},
            {"HIGHWAY","HWY"},
            {"HIGHWY","HWY"},
            {"HIWAY","HWY"},
            {"HIWY","HWY"},
            {"HWAY","HWY"},
            {"HWY","HWY"},
            {"HILL","HL"},
            {"HL","HL"},
            {"HILLS","HLS"},
            {"HLS","HLS"},
            {"HLLW","HOLW"},
            {"HOLLOW","HOLW"},
            {"HOLLOWS","HOLW"},
            {"HOLW","HOLW"},
            {"HOLWS","HOLW"},
            {"INLET","INLT"},
            {"INLT","INLT"},
            {"IS","IS"},
            {"ISLAND","IS"},
            {"ISLND","IS"},
            {"ISLANDS","ISS"},
            {"ISLNDS","ISS"},
            {"ISS","ISS"},
            {"ISLE","ISLE"},
            {"ISLES","ISLE"},
            {"JCT","JCT"},
            {"JCTION","JCT"},
            {"JCTN","JCT"},
            {"JUNCTION","JCT"},
            {"JUNCTN","JCT"},
            {"JUNCTON","JCT"},
            {"JCTNS","JCTS"},
            {"JCTS","JCTS"},
            {"JUNCTIONS","JCTS"},
            {"KEY","KY"},
            {"KY","KY"},
            {"KEYS","KYS"},
            {"KYS","KYS"},
            {"KNL","KNL"},
            {"KNOL","KNL"},
            {"KNOLL","KNL"},
            {"KNLS","KNLS"},
            {"KNOLLS","KNLS"},
            {"LAKE","LK"},
            {"LK","LK"},
            {"LAKES","LKS"},
            {"LKS","LKS"},
            {"LAND","LAND"},
            {"LANDING","LNDG"},
            {"LNDG","LNDG"},
            {"LNDNG","LNDG"},
            {"LA","LN"},
            {"LANE","LN"},
            {"LANES","LN"},
            {"LN","LN"},
            {"LGT","LGT"},
            {"LIGHT","LGT"},
            {"LIGHTS","LGTS"},
            {"LF","LF"},
            {"LOAF","LF"},
            {"LCK","LCK"},
            {"LOCK","LCK"},
            {"LCKS","LCKS"},
            {"LOCKS","LCKS"},
            {"LDG","LDG"},
            {"LDGE","LDG"},
            {"LODG","LDG"},
            {"LODGE","LDG"},
            {"LOOP","LOOP"},
            {"LOOPS","LOOP"},
            {"MALL","MALL"},
            {"MANOR","MNR"},
            {"MNR","MNR"},
            {"MANORS","MNRS"},
            {"MNRS","MNRS"},
            {"MDW","MDW"},
            {"MEADOW","MDW"},
            {"MDWS","MDWS"},
            {"MEADOWS","MDWS"},
            {"MEDOWS","MDWS"},
            {"MEWS","MEWS"},
            {"MILL","ML"},
            {"ML","ML"},
            {"MILLS","MLS"},
            {"MLS","MLS"},
            {"MISSION","MSN"},
            {"MISSN","MSN"},
            {"MSN","MSN"},
            {"MSSN","MSN"},
            {"MOTORWAY","MTWY"},
            {"MNT","MT"},
            {"MOUNT","MT"},
            {"MT","MT"},
            {"MNTAIN","MTN"},
            {"MNTN","MTN"},
            {"MOUNTAIN","MTN"},
            {"MOUNTIN","MTN"},
            {"MTIN","MTN"},
            {"MTN","MTN"},
            {"MNTNS","MTNS"},
            {"MOUNTAINS","MTNS"},
            {"NCK","NCK"},
            {"NECK","NCK"},
            {"ORCH","ORCH"},
            {"ORCHARD","ORCH"},
            {"ORCHRD","ORCH"},
            {"OVAL","OVAL"},
            {"OVL","OVAL"},
            {"OVERPASS","OPAS"},
            {"PARK","PARK"},
            {"PK","PARK"},
            {"PRK","PARK"},
            {"PARKS","PARK"},
            {"PARKWAY","PKWY"},
            {"PARKWY","PKWY"},
            {"PKWAY","PKWY"},
            {"PKWY","PKWY"},
            {"PKY","PKWY"},
            {"PARKWAYS","PKWY"},
            {"PKWYS","PKWY"},
            {"PASS","PASS"},
            {"PASSAGE","PSGE"},
            {"PATH","PATH"},
            {"PATHS","PATH"},
            {"PIKE","PIKE"},
            {"PIKES","PIKE"},
            {"PINE","PNE"},
            {"PINES","PNES"},
            {"PNES","PNES"},
            {"PL","PL"},
            {"PLACE","PL"},
            {"PLAIN","PLN"},
            {"PLN","PLN"},
            {"PLAINES","PLNS"},
            {"PLAINS","PLNS"},
            {"PLNS","PLNS"},
            {"PLAZA","PLZ"},
            {"PLZ","PLZ"},
            {"PLZA","PLZ"},
            {"POINT","PT"},
            {"PT","PT"},
            {"POINTS","PTS"},
            {"PTS","PTS"},
            {"PORT","PRT"},
            {"PRT","PRT"},
            {"PORTS","PRTS"},
            {"PRTS","PRTS"},
            {"PR","PR"},
            {"PRAIRIE","PR"},
            {"PRARIE","PR"},
            {"PRR","PR"},
            {"RAD","RADL"},
            {"RADIAL","RADL"},
            {"RADIEL","RADL"},
            {"RADL","RADL"},
            {"RAMP","RAMP"},
            {"RANCH","RNCH"},
            {"RANCHES","RNCH"},
            {"RNCH","RNCH"},
            {"RNCHS","RNCH"},
            {"RAPID","RPD"},
            {"RPD","RPD"},
            {"RAPIDS","RPDS"},
            {"RPDS","RPDS"},
            {"REST","RST"},
            {"RST","RST"},
            {"RDG","RDG"},
            {"RDGE","RDG"},
            {"RIDGE","RDG"},
            {"RDGS","RDGS"},
            {"RIDGES","RDGS"},
            {"RIV","RIV"},
            {"RIVER","RIV"},
            {"RIVR","RIV"},
            {"RVR","RIV"},
            {"RD","RD"},
            {"ROAD","RD"},
            {"RDS","RDS"},
            {"ROADS","RDS"},
            {"ROUTE","RTE"},
            {"ROW","ROW"},
            {"RUE","RUE"},
            {"RUN","RUN"},
            {"SHL","SHL"},
            {"SHOAL","SHL"},
            {"SHLS","SHLS"},
            {"SHOALS","SHLS"},
            {"SHOAR","SHR"},
            {"SHORE","SHR"},
            {"SHR","SHR"},
            {"SHOARS","SHRS"},
            {"SHORES","SHRS"},
            {"SHRS","SHRS"},
            {"SKYWAY","SKWY"},
            {"SPG","SPG"},
            {"SPNG","SPG"},
            {"SPRING","SPG"},
            {"SPRNG","SPG"},
            {"SPGS","SPGS"},
            {"SPNGS","SPGS"},
            {"SPRINGS","SPGS"},
            {"SPRNGS","SPGS"},
            {"SPUR","SPUR"},
            {"SPURS","SPUR"},
            {"SQ","SQ"},
            {"SQR","SQ"},
            {"SQRE","SQ"},
            {"SQU","SQ"},
            {"SQUARE","SQ"},
            {"SQRS","SQS"},
            {"SQUARES","SQS"},
            {"STA","STA"},
            {"STATION","STA"},
            {"STATN","STA"},
            {"STN","STA"},
            {"STRA","STRA"},
            {"STRAV","STRA"},
            {"STRAVE","STRA"},
            {"STRAVEN","STRA"},
            {"STRAVENUE","STRA"},
            {"STRAVN","STRA"},
            {"STRVN","STRA"},
            {"STRVNUE","STRA"},
            {"STREAM","STRM"},
            {"STREME","STRM"},
            {"STRM","STRM"},
            {"ST","ST"},
            {"STR","ST"},
            {"STREET","ST"},
            {"STRT","ST"},
            {"STREETS","STS"},
            {"SMT","SMT"},
            {"SUMIT","SMT"},
            {"SUMITT","SMT"},
            {"SUMMIT","SMT"},
            {"TER","TER"},
            {"TERR","TER"},
            {"TERRACE","TER"},
            {"THROUGHWAY","TRWY"},
            {"TRACE","TRCE"},
            {"TRACES","TRCE"},
            {"TRCE","TRCE"},
            {"TRACK","TRAK"},
            {"TRACKS","TRAK"},
            {"TRAK","TRAK"},
            {"TRK","TRAK"},
            {"TRKS","TRAK"},
            {"TRAFFICWAY","TRFY"},
            {"TRFY","TRFY"},
            {"TR","TRL"},
            {"TRAIL","TRL"},
            {"TRAILS","TRL"},
            {"TRL","TRL"},
            {"TRLS","TRL"},
            {"TUNEL","TUNL"},
            {"TUNL","TUNL"},
            {"TUNLS","TUNL"},
            {"TUNNEL","TUNL"},
            {"TUNNELS","TUNL"},
            {"TUNNL","TUNL"},
            {"TPK","TPKE"},
            {"TPKE","TPKE"},
            {"TRNPK","TPKE"},
            {"TRPK","TPKE"},
            {"TURNPIKE","TPKE"},
            {"TURNPK","TPKE"},
            {"UNDERPASS","UPAS"},
            {"UN","UN"},
            {"UNION","UN"},
            {"UNIONS","UNS"},
            {"VALLEY","VLY"},
            {"VALLY","VLY"},
            {"VLLY","VLY"},
            {"VLY","VLY"},
            {"VALLEYS","VLYS"},
            {"VLYS","VLYS"},
            {"VDCT","VIA"},
            {"VIA","VIA"},
            {"VIADCT","VIA"},
            {"VIADUCT","VIA"},
            {"VIEW","VW"},
            {"VW","VW"},
            {"VIEWS","VWS"},
            {"VWS","VWS"},
            {"VILL","VLG"},
            {"VILLAG","VLG"},
            {"VILLAGE","VLG"},
            {"VILLG","VLG"},
            {"VILLIAGE","VLG"},
            {"VLG","VLG"},
            {"VILLAGES","VLGS"},
            {"VLGS","VLGS"},
            {"VILLE","VL"},
            {"VL","VL"},
            {"VIS","VIS"},
            {"VIST","VIS"},
            {"VISTA","VIS"},
            {"VST","VIS"},
            {"VSTA","VIS"},
            {"WALK","WALK"},
            {"WALKS","WALK"},
            {"WALL","WALL"},
            {"WAY","WAY"},
            {"WY","WAY"},
            {"WAYS","WAYS"},
            {"WELL","WL"},
            {"WELLS","WLS"},
            {"WLS","WLS"}        
        };

        #endregion
    }

    public class AddressHelper2
    {
        #region singleton

        private static readonly Lazy<AddressHelper2> Singleton = new Lazy<AddressHelper2>(() => new AddressHelper2());
        public static AddressHelper2 I { get { return Singleton.Value; } }

        #endregion

        //private readonly IEasyPostClient _easyPost;

        //public AddressHelper2(IEasyPostClient easyPost = null)
        //{
        //    _easyPost = easyPost ?? new EasyPostClient("rPtF_txXZkE7VbUjffC9Cg");
        //}

        public class VerifiableAddress
        {
            public string Street1 { get; set; }
            public string Street2 { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string Zip { get; set; }
            public string Country { get; set; }
            public string Name { get; set; }
            public string Company { get; set; }

            public bool Verified { get; internal set; }
            public string Message { get; internal set; }
        }

        public VerifiableAddress TryVerifyAddres(VerifiableAddress toVerify)
        {
            return null;
            //var addr = _easyPost.CreateAddress(new Address
            //{
            //    Street1 = toVerify.Street1,
            //    Street2 = toVerify.Street2,
            //    City = toVerify.City,
            //    State = toVerify.State,
            //    Zip = toVerify.Zip,
            //    Country = toVerify.Country,
            //    Name = toVerify.Name,
            //    Company = toVerify.Company,
            //});

            //var ver = _easyPost.VerifyAddress(addr.Id);

            //if (ver.Address == null)
            //{
            //    return new VerifiableAddress
            //    {
            //        Verified = false, 
            //        Message = ver.Message,
            //    };
            //}

            //return new VerifiableAddress
            //{
            //    Verified = true,
            //    Street1 = ver.Address.Street1,
            //    Street2 = ver.Address.Street2,
            //    City = ver.Address.City,
            //    State = ver.Address.State,
            //    Zip = ver.Address.Zip,
            //    Country = ver.Address.Country,
            //    Name = ver.Address.Name,
            //    Company = ver.Address.Company,
            //    Message = ver.Message,
            //};
        }
    }
}

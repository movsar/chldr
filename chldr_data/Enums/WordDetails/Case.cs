using MongoDB.Bson;

namespace chldr_data.Enums.WordDetails
{
    public enum Case
    {
        Undefined = 0,

        AbsolutiveSingular = 1,
        GenitiveSingular = 2,
        DativeSingular = 3,
        ErgativeSingular = 4,
        AllativeSingular = 5,
        InstrumentalSingular = 6,
        LocativeSingular = 7,
        ComparativeSingular = 8,

        AbsolutivePlural = 9,
        GenitivePlural = 10,
        DativePlural = 11,
        ErgativePlural = 12,
        AllativePlural = 13,
        InstrumentalPlural = 14,
        LocativePlural = 15,
        ComparativePlural = 16,
    }
}
/*
    ц1ерниг - мила? х1ун?
    доланиг - хьенан? стенан?
    лург - хьанна? стенна?
    дийриг - хьан? стен?
    коьчалниг - хьаьнца? стенца?
    хотталург - хьанах? стенах?
    меттигниг - хьаьнга? стенга?
    дустург - хьанал? стенал?
*/
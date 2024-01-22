using Arinc.Spec424.Building;
using Arinc.Spec424.Records;
using Arinc.Spec424.Records.Subsequences;

namespace Arinc.Spec424;

internal class Parser424
{
    private readonly Queue<string> skippedStrings = [];

    private readonly Dictionary<Type, Queue<string>> recordStrings = [];

    internal Parser424()
    {
        foreach (var (type, info) in Meta424.RecordInfo)
            recordStrings.Add(type, []);
    }

    /// <summary>
    /// Checks that one of info matches the string and enqueue the matched to an associated queue.
    /// </summary>
    /// <param name="strings">Strings to check.</param>
    private void ProcessStrings(IEnumerable<string> strings)
    {
        foreach (string @string in strings)
        {
            if (!TryEnqueue(@string))
                skippedStrings.Enqueue(@string);
        }
    }

    private bool TryEnqueue(string @string)
    {
        foreach (var (type, strings) in recordStrings)
        {
            if (!Meta424.RecordInfo[type].IsMatch(@string))
                continue;
            
            strings.Enqueue(@string);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Constructs <see cref="Record424"/> objects type of <typeparamref name="TRecord"/>.
    /// </summary>
    /// <typeparam name="TRecord">Target type of records.</typeparam>
    /// <returns>Constructed records or empty.</returns>
    private Queue<TRecord> Construct<TRecord>() where TRecord : Record424, new()
    {
        var type = typeof(TRecord);

        Queue<TRecord> records = [];

        while (recordStrings[type].TryDequeue(out string? @string))
            records.Enqueue(RecordBuilder<TRecord>.Build(@string));

        return records;
    }

    /// <summary>
    /// Constructs <see cref="SequencedRecord424{TSequence}"/> type of <typeparamref name="TSequencedRecord"/> with sequence type of <typeparamref name="TSubsequence"/>.
    /// </summary>
    /// <typeparam name="TSequencedRecord">Target type of records.</typeparam>
    /// <typeparam name="TSubsequence">Target sequence type.</typeparam>
    /// <returns>Constructed records or empty.</returns>
    private Queue<TSequencedRecord> Construct<TSequencedRecord, TSubsequence>() where TSequencedRecord : SequencedRecord424<TSubsequence>, new()
                                                                                where TSubsequence : Record424, new()
    {
        var type = typeof(TSequencedRecord);

        var info = (SequencedRecordInfo)Meta424.RecordInfo[type];

        Queue<string> sequence = [];
        Queue<TSequencedRecord> records = [];

        if (!recordStrings[type].TryPeek(out string? @string))
            return records;

        int number = int.Parse(@string[info.SequenceNumberRange]);

        while (recordStrings[type].TryDequeue(out @string))
        {
            int next = int.Parse(@string[info.SequenceNumberRange]);

            if (next < number)
                records.Enqueue(RecordBuilder<TSequencedRecord, TSubsequence>.Build(sequence));

            number = next;

            sequence.Enqueue(@string);
        }

        records.Enqueue(RecordBuilder<TSequencedRecord, TSubsequence>.Build(sequence));

        return records;
    }

    [Obsolete("TODO multithreading")]
    internal Data424 Parse(IEnumerable<string> strings)
    {
        ProcessStrings(strings);

        var runways = Construct<Runway>();
        var airports = Construct<Airport>();

        var waypoints = Construct<Waypoint>();
        var cruisingTables = Construct<CruisingTable>();
        var holdingPatterns = Construct<HoldingPattern>();
        var flightPlannings = Construct<FlightPlanning>();
        var veryHighFrequencyAids = Construct<VeryHighFrequencyAid>();
        var nonDirectionalBeacons = Construct<NonDirectionalBeacon>();
        var microwaveLandingSystems = Construct<MicrowaveLandingSystem>();

        var airways = Construct<Airway, AirwayPoint>();
        var airportApproaches = Construct<AirportApproach, ProcedurePoint>();
        var standardTerminalArrivals = Construct<StandardTerminalArrival, ProcedurePoint>();
        var standardInstrumentDepartures = Construct<StandardInstrumentDeparture, ProcedurePoint>();

        var flightInfoRegions = Construct<FlightInfoRegion, BoundaryPoint>();
        var controlledAirspaces = Construct<ControlledAirspace, BoundaryPoint>();
        var restrictiveAirspaces = Construct<RestrictiveAirspace, BoundaryPoint>();

        return new Data424
        {
            Runways = runways,
            Airways = airways,
            Airports = airports.Link(runways).Link(airportApproaches).Link(standardTerminalArrivals).Link(standardInstrumentDepartures).Link(veryHighFrequencyAids).Link(nonDirectionalBeacons),
            Waypoints = waypoints,
            CruisingTables = cruisingTables,
            HoldingPatterns = holdingPatterns,
            FlightPlannings = flightPlannings,
            AirportApproaches = airportApproaches,
            VeryHighFrequencyAids = veryHighFrequencyAids,
            NonDirectionalBeacons = nonDirectionalBeacons,
            MicrowaveLandingSystems = microwaveLandingSystems,
            StandardTerminalArrivals = standardTerminalArrivals,
            StandardInstrumentDepartures = standardInstrumentDepartures,

            FlightInfoRegions = flightInfoRegions,
            ControlledAirspaces = controlledAirspaces,
            RestrictiveAirspaces = restrictiveAirspaces
        };
    }
}

using Arinc.Spec424;

namespace FAAtests;

public static class FAADataTest
{
    private static string[]? strings;

    private static void Main(string[] args)
    {
        ReadData();
        LoadDictionaries();
    }

    public static void ReadData() => strings = File.ReadAllLines("/Users/timothyjuntunen/Library/Mobile Documents/com~apple~CloudDocs/SoftwareDev/SRPilot/Resources/FAA_CIFP/CIFP_240222/FAACIFP18");

    public static void LoadDictionaries()
    {
        Data424 data;
        if (strings != null)
        {
            data = Data424.Load(strings);

        }
    }
}

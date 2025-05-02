using RouteBeheerBL.Interfaces;
using RouteBeheerBL.Managers;
using RouteBeheerDL;

namespace ConsoleAppImportGegevens {
    public class Program {
        static void Main() {
            //strings for pc
            const string connectionString = @"Data Source=NATHAN\SQLExpress;Initial Catalog=NetworkControlTesting;Integrated Security=True;Trust Server Certificate=True";
            const string pathFacilities = @"C:\Users\natha\programmerenGevorderd\EindopdrachtProgrammerenGevorderdRouteBeheer\InitializationFiles\faciliteiten.txt";
            const string pathNetworkPoints = @"C:\Users\natha\programmerenGevorderd\EindopdrachtProgrammerenGevorderdRouteBeheer\InitializationFiles\network_points.txt";
            const string pathStretches = @"C:\Users\natha\programmerenGevorderd\EindopdrachtProgrammerenGevorderdRouteBeheer\InitializationFiles\network_stretches.txt";
            const string pathFacilitiesLocations = @"C:\Users\natha\programmerenGevorderd\EindopdrachtProgrammerenGevorderdRouteBeheer\InitializationFiles\faciliteiten_locaties.txt";

            //strings for laptop
            //const string connectionString = @"Data Source=nathans-laptop\sqlexpress;Initial Catalog=NetworkControlTesting;Integrated Security=True;Trust Server Certificate=True";
            //const string pathFacilities = @"C:\Users\natha\programmerenGevorderd\Eindopdracht\EindopdrachtProgrammerenGevorderdRouteBeheer\InitializationFiles\faciliteiten.txt";
            //const string pathNetworkPoints = @"C:\Users\natha\programmerenGevorderd\Eindopdracht\EindopdrachtProgrammerenGevorderdRouteBeheer\InitializationFiles\network_points.txt";
            //const string pathStretches = @"C:\Users\natha\programmerenGevorderd\Eindopdracht\EindopdrachtProgrammerenGevorderdRouteBeheer\InitializationFiles\network_stretches.txt";
            //const string pathFacilitiesLocations = @"C:\Users\natha\programmerenGevorderd\Eindopdracht\EindopdrachtProgrammerenGevorderdRouteBeheer\InitializationFiles\faciliteiten_locaties.txt";

            INetworkRepository networkRepository = new NetworkRepository(connectionString);
            FileManager fm = new(pathFacilities, pathNetworkPoints, pathStretches, pathFacilitiesLocations, networkRepository);
            fm.InitializeNetwork();

            NetworkManager nm = new(networkRepository);
        }
    }
}

using RouteBeheerBL.Model;
using RouteBeheerBL.Managers;
using RouteBeheerDL;
using RouteBeheerBL.Interfaces;
using System.Net;

namespace ConsoleAppImportGegevens {
    public class Program {
        static void Main() {
            //strings for pc
            //const string connectionString = @"Data Source=NATHAN\SQLExpress;Initial Catalog=NetworkControlTesting;Integrated Security=True;Trust Server Certificate=True";
            //const string pathFacilities = @"C:\Users\natha\programmerenGevorderd\EindopdrachtProgrammerenGevorderdRouteBeheer\InitializationFiles\faciliteiten.txt";
            //const string pathNetworkPoints = @"C:\Users\natha\programmerenGevorderd\EindopdrachtProgrammerenGevorderdRouteBeheer\InitializationFiles\network_points.txt";
            //const string pathStretches = @"C:\Users\natha\programmerenGevorderd\EindopdrachtProgrammerenGevorderdRouteBeheer\InitializationFiles\network_stretches.txt";
            //const string pathFacilitiesLocations = @"C:\Users\natha\programmerenGevorderd\EindopdrachtProgrammerenGevorderdRouteBeheer\InitializationFiles\faciliteiten_locaties.txt";

            //strings for laptop
            const string connectionString = @"Data Source=nathans-laptop\sqlexpress;Initial Catalog=NetworkControlTesting;Integrated Security=True;Trust Server Certificate=True";
            const string pathFacilities = @"C:\Users\natha\programmerenGevorderd\Eindopdracht\EindopdrachtProgrammerenGevorderdRouteBeheer\InitializationFiles\faciliteiten.txt";
            const string pathNetworkPoints = @"C:\Users\natha\programmerenGevorderd\Eindopdracht\EindopdrachtProgrammerenGevorderdRouteBeheer\InitializationFiles\network_points.txt";
            const string pathStretches = @"C:\Users\natha\programmerenGevorderd\Eindopdracht\EindopdrachtProgrammerenGevorderdRouteBeheer\InitializationFiles\network_stretches.txt";
            const string pathFacilitiesLocations = @"C:\Users\natha\programmerenGevorderd\Eindopdracht\EindopdrachtProgrammerenGevorderdRouteBeheer\InitializationFiles\faciliteiten_locaties.txt";

            INetworkRepository networkRepository = new NetworkRepository(connectionString);
            //FileManager fm = new(pathFacilities, pathNetworkPoints, pathStretches, pathFacilitiesLocations, networkRepository);
            //fm.InitializeNetwork();

            NetworkManager nm = new(networkRepository);
            List<Stretch> stretches = nm.ReadNetwork();
            foreach (var stretch in stretches) {
                Console.WriteLine(stretch.Id);
                foreach(var point in stretch.NetworkPoints) {
                    Console.WriteLine($"    id:{point.Id} X:{point.X} Y:{point.Y}");
                    foreach(var facility in point.Facilities) {
                        Console.WriteLine($"        id:{facility.Id} name:{facility.Name}");
                    }
                }
            }
        }
    }
}

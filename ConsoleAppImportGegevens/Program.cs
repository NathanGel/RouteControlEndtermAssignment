using RouteBeheerBL.Model;
using RouteBeheerBL.Managers;
using RouteBeheerDL;
using RouteBeheerBL.Interfaces;
using System.Net;

namespace ConsoleAppImportGegevens {
    public class Program {
        static void Main() {
            string connectionString = @"Data Source=NATHAN\SQLExpress;Initial Catalog=NetworkControl;Integrated Security=True;Trust Server Certificate=True";
            string pathFacilities = @"C:\Users\natha\Downloads\faciliteiten.txt";
            string pathNetworkPoints = @"C:\Users\natha\Downloads\network_points.txt";
            string pathStretches = @"C:\Users\natha\Downloads\network_stretches.txt";
            string pathFacilitiesLocations = @"C:\Users\natha\Downloads\faciliteiten_locaties.txt";
            Program p = new Program();
            //p.ImportFacilities(pathFacilities, connectionString);
            p.InitializeNetwork(pathFacilities ,pathNetworkPoints, pathStretches, pathFacilitiesLocations, connectionString);
        }

        public void InitializeNetwork(string pathFacilities, string pathNetworkPoints, string pathStretches, string pathFacilitiesLocaties, string connectionString) {
            IFacilityRepository facilityRepository = new FaciltyRepository(connectionString);
            INetworkRepository networkRepository = new NetworkRepository(connectionString);
            FileManager fileManager = new FileManager(pathFacilities, pathNetworkPoints, pathStretches, connectionString, facilityRepository, networkRepository);
            fileManager.ReadFacilities();
            fileManager.ReadFacilitiesLocations();
            fileManager.ReadNetworkPoints();
            fileManager.ReadStretches();
        }
    }
}

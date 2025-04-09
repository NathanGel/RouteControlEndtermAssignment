using RouteBeheerBL.Model;
using RouteBeheerBL.Managers;
using RouteBeheerDL;
using RouteBeheerBL.Interfaces;

namespace ConsoleAppImportGegevens {
    public class Program {
        static void Main() {
            string connectionString = @"Data Source=NATHAN\SQLExpress;Initial Catalog=NetworkControl;Integrated Security=True;Trust Server Certificate=True";
            //string pathFacilities = @"C:\Users\natha\Downloads\faciliteiten.txt";
            string pathNetworkPoints = @"C:\Users\natha\Downloads\network_points.txt";
            Program p = new Program();
            //p.ImportFacilities(pathFacilities, connectionString);
            p.InitializeNetwork(pathNetworkPoints, connectionString);
        }

        public void ImportFacilities(string path, string connectionString) {
            FacilityManager fManager = new FacilityManager(new FaciltyRepository(connectionString));
            List<Facility> facilities = new();
            using (StreamReader sr = new(path)) {
                string line;
                while ((line = sr.ReadLine()) != null) {
                    string[] parts = line.Split(',');
                    facilities.Add(new(parts[1]));
                }
                foreach (var facility in facilities) {
                    fManager.AddFacility(facility);
                }
            }
        }

        public void InitializeNetwork(string path, string connectionString) {
            NetworkManager nManager = new NetworkManager(new NetworkRepository(connectionString));
            List<NetworkPoint> networkPoints = new();
            using (StreamReader sr = new(path)) {
                string line;
                while ((line = sr.ReadLine()) != null) {
                    string[] parts = line.Split(',');
                    networkPoints.Add(new(double.Parse(parts[1]), double.Parse(parts[2])));
                }
                foreach (var networkPoint in networkPoints) {
                    nManager.AddNetworkPoint(networkPoint);
                }
            }
        }
    }
}

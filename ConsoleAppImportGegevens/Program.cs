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
            Program p = new Program();
            //p.ImportFacilities(pathFacilities, connectionString);
            p.InitializeNetwork(pathFacilities ,pathNetworkPoints, pathStretches, connectionString);
        }

        public void InitializeNetwork(string pathFacilities, string pathNetworkPoints, string pathStretches, string connectionString) {
            FileManager fileManager = new FileManager(pathFacilities, pathNetworkPoints, pathStretches, connectionString);
            //fileManager.ReadFacilities();
            //List<int> ids = fileManager.ReadNetworkPoints();
            List<int> ids= new();
            fileManager.ReadStretches(ids);
        }
    }
}

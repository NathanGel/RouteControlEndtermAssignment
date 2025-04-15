using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RouteBeheerBL.Interfaces;
using RouteBeheerBL.Model;

namespace RouteBeheerBL.Managers {
    public class FileManager {
        private readonly string _pathFacilities;
        private readonly string _pathNetworkPoints;
        private readonly string _pathStretches;
        private readonly string _connectionString;

        private IReadOnlyList<Facility> _facilities;
        private IReadOnlyList<NetworkPoint> _networkPoints;
        private IReadOnlyList<Stretch> _stretches;


        private IFacilityRepository _facilityRepository;
        public FileManager(string pathFacilities, string pathNetworkPoints, string pathStretches, string connectionString) {
            _pathFacilities = pathFacilities;
            _pathNetworkPoints = pathNetworkPoints;
            _pathStretches = pathStretches;
            _connectionString = connectionString;
        }
        
        public void ReadFacilities() {

        }
    }
}

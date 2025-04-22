using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using RouteBeheerBL.Exceptions;
using RouteBeheerBL.Interfaces;
using RouteBeheerBL.Model;

namespace RouteBeheerBL.Managers {
    public class FileManager {
        private readonly string _pathFacilities;
        private readonly string _pathNetworkPoints;
        private readonly string _pathStretches;
        private readonly string _pathFacilitiesLocations;
        public FileManager(string pathFacilities, string pathNetworkPoints, string pathStretches, string pathFacilitiesLocations, INetworkRepository networkRepo) {
            _pathFacilities = pathFacilities;
            _pathNetworkPoints = pathNetworkPoints;
            _pathStretches = pathStretches;
            _pathFacilitiesLocations = pathFacilitiesLocations;
            _networkPointRepository = networkRepo;
        }

        private List<Facility> _facilities = new();
        private Dictionary<int, int> _facilityMapping = new();
        private List<NetworkPoint> _networkPoints = new();
        private Dictionary<int, int> _networkPointMapping = new();

        private INetworkRepository _networkPointRepository;

        public void InitializeNetwork() {
            try {
                ReadFacilities();
                ReadNetworkPoints();
                ReadFacilitiesLocations();
                ReadStretches();
                //_networkPointRepository.InitializeNetwork(_facilities, _networkPoints, _stretches, _networkPointMapping, _facilityMapping);
            } catch (Exception ex) {
                throw new NetworkInitializationException("Error initializing network", ex);
            }
        }

        public void ReadFacilities() {
            try {
                using (StreamReader sr = new(_pathFacilities)) {
                    string line;
                    while ((line = sr.ReadLine()) != null) {
                        string[] parts = line.Split(',');
                        _facilityMapping.Add(int.Parse(parts[0]), int.Parse(parts[0])); //dictionary die de id van de faciliteit koppelt aan de id van de facility in de database
                        Facility f = new(int.Parse(parts[0]), parts[1]);
                        _facilities.Add(f);
                    }
                }
            } catch (Exception ex) {
                throw new NetworkInitializationException("Error reading facilities file", ex);
            }
        }

        public void ReadNetworkPoints() {
            try {
                using (StreamReader sr = new(_pathNetworkPoints)) {
                    string line;
                    while ((line = sr.ReadLine()) != null) {
                        string[] parts = line.Split('|');
                        int id = int.Parse(parts[0]);
                        double x = double.Parse(parts[1]);
                        double y = double.Parse(parts[2]);
                        NetworkPoint np = new(x, y);
                        np.Id = id;
                        _networkPoints.Add(np);
                        _networkPointMapping.Add(id, id); //dictionary die de id van de networkpoint koppelt aan de id van de networkpoint in de database
                    }
                }
            } catch (Exception ex) {
                throw new NetworkInitializationException("Error reading network points file", ex);
            }
        }

        public void ReadFacilitiesLocations() {
            try {
                using (StreamReader sr = new(_pathFacilitiesLocations)) {
                    string line;
                    int npId = 1;
                    while ((line = sr.ReadLine()) != null) {
                        string[] parts = line.Split(',');
                        if (int.Parse(parts[0]) != npId)//check om te kijken of het nog steeds over hetzellfde networkpoint gaat
                            npId = int.Parse(parts[0]);
                        int indexNp = _networkPoints.FindIndex(np => np.Id == npId);
                        int indexFacility = _facilities.FindIndex(f => f.Id == int.Parse(parts[1]));
                        _networkPoints[indexNp].Facilities.Add(_facilities[indexFacility]);
                    }
                }
            } catch (Exception ex) {
                throw new NetworkInitializationException("Error reading facilities locations file", ex);
            }
        }

        //public void ReadStretches() {
        //    using (StreamReader sr = new(_pathStretches)) {
        //        string line;
        //        while ((line = sr.ReadLine()) != null) {
        //            if (line.StartsWith("stretch")) {
        //                continue;
        //            } else {
        //                line.Trim();
        //                Stretch stretch;
        //                List<NetworkPoint> points = new();
        //                string[] parts = line.Split(")");
        //                for (int i = 0; i < parts.Length; i++) {
        //                    parts[i] = parts[i].Replace("(", "");
        //                    if (string.IsNullOrWhiteSpace(parts[i])) continue;
        //                    int indexOfNetworkPoint = _networkPoints.FindIndex(np => np.Id == int.Parse(parts[i]));
        //                    points.Add(_networkPoints[indexOfNetworkPoint]);
        //                }
        //                stretch = new(points);
        //                _stretches.Add(stretch);
        //            }
        //        }
        //    }
        //}
        //public void ReadStretches() {
        //    try {
        //        using (StreamReader sr = new(_pathStretches)) {
        //            string line;
        //            int current = 0;
        //            Dictionary<int, NetworkPoint> points = null; // Initialize as null to avoid shared reference issues
        //            while ((line = sr.ReadLine()) != null) {
        //                if (line.StartsWith("Network")) {
        //                    if (points != null && points.Count > 0) { // Ensure points is not null before adding
        //                        _stretches.Add(new Stretch(new Dictionary<int, NetworkPoint>(points)));
        //                    }// Create a new list to avoid shared references
        //                    points = new Dictionary<int, NetworkPoint>(); // Reinitialize points for the next stretch
        //                    current = 0; // Reset current for the new stretch
        //                    continue;
        //                }
                        
        //                string[] parts = line.Split(')');
        //                foreach (var part in parts) {
        //                    string[] splitPart = part.Replace(" ", "").Split('|');
        //                    if (!string.IsNullOrWhiteSpace(part)) {
        //                        int indexOfNetworkPoint = _networkPoints.FindIndex(np =>
        //                            np.X == double.Parse(splitPart[1]) &&
        //                            np.Y == double.Parse(splitPart[2].Replace(")", "")));
        //                        points.Add(current++ ,_networkPoints[indexOfNetworkPoint]);
        //                        //Console.WriteLine($"{current} {_networkPoints[indexOfNetworkPoint].X}");
        //                    }
        //                }
        //            }
        //            // Add the last stretch if points are not empty
        //            if (points != null && points.Count > 0)
        //                _stretches.Add(new Stretch(new Dictionary<int, NetworkPoint>(points)));
        //        }
        //        //foreach (var stretch in _stretches) {
        //        //    Console.WriteLine(stretch);
        //        //    foreach (var point in stretch.NetworkPoints) {
        //        //        Console.WriteLine($"({point.Id}|X:{point.X}|Y:{point.Y})");
        //        //    }
        //        //}
        //    } catch (Exception ex) {
        //        throw new NetworkInitializationException("Error reading stretches file", ex);
        //    }
        //}
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using RouteBeheerBL.Interfaces;
using RouteBeheerBL.Model;

namespace RouteBeheerBL.Managers {
    public class FileManager {
        private readonly string _pathFacilities;
        private readonly string _pathNetworkPoints;
        private readonly string _pathStretches;
        private readonly string _pathFacilitiesLocations;
        private readonly string _connectionString;

        private List<Facility> _facilities;
        private List<NetworkPoint> _networkPoints;
        private List<Stretch> _stretches;


        private IFacilityRepository _facilityRepository;
        private INetworkRepository _networkPointRepository;
        public FileManager(string pathFacilities, string pathNetworkPoints, string pathStretches, string connectionString, IFacilityRepository facilityrepo, INetworkRepository networkRepo) {
            _pathFacilities = pathFacilities;
            _pathNetworkPoints = pathNetworkPoints;
            _pathStretches = pathStretches;
            _connectionString = connectionString;
            _facilityRepository = facilityrepo;
            _networkPointRepository = networkRepo;
        }
        
        public void ReadFacilities() {
            using (StreamReader sr = new(_pathFacilities)) {
                string line;
                while((line = sr.ReadLine()) != null) {
                    string[] parts = line.Split(',');
                    Facility f = new(parts[1]);
                    _facilities.Add(f);
                }
            }
            WriteFacilities();
        }

        public void WriteFacilities() {
            foreach (var facility in _facilities) {
                _facilityRepository.AddFacility(facility);
            }
        }

        public void ReadFacilitiesLocations() {
            using (StreamReader sr = new("")) {
                string line;
                while ((line = sr.ReadLine()) != null) {
                    
                }
            }
        }

        public void WriteFacilitiesLocations() {
            
        }

        public void ReadNetworkPoints() {
            using (StreamReader sr = new(_pathNetworkPoints)) {
                string line;
                while ((line = sr.ReadLine()) != null) {
                    string[] parts = line.Split('|');
                    double x = double.Parse(parts[1], CultureInfo.InvariantCulture); //InvariuantCulture is used to ensure that the decimal separator is a dot
                    double y = double.Parse(parts[2], CultureInfo.InvariantCulture);
                    NetworkPoint np = new(x, y);
                    _networkPoints.Add(np);
                }
            }
            WriteNetworkPoints();
        }

        public List<NetworkPoint> WriteNetworkPoints() {
            foreach (var networkPoint in _networkPoints) {
                int id = _networkPointRepository.AddNetworkPoint(networkPoint);
                networkPoint.Id = id;
                _networkPoints.Add(networkPoint);
            }
            return _networkPoints;
        }

        public void ReadStretches() {
            int currentId = 0;
            using (StreamReader sr = new(_pathStretches)) {
                string line;
                while ((line = sr.ReadLine()) != null) {
                    if (line.StartsWith("stretch")) {
                        continue;
                    } else {
                        Stretch stretch;
                        List<NetworkPoint> points = new();
                        string[] parts = line.Split(")");
                        for (int i = 0; i < parts.Length; i++) {
                            parts[i] = parts[i].Replace("(", "");
                            points.Add(_networkPoints[currentId]);
                            currentId++;
                        }
                        stretch = new(points);
                        _stretches.Add(stretch);
                    }
                }
            }
        }

        public void WriteStretches() {
            foreach (var stretch in _stretches) {
                for (int i = 0; i < stretch.NetworkPoints.Count; i++) {
                    if (i == stretch.NetworkPoints.Count - 1)
                        return;
                    else {
                        _networkPointRepository.ConnectNetworkPoint(stretch.NetworkPoints[i], stretch.NetworkPoints[i + 1]);
                    }
                }
            }
        }
    }
}

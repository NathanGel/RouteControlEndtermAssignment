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

        private Dictionary<int, Facility> _facilityMapping = new();
        private Dictionary<int, NetworkPoint> _networkPointMapping = new();
        private List<Segment> _segments = new();

        private INetworkRepository _networkPointRepository;

        public void InitializeNetwork() {
            try {
                ReadFacilities();
                ReadNetworkPoints();
                ReadFacilitiesLocations();
                ReadStretches();
                _networkPointRepository.InitializeNetwork(_networkPointMapping, _facilityMapping, _segments);
            } catch (ApplicationException ex) {
                Console.WriteLine("InitializeNetwork", ex);
            } catch (NetworkInitializationException ex) {
                Console.WriteLine("An error occured while initializing the network: " + ex.Source);
            } catch (Exception ex) {
                Console.WriteLine("Unexpected exception: " + ex);
            }
        }

        public void ReadFacilities() {
            try {
                using (StreamReader sr = new(_pathFacilities)) {
                    string line;
                    while ((line = sr.ReadLine()) != null) {
                        string[] parts = line.Split(',');
                        int id = int.Parse(parts[0]);
                        _facilityMapping.Add(id, new(id, parts[1])); //dictionary die de id van de faciliteit koppelt aan de id van de facility in de database
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
                        _networkPointMapping.Add(id, new(id, double.Parse(parts[1]), double.Parse(parts[2]))); //dictionary die de id van de networkpoint koppelt aan de id van de networkpoint in de database
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
                    while ((line = sr.ReadLine()) != null) {
                        string[] parts = line.Split(',');
                        int npId = int.Parse(parts[0]);
                        int idFacility = int.Parse(parts[1]);
                        _networkPointMapping[npId].Facilities.Add(_facilityMapping[idFacility]); //voeg de faciliteit toe aan het networkpoint
                    }
                }
            } catch (Exception ex) {
                throw new NetworkInitializationException("Error reading facilities locations file", ex);
            }
        }

        public void ReadStretches() {
            try {
                using (StreamReader sr  = new(_pathStretches)) {
                    string line;
                    while((line = sr.ReadLine()) != null) {
                        if (line.StartsWith("stretch")) continue; //sla de lijn met stretch + nummer over
                        string[] parts = line.Split(')');
                        List<int> partsTrimmed = new List<int>();
                        foreach (string part in parts) {
                            if(string.IsNullOrWhiteSpace(part)) continue;
                            partsTrimmed.Add(int.Parse(part.Replace(")", "").Replace("(", "").Trim())); //De delen van parts opschonen door de haakjes/spaties te verwijderen en ze dan om te zetten naar integers 
                        }
                        for(int i = 1; i<partsTrimmed.Count; i++) {
                            _segments.Add(new(_networkPointMapping[partsTrimmed[i-1]], _networkPointMapping[partsTrimmed[i]])); //Segment toevoegen met start en eindpunt op basis van de id in partsTrimmed
                            //Console.WriteLine($"Segment {i} X1:{_networkPointMapping[partsTrimmed[i - 1]].X} Y1:{_networkPointMapping[partsTrimmed[i - 1]].Y} X2:{_networkPointMapping[partsTrimmed[i]].X} Y2:{_networkPointMapping[partsTrimmed[i]].Y}");
                        }
                    }
                }
            }catch(Exception ex) {
                throw new NetworkInitializationException("Error reading stretches file", ex);
            }
        }
    }
}

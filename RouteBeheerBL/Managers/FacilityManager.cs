using RouteBeheerBL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RouteBeheerBL.Model;
using RouteBeheerBL.Exceptions;

namespace RouteBeheerBL.Managers {
    public class FacilityManager {
        private IFacilityRepository repo;
        public FacilityManager(IFacilityRepository repo) {
            this.repo = repo;
        }
        
        public void AddFacility(Facility facility) {
            if (facility == null) throw new FacilityException("AddFacility Invalid Null");
            repo.AddFacility(facility);
        }

        public void RemoveFacility(int id) {
            if (id <= 0) throw new FacilityException("RemoveFacility Invalid Id ");
            repo.RemoveFacility(id);
        }


        public void UpdateFacility(Facility facility) {
            if (facility == null) throw new FacilityException("UpdateFacility Invalid Null");
            repo.UpdateFacility(facility);
        }
        public void GetFacility(int id) {
            if (id <= 0) throw new FacilityException("UpdateFacility Invalid Null");
            repo.GetFacility(id);
        }

        public List<Facility> GetAllFacilities() {
            return repo.GetAllFacilities();
        }
    }
}

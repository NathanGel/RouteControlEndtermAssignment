using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RouteBeheerBL.Model;

namespace RouteBeheerBL.Interfaces {
    public interface IFacilityRepository {
        void AddFacility(Facility facility);
        void RemoveFacility(int id);
        Facility GetFacility(int id);
        void UpdateFacility(Facility facility);
        List<Facility> GetAllFacilities();
    }
}

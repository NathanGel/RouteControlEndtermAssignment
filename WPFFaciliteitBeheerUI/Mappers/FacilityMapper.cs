using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFFaciliteitBeheerUI.Model;
using RouteBeheerBL.Model;

namespace WPFFaciliteitBeheerUI.Mappers {
    public static class FacilityMapper {
        public static Facility MapToDomain(FacilityUI facility) {
            return new(facility.Id, facility.Name);
        }

        public static FacilityUI MapFromDomain(Facility facility) {
            return new FacilityUI(facility.Id, facility.Name);
        }
    }
}

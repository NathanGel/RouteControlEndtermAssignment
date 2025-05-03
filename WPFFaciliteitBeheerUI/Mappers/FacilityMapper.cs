using RouteBeheerBL.Model;
using WPFFaciliteitBeheerUI.Model;

namespace WPFFaciliteitBeheerUI.Mappers {
    public static class FacilityMapper {
        public static Facility MapToDomain(FacilityUI facility) {
            return new(facility.Id, facility.Name);
        }

        public static Facility MapToDomainNoId(FacilityUI facility) {
            return new(facility.Name);
        }

        public static FacilityUI MapFromDomain(Facility facility) {
            return new FacilityUI(facility.Id, facility.Name);
        }
    }
}

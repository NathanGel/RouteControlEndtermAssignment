using RouteBeheerBL.Model;
using System.Collections.ObjectModel;
using WPFNetwerkBeheerUI.Model;

namespace WPFNetwerkBeheerUI.Mappers {
    public static class NetworkPointMapper {
        public static NetworkPoint MapToDomain(NetworkPointUI networkPoint) {
            return new(networkPoint.Id, networkPoint.X, networkPoint.Y, networkPoint.Facilities.ToList());
         }

        public static NetworkPointUI MapFromDomain(NetworkPoint networkPoint) {
            ObservableCollection<Facility> facilities = new ObservableCollection<Facility>(networkPoint.Facilities);
            return new NetworkPointUI(networkPoint.Id, networkPoint.X, networkPoint.Y, facilities);
        }
    }
}

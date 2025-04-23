using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RouteBeheerBL.Model;
using WPFNetwerkBeheerUI.Model;

namespace WPFNetwerkBeheerUI.Mappers {
    public static class NetworkPointMapper {
        public static NetworkPoint MapToDomain(NetworkPointUI networkPoint) {
            List<Facility> facilities = new();
            foreach (var item in networkPoint.Facilities) {
                facilities.Add(FacilityMapper.MapToDomain(item));
            }
            return new(networkPoint.Id, networkPoint.X, networkPoint.Y, facilities);
        }

        public static NetworkPointUI MapFromDomain(NetworkPoint networkPoint) {
            ObservableCollection<FacilityUI> facilities = new();
            foreach (var item in networkPoint.Facilities) {
                facilities.Add(FacilityMapper.MapFromDomain(item));
            }
            return new NetworkPointUI(networkPoint.Id, networkPoint.X, networkPoint.Y, facilities);
        }
    }
}

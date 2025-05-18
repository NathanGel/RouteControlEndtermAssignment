using RouteBeheerBL.Model;
using System.Collections.Generic;
using System.Linq;
using WPFRouteBeheerUI.Model;

namespace WPFRouteBeheerUI.Mappers {
    public static class NetworkPointStopsMapper {
        public static List<NetworkPointStopsUI> MapToUIModel(List<(NetworkPoint point, bool isStop)> stops) {
            return stops.Select(s => new NetworkPointStopsUI(
                s.point,
                s.isStop
            )).ToList();
        }

        public static List<(NetworkPoint point, bool isStop)> MapFromUIModel(IEnumerable<NetworkPointStopsUI> entries) {
            return entries.Select(e => (
                e.point,
                e.IsStop
            )).ToList();
        }
    }
}

using RouteBeheerBL.Model;
using WPFNetwerkBeheerUI.Model;

namespace WPFNetwerkBeheerUI.Mappers {
    public static class SegmentMapper {
        public static Segment MapToDomain(SegmentUI segment) {
            return new(segment.Id, NetworkPointMapper.MapToDomain(segment.StartPoint), NetworkPointMapper.MapToDomain(segment.EndPoint));
        }

        public static Segment MapToDomainWithoutId(SegmentUI segment) {
            return new(NetworkPointMapper.MapToDomain(segment.StartPoint), NetworkPointMapper.MapToDomain(segment.EndPoint));
        }

        public static SegmentUI MapFromDomain(Segment segment) {
            return new(segment.Id, NetworkPointMapper.MapFromDomain(segment.StartPoint), NetworkPointMapper.MapFromDomain(segment.EndPoint));
        }
    }
}

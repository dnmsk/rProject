using Project_B.CodeServerSide.Enums;

namespace Project_B.CodeServerSide.Entity.Interface {
    public interface IRawLinkEntity {
        int LinkToEntityID { get; }
        BrokerEntityType EntityType { get; }
    }
}
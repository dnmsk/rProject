using Project_B.CodeServerSide.Entity.Interface;

namespace Project_B.CodeServerSide.DataProvider.DataHelper.RawData {
    public class RawObject : IKeyBrokerEntity  {
        public int ID { get; set; }
        public int ParentID { get; set; }
    }
}
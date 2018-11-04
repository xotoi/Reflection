using MyIoC.Attributes;

namespace IoCSample.TestEntities
{
    public interface ICustomerDAL
    {
    }

    [Export(typeof(ICustomerDAL))]
    public class CustomerDAL : ICustomerDAL
    {
        public CustomerDAL() { }
    }
}

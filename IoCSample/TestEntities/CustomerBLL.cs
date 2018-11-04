using MyIoC.Attributes;

namespace IoCSample.TestEntities
{
    [ImportConstructor]
    public class CustomerBLL
    {
        public CustomerBLL(ICustomerDAL dal, Logger logger)
        { }
    }

    public class CustomerBLL2
    {
        [Import]
        public ICustomerDAL CustomerDAL { get; set; }
        [Import]
        public Logger Logger { get; set; }
    }
}

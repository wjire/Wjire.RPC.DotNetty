using System.ServiceModel;

namespace IServices
{
    [ServiceContract]
    public interface IFoo
    {
        Person Get();
    }
}

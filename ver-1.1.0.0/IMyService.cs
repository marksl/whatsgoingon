using System.ServiceModel;

namespace ServiceExample
{
    [ServiceContract]
    public interface IMyService
    {
        [OperationContract]
        string Add(int a, int b);

        [OperationContract]
        string Sub(int a, int b);
    }
}
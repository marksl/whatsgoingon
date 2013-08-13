using System.Runtime.Serialization;
using System.ServiceModel;

namespace ServiceExample
{
    [ServiceContract]
    public interface IMyService
    {
        [OperationContract]
        EchoData Echo(EchoData data);
    }

    [DataContract]
    public class EchoData
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}
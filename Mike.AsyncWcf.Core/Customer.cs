using System.Runtime.Serialization;

namespace Mike.AsyncWcf.Core
{
    [DataContract]
    public class Customer
    {
        [DataMember]
        public int Id { get; private set; }

        [DataMember]
        public string Name { get; private set; }

        public Customer(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
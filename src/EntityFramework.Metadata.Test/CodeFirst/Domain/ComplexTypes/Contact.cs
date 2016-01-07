namespace EntityFramework.Metadata.Test.CodeFirst.Domain.ComplexTypes
{
    public class Contact
    {
        public string PhoneNumber { get; set; }
        public Address BusinessAddress { get; set; }
        public Address ShippingAddress { get; set; }
    }
}
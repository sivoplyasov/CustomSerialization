using System;
using System.IO;
using System.Threading.Tasks;
using SerializerTests.Interfaces;
using SerializerTests.Nodes;

namespace SerializerTests.Implementations
{
    //Specify your class\file name and complete implementation.
    public class JohnSmithSerializer : IListSerializer
    {
        //the constructor with no parameters is required and no other constructors can be used.
        public JohnSmithSerializer()
        {
            //...
        }

        public async Task<ListNode> DeepCopy(ListNode head)
        {
            throw new NotImplementedException();
        }

        public async Task<ListNode> Deserialize(Stream s)
        {
            throw new NotImplementedException();
        }

        public async Task Serialize(ListNode head, Stream s)
        {
            throw new NotImplementedException();
        }
    }
}

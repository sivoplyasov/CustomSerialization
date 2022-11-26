using System.Text;
using System.Text.Json;
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
            var copedNodes = new Dictionary<int, ListNode>();

            var result = await MakeCopy(null, head, copedNodes);

            return result;
        }

        public async Task<ListNode> Deserialize(Stream s)
        {
            throw new NotImplementedException();
        }

        public async Task Serialize(ListNode head, Stream s)
        {
            throw new NotImplementedException();
        }

        #region Private methods

        private async Task<ListNode> MakeCopy(ListNode? prvious, ListNode nodeToCopy, Dictionary<int, ListNode> copedNodes)
        {
            var result = new ListNode { Data = nodeToCopy.Data, Previous = prvious };
            copedNodes.Add(nodeToCopy.GetHashCode(), result);

            if (nodeToCopy.Next != null)
                result.Next = await MakeCopy(result, nodeToCopy.Next, copedNodes);

            var resultHash = result.GetHashCode();
            copedNodes.Add(resultHash, result);

            if (nodeToCopy.Random != null)
            {
                if (nodeToCopy.Random == nodeToCopy)
                {
                    result.Random = result;
                }
                else
                {
                    var randomHash = nodeToCopy.Random.GetHashCode();
                    result.Random = copedNodes[randomHash];
                }
            }

            return result;
        }

        private async Task SerializeInternal(ListNode node, SerializableNode previousNode, Stream s)
        {
            var nodeToSerialize = new SerializableNode { HashCode = node.GetHashCode(), Data = node.Data };

            if (node.Random != null)
                nodeToSerialize.RandomHashCode = node.Random.GetHashCode();

            if (previousNode != null)
            {
                nodeToSerialize.Previous = previousNode;
                nodeToSerialize.PrevHashCode = previousNode.HashCode;
            }

            if (node.Next != null)
            {
                nodeToSerialize.NextHashCode = node.Next.GetHashCode();
                await SerializeInternal(node.Next, nodeToSerialize, s);
            }

            await JsonSerializer.SerializeAsync(s, nodeToSerialize, typeof(SerializableNode));
        }

        #endregion

        private class SerializableNode
        {
            public int HashCode { get; set; }

            public SerializableNode? Previous { get; set; }

            public SerializableNode? Next { get; set; }

            public string? Data { get; set; }

            public int? PrevHashCode { get; set; }

            public int? NextHashCode { get; set; }

            public int? RandomHashCode { get; set; }

            public string? NextJsonContent { get; set; }
        }
    }
}

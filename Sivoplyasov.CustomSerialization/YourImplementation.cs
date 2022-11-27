using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Xml.Linq;
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

        public async Task<ListNode?> Deserialize(Stream s)
        {
            ListNode result = null;

            if (s.CanRead)
            {
                var bytes = new byte[s.Length];
                var serializedNodes = new Dictionary<int, ListNode>();
                s.Seek(0, SeekOrigin.Begin);

                await s.ReadAsync(bytes);

                result = await DeserializeInternal(bytes, serializedNodes);
            }
            else
            {
                throw new ArgumentException($"Error in argument {nameof(s)}: this type of streams is not supports reading");
            }

            return result;
        }

        public async Task Serialize(ListNode head, Stream s)
        {
            var bytes = await SerializeInternal(head, null, s);
            await s.WriteAsync(bytes);
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

        private async Task<byte[]?> SerializeInternal(ListNode node, SerializableNode? previousNode, Stream s)
        {
            var nodeToSerialize = new SerializableNode { OriginalHashCode = node.GetHashCode(), Data = node.Data };

            if (node.Random != null)
                nodeToSerialize.RandomHashCode = node.Random.GetHashCode();

            if (node.Next != null)
            {
                nodeToSerialize.NextJsonBytes = await SerializeInternal(node.Next, nodeToSerialize, s);
            }

            var bytes = JsonSerializer.SerializeToUtf8Bytes(nodeToSerialize);
            return bytes;
        }

        private async Task<ListNode> DeserializeInternal(byte[] bytes, Dictionary<int, ListNode> serializedNodes)
        {
            SerializableNode? nextNode = null;
            var result = new ListNode();

            try
            {
                nextNode = JsonSerializer.Deserialize<SerializableNode>(bytes);
            }
            catch(Exception ex)
            {
                throw new ArgumentException("Invalid data format. Can't deserialize from this stream. See inner exception(s) for details", ex);
            }

            if (nextNode != null)
            {
                result.Data = nextNode.Data;
                serializedNodes.Add(nextNode.OriginalHashCode, result);

                if (nextNode.NextJsonBytes != null)
                {
                    var next = await DeserializeInternal(nextNode.NextJsonBytes, serializedNodes);
                    result.Next = next;
                    next.Previous = result;
                }

                if (nextNode.RandomHashCode.HasValue)
                    result.Random = serializedNodes[nextNode.RandomHashCode.Value];
            }

            return result;
        }

        #endregion

        #region Private classes

        private class SerializableNode
        {
            public int OriginalHashCode { get; set; }

            public string? Data { get; set; }

            public byte[]? NextJsonBytes { get; set; }

            public int? RandomHashCode { get; set; }
        }

        #endregion
    }
}

using Newtonsoft.Json;
using SerializerTests.Interfaces;
using SerializerTests.Nodes;
using System.Text;
using Newtonsoft.Json.Linq;

namespace SerializerTests.Implementations
{
    public class SivoplyasovSerializer : IListSerializer
    {
        //the constructor with no parameters is required and no other constructors can be used.
        public SivoplyasovSerializer()
        {
            //...
        }

        public async Task<ListNode> DeepCopy(ListNode head)
        {
            var result = await MakeCopy(head);

            return result;
        }

        public async Task<ListNode> Deserialize(Stream s)
        {
            if (s.CanRead)
            {
                s.Seek(0, SeekOrigin.Begin);
                var result = await DeserializeInternal(s);

                return result;
            }
            else
            {
                throw new ArgumentException($"Error in argument {nameof(s)}: this type of streams is not supports reading");
            }
        }

        public async Task Serialize(ListNode head, Stream s)
        {
            await SerializeInternal(head, s);
        }

        #region Private methods

        private async Task<ListNode?> MakeCopy(ListNode? nodeToCopy)
        {
            ListNode? copedHead = null;
            ListNode? previous = null;

            var copedNodes = new Dictionary<int, ListNode>();

            while (nodeToCopy != null)
            {
                ListNode? copedNode;
                var nodeOriginalHash = nodeToCopy.GetHashCode();

                if (copedNodes.ContainsKey(nodeOriginalHash))
                {
                    copedNode = copedNodes[nodeOriginalHash];
                }
                else
                {
                    copedNode = new ListNode { Data = nodeToCopy.Data };
                    copedNodes.Add(nodeOriginalHash, copedNode);
                }

                copedNode.Previous = previous;

                if (previous != null)
                    previous.Next = copedNode;

                if (nodeToCopy.Random != null)
                {
                    ListNode copyOfRandom;
                    var randomOriginalHash = nodeToCopy.Random.GetHashCode();

                    if (copedNodes.ContainsKey(randomOriginalHash))
                    {
                        copyOfRandom = copedNodes[randomOriginalHash];
                    }
                    else
                    {
                        copyOfRandom = new ListNode { Data = nodeToCopy.Random.Data };
                        copedNodes.Add(randomOriginalHash, copyOfRandom);
                    }

                    copedNode.Random = copyOfRandom;
                }

                if (copedHead == null)
                    copedHead = copedNode;

                previous = copedNode;
                nodeToCopy = nodeToCopy.Next;
            }

            return copedHead;
        }

        private async Task SerializeInternal(ListNode? node, Stream s)
        {
            using var stringWriter = new StreamWriter(s, Encoding.UTF8, leaveOpen: true);
            using var writer = new JsonTextWriter(stringWriter);

            writer.WriteStartArray();

            while (node != null)
            {
                var nodeToSerialize = new SerializableNode { OriginalHashCode = node.GetHashCode(), Data = node.Data };

                if (node.Random != null)
                    nodeToSerialize.RandomHashCode = node.Random.GetHashCode();

                //"it's allowed to utilize third-party libraries for serializing 1 node"
                writer.WriteRaw(JsonConvert.SerializeObject(nodeToSerialize));
                writer.WriteRaw(",");

                node = node.Next;
            }

            writer.WriteEndArray();
        }

        private async Task<ListNode> DeserializeInternal(Stream s)
        {
            var deserializedNodes = new Dictionary<int, ListNode>();
            var nodesWithoutRandomLink = new Queue<SerializableNode>();

            SerializableNode? deserializedNode = null;
            ListNode result = null;
            ListNode previousListNode = null;

            using var textReader = new StreamReader(s, leaveOpen: true);
            using var jsonReader = new JsonTextReader(textReader);

            while (jsonReader.Read())
            {
                if (jsonReader.TokenType == JsonToken.StartObject)
                {
                    var jObj = JObject.Load(jsonReader);
                    deserializedNode = jObj.ToObject<SerializableNode>();

                    if (deserializedNode.OriginalHashCode == default)
                        throw new ArgumentException("Can't deserialize element. Invalid data.");

                    var currentListNode = new ListNode { Data = deserializedNode.Data };

                    if (result == null)
                        result = currentListNode;

                    if (!deserializedNodes.ContainsKey(deserializedNode.OriginalHashCode))
                        deserializedNodes.Add(deserializedNode.OriginalHashCode, currentListNode);

                    // I can't resolve all the "random" links by using this approach (using hash codes) during deserialization
                    // so I am trying to resolve part of them to reduce time complexety a litle
                    // For resolving all the links that left, I am using "ResolveRandomLinks" method
                    if (deserializedNode.RandomHashCode.HasValue)
                    {
                        var randomHash = deserializedNode.RandomHashCode.Value;

                        if (deserializedNodes.ContainsKey(randomHash))
                            currentListNode.Random = deserializedNodes[randomHash];
                        else
                            nodesWithoutRandomLink.Enqueue(deserializedNode);
                    }

                    if (previousListNode != null)
                    {
                        currentListNode.Previous = previousListNode;
                        previousListNode.Next = currentListNode;
                    }

                    previousListNode = currentListNode;
                }
            }

            //To resolve random links that was not resolved
            ResolveRandomLinks(deserializedNodes, nodesWithoutRandomLink);

            if (result == null)
                throw new ArgumentException("Invalid json format. No elements to deserialize");

            return result;
        }

        private void ResolveRandomLinks(Dictionary<int, ListNode> deserializedNodes, Queue<SerializableNode> nodesWithoutRandomLink)
        {
            while (nodesWithoutRandomLink.Any())
            {
                var node = nodesWithoutRandomLink.Dequeue();
                var originalHash = node.OriginalHashCode;
                var randomHash = node.RandomHashCode!.Value;

                var deserializedNode = deserializedNodes[originalHash];
                deserializedNode.Random = deserializedNodes[randomHash];
            }
        }

        #endregion

        #region Private classes

        /// <summary>
        /// Class for internal implementation of the serialization class
        /// </summary>
        private class SerializableNode
        {
            /// <summary>
            /// For resolving of random references after the serialization 
            /// </summary>
            public int OriginalHashCode { get; set; }

            public string? Data { get; set; }

            public int? RandomHashCode { get; set; }
        }

        #endregion
    }
}

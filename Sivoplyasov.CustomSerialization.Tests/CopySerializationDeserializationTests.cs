using SerializerTests.Implementations;
using SerializerTests.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sivoplyasov.CustomSerialization.Tests
{
    public class CopySerializationDeserializationTests
    {
        private JohnSmithSerializer _serializer;

        private ListNode _head10ElementsList;
        private ListNode _head5ElementsList;
        private ListNode _headWithoutElements;
        private ListNode _headWithRandomRefItself;
        private ListNode _nullHead;

        [SetUp]
        public void Setup()
        {
            _serializer = new JohnSmithSerializer();

            #region 10 elements setup

            var tenTenth = new ListNode { Data = "10 elements. Tenth" };
            var tenNinth = new ListNode { Data = "10 elements. Ninth", Next = tenTenth };
            var tenEighth = new ListNode { Data = "10 elements. Eighth", Next = tenNinth };
            var tenSeventh = new ListNode { Data = "10 elements. Seventh", Next = tenEighth };
            var tenSixth = new ListNode { Data = "10 elements. Sixth", Next = tenSeventh };
            var tenFifth = new ListNode { Data = "10 elements. Fifth", Next = tenSixth };
            var tenFourth = new ListNode { Data = "10 elements. Fourth", Next = tenFifth };
            var tenThird = new ListNode { Data = "10 elements. Third", Next = tenFourth };
            var tenSecond = new ListNode { Data = "10 elements. Second", Next = tenThird };
            var tenHead = new ListNode { Data = "10 elements. Head", Next = tenSecond };

            tenSecond.Previous = tenHead;
            tenThird.Previous = tenSecond;
            tenFourth.Previous = tenThird;
            tenFifth.Previous = tenFourth;
            tenSixth.Previous = tenFifth;
            tenSeventh.Previous = tenSixth;
            tenEighth.Previous = tenSeventh;
            tenNinth.Previous = tenEighth;
            tenTenth.Previous = tenNinth;

            _head10ElementsList = tenHead;

            #endregion

            #region 5 elements setup

            var fiveFifth = new ListNode { Data = "5 elements. Fifth" };
            var fiveFourth = new ListNode { Data = "5 elements. Fourth", Next = fiveFifth };
            var fiveThird = new ListNode { Data = "5 elements. Third", Next = fiveFourth };
            var fiveSecond = new ListNode { Data = "5 elements. Second", Next = fiveThird };
            var fiveHead = new ListNode { Data = "5 elements. Head", Next = fiveSecond };

            fiveSecond.Previous = fiveHead;
            fiveThird.Previous = fiveSecond;
            fiveFourth.Previous = fiveThird;
            fiveFifth.Previous = fiveFourth;

            _head5ElementsList = fiveHead;

            #endregion

            #region Without elements

            _headWithoutElements = new ListNode { Data = "No elements next" };

            #endregion

            #region Random property refers itself

            _headWithRandomRefItself = new ListNode { Data = "RND RefersItself" };
            _headWithRandomRefItself.Random = _headWithRandomRefItself;

            #endregion

            _nullHead = null;
        }

        [Test]
        public async Task CopySerealizeDeserializeCountTest()
        {
            using var stream = new MemoryStream();

            var copedHead = await _serializer.DeepCopy(_head10ElementsList);

            await _serializer.Serialize(copedHead, stream);
            var deserializedHead = await _serializer.Deserialize(stream);

            var counter = 1;
            var currentElement = deserializedHead;

            while (currentElement.Next != null)
            {
                counter++;
                currentElement = currentElement.Next;
            }

            Assert.That(counter, Is.EqualTo(10));
        }

        [Test]
        public async Task CopySerializeDeserializeDataTest()
        {
            var second = _head5ElementsList.Next;
            var third = second!.Next;
            var forth = third!.Next;
            var fifth = forth!.Next;

            using var stream = new MemoryStream();

            var copedHead = await _serializer.DeepCopy(_head5ElementsList);

            await _serializer.Serialize(copedHead, stream);
            var deserializedHead = await _serializer.Deserialize(stream);
            var deserializedSecond = deserializedHead!.Next;
            var deserializedThird = deserializedSecond!.Next;
            var deserializedForth = deserializedThird!.Next;
            var deserializedFifth = deserializedForth!.Next;

            Assert.That(_head5ElementsList.Data, Is.EqualTo(deserializedHead.Data));
            Assert.That(second.Data, Is.EqualTo(deserializedSecond.Data));
            Assert.That(third.Data, Is.EqualTo(deserializedThird.Data));
            Assert.That(forth.Data, Is.EqualTo(deserializedForth.Data));
            Assert.That(fifth!.Data, Is.EqualTo(deserializedFifth!.Data));
        }
    }
}

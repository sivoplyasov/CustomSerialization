using SerializerTests.Implementations;
using SerializerTests.Nodes;
using System.Collections.Generic;

namespace Sivoplyasov.CustomSerialization.Tests
{
    public class DeepCopyTests
    {
        private SivoplyasovSerializer _serializer;

        private ListNode _head10ElementsList;
        private ListNode _head5ElementsList;
        private ListNode _headWithoutElements;
        private ListNode _headWithRandomRefItself;
        private ListNode _nullHead;

        [SetUp]
        public void Setup()
        {
            _serializer = new SivoplyasovSerializer();

            #region 10 elements setup

            var tenTenth = new ListNode { Data = "10 elements. Tenth" };
            var tenNinth = new ListNode { Data = "10 elements. Ninth", Next = tenTenth };
            var tenEighth = new ListNode { Data = "10 elements. Eighth", Next = tenNinth };
            var tenSeventh = new ListNode { Data = "10 elements. Seventh", Next = tenEighth };
            var tenSixth = new ListNode { Data = "10 elements. Sixth", Next = tenSeventh };
            var tenFifth = new ListNode { Data = "10 elements. Fifth", Next = tenSixth };
            var tenFourth = new ListNode { Data = "10 elements. Fourth", Next = tenFifth };
            var tenThird = new ListNode { Data = "10 elements. Third", Next = tenFourth };
            var tenSecond = new ListNode { Data = "10 elements. Second", Next = tenThird};
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
        public async Task TenElementsCopyCountTest()
        {
            var elementCounter = 1;
            var copedElementCounter = 1;

            var currentElement = _head10ElementsList;
            var copedHead = await _serializer.DeepCopy(_head10ElementsList);

            while (currentElement.Next != null)
            {
                elementCounter++;
                currentElement = currentElement.Next;
            }

            while (copedHead.Next != null)
            {
                copedElementCounter++;
                copedHead = copedHead.Next;
            }

            Assert.That(copedElementCounter, Is.EqualTo(10));
            Assert.That(elementCounter, Is.EqualTo(10));
            Assert.That(copedElementCounter, Is.EqualTo(elementCounter));
        }

        [Test]
        public async Task FiveElementsCopyCountTest()
        {
            var elementCounter = 1;
            var copedElementCounter = 1;

            var currentElement = _head5ElementsList;
            var copedHead = await _serializer.DeepCopy(_head5ElementsList);

            while (currentElement.Next != null)
            {
                elementCounter++;
                currentElement = currentElement.Next;
            }

            while (copedHead.Next != null)
            {
                copedElementCounter++;
                copedHead = copedHead.Next;
            }

            Assert.That(copedElementCounter, Is.EqualTo(5));
            Assert.That(elementCounter, Is.EqualTo(5));
            Assert.That(copedElementCounter, Is.EqualTo(elementCounter));
        }


        [Test]
        public async Task DataEqualTest()
        {
            var second = _head5ElementsList.Next;
            var third = second!.Next;
            var forth = third!.Next;
            var fifth = forth!.Next;

            var copedHead = await _serializer.DeepCopy(_head5ElementsList);
            var copedSecond = copedHead!.Next;
            var copedThird = copedSecond!.Next;
            var copedForth = copedThird!.Next;
            var copedFifth = copedForth!.Next;

            Assert.That(_head5ElementsList.Data, Is.EqualTo(copedHead.Data));
            Assert.That(second.Data, Is.EqualTo(copedSecond.Data));
            Assert.That(third.Data, Is.EqualTo(copedThird.Data));
            Assert.That(forth.Data, Is.EqualTo(copedForth.Data));
            Assert.That(fifth!.Data, Is.EqualTo(copedFifth!.Data));
        }

        [Test]
        public async Task PreviousReferencesDataCorrectnessTest()
        {
            var second = _head5ElementsList.Next;
            var third = second!.Next;
            var forth = third!.Next;
            var fifth = forth!.Next;

            var copedHead = await _serializer.DeepCopy(_head5ElementsList);
            var copedSecond = copedHead!.Next;
            var copedThird = copedSecond!.Next;
            var copedForth = copedThird!.Next;
            var copedFifth = copedForth!.Next;

            Assert.IsNull(_head5ElementsList.Previous);
            Assert.IsNull(copedHead.Previous);

            Assert.That(second.Previous!.Data, Is.EqualTo(copedSecond.Previous!.Data));
            Assert.That(third.Previous!.Data, Is.EqualTo(copedThird.Previous!.Data));
            Assert.That(forth.Previous!.Data, Is.EqualTo(copedForth.Previous!.Data));
            Assert.That(fifth.Previous!.Data, Is.EqualTo(copedFifth!.Previous!.Data));
        }

        [Test]
        public async Task LastReferencesCorrectnessTest()
        {
            var second = _head5ElementsList.Next;
            var third = second!.Next;
            var forth = third!.Next;
            var fifth = forth!.Next;

            var copedHead = await _serializer.DeepCopy(_head5ElementsList);
            var copedSecond = copedHead!.Next;
            var copedThird = copedSecond!.Next;
            var copedForth = copedThird!.Next;
            var copedFifth = copedForth!.Next;

            Assert.IsNull(fifth!.Next);
            Assert.IsNull(copedFifth!.Next);
        }

        [Test]
        public async Task RandomReferencesCorrectnessTest()
        {
            var second = _head5ElementsList.Next;
            var third = second!.Next;
            var forth = third!.Next;
            var fifth = forth!.Next;

            second.Random = forth;
            forth.Random = forth;
            third.Random = fifth;
            _head5ElementsList.Random = third;

            var copedHead = await _serializer.DeepCopy(_head5ElementsList);
            var copedSecond = copedHead!.Next;
            var copedThird = copedSecond!.Next;
            var copedForth = copedThird!.Next;
            var copedFifth = copedForth!.Next;

            Assert.That(copedSecond.Random, Is.EqualTo(copedForth));
            Assert.That(copedForth.Random, Is.EqualTo(copedForth));
            Assert.That(copedThird.Random, Is.EqualTo(copedFifth));
            Assert.That(copedHead.Random, Is.EqualTo(copedThird));
        }

        [Test]
        public async Task DifferentReferencesSameDataTest()
        {
            var second = _head5ElementsList.Next;
            var third = second!.Next;
            var forth = third!.Next;
            var fifth = forth!.Next;

            var copedHead = await _serializer.DeepCopy(_head5ElementsList);
            var copedSecond = copedHead!.Next;
            var copedThird = copedSecond!.Next;
            var copedForth = copedThird!.Next;
            var copedFifth = copedForth!.Next;

            //Equal data
            Assert.That(_head5ElementsList.Data, Is.EqualTo(copedHead.Data));
            Assert.That(second.Data, Is.EqualTo(copedSecond.Data));
            Assert.That(third.Data, Is.EqualTo(copedThird.Data));
            Assert.That(forth.Data, Is.EqualTo(copedForth.Data));
            Assert.That(fifth!.Data, Is.EqualTo(copedFifth!.Data));

            //Different objects (references)
            Assert.That(_head5ElementsList, Is.Not.EqualTo(copedHead));
            Assert.That(second, Is.Not.EqualTo(copedSecond));
            Assert.That(third, Is.Not.EqualTo(copedThird));
            Assert.That(forth, Is.Not.EqualTo(copedForth));
            Assert.That(fifth, Is.Not.EqualTo(copedFifth));
        }

        [Test]
        public async Task RandomReferencesItselfCorrectnessTest()
        {
            var copedHead = await _serializer.DeepCopy(_headWithRandomRefItself);

            Assert.That(copedHead.Random, Is.EqualTo(copedHead));
        }

        [Test]
        public async Task CopyOfSingleElementTest()
        {
            var copedHead = await _serializer.DeepCopy(_headWithoutElements);

            Assert.IsNull(copedHead.Previous);
            Assert.IsNull(copedHead.Random);
            Assert.IsNull(copedHead.Next);
        }

        [Test]
        public async Task CopyOfSingleElementDataTest()
        {
            var copedHead = await _serializer.DeepCopy(_headWithoutElements);

            Assert.That(_headWithoutElements.Data, Is.EqualTo(copedHead.Data));
        }

        [Test]
        public async Task CopyOfNullElement()
        {
            var copedHead = await _serializer.DeepCopy(_nullHead);

            Assert.IsNull(copedHead);
        }
    }
}

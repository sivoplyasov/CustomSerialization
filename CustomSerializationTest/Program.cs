using SerializerTests.Implementations;
using SerializerTests.Nodes;

var eighth = new ListNode
{
    Data = "8",
};

var seventh = new ListNode
{
    Data = "7",
    Next = eighth,
    Random = eighth 
};

var sixth = new ListNode
{
    Data = "6",
    Next = seventh,
    Random = eighth 
};

var fifth = new ListNode
{
    Data = "5",
    Next = sixth,
    Random = eighth
};

var forth = new ListNode
{
    Data = "4",
    Next = fifth,
    Random = sixth
};


var third = new ListNode
{
    Data = "3",
    Next = forth,
    Random = forth
};

var second = new ListNode
{
    Data = "2",
    Next = third,
    Random = fifth
};

var head = new ListNode
{
    Data = "1",
    Next = second,
    Random = second
};

JohnSmithSerializer johnSmithSerializer = new JohnSmithSerializer();

var copy = await johnSmithSerializer.DeepCopy(head);

Console.ReadLine();
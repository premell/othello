using System;
using Xunit;


public class ExampleTest
{

    [Fact]
    public void Test1()
    {
        Assert.Equal(5, Add(2, 2));
    }

    [Fact]
    public void Test2()
    {
        Assert.Equal(4, Add(2, 2));
    }



    int Add(int x, int y)
    {
        return x + y;
    }
}

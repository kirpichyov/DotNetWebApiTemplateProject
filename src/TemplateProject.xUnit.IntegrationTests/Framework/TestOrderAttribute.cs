namespace TemplateProject.xUnit.IntegrationTests.Framework;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class TestOrderAttribute : Attribute
{
    public int Order { get; private set; }

    public TestOrderAttribute(int order)
    {
        Order = order;
    }
}
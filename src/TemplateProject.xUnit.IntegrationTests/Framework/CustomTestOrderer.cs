using Xunit.Abstractions;
using Xunit.Sdk;

namespace TemplateProject.xUnit.IntegrationTests.Framework;

public class CustomTestOrderer : ITestCaseOrderer
{
    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
        where TTestCase : ITestCase
    {
        var sortedMethods = new SortedDictionary<int, TTestCase>();

        foreach (var testCase in testCases)
        {
            var orderAttribute = testCase.TestMethod.Method.GetCustomAttributes(typeof(TestOrderAttribute)).FirstOrDefault();
            
            if (orderAttribute is null)
            {
                sortedMethods.Add(0, testCase);
                continue;
            }
            
            var order = orderAttribute.GetNamedArgument<int>("Order");
            sortedMethods.Add(order, testCase);
        }

        return sortedMethods.Values;
    }
}
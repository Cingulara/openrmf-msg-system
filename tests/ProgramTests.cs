using System;
using System.Reflection;
using MongoDB.Bson;
using openrmf_msg_system.Models;
using Xunit;

namespace openrmf_msg_system_tests;

public class ProgramTests
{
    [Fact]
    public void GetInternalId_WithValidObjectIdString_ReturnsParsedObjectId()
    {
        var objectId = ObjectId.GenerateNewId();

        var result = InvokeGetInternalId(objectId.ToString());

        Assert.Equal(objectId, result);
        Assert.NotEqual(ObjectId.Empty, result);
    }

    [Fact]
    public void GetInternalId_WithInvalidObjectIdString_ReturnsEmptyObjectId()
    {
        var result = InvokeGetInternalId("not-an-object-id");

        Assert.Equal(ObjectId.Empty, result);
        Assert.NotEqual(ObjectId.GenerateNewId(), result);
    }

    private static ObjectId InvokeGetInternalId(string id)
    {
        var assembly = typeof(Artifact).Assembly;
        var programType = assembly.GetType("openrmf_msg_system.Program", throwOnError: true);
        var method = programType!.GetMethod("GetInternalId", BindingFlags.NonPublic | BindingFlags.Static);

        Assert.NotNull(method);

        var value = method!.Invoke(obj: null, parameters: new object[] { id });
        Assert.NotNull(value);

        return (ObjectId)value!;
    }
}

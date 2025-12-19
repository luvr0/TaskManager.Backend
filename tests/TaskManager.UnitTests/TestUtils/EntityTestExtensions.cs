using System.Reflection;

namespace TaskManager.UnitTests.TestUtils;

internal static class EntityTestExtensions
{
  private const string IdPropertyName = "Id";

  public static TEntity WithEntityId<TEntity, TId>(this TEntity entity, TId value)
  {
    var property = entity!
      .GetType()
      .GetProperty(IdPropertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

    property!.SetValue(entity, value);
    return entity;
  }
}

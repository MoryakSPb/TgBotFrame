using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TgBotFrame.Data;

public interface IModelEntityScheme<T> where T : class
{
    static abstract void EntityBuild(EntityTypeBuilder<T> entity);
}
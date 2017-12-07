namespace TF.Common.ValueInjecter.Injections
{
    public interface IValueInjection
    {
        object Map(object source, object target);
    }
}
namespace AudioBookPlayer.App.Domain.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBuilder<out T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        T Build();
    }
}
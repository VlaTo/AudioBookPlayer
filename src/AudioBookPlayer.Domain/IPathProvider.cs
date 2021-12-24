namespace AudioBookPlayer.Domain
{
    public interface IPathProvider
    {
        string GetPath(string filename);
    }
}
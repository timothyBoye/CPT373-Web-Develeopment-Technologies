namespace GoLA2.Models.Logic
{
    public interface ITemplate
    {
        string Name { get; }
        int Height { get; }
        int Width { get; }
        Cell[][] Cells { get; }
    }
}

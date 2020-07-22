public interface IVisualData<D, R> where D : IVisualData<D, R>
    where R : IVisualRenderer<D, R>
{
    void ConnectRenderer(R renderer);
    void DisconnectRenderer();
}
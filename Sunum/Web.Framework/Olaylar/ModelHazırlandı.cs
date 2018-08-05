namespace Web.Framework.Olaylar
{
    public class ModelHazırlandı<T>
    {
        #region Ctor
        public ModelHazırlandı(T model)
        {
            this.Model = model;
        }
        #endregion

        #region Properties
        public T Model { get; private set; }
        #endregion
    }
}

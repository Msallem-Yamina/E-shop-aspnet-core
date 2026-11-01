namespace TP_Product.Models.Repositories
{
    public interface IOrderRepository
    {
        Order GetById(int Id);
        List<Order> GetAll();
        void Add(Order o);

        List<Order> GetByUserId(string userId);
    }
}
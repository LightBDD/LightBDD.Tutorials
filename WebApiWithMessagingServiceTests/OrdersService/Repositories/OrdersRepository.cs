using System;
using LiteDB;
using OrdersService.Models;

namespace OrdersService.Repositories
{
    public class OrdersRepository
    {
        private readonly ILiteCollection<Order> _collection;

        public OrdersRepository(LiteDatabase db)
        {
            _collection = db.GetCollection<Order>();
            _collection.EnsureIndex(x => x.Id, true);
        }

        public void Upsert(Order order) => _collection.Upsert(order);

        public Order? GetById(Guid orderId) => _collection.FindById(orderId);
    }
}
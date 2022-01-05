using ResturantApp.Models;
using ResturantApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ResturantApp.Repositories
{
   
    public class OrderRepository
    {
        
        private ResturantDBEntities objResturantDBEntities;
        PaymentRepository obj = new PaymentRepository();
      
        public OrderRepository()
        {
            objResturantDBEntities = new ResturantDBEntities();
        }

        public bool AddOrder(OrderViewModel objOrderViewModel)
        {
            Order objOrder = new Order();
            objOrder.CustomerId = objOrderViewModel.CustomerId;
            objOrder.FinalTotal = objOrderViewModel.FinalTotal;
            objOrder.OrderDate = DateTime.Now;
            string OrderNumber = String.Format("{0:ddmmmyyyyhhmmss}", DateTime.Now);
            objOrder.OrderNumber = OrderNumber;
            obj.orderNumber = OrderNumber;
            objOrder.PaymentTypeId = objOrderViewModel.PaymentTypeId;
            objResturantDBEntities.Orders.Add(objOrder);
            objResturantDBEntities.SaveChanges();
            int OrderId = objOrder.OrderId;
      
            foreach (var item in objOrderViewModel.ListOfOrderDetailViewModel)
            {
                OrderDetail objOrderDetail = new OrderDetail();
                objOrderDetail.OrderId = OrderId;
                objOrderDetail.Discount = item.Discount;
                objOrderDetail.ItemId = item.ItemId;
                objOrderDetail.Total = item.Total;
                objOrderDetail.UnitPrice = item.UnitPrice;
                objOrderDetail.Quantity = item.Quantity;
                objResturantDBEntities.OrderDetails.Add(objOrderDetail);
                objResturantDBEntities.SaveChanges();


                Transaction objTransaction = new Transaction();
                objTransaction.ItemId = item.ItemId;
                objTransaction.Quantity = (-1)*item.Quantity;
                objTransaction.TransactionDate = DateTime.Now;
                objTransaction.TypeId = 2;
                objResturantDBEntities.Transactions.Add(objTransaction);
                objResturantDBEntities.SaveChanges();
            }

            return true;
        }

    }
}
using Bakery.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace Bakery.ImportConsole
{
    public class ImportController
    {
        private const string ProductFileName = "Products.csv";
        private const string OrderItemFileName = "OrderItems.csv";
        public static IEnumerable<Product> ReadFromCsv()
        {
            string[][] productMatrix = MyFile.ReadStringMatrixFromCsv(ProductFileName, true);
            string[][] orderMatrix = MyFile.ReadStringMatrixFromCsv(OrderItemFileName, true);

            var products = productMatrix.GroupBy(line => line[0] + ';' +
                                                         line[1] + ';' +
                                                         line[2] + ';')
                                        .Select(grp => new Product
                                        {
                                            ProductNr = grp.Key.Split(';')[0],
                                            Name = grp.Key.Split(';')[1],
                                            Price = Convert.ToDouble(grp.Key.Split(';')[2]),
                                        })
                                        .ToArray();


            var customers = orderMatrix.GroupBy(line => line[2] + ';' +
                                                       line[3] + ';' +
                                                       line[4])
                                       .Select(grp => new Customer
                                       {
                                           CustomerNr = grp.Key.Split(';')[0],
                                           FirstName = grp.Key.Split(';')[1],
                                           LastName = grp.Key.Split(';')[2]
                                       })
                                       .ToArray();

            var orders = orderMatrix.GroupBy(line => line[0] + ';' +
                                                    line[1] + ';' +
                                                    line[2])
                                   .Select(grp => new Order
                                   {
                                       OrderNr = grp.Key.Split(';')[0],
                                       Date = Convert.ToDateTime(grp.Key.Split(';')[1]),
                                       Customer = customers.SingleOrDefault(c => c.CustomerNr.Equals(grp.Key.Split(';')[2]))
                                   })
                                   .ToArray();

            var orderItems = orderMatrix
                            .Select(line => new OrderItem
                            {
                                Order = orders.SingleOrDefault(o => o.OrderNr.Equals(line[0])),
                                Product = products.SingleOrDefault(p => p.ProductNr.Equals(line[5])),
                                Amount = Convert.ToInt32(line[6]),
                            })
                            .ToArray();

            foreach (var product in products)
            {
                product.OrderItems = orderItems.Where(oi => oi.Product.Equals(product))
                                               .ToArray();
            }

            foreach (var order in orders)
            {
                order.OrderItems = orderItems.Where(oi => oi.Order.Equals(order))
                                             .ToList();
            }
            return products;
        }
    }
}

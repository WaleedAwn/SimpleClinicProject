using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonsAPIDataAccessLayer.Payments
{
    public class PaymentMethodDTO
    {
        public int Id {  get; set; }
        public string Name { get; set; }
        public PaymentMethodDTO(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}

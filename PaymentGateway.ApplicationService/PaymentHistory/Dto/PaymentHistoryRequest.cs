using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.ApplicationService.PaymentHistory.Dto
{
    public class PaymentHistoryRequest
    {
        public Guid? MerchantOrderId { get; set; }

        [StringLength(16, MinimumLength = 16, ErrorMessage = "Card number is not valid")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Card number is not valid")]
        public string? CardNumber { get; set; }

        [RegularExpression("(\\d{4})-(\\d{2})-(\\d{2}) (\\d{2}):(\\d{2}):(\\d{2})", ErrorMessage = "Start date is not valid")]
        public DateTime? StartDate { get; set; }

        [RegularExpression("(\\d{4})-(\\d{2})-(\\d{2}) (\\d{2}):(\\d{2}):(\\d{2})", ErrorMessage = "End date is not valid")]
        public DateTime? EndDate { get; set; }
    }
}

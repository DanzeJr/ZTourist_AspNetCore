using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ZTourist.Models
{
    public class Cart
    {
        private List<CartLine> lineCollection = new List<CartLine>();

        public virtual void AddItem(Tour tour, int adultTicket, int kidTicket)
        {
            CartLine line = lineCollection
                .Where(x => x.Tour.Id == tour.Id)
                .FirstOrDefault();

            if (line == null)
            {
                lineCollection.Add(new CartLine
                {
                    Tour = tour,
                    AdultTicket = adultTicket,
                    KidTicket = kidTicket
                });
            }
            else
            {
                line.AdultTicket += adultTicket;
                line.KidTicket += kidTicket;
            }
        }

        public virtual void RemoveLine(Tour tour) =>
            lineCollection.RemoveAll(x => x.Tour.Id == tour.Id);

        public virtual decimal ComputeTotalValue() =>
            lineCollection.Sum(x => x.Tour.AdultFare * x.AdultTicket + x.Tour.KidFare * (x.KidTicket ?? 0));

        public virtual void Clear() => lineCollection.Clear();

        public virtual IEnumerable<CartLine> Lines => lineCollection;
    }

    public class CartLine
    {
        public Tour Tour { get; set; }

        [Required(ErrorMessage = "Please choose number of adult tickets")]
        public int AdultTicket { get; set; }

        public int? KidTicket { get; set; } = 0;
    }
}

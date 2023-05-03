using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Flighta.Data
{
    public class FlightM
    {
        public FlightM() { }
        public FlightM(string from, string to, int seats, DateTime flightTime, decimal cost)
        {
            From = from;
            To = to;
            this.seats = seats;
            this.flightTime = flightTime;
            this.cost = cost;
        }

        public FlightM(int flightId, string from, string to, int seats, DateTime flightTime, decimal cost)
        {
            this.flightId = flightId;
            From = from;
            To = to;
            this.seats = seats;
            this.flightTime = flightTime;
            this.cost = cost;
        }
        [DisplayName("Flight Number")]
        public int flightId { get; set; }
        public string From { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
        public int seats { get; set; } = 0;
        [DisplayName("Flight Time")]
        public DateTime flightTime { get; set; }
        public decimal cost { get; set; }
    }
}
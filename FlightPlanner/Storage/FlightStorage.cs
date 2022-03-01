using System;
using FlightPlanner.Models;

namespace FlightPlanner.Storage
{
    public static class FlightStorage
    {
        private static int _id;
        private static readonly object _flightLocker = new object();

        public static Flight ConvertToFlight(AddFlightRequest request)
        {
            lock (_flightLocker)
            {
                var flight = new Flight
                {
                    From = request.From,
                    To = request.To,
                    Carrier = request.Carrier,
                    ArrivalTime = request.ArrivalTime,
                    DepartureTime = request.DepartureTime
                };

                return flight;
            }
        }

        public static bool IsValid(AddFlightRequest request)
        {
            lock (_flightLocker)
            {
                if (request == null)
                {
                    return false;
                }

                if (string.IsNullOrEmpty(request.ArrivalTime) || string.IsNullOrEmpty(request.DepartureTime) ||
                    string.IsNullOrEmpty(request.Carrier))
                {
                    return false;
                }

                if (request.From == null || request.To == null)
                {
                    return false;
                }

                if (string.IsNullOrEmpty(request.From.AirportName) || string.IsNullOrEmpty(request.From.City) ||
                    string.IsNullOrEmpty(request.From.Country))
                {
                    return false;
                }

                if (string.IsNullOrEmpty(request.To.AirportName) || string.IsNullOrEmpty(request.To.City) ||
                    string.IsNullOrEmpty(request.To.Country))
                {
                    return false;
                }

                if (request.From.Country.ToLower().Trim() == request.To.Country.ToLower().Trim() &&
                    request.From.City.ToLower().Trim() == request.To.City.ToLower().Trim() &&
                    request.From.AirportName.ToLower().Trim() == request.To.AirportName.ToLower().Trim())
                {
                    return false;
                }

                var arrivalTime = DateTime.Parse(request.ArrivalTime);
                var departureTime = DateTime.Parse(request.DepartureTime);

                if (arrivalTime <= departureTime)
                {
                    return false;
                }

                return true;
            }
        }

        public static bool IsValidSearch(SearchFlightsRequest request)
        {
            lock (_flightLocker)
            {
                if (string.IsNullOrEmpty(request.From) || string.IsNullOrEmpty(request.To) ||
                    string.IsNullOrEmpty(request.DepartureDate))
                {
                    return false;
                }

                if (request.From == request.To)
                {
                    return false;
                }

                return true;
            }
        }
    }
}

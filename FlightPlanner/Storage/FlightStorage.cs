using System;
using System.Collections.Generic;
using System.Linq;
using FlightPlanner.Models;

namespace FlightPlanner.Storage
{
    public static class FlightStorage
    {
        private static List<Flight> _flights = new List<Flight>();
        private static int _id;

        public static Flight AddFlight(AddFlightRequest request)
        {
            var flight = new Flight
            {
                From = request.From,
                To = request.To,
                Carrier = request.Carrier,
                ArrivalTime = request.ArrivalTime,
                DepartureTime = request.DepartureTime,
                Id = ++_id
            };

            _flights.Add(flight);

            return flight;
        }

        public static Flight GetFlight(int id)
        {
            return _flights.SingleOrDefault(f => f.Id == id);
        }

        public static void DeleteFlights(int id)
        {
            var flight = GetFlight(id);

            if (flight != null)
            {
                _flights.Remove(flight);
            }
        }

        public static List<Airport> SearchAirports(string input)
        {
            input = input.ToLower().Trim();

            var fromAirports = _flights.Where(f =>
                f.From.AirportName.ToLower().Trim().Contains(input) ||
                f.From.City.ToLower().Trim().Contains(input) ||
                f.From.Country.ToLower().Trim().Contains(input)).Select(f => f.From).ToList();

            var toAirports = _flights.Where(f =>
                f.To.AirportName.ToLower().Trim().Contains(input) ||
                f.To.City.ToLower().Trim().Contains(input) ||
                f.To.Country.ToLower().Trim().Contains(input)).Select(f => f.To).ToList();

            return fromAirports.Concat(toAirports).ToList();
        }

        public static void ClearFlights()
        {
            _flights.Clear();
            _id = 0;
        }

        public static bool Exists(AddFlightRequest request)
        {
            return _flights.Any(f =>
                f.Carrier.ToLower().Trim() == request.Carrier.ToLower().Trim() &&
                f.From.AirportName.ToLower().Trim() == request.From.AirportName.ToLower().Trim() &&
                f.To.AirportName.ToLower().Trim() == request.To.AirportName.ToLower().Trim() &&
                f.ArrivalTime == request.ArrivalTime &&
                f.DepartureTime == request.DepartureTime);
    }

        public static PageResult SearchFlight(SearchFlightsRequest request)
        {
            var pageResult = new PageResult();

            var flights = _flights.Where(f =>
                    f.From.AirportName.ToLower().Trim().Contains(request.From.ToLower().Trim()) ||
                    f.To.AirportName.ToLower().Trim().Contains(request.To.ToLower().Trim()) ||
                    f.DepartureTime == request.DepartureDate)
                .Select(f => f).ToList();

            foreach (var flight in flights)
            {
                if (flight == null)
                {
                    return pageResult;
                }

                pageResult.Items.Add(flight);
            }

            pageResult.TotalItems = pageResult.Items.Count;
            pageResult.Page = pageResult.TotalItems;
            return pageResult;
        }

        public static bool IsValid(AddFlightRequest request)
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

        public static bool IsValidSearch(SearchFlightsRequest request)
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